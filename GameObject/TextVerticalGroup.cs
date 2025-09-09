using System.Text;

internal class TextVerticalGroup : IGameObject
{
    private readonly List<ITextRenderer> textElements = new();

    public void Add(ITextRenderer textElement)
    {
        textElements.Add(textElement);
    }
    

    void IGameObject.Update(Game game, StringBuilder frameBuffer)
    {
        foreach (ITextRenderer element in textElements.Where(element => element.IsActive))
        {
            element.Render(frameBuffer);
            frameBuffer.AppendLine();
        }
    }
}