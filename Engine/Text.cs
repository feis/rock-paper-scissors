using System.Text;

namespace Engine;

internal class Text : ITextRenderer
{
    private string content;
    public bool IsActive { get; private set; }

    public Text()
    {
        content = string.Empty;
    }
    
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
        buffer.Append(content);
    }
}