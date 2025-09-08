using System.Text;

internal class TextVerticalGroup : ITextRenderer
{
    private readonly List<ITextRenderer> textElements = new();
    public bool IsActive { get; set; } = true;

    public void Add(ITextRenderer textElement)
    {
        textElements.Add(textElement);
    }

    public void Render(StringBuilder buffer)
    {
        if (!IsActive) return;

        foreach (ITextRenderer element in textElements.Where(element => element.IsActive))
        {
            element.Render(buffer);
            buffer.AppendLine();
        }
    }
}