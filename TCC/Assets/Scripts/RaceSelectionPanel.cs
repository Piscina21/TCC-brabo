using UnityEngine;
using UnityEngine.UI;

public class RaceSelectionPanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public Transform raceButtonContainer;
    public GameObject raceButtonPrefab;
    public Text raceDescriptionText;
    public Image racePreviewImage;

    [Header("Dados")]
    public RaceData[] availableRaces;   // preencher no Inspector com seus assets

    private void OnEnable()
    {
        PopulateRaceButtons();
        manager.onRaceChanged.AddListener(UpdateDescription);
    }

    private void OnDisable()
    {
        manager.onRaceChanged.RemoveListener(UpdateDescription);
    }

    void PopulateRaceButtons()
    {
        // Limpa botões antigos (caso o painel seja reaberto)
        foreach (Transform child in raceButtonContainer)
            Destroy(child.gameObject);

        // Cria um botão para cada raça
        foreach (RaceData race in availableRaces)
        {
            GameObject btnObj = Instantiate(raceButtonPrefab, raceButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();

            if (btnText != null)
                btnText.text = race.raceName;

            // Ícone (opcional): se houver um componente Image, trocar o sprite
            // Image img = btnObj.GetComponent<Image>();
            // if (img != null && race.icon != null) img.sprite = race.icon;

            // Ao clicar, informa o Manager qual raça foi escolhida
           Debug.Log("Botão da raça clicado: " + race.raceName);
            btn.onClick.AddListener(() => manager.SelectRace(race));
        }
    }

   void UpdateDescription()
{
    Debug.Log("UpdateDescription chamado");
    RaceData selected = manager.GetSelectedRace();
    if (selected != null)
    {
        raceDescriptionText.text = selected.description;
        if (racePreviewImage != null)
        {
            racePreviewImage.sprite = selected.icon;
            racePreviewImage.enabled = selected.icon != null;
        }
    }
}
}