using System;
using System.Collections.Generic;
using UnityEngine;

public enum AppearanceCategory
{
    Hair,
    Face,
    Brow,
    Nose,
    Ear,
    Eyes,
    Beard
}

// Categorias de cor independentes (permite pintar o corpo e agrupar partes como Orelha/Nariz na cor da pele)
public enum ColorCategory
{
    Skin,
    Hair,
    Eyebrows,
    Eyes,
    Beard
}

[Serializable]
public class AttributeSaveData
{
    public AttributeType type;
    public int baseValue;
    public int allocatedValue;
    public int totalValue;
}

[Serializable]
public class SerializableAppearance
{
    public AppearanceCategory category;
    public string selectedOptionId;
    public string selectedOptionName;
}

[Serializable]
public class SerializableColor
{
    public ColorCategory category;
    public string colorHex;
}

[Serializable]
public class CharacterBuild
{
    public string characterName;
    public string pronoun;
    public string raceName;
    public string className;

    public List<AttributeSaveData> attributes = new List<AttributeSaveData>();
    public List<string> abilities = new List<string>();
    public List<SerializableAppearance> appearance = new List<SerializableAppearance>();
    public List<SerializableColor> colors = new List<SerializableColor>();
}