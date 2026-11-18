using UnityEngine;
using UnityEngine.UI;

public class ClassSelectionPanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public Transform classButtonContainer;
    public GameObject classButtonPrefab;
    public Text classDescriptionText;

    [Header("Dados")]
    public ClassData[] availableClasses;

    [Header("Comportamento de Restrição")]
    [Tooltip("Se marcado, esconde classes proibidas. Se desmarcado, mantém o botão visível mas desativado.")]
    public bool hideRestrictedClasses = false;

    private void OnEnable()
    {
        PopulateClassButtons();
        manager.onClassChanged.AddListener(UpdateDescription);
        UpdateDescription();
    }

    private void OnDisable()
    {
        manager.onClassChanged.RemoveListener(UpdateDescription);
    }

    private void PopulateClassButtons()
    {
        foreach (Transform child in classButtonContainer)
            Destroy(child.gameObject);

        RaceData currentRace = manager != null ? manager.GetSelectedRace() : null;

        foreach (ClassData classData in availableClasses)
        {
            if (classData == null) continue;

            bool isAllowed = classData.IsAllowedForRace(currentRace);

            if (!isAllowed && hideRestrictedClasses)
                continue;

            GameObject btnObj = Instantiate(classButtonPrefab, classButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();

            if (btnText != null)
            {
                btnText.text = isAllowed
                    ? classData.className
                    : $"{classData.className} (Indisponível)";
            }

            if (btn != null)
            {
                btn.interactable = isAllowed;
                if (isAllowed)
                {
                    ClassData capturedClass = classData;
                    btn.onClick.AddListener(() => manager.SelectClass(capturedClass));
                }
            }
        }
    }

    private void UpdateDescription()
    {
        ClassData selected = manager.GetSelectedClass();
        if (selected != null)
            classDescriptionText.text = selected.description;
        else
            classDescriptionText.text = "";
    }
}