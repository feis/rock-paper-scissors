using JetBrains.Annotations;

namespace Engine;

[MeansImplicitUse(ImplicitUseTargetFlags.WithInheritors)]
internal class GameObject
{
    private readonly List<Component> components = new();
    public bool isActive = true;

    public string? Name { get; set; }
    public static readonly Dictionary<string, GameObject> NamedGameObjects = new();

    public Component? GetComponent(Type componentType)
    {
        return components.FirstOrDefault(componentType.IsInstanceOfType);
    }
    
    public T? GetComponent<T>() where T : Component
    {
        return components.FirstOrDefault(component => component is T) as T;
    }

    public void AddComponent<T>(T component) where T : Component
    {
        component.GameObject = this;
        components.Add(component);
    }

    public void Update(double deltaTime)
    {
        foreach (Component component in components)
        {
            if (component.isEnabled)
            {
                component.Update(deltaTime);
            }
        }
    }
    
    public void Render(RenderingPipeline renderingPipeline)
    {
        foreach (Component component in components)
        {
            if (component.isEnabled)
            {
                component.Render(renderingPipeline);
            }
        }
    }

    public void Awake()
    {
        foreach (Component component in components)
        {
            component.Awake();
        }
    }
}