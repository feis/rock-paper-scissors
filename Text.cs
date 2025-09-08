using System.Text;

internal class Text : ITextRenderer
{
    private string content;
    public bool IsActive { get; set; } = true;

    public Text(string content)
    {
        this.content = content;
    }

    public void SetContent(string value)
    {
        content = value;
    }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    public void Render(StringBuilder buffer)
    {
        if (!IsActive) return;
        
        buffer.Append(content);
    }
}