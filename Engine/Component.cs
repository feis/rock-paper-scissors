namespace Engine;

internal abstract class Component
{
    public bool isEnabled { get; set; } = true;
    public GameObject? GameObject { get; internal set; }

    public virtual void Awake() {}

    public virtual void Update(double deltaTime) { }
    public virtual void Render(RenderingPipeline rp) { }

    protected T? GetComponent<T>() where T : Component
    {
        return GameObject?.GetComponent<T>();
    }
}