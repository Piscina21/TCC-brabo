using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearanceController : MonoBehaviour
{
    [Serializable]
    public struct AppearanceAnchor
    {
        public AppearanceCategory category;
        public Transform anchorTransform;
        public Vector3 positionOffset; // Cole aqui o Position que encaixou no rosto
        public Vector3 rotationOffset; // Cole aqui o Rotation (se precisar)
    }

    [Serializable]
    public struct CategoryColorLink
    {
        public AppearanceCategory appearanceCategory;
        public ColorCategory colorCategory;
    }

    [Header("Referências")]
    public CharacterCreatorManager manager;

    [Header("Malhas do Corpo Base (Pele)")]
    public List<Renderer> bodySkinRenderers = new List<Renderer>();

    [Header("Pontos de Encaixe (Ossos)")]
    public List<AppearanceAnchor> anchors = new List<AppearanceAnchor>();

    [Header("Qual cor cada parte usa?")]
    public List<CategoryColorLink> colorLinks = new List<CategoryColorLink>();

    private readonly Dictionary<AppearanceCategory, GameObject> spawnedParts = new Dictionary<AppearanceCategory, GameObject>();
    private MaterialPropertyBlock propBlock;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        if (manager != null)
            manager.onAppearanceChanged.AddListener(RefreshCharacterVisuals);
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.onAppearanceChanged.RemoveListener(RefreshCharacterVisuals);
    }

    public void RefreshCharacterVisuals()
    {
        if (manager == null) return;

        var selections = manager.GetAllSelectedAppearance();
        var colors = manager.GetAllSelectedColors();

        // 1. Instancia e posiciona as peças 3D
        foreach (var anchor in anchors)
        {
            if (!selections.TryGetValue(anchor.category, out AppearanceOptionData selectedOption))
                continue;

            if (!spawnedParts.TryGetValue(anchor.category, out GameObject currentObj) ||
                currentObj == null || currentObj.name != selectedOption.optionName)
            {
                if (currentObj != null) Destroy(currentObj);

                if (selectedOption.prefab != null && anchor.anchorTransform != null)
                {
                    GameObject newPart = Instantiate(selectedOption.prefab, anchor.anchorTransform);
                    newPart.name = selectedOption.optionName;
                    newPart.transform.localPosition = anchor.positionOffset;
                    newPart.transform.localRotation = Quaternion.Euler(anchor.rotationOffset);
                    spawnedParts[anchor.category] = newPart;
                }
            }
        }

        // 2. Aplica cor de pele no corpo base
        if (colors.TryGetValue(ColorCategory.Skin, out Color skinColor))
        {
            foreach (var bodyRenderer in bodySkinRenderers)
            {
                ApplyColorToObject(bodyRenderer != null ? bodyRenderer.gameObject : null, skinColor);
            }
        }

        // 3. Aplica cores nas peças faciais/cabelo
        foreach (var partPair in spawnedParts)
        {
            if (partPair.Value == null) continue;

            ColorCategory targetColorCat = GetLinkedColorCategory(partPair.Key);
            if (colors.TryGetValue(targetColorCat, out Color partColor))
            {
                ApplyColorToObject(partPair.Value, partColor);
            }
        }
    }

    private ColorCategory GetLinkedColorCategory(AppearanceCategory appCat)
    {
        foreach (var link in colorLinks)
        {
            if (link.appearanceCategory == appCat)
                return link.colorCategory;
        }
        return ColorCategory.Skin;
    }

    private void ApplyColorToObject(GameObject target, Color color)
    {
        if (target == null) return;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor(BaseColorID, color);
            rend.SetPropertyBlock(propBlock);
        }
    }
}