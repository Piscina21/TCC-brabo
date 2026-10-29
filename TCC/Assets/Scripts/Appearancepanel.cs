using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AppearancePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public CharacterAppearanceController appearanceController;

    [Header("Categorias (Listas de ScriptableObjects)")]
    public List<HairStyleData> availableHairStyles;
    public List<FaceData> availableFaces;
    public List<SkinColorData> availableSkinColors;
    public List<GenderData> availableGenders;

    [Header("Contêineres de Botões na UI")]
    public Transform hairButtonContainer;
    public Transform faceButtonContainer;
    public Transform skinButtonContainer;
    public Transform genderButtonContainer;

    [Header("Prefab de Botão Genérico")]
    public GameObject optionButtonPrefab;

    private void OnEnable()
    {
        PopulateButtons(hairButtonContainer, availableHairStyles, (data) => {
            appearanceController.ApplyHair(data);
            manager.SetHairStyleIndex(availableHairStyles.IndexOf(data)); // guarda a escolha
        });

        PopulateButtons(faceButtonContainer, availableFaces, (data) => {
            appearanceController.ApplyFace(data);
        });

        PopulateButtons(skinButtonContainer, availableSkinColors, (data) => {
            appearanceController.ApplySkinColor(data);
            manager.SetSkinColorIndex(availableSkinColors.IndexOf(data));
        });

        PopulateButtons(genderButtonContainer, availableGenders, (data) => {
            appearanceController.ApplyGender(data);
        });
    }

    // Método genérico que cria botões para qualquer lista
    private void PopulateButtons<T>(Transform container, List<T> options, System.Action<T> onSelected) where T : AppearanceOptionData
    {
        // Limpa botões antigos
        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (T option in options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, container);
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = option.optionName;

            // Configura ícone se houver
            Image icon = btnObj.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && option.previewIcon != null) icon.sprite = option.previewIcon;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => onSelected(option));
        }
    }
}