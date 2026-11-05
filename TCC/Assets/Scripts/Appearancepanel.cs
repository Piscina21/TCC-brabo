using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AppearancePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public CharacterAppearanceController appearanceController;
    public GameObject optionButtonPrefab;

    [Header("Categorias (preencher no Inspector)")]
    public List<HairStyleData> availableHairStyles;
    public List<FaceData> availableFaces;
    public List<BrowData> availableBrows;
    public List<NoseData> availableNoses;
    public List<EarData> availableEars;
    public List<EyesData> availableEyes;
    public List<BeardData> availableBeards;

    [Header("Contêineres de Botões")]
    public Transform hairButtonContainer;
    public Transform faceButtonContainer;
    public Transform browButtonContainer;
    public Transform noseButtonContainer;
    public Transform earButtonContainer;
    public Transform eyesButtonContainer;
    public Transform beardButtonContainer;

    private void OnEnable()
    {
        Populate(hairButtonContainer, availableHairStyles, d => appearanceController.ApplyHair(d));
        Populate(faceButtonContainer, availableFaces, d => appearanceController.ApplyFace(d));
        Populate(browButtonContainer, availableBrows, d => appearanceController.ApplyBrow(d));
        Populate(noseButtonContainer, availableNoses, d => appearanceController.ApplyNose(d));
        Populate(earButtonContainer, availableEars, d => appearanceController.ApplyEar(d));
        Populate(eyesButtonContainer, availableEyes, d => appearanceController.ApplyEyes(d));
        Populate(beardButtonContainer, availableBeards, d => appearanceController.ApplyBeard(d));
    }

    private void Populate<T>(Transform container, List<T> options, System.Action<T> onSelected) where T : AppearanceOptionData
    {
        if (container == null || options == null) return;

        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (T option in options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, container);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = option.optionName;

            Image icon = btnObj.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && option.previewIcon != null)
            {
                icon.sprite = option.previewIcon;
                icon.enabled = true;
            }

            T captured = option; // evita bug de closure
            btn.onClick.AddListener(() => onSelected(captured));
        }
    }
}