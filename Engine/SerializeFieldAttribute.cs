using JetBrains.Annotations;

namespace Engine;

[AttributeUsage(AttributeTargets.Field)]
[MeansImplicitUse(ImplicitUseKindFlags.Assign)]
internal class SerializeFieldAttribute : Attribute
{
    public string? Name { get; set; }
}