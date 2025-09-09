using System.Text;
using Engine;

internal class TextVerticalGroup : GameObject
{
    [SerializeField]
    private List<ITextRenderer> textElements = new();

    public void Add(ITextRenderer textElement)
    {
        textElements.Add(textElement);
    }

    public override void Update(double deltaTime)
    {
    }

    public override void Render(StringBuilder frameBuffer)
    {
        foreach (ITextRenderer element in textElements.Where(element => element.IsActive))
        {
            element.Render(frameBuffer);
            frameBuffer.AppendLine();
        }
    }
}