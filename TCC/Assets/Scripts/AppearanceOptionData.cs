using UnityEngine;

[CreateAssetMenu(fileName = "NewAppearanceOption", menuName = "RPG/Appearance/Option")]
public class AppearanceOptionData : ScriptableObject
{
    [Tooltip("ID único salvo no JSON (ex: hair_short_01)")]
    public string optionId;
    public string optionName;
    public AppearanceCategory category;
    public Sprite previewIcon;
    public GameObject prefab;
}