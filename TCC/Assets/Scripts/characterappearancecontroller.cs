using UnityEngine;

public class CharacterAppearanceController : MonoBehaviour
{
    [Header("Renderer do Corpo Base (para o esqueleto)")]
    public SkinnedMeshRenderer bodyRenderer;

    [Header("Anchors (Pontos de encaixe no modelo)")]
    public Transform hairAnchor;
    public Transform faceAnchor;
    public Transform browAnchor;
    public Transform noseAnchor;
    public Transform earAnchor;
    public Transform eyesAnchor;
    public Transform beardAnchor;

    // Instâncias ativas
    private GameObject currentHair;
    private GameObject currentFace;
    private GameObject currentBrow;
    private GameObject currentNose;
    private GameObject currentEar;
    private GameObject currentEyes;
    private GameObject currentBeard;

    // --- Métodos públicos chamados pelo painel ---
    public void ApplyHair(HairStyleData data) { currentHair = Replace(currentHair, data != null ? data.hairPrefab : null, hairAnchor); }
    public void ApplyFace(FaceData data) { currentFace = Replace(currentFace, data != null ? data.facePrefab : null, faceAnchor); }
    public void ApplyBrow(BrowData data) { currentBrow = Replace(currentBrow, data != null ? data.browPrefab : null, browAnchor); }
    public void ApplyNose(NoseData data) { currentNose = Replace(currentNose, data != null ? data.nosePrefab : null, noseAnchor); }
    public void ApplyEar(EarData data) { currentEar = Replace(currentEar, data != null ? data.earPrefab : null, earAnchor); }
    public void ApplyEyes(EyesData data) { currentEyes = Replace(currentEyes, data != null ? data.eyesPrefab : null, eyesAnchor); }
    public void ApplyBeard(BeardData data) { currentBeard = Replace(currentBeard, data != null ? data.beardPrefab : null, beardAnchor); }

    // --- Lógica genérica de troca ---
    private GameObject Replace(GameObject current, GameObject prefab, Transform anchor)
    {
        if (current != null) Destroy(current);
        if (prefab == null || anchor == null) return null;

        GameObject instance = Instantiate(prefab, anchor);

        // Se a peça for SkinnedMeshRenderer, herda o esqueleto do corpo
        SkinnedMeshRenderer smr = instance.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null && bodyRenderer != null)
        {
            smr.rootBone = bodyRenderer.rootBone;
            smr.bones = bodyRenderer.bones;
        }

        return instance;
    }
}