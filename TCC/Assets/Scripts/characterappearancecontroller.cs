using UnityEngine;
using System.Collections.Generic;

public class CharacterAppearanceController : MonoBehaviour
{
    [Header("Anchors (Pontos de encaixe no esqueleto)")]
    public Transform hairAnchor;
    public Transform faceAnchor;

    [Header("Renderer do Corpo Base")]
    public SkinnedMeshRenderer bodyRenderer;

    // Referências às instâncias ativas
    private GameObject currentHair;
    private GameObject currentFace;
    private GameObject currentBody;

    // --- MÉTODOS PARA APLICAR CADA TIPO DE OPÇÃO ---

    public void ApplyHair(HairStyleData data)
    {
        if (currentHair != null) Destroy(currentHair);
        if (data != null && data.hairPrefab != null)
        {
            currentHair = Instantiate(data.hairPrefab, hairAnchor);
            // Garante que o cabelo compartilhe o mesmo esqueleto
            SetupSkinnedMesh(currentHair);
        }
    }

    public void ApplyFace(FaceData data)
    {
        if (currentFace != null) Destroy(currentFace);
        if (data != null && data.facePrefab != null)
        {
            currentFace = Instantiate(data.facePrefab, faceAnchor);
            SetupSkinnedMesh(currentFace);
        }
    }

    public void ApplySkinColor(SkinColorData data)
    {
        if (bodyRenderer != null && data != null && data.skinMaterial != null)
        {
            bodyRenderer.material = data.skinMaterial;
        }
    }

    public void ApplyGender(GenderData data)
    {
        // Exemplo: trocar o corpo base. Adapte conforme seu asset.
        if (currentBody != null) Destroy(currentBody);
        if (data != null && data.bodyPrefab != null)
        {
            currentBody = Instantiate(data.bodyPrefab, transform);
            bodyRenderer = currentBody.GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    // Configura o SkinnedMeshRenderer para usar o esqueleto do personagem
    private void SetupSkinnedMesh(GameObject part)
    {
        SkinnedMeshRenderer smr = part.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            smr.rootBone = bodyRenderer.rootBone;
            smr.bones = bodyRenderer.bones;
        }
    }
}