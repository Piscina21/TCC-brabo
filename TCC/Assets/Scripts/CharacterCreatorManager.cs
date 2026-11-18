using System;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation.LowLevel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SaveLocation
{
    PersistentDataPath, // Pasta padrão segura da Unity (AppData/LocalLow)
    ProjectAssets,      // Cria uma pasta "Saves" dentro de Assets (aparece na Unity)
    Desktop,            // Salva direto na Área de Trabalho
    CustomPath          // Caminho personalizado digitado no Inspector
}
public class CharacterCreatorManager : MonoBehaviour
{
    [Header("Fluxo de Painéis (Arraste na ordem desejada)")]
    public List<GameObject> stepPanels = new List<GameObject>();
    private int currentStepIndex = 0;

    [Header("Botões de Navegação")]
    public Button nextButton;
    public Button previousButton;

    [Header("Regras para Avançar")]
    [Tooltip("Se marcado, o jogador precisa gastar todos os pontos antes de avançar dos atributos.")]
    public bool requireAllAttributePoints = true;
    [Tooltip("Se marcado, o jogador precisa escolher ao menos uma peça ou cor na aparência.")]
    public bool requireAppearanceSelection = false;

    [Header("Events")]
    public UnityEvent onRaceChanged;
    public UnityEvent onClassChanged;
    public UnityEvent onAttributesChanged;
    public UnityEvent onAppearanceChanged;

    [Header("Configuração de Atributos")]
    public int pointsToDistribute = 10;
    private int remainingPoints;

    // Estado em tempo de execução (Runtime)
    private RaceData selectedRace;
    private ClassData selectedClass;
    private Dictionary<AttributeType, int> baseAttributes = new Dictionary<AttributeType, int>();
    private Dictionary<AttributeType, int> allocatedAttributes = new Dictionary<AttributeType, int>();
    private Dictionary<AppearanceCategory, AppearanceOptionData> selectedAppearance = new Dictionary<AppearanceCategory, AppearanceOptionData>();
    private Dictionary<ColorCategory, Color> selectedColors = new Dictionary<ColorCategory, Color>();

    // Objeto limpo pronto para JSON
    public CharacterBuild currentBuild = new CharacterBuild();

    private void Awake()
    {
        InitializeDictionaries();
    }

    private void Start()
    {
        remainingPoints = pointsToDistribute;
        SyncBuildData();
        ShowStep(0);
    }

    private void InitializeDictionaries()
    {
        baseAttributes.Clear();
        allocatedAttributes.Clear();

        foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
        {
            baseAttributes[attr] = 0;
            allocatedAttributes[attr] = 0;
        }
    }

    // ---------- VALIDAÇÃO E NAVEGAÇÃO MODULAR ----------
    public bool CanAdvanceFromCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= stepPanels.Count) return false;

        GameObject currentPanel = stepPanels[currentStepIndex];
        if (currentPanel == null) return false;

        if (currentPanel.GetComponent("RaceSelectionPanel") != null)
            return selectedRace != null;

        if (currentPanel.GetComponent("ClassSelectionPanel") != null)
            return selectedClass != null;

        if (currentPanel.GetComponent("AttributePanel") != null)
            return !requireAllAttributePoints || remainingPoints == 0;

        if (currentPanel.GetComponent("AppearancePanel") != null || currentPanel.GetComponent("Appearancepanel") != null)
            return !requireAppearanceSelection || (selectedAppearance.Count > 0 || selectedColors.Count > 0);

        if (currentPanel.GetComponent("NamePanel") != null)
            return !string.IsNullOrWhiteSpace(currentBuild.characterName);

        return true;
    }

    public void UpdateNavigationButtons()
    {
        if (nextButton != null)
            nextButton.interactable = CanAdvanceFromCurrentStep();

        if (previousButton != null)
            previousButton.interactable = currentStepIndex > 0;
    }

    public void GoToNextStep()
    {
        if (!CanAdvanceFromCurrentStep()) return;

        if (currentStepIndex + 1 < stepPanels.Count)
        {
            ShowStep(currentStepIndex + 1);
        }
    }

    public void GoToPreviousStep()
    {
        if (currentStepIndex - 1 >= 0)
        {
            ShowStep(currentStepIndex - 1);
        }
    }

    public void ShowStep(int index)
    {
        if (index < 0 || index >= stepPanels.Count) return;

        currentStepIndex = index;
        for (int i = 0; i < stepPanels.Count; i++)
        {
            if (stepPanels[i] != null)
                stepPanels[i].SetActive(i == currentStepIndex);
        }

        UpdateNavigationButtons();
    }

    // ---------- RAÇA E CLASSE ----------
    public void SelectRace(RaceData race)
    {
        selectedRace = race;

        // Se a classe já selecionada for proibida para a nova raça, limpa a seleção
        if (selectedClass != null && !selectedClass.IsAllowedForRace(selectedRace))
        {
            selectedClass = null;
            onClassChanged?.Invoke();
        }

        RecalculateBaseStats();
        onRaceChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public void SelectClass(ClassData classData)
    {
        if (classData != null && !classData.IsAllowedForRace(selectedRace))
            return;

        selectedClass = classData;
        RecalculateBaseStats();
        onClassChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public RaceData GetSelectedRace() => selectedRace;
    public ClassData GetSelectedClass() => selectedClass;

    private void RecalculateBaseStats()
    {
        foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
        {
            baseAttributes[attr] = 0;
        }

        if (selectedClass != null)
        {
            foreach (var baseAttr in selectedClass.baseAttributes)
                baseAttributes[baseAttr.attribute] += baseAttr.baseValue;
        }

        if (selectedRace != null)
        {
            foreach (var bonus in selectedRace.baseBonuses)
                baseAttributes[bonus.attribute] += bonus.bonusValue;
        }

        SyncBuildData();
        onAttributesChanged?.Invoke();
    }

    // ---------- ATRIBUTOS (COM TRAVA DE VALOR BASE) ----------
    public void IncreaseAttribute(AttributeType attr)
    {
        if (remainingPoints <= 0) return;

        allocatedAttributes[attr]++;
        remainingPoints--;

        SyncBuildData();
        onAttributesChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public void DecreaseAttribute(AttributeType attr)
    {
        if (allocatedAttributes[attr] <= 0) return;

        allocatedAttributes[attr]--;
        remainingPoints++;

        SyncBuildData();
        onAttributesChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public int GetBaseAttributeValue(AttributeType attr) => baseAttributes.TryGetValue(attr, out int val) ? val : 0;
    public int GetAllocatedAttributeValue(AttributeType attr) => allocatedAttributes.TryGetValue(attr, out int val) ? val : 0;
    public int GetTotalAttributeValue(AttributeType attr) => GetBaseAttributeValue(attr) + GetAllocatedAttributeValue(attr);
    public int GetRemainingPoints() => remainingPoints;

    // ---------- APARÊNCIA E CORES ----------
    public void SelectAppearance(AppearanceOptionData option)
    {
        if (option == null) return;

        selectedAppearance[option.category] = option;
        SyncBuildData();
        onAppearanceChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public AppearanceOptionData GetSelectedAppearance(AppearanceCategory category)
    {
        return selectedAppearance.TryGetValue(category, out var option) ? option : null;
    }

    public Dictionary<AppearanceCategory, AppearanceOptionData> GetAllSelectedAppearance() => selectedAppearance;

    public void SelectColor(ColorCategory category, Color color)
    {
        selectedColors[category] = color;
        SyncBuildData();
        onAppearanceChanged?.Invoke();
        UpdateNavigationButtons();
    }

    public Dictionary<ColorCategory, Color> GetAllSelectedColors() => selectedColors;

    // ---------- NOME, PRONOME E JSON ----------
    public void SetCharacterIdentity(string characterName, string pronoun)
    {
        currentBuild.characterName = characterName;
        currentBuild.pronoun = pronoun;
        UpdateNavigationButtons();
    }

    public void SyncBuildData()
    {
        currentBuild.raceName = selectedRace != null ? selectedRace.raceName : string.Empty;
        currentBuild.className = selectedClass != null ? selectedClass.className : string.Empty;

        currentBuild.attributes.Clear();
        foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
        {
            int baseVal = GetBaseAttributeValue(attr);
            int allocVal = GetAllocatedAttributeValue(attr);
            currentBuild.attributes.Add(new AttributeSaveData
            {
                type = attr,
                baseValue = baseVal,
                allocatedValue = allocVal,
                totalValue = baseVal + allocVal
            });
        }

        currentBuild.abilities.Clear();
        if (selectedRace != null && selectedRace.racialAbilities != null)
        {
            foreach (var ab in selectedRace.racialAbilities)
                if (ab != null) currentBuild.abilities.Add(ab.name);
        }
        if (selectedClass != null && selectedClass.classAbilities != null)
        {
            foreach (var ab in selectedClass.classAbilities)
                if (ab != null) currentBuild.abilities.Add(ab.name);
        }

        currentBuild.appearance.Clear();
        foreach (var pair in selectedAppearance)
        {
            currentBuild.appearance.Add(new SerializableAppearance
            {
                category = pair.Key,
                selectedOptionId = string.IsNullOrEmpty(pair.Value.optionId) ? pair.Value.name : pair.Value.optionId,
                selectedOptionName = pair.Value.optionName
            });
        }

        currentBuild.colors.Clear();
        foreach (var pair in selectedColors)
        {
            currentBuild.colors.Add(new SerializableColor
            {
                category = pair.Key,
                colorHex = "#" + ColorUtility.ToHtmlStringRGB(pair.Value)
            });
        }
    }

    public void FinalizeCharacter()
    {
        SyncBuildData();
        string json = JsonUtility.ToJson(currentBuild, true);

        // Mantém salvo no PlayerPrefs para fácil leitura entre cenas
        PlayerPrefs.SetString("LastCharacter", json);
        PlayerPrefs.Save();

        // Garante que a pasta de destino exista
        string folder = GetSaveFolderPath();
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Define o nome do arquivo (limpando caracteres inválidos como / \ : * ?)
        string fileName = (useCharacterNameAsFileName && !string.IsNullOrWhiteSpace(currentBuild.characterName))
            ? SanitizeFileName(currentBuild.characterName)
            : "personagem";

        string filePath = Path.Combine(folder, fileName + ".json");
        File.WriteAllText(filePath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log("Arquivo JSON salvo em: " + filePath + "\n" + json);
    }
    [Header("Configuração de Salvamento")]
    public SaveLocation saveLocation = SaveLocation.ProjectAssets;
    [Tooltip("Preencha apenas se escolher CustomPath (Ex: C:/TCC/Personagens)")]
    public string customFolderPath = "";
    [Tooltip("Se marcado, usa o nome do personagem no arquivo (Ex: Arannis.json)")]
    public bool useCharacterNameAsFileName = true;
    private string GetSaveFolderPath()
    {
        switch (saveLocation)
        {
            case SaveLocation.ProjectAssets:
                return Path.Combine(Application.dataPath, "Saves");

            case SaveLocation.Desktop:
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            case SaveLocation.CustomPath:
                return !string.IsNullOrWhiteSpace(customFolderPath)
                    ? customFolderPath
                    : Application.persistentDataPath;

            default:
                return Application.persistentDataPath;
        }
    }

    private string SanitizeFileName(string rawName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            rawName = rawName.Replace(c, '_');
        return rawName.Trim();
    }
}