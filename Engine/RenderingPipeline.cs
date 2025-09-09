using System.Text;

namespace Engine;

internal class RenderingPipeline
{
    private readonly Dictionary<int, string> lines = new();

    readonly StringBuilder frameBuffer = new();

    public void Begin()
    {
        lines.Clear();
    }

    public void End()
    {
        frameBuffer.Clear();
        foreach (KeyValuePair<int, string> kvp in lines.OrderBy(e => e.Key))
        {
            frameBuffer.AppendLine(kvp.Value);
        }
    }

    public void SwapBuffers()
    {
        SwapBuffer(frameBuffer.ToString());
    }
    
    private static void SwapBuffer(string output)
    {
        if (Application.IsPlaying)
        {
            Console.Clear();
        }
        Console.Write(output);
    }

    public void Draw(int order, string content)
    {
        if (order >= 0)
        {
            lines[order] = content;
        }
    }
}