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
    public ClassData[] availableClasses;   // preencher no Inspector com seus assets de classe

    private void OnEnable()
    {
        PopulateClassButtons();
        manager.onClassChanged.AddListener(UpdateDescription);
    }

    private void OnDisable()
    {
        manager.onClassChanged.RemoveListener(UpdateDescription);
    }

    void PopulateClassButtons()
    {
        // Limpa botões antigos
        foreach (Transform child in classButtonContainer)
            Destroy(child.gameObject);

        // Cria um botão para cada classe
        foreach (ClassData classData in availableClasses)
        {
            GameObject btnObj = Instantiate(classButtonPrefab, classButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();

            if (btnText != null)
                btnText.text = classData.className;

            btn.onClick.AddListener(() => manager.SelectClass(classData));
        }
    }

    void UpdateDescription()
    {
        ClassData selected = manager.GetSelectedClass(); // precisaremos criar esse getter
        if (selected != null)
            classDescriptionText.text = selected.description;
        else
            classDescriptionText.text = "";
    }
}