// CharacterCreatorManager.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CharacterCreatorManager : MonoBehaviour
{
    [Header("Steps Panels")]
    public GameObject raceSelectionPanel;
    public GameObject classSelectionPanel;
    public GameObject attributePanel;
    public GameObject appearancePanel;
    public GameObject namePanel;
    public GameObject confirmPanel;

    [Header("Events")]
    public UnityEvent onRaceChanged;
    public UnityEvent onClassChanged;
    public UnityEvent onAttributesChanged;
    public UnityEvent onAppearanceChanged;
    [Header("Botões de Navegação")]
    public UnityEngine.UI.Button nextButton;
    public UnityEngine.UI.Button previousButton;

    // Personagem em construção
    public CharacterBuild currentBuild = new CharacterBuild();

    // Pontos extras para distribuir (ex: 10 pontos)
    public int pointsToDistribute = 10;
    private int remainingPoints;

    private void Start()
    {
        ShowPanel(raceSelectionPanel);
        remainingPoints = pointsToDistribute;

        foreach (AttributeType attr in System.Enum.GetValues(typeof(AttributeType)))
        {
            currentBuild.attributes[attr] = 0;
        }
    }

    // ---------- FLUXO DE ETAPAS ----------
    private void UpdateNextButtonState()
    {
        if (nextButton != null)
            nextButton.interactable = CanProceed();
    }
    private void UpdatePreviousButtonState()
    {
        if (previousButton != null)
            previousButton.interactable = CanGoBack();
    }

    public void GoToNextStep()
    {
        if (!CanProceed())
        {
            Debug.Log("Não é possível avançar: seleção obrigatória pendente.");
            return;
        }

        if (raceSelectionPanel.activeSelf)
        {
            ShowPanel(classSelectionPanel);
        }
        else if (classSelectionPanel.activeSelf)
        {
            ApplyRaceAndClassBonuses();
            remainingPoints = pointsToDistribute;
            onAttributesChanged.Invoke();
            ShowPanel(attributePanel);
        }
        else if (attributePanel.activeSelf)
        {
            ShowPanel(appearancePanel);
        }
        else if (appearancePanel.activeSelf)
        {
            ShowPanel(namePanel);
        }
        else if (namePanel.activeSelf)
        {
            ShowPanel(confirmPanel);
        }
    }
    public bool CanProceed()
    {
        // Raça
        if (raceSelectionPanel.activeSelf)
            return currentBuild.selectedRace != null;

        // Classe
        if (classSelectionPanel.activeSelf)
            return currentBuild.selectedClass != null;

        // Atributos: sempre pode avançar (ou exigir que todos os pontos sejam gastos)
        if (attributePanel.activeSelf)
            return true; // troque por: return remainingPoints == 0; se quiser exigir gasto total

        // Aparência: sempre pode
        if (appearancePanel.activeSelf)
            return true;

        // Nome: exige nome não vazio
        if (namePanel.activeSelf)
            return !string.IsNullOrWhiteSpace(currentBuild.characterName);

        // Confirmação: já é a última etapa
        if (confirmPanel.activeSelf)
            return true;

        return false;
    }
  

    // ---------- SELEÇÕES ----------
    public void SelectRace(RaceData race)
    {
        currentBuild.selectedRace = race;
        onRaceChanged.Invoke();
        UpdateNextButtonState();  // <-- adicionar
    }

    public void SelectClass(ClassData classData)
    {
        currentBuild.selectedClass = classData;
        onClassChanged.Invoke();
        UpdateNextButtonState();  // <-- adicionar
    }


    // Aplicar bônus raciais + classe base aos atributos
    private void ApplyRaceAndClassBonuses()
    {
        // Limpa atributos — iterando sobre o enum, não sobre o Dictionary
        foreach (AttributeType attr in System.Enum.GetValues(typeof(AttributeType)))
        {
            currentBuild.attributes[attr] = 0;
        }

        // Aplica base da classe
        if (currentBuild.selectedClass != null)
        {
            foreach (var baseAttr in currentBuild.selectedClass.baseAttributes)
            {
                currentBuild.attributes[baseAttr.attribute] += baseAttr.baseValue;
            }
        }

        // Aplica bônus raciais
        if (currentBuild.selectedRace != null)
        {
            foreach (var bonus in currentBuild.selectedRace.baseBonuses)
            {
                currentBuild.attributes[bonus.attribute] += bonus.bonusValue;
            }
        }

        // Combina habilidades
        currentBuild.abilities.Clear();
        if (currentBuild.selectedRace != null)
            currentBuild.abilities.AddRange(currentBuild.selectedRace.racialAbilities);
        if (currentBuild.selectedClass != null)
            currentBuild.abilities.AddRange(currentBuild.selectedClass.classAbilities);
    }

    // Distribuir pontos durante a etapa de atributos
    public int maxAttributeValue = 20;

    public void IncreaseAttribute(AttributeType attr)
    {
        if (remainingPoints > 0 && currentBuild.attributes[attr] < maxAttributeValue)
        {
            currentBuild.attributes[attr]++;
            remainingPoints--;
            onAttributesChanged.Invoke();
        }
    }

    [Header("Limites de Atributo")]
    public int minAttributeValue = -5;  // valor mínimo que o jogador pode atingir

    public void DecreaseAttribute(AttributeType attr)
    {
        // Só impede de ir abaixo do mínimo configurado
        if (currentBuild.attributes[attr] > minAttributeValue)
        {
            currentBuild.attributes[attr]--;
            remainingPoints++;
            onAttributesChanged.Invoke();
        }
    }

    public int GetAttributeValue(AttributeType attr)
    {
        return currentBuild.attributes.ContainsKey(attr) ? currentBuild.attributes[attr] : 0;
    }

    public int GetRemainingPoints() => remainingPoints;

    // ---------- NOME E FINALIZAÇÃO ----------
    public void SetCharacterName(string name)
    {
        currentBuild.characterName = name;
        UpdateNextButtonState();  // <-- adicionar
    }

    public void FinalizeCharacter()
    {
        // Salva o personagem (exemplo com JSON)
        string json = JsonUtility.ToJson(currentBuild);
        PlayerPrefs.SetString("LastCharacter", json);
        PlayerPrefs.Save();
        Debug.Log("Personagem salvo:\n" + json);
        // Aqui você pode carregar a cena do jogo ou um resumo.
    }

    private void ShowPanel(GameObject panel)
    {
        if (raceSelectionPanel != null) raceSelectionPanel.SetActive(false);
        if (classSelectionPanel != null) classSelectionPanel.SetActive(false);
        if (attributePanel != null) attributePanel.SetActive(false);
        if (appearancePanel != null) appearancePanel.SetActive(false);
        if (namePanel != null) namePanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        if (panel != null) panel.SetActive(true);
        UpdateNextButtonState();
        UpdatePreviousButtonState();
    }

    // ---------- NAVEGAÇÃO PARA TRÁS ----------
    public bool CanGoBack()
    {
        // Na primeira etapa (raça) não há para onde voltar
        if (raceSelectionPanel.activeSelf)
            return false;
        return true;
    }

    public void GoToPreviousStep()
    {
        Debug.Log("GoToPreviousStep chamado. CanGoBack = " + CanGoBack());
        if (!CanGoBack()) return;

        if (classSelectionPanel.activeSelf) Debug.Log("Voltando de Classe para Raça");
        else if (attributePanel.activeSelf) Debug.Log("Voltando de Atributos para Classe");
        else if (appearancePanel.activeSelf) Debug.Log("Voltando de Aparência para Atributos");
        else if (namePanel.activeSelf) Debug.Log("Voltando de Nome para Aparência");
        else if (confirmPanel.activeSelf) Debug.Log("Voltando de Confirmação para Nome");

        if (classSelectionPanel.activeSelf) ShowPanel(raceSelectionPanel);
        else if (attributePanel.activeSelf) ShowPanel(classSelectionPanel);
        else if (appearancePanel.activeSelf) ShowPanel(attributePanel);
        else if (namePanel.activeSelf) ShowPanel(appearancePanel);
        else if (confirmPanel.activeSelf) ShowPanel(namePanel);
    }
    public RaceData GetSelectedRace()
{
    // Se você armazena a referência direta no CharacterBuild:
    return currentBuild.selectedRace;
    
    // Se você armazena só o nome (string), precisaria buscar na lista de raças.
    // Mas por enquanto, use referência direta.

}
public ClassData GetSelectedClass()
{
    return currentBuild.selectedClass;   // exige que currentBuild tenha o campo ClassData
}
    // ---------- APARÊNCIA ----------
    public int GetHairStyleIndex() => currentBuild.hairStyleIndex;
    public void SetHairStyleIndex(int index)
    {
        currentBuild.hairStyleIndex = index;
        onAppearanceChanged.Invoke();
    }

    public int GetSkinColorIndex() => currentBuild.skinColorIndex;
    public void SetSkinColorIndex(int index)
    {
        currentBuild.skinColorIndex = index;
        onAppearanceChanged.Invoke();
    }

    // ---------- NOME ----------
    public string GetCharacterName() => currentBuild.characterName;

}
