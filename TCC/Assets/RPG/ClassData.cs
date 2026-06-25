using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/Class Data")]
public class ClassData : ScriptableObject
{
    public string className;
    [TextArea] public string description;
    public Sprite icon;

    public List<AttributeBaseValue> baseAttributes;
    public List<AbilityData> classAbilities;
    public List<RaceData> allowedRaces;
}

[System.Serializable]
public class AttributeBaseValue
{
    public AttributeType attribute;
    public int baseValue;
}