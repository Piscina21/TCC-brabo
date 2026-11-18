using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AppearancePanel : MonoBehaviour
{
    [Serializable]
    public struct CategoryContainer
    {
        public AppearanceCategory category;
        public Transform container;
    }

    [Serializable]
    public struct ColorPaletteConfig
    {
        public ColorCategory category;
        public Transform container;
        public List<Color> availableColors;
    }

    [Header("Referências")]
    public CharacterCreatorManager manager;
    public GameObject optionButtonPrefab; // Seu Prefab de botão com Text padrão
    public GameObject colorSwatchPrefab;  // Um Prefab simples de Button com Image branca

    [Header("Peças de Aparência")]
    public List<AppearanceOptionData> allAppearanceOptions = new List<AppearanceOptionData>();
    public List<CategoryContainer> categoryContainers = new List<CategoryContainer>();

    [Header("Paletas de Cores")]
    public List<ColorPaletteConfig> colorPalettes = new List<ColorPaletteConfig>();

    private void OnEnable()
    {
        PopulateParts();
        PopulateColors();
    }

    private void PopulateParts()
    {
        foreach (var catConfig in categoryContainers)
        {
            if (catConfig.container == null) continue;

            foreach (Transform child in catConfig.container)
                Destroy(child.gameObject);

            var options = allAppearanceOptions.Where(o => o != null && o.category == catConfig.category);

            foreach (var option in options)
            {
                GameObject btnObj = Instantiate(optionButtonPrefab, catConfig.container);
                Button btn = btnObj.GetComponent<Button>();
                Text btnText = btnObj.GetComponentInChildren<Text>();

                if (btnText != null)
                    btnText.text = option.optionName;

                AppearanceOptionData captured = option;
                btn.onClick.AddListener(() => manager.SelectAppearance(captured));
            }
        }
    }

    private void PopulateColors()
    {
        if (colorSwatchPrefab == null) return;

        foreach (var palette in colorPalettes)
        {
            if (palette.container == null) continue;

            foreach (Transform child in palette.container)
                Destroy(child.gameObject);

            foreach (Color col in palette.availableColors)
            {
                GameObject swatchObj = Instantiate(colorSwatchPrefab, palette.container);
                Button btn = swatchObj.GetComponent<Button>();
                Image img = swatchObj.GetComponent<Image>();

                if (img != null)
                    img.color = col;

                Color capturedColor = col;
                ColorCategory capturedCategory = palette.category;
                btn.onClick.AddListener(() => manager.SelectColor(capturedCategory, capturedColor));
            }
        }
    }
}
