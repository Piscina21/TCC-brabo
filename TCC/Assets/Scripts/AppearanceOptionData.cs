using UnityEngine;

public abstract class AppearanceOptionData : ScriptableObject
{
    public string optionName;
    public Sprite previewIcon;
}

[CreateAssetMenu(fileName = "NewHairStyle", menuName = "RPG/Appearance/Hair Style")]
public class HairStyleData : AppearanceOptionData
{
    public GameObject hairPrefab;
}

[CreateAssetMenu(fileName = "NewFace", menuName = "RPG/Appearance/Face")]
public class FaceData : AppearanceOptionData
{
    public GameObject facePrefab;
}

[CreateAssetMenu(fileName = "NewEyebrow", menuName = "RPG/Appearance/Eyebrow")]
public class BrowData : AppearanceOptionData
{
    public GameObject browPrefab;
}

[CreateAssetMenu(fileName = "NewNose", menuName = "RPG/Appearance/Nose")]
public class NoseData : AppearanceOptionData
{
    public GameObject nosePrefab;
}

[CreateAssetMenu(fileName = "NewEar", menuName = "RPG/Appearance/Ear")]
public class EarData : AppearanceOptionData
{
    public GameObject earPrefab;
}

[CreateAssetMenu(fileName = "NewEyes", menuName = "RPG/Appearance/Eyes")]
public class EyesData : AppearanceOptionData
{
    public GameObject eyesPrefab;
}

[CreateAssetMenu(fileName = "NewBeard", menuName = "RPG/Appearance/Beard")]
public class BeardData : AppearanceOptionData
{
    public GameObject beardPrefab;
}