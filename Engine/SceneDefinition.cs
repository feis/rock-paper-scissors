namespace Engine;

[Serializable]
internal class SceneDefinition
{
    public string Name { get; set; } = "";
    public List<GameObjectDefinition> GameObjects { get; set; } = new();
}