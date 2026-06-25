[System.Serializable]
public class SerializableAttribute
{
    public AttributeType type;
    public int value;
}
// E na classe CharacterBuild, ter uma List<SerializableAttribute> serializedAttributes
// e antes de salvar, preencher com os dados do dicionário.