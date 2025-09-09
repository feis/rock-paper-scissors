using JetBrains.Annotations;

namespace Engine;

[UsedImplicitly]
internal class Text : Component
{
    private int order = -1;
    
    [SerializeField]
    private string content = string.Empty;
    
    public string Content 
    { 
        get => content; 
        set => content = value; 
    }

    public bool IsActive { get; private set; } = true;

    public void SetActive(bool value)
    {
        IsActive = value;
    }

    public override void Render(RenderingPipeline rp)
    {
        rp.Draw(order, content);
    }

    public void SetOrder(int value)
    {
        order = value;
    }
}