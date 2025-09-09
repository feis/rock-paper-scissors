using System.Text;

namespace Engine;

internal interface ITextRenderer
{
    bool IsActive { get; }
    void Render(StringBuilder buffer);
}