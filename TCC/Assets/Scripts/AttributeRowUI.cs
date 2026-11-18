// AttributeRowUI.cs (Coloque no Prefab da linha de atributo)
using UnityEngine;
using UnityEngine.UI;

public class AttributeRowUI : MonoBehaviour
{
    public Text attributeNameText;
    public Text valueText;
    public Button increaseButton;
    public Button decreaseButton;

    private AttributeType attributeType;
    private CharacterCreatorManager manager;

    public void Setup(AttributeType type, CharacterCreatorManager creatorManager)
    {
        attributeType = type;
        manager = creatorManager;

        if (attributeNameText != null)
            attributeNameText.text = type.ToString();

        increaseButton.onClick.RemoveAllListeners();
        increaseButton.onClick.AddListener(() => manager.IncreaseAttribute(attributeType));

        decreaseButton.onClick.RemoveAllListeners();
        decreaseButton.onClick.AddListener(() => manager.DecreaseAttribute(attributeType));

        Refresh();
    }

    public void Refresh()
    {
        if (manager == null) return;

        int total = manager.GetTotalAttributeValue(attributeType);
        int allocated = manager.GetAllocatedAttributeValue(attributeType);

        if (valueText != null)
            valueText.text = total.ToString();

        // Desativa os botões automaticamente nos limites válidos
        if (increaseButton != null)
            increaseButton.interactable = manager.GetRemainingPoints() > 0;

        if (decreaseButton != null)
            decreaseButton.interactable = allocated > 0;
    }
}