using System.Text.Json;

namespace Engine;

[Serializable]
internal class ComponentDefinition
{
    public string Type { get; set; } = "";
    public Dictionary<string, JsonElement>? Properties { get; set; }
}