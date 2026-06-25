using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;

    // Textos que mostram o valor atual de cada atributo
    public Text strengthValueText;
    public Text dexterityValueText;
    public Text intelligenceValueText;
    public Text constitutionValueText;
    public Text wisdomValueText;
    public Text charismaValueText;

    // Texto que mostra os pontos restantes
    public Text remainingPointsText;

    // Botões (opcional, se quiser desabilitar quando zerar pontos)
    // mas para simplicidade, vamos só atualizar os textos

    private void OnEnable()
    {
        manager.onAttributesChanged.AddListener(UpdateUI);
        UpdateUI();
    }

    private void OnDisable()
    {
        manager.onAttributesChanged.RemoveListener(UpdateUI);
    }

    // Métodos chamados pelos botões + e - de cada atributo
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

    void UpdateUI()
    {
        strengthValueText.text = manager.GetAttributeValue(AttributeType.Strength).ToString();
        dexterityValueText.text = manager.GetAttributeValue(AttributeType.Dexterity).ToString();
        intelligenceValueText.text = manager.GetAttributeValue(AttributeType.Intelligence).ToString();
        constitutionValueText.text = manager.GetAttributeValue(AttributeType.Constitution).ToString();
        wisdomValueText.text = manager.GetAttributeValue(AttributeType.Wisdom).ToString();
        charismaValueText.text = manager.GetAttributeValue(AttributeType.Charisma).ToString();

        remainingPointsText.text = "Pontos restantes: " + manager.GetRemainingPoints();
    }
}