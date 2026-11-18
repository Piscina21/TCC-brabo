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

    public bool IsAllowedForRace(RaceData race)
    {
        // Se nenhuma raça específica foi definida na lista, libera para todas
        if (race == null || allowedRaces == null || allowedRaces.Count == 0)
            return true;

        return allowedRaces.Contains(race);
    }
}

[System.Serializable]
public class AttributeBaseValue
{
    public AttributeType attribute;
    public int baseValue;
}