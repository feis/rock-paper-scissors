namespace Engine;

[Serializable]
internal class GameObjectDefinition
{
    public string Name { get; set; } = "";
    public List<ComponentDefinition> Components { get; set; } = new();
}