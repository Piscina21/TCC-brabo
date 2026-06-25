using System;
using System.Collections.Generic;

[System.Serializable]
public class CharacterBuild
{
    public string characterName;
    public RaceData selectedRace;
    public ClassData selectedClass;

    // Atributos finais já com bônus, mas ainda editáveis na etapa de distribuição
    public Dictionary<AttributeType, int> attributes = new Dictionary<AttributeType, int>();

    // Lista de habilidades adquiridas (raciais + classe)
    public List<AbilityData> abilities = new List<AbilityData>();

    // Aparência (aqui você pode guardar índices de meshes, cores, etc.)
    public int hairStyleIndex;
    public int skinColorIndex;
    // ...
}