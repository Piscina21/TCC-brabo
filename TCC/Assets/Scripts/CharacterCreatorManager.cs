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

    // Personagem em construção
    public CharacterBuild currentBuild = new CharacterBuild();

    // Pontos extras para distribuir (ex: 10 pontos)
    public int pointsToDistribute = 10;
    private int remainingPoints;

    private void Start()
    {
        // Inicia na primeira etapa
        ShowPanel(raceSelectionPanel);
        remainingPoints = pointsToDistribute;
        // Inicializa dicionário de atributos com 0 ou valores padrão
        foreach (AttributeType attr in System.Enum.GetValues(typeof(AttributeType)))
        {
            currentBuild.attributes[attr] = 0;
        }
    }

    // ---------- FLUXO DE ETAPAS ----------
    public void GoToNextStep()
    {
        // Lógica simples: desativar painel atual e ativar o próximo.
        // (Pode ser uma fila de GameObjects para facilitar)
        // Exemplo simplificado:
        if (raceSelectionPanel.activeSelf)
        {
            ShowPanel(classSelectionPanel);
        }
        else if (classSelectionPanel.activeSelf)
        {
            // Aplica os bônus raciais + base da classe nos atributos
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
        Debug.Log("Avançando para: " + (classSelectionPanel != null ? classSelectionPanel.name : "null"));
    }

    // ---------- SELEÇÕES ----------
public void SelectRace(RaceData race)
{
    currentBuild.selectedRace = race;
    onRaceChanged.Invoke(); // ESSENCIAL
    Debug.Log("Raça selecionada: " + (race != null ? race.raceName : "null")); // Adicione para depurar
}

    public void SelectClass(ClassData classData)
    {
        currentBuild.selectedClass = classData;
        onClassChanged.Invoke();
    }

    // Aplicar bônus raciais + classe base aos atributos
    private void ApplyRaceAndClassBonuses()
    {
        // Limpa atributos
        foreach (AttributeType attr in currentBuild.attributes.Keys)
            currentBuild.attributes[attr] = 0;

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
    public void IncreaseAttribute(AttributeType attr)
    {
        if (remainingPoints > 0)
        {
            currentBuild.attributes[attr]++;
            remainingPoints--;
            onAttributesChanged.Invoke();
        }
    }

    public void DecreaseAttribute(AttributeType attr)
    {
        // Não pode reduzir abaixo do valor base (precisamos guardar o base)
        // Para simplificar, vamos só impedir de ficar < 0
        if (currentBuild.attributes[attr] > 0 && remainingPoints < pointsToDistribute)
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
        // Desativa todos e ativa o escolhido
        raceSelectionPanel.SetActive(false);
        classSelectionPanel.SetActive(false);
        attributePanel.SetActive(false);
        appearancePanel.SetActive(false);
        namePanel.SetActive(false);
        confirmPanel.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
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

}