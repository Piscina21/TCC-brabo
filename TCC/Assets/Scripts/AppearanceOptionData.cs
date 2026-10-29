using UnityEngine;

public abstract class AppearanceOptionData : ScriptableObject
{
    public string optionName;
    public Sprite previewIcon; // Ícone para o botão da UI
}
[CreateAssetMenu(fileName = "NewHairStyle", menuName = "RPG/Appearance/Hair Style")]
public class HairStyleData : AppearanceOptionData
{
    public GameObject hairPrefab; // Prefab do cabelo (com SkinnedMeshRenderer)
}

[CreateAssetMenu(fileName = "NewFace", menuName = "RPG/Appearance/Face")]
public class FaceData : AppearanceOptionData
{
    public GameObject facePrefab; // Prefab do rosto (BlendShapes ou mesh)
}

[CreateAssetMenu(fileName = "NewSkinColor", menuName = "RPG/Appearance/Skin Color")]
public class SkinColorData : AppearanceOptionData
{
    public Material skinMaterial; // Material com a cor da pele
}

[CreateAssetMenu(fileName = "NewGender", menuName = "RPG/Appearance/Gender")]
public class GenderData : AppearanceOptionData
{
    public GameObject bodyPrefab; // Prefab do corpo base
}