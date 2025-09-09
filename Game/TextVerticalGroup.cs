using System.Text;

namespace Game;

internal class TextVerticalGroup : IGameObject
{
    private readonly List<ITextRenderer> textElements = new();

    public void Add(ITextRenderer textElement)
    {
        textElements.Add(textElement);
    }

    public void Update(double deltaTime)
    {
    }

    public void Render(StringBuilder frameBuffer)
    {
        foreach (ITextRenderer element in textElements.Where(element => element.IsActive))
        {
            element.Render(frameBuffer);
            frameBuffer.AppendLine();
        }
    }
}