using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;

    [Header("Textos de Valor dos Atributos")]
    public Text strengthValueText;
    public Text dexterityValueText;
    public Text intelligenceValueText;
    public Text constitutionValueText;
    public Text wisdomValueText;
    public Text charismaValueText;

    [Header("Texto de Pontos Restantes")]
    public Text remainingPointsText;

    private void OnEnable()
    {
        // Inscreve no evento para atualizar a UI sempre que um atributo mudar
        manager.onAttributesChanged.AddListener(UpdateUI);
        UpdateUI(); // atualiza imediatamente ao abrir o painel
    }

    private void OnDisable()
    {
        manager.onAttributesChanged.RemoveListener(UpdateUI);
    }

    // --- Métodos chamados pelos botões "+" e "-" de cada atributo ---

    public void IncreaseStrength() => manager.IncreaseAttribute(AttributeType.Strength);
    public void DecreaseStrength() => manager.DecreaseAttribute(AttributeType.Strength);

    public void IncreaseDexterity() => manager.IncreaseAttribute(AttributeType.Dexterity);
    public void DecreaseDexterity() => manager.DecreaseAttribute(AttributeType.Dexterity);

    public void IncreaseIntelligence() => manager.IncreaseAttribute(AttributeType.Intelligence);
    public void DecreaseIntelligence() => manager.DecreaseAttribute(AttributeType.Intelligence);

    public void IncreaseConstitution() => manager.IncreaseAttribute(AttributeType.Constitution);
    public void DecreaseConstitution() => manager.DecreaseAttribute(AttributeType.Constitution);

    public void IncreaseWisdom() => manager.IncreaseAttribute(AttributeType.Wisdom);
    public void DecreaseWisdom() => manager.DecreaseAttribute(AttributeType.Wisdom);

    public void IncreaseCharisma() => manager.IncreaseAttribute(AttributeType.Charisma);
    public void DecreaseCharisma() => manager.DecreaseAttribute(AttributeType.Charisma);

    // --- Atualiza todos os textos com os valores atuais ---

    void UpdateUI()
    {
        if (manager == null) { Debug.LogError("Manager não atribuído no AttributePanel!"); return; }

        if (strengthValueText != null)
            strengthValueText.text = manager.GetAttributeValue(AttributeType.Strength).ToString();
        else Debug.LogWarning("strengthValueText não atribuído!");

        if (dexterityValueText != null)
            dexterityValueText.text = manager.GetAttributeValue(AttributeType.Dexterity).ToString();
        else Debug.LogWarning("dexterityValueText não atribuído!");

        if (intelligenceValueText != null)
            intelligenceValueText.text = manager.GetAttributeValue(AttributeType.Intelligence).ToString();
        else Debug.LogWarning("intelligenceValueText não atribuído!");

        if (constitutionValueText != null)
            constitutionValueText.text = manager.GetAttributeValue(AttributeType.Constitution).ToString();
        else Debug.LogWarning("constitutionValueText não atribuído!");

        if (wisdomValueText != null)
            wisdomValueText.text = manager.GetAttributeValue(AttributeType.Wisdom).ToString();
        else Debug.LogWarning("wisdomValueText não atribuído!");

        if (charismaValueText != null)
            charismaValueText.text = manager.GetAttributeValue(AttributeType.Charisma).ToString();
        else Debug.LogWarning("charismaValueText não atribuído!");

        if (remainingPointsText != null)
            remainingPointsText.text = "Pontos restantes: " + manager.GetRemainingPoints();
        else Debug.LogWarning("remainingPointsText não atribuído!");
    }
}