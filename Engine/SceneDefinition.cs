namespace Engine;

[Serializable]
internal class SceneDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<GameObjectDefinition> GameObjects { get; set; } = new();
}