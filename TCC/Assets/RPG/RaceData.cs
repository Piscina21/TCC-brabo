using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRace", menuName = "RPG/Race Data")]
public class RaceData : ScriptableObject
{
    public string raceName;
    [TextArea] public string description;
    public Sprite icon;

    public List<AttributeBonus> baseBonuses;
    public List<AbilityData> racialAbilities;
}

[System.Serializable]
public class AttributeBonus
{
    public AttributeType attribute;
    public int bonusValue;
}