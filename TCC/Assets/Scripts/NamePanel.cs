using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public InputField nameInputField;
    public Dropdown pronounDropdown;

    [Header("Opções de Pronomes")]
    public List<string> availablePronouns = new List<string>
    {
        "Ele / Dele",
        "Ela / Dela",
        "Elu / Delu"
    };

    private void Awake()
    {
        SetupDropdown();
    }

    private void OnEnable()
    {
        if (nameInputField != null)
            nameInputField.onValueChanged.AddListener(OnNameChanged);

        if (pronounDropdown != null)
            pronounDropdown.onValueChanged.AddListener(OnPronounChanged);

        // Garante que o pronome inicial já fique salvo mesmo sem o jogador abrir o Dropdown
        SyncIdentity();
    }

    private void OnDisable()
    {
        if (nameInputField != null)
            nameInputField.onValueChanged.RemoveListener(OnNameChanged);

        if (pronounDropdown != null)
            pronounDropdown.onValueChanged.RemoveListener(OnPronounChanged);
    }

    private void SetupDropdown()
    {
        if (pronounDropdown == null || availablePronouns.Count == 0) return;

        pronounDropdown.ClearOptions();
        pronounDropdown.AddOptions(availablePronouns);
    }

    private void OnNameChanged(string _) => SyncIdentity();
    private void OnPronounChanged(int _) => SyncIdentity();

    private void SyncIdentity()
    {
        if (manager == null) return;

        string charName = nameInputField != null ? nameInputField.text.Trim() : string.Empty;
        string selectedPronoun = (pronounDropdown != null && pronounDropdown.options.Count > 0)
            ? pronounDropdown.options[pronounDropdown.value].text
            : string.Empty;

        manager.SetCharacterIdentity(charName, selectedPronoun);
    }
}