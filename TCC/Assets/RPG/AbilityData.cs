using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;
    public Sprite icon;
    // Futuramente: dano, custo de mana...
}