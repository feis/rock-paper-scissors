using Engine;
using JetBrains.Annotations;

[UsedImplicitly]
internal class TextVerticalGroup : Component
{
    [SerializeField(Name = "Texts")] 
    private List<Text> texts = new();

    public override void Update(double deltaTime)
    {
        int order = 0;
        foreach (Text text in texts)
        {
            if (text.IsActive)
            {
                text.SetOrder(order);
                ++order;
            }
            else
            {
                text.SetOrder(-1);
            }
        }
    }
}