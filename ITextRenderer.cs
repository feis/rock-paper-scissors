using System.Text;

internal interface ITextRenderer
{
    bool IsActive { get; }
    void Render(StringBuilder buffer);
}