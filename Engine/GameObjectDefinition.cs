using System.Text.Json;

namespace Engine;

[Serializable]
internal class GameObjectDefinition
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public Dictionary<string, JsonElement>? Properties { get; set; }
}