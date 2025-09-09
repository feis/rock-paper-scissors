using System.Text;

namespace Game;

internal class SpinnerText : IGameObject
{
    private readonly Text text = new();
    private readonly char[] spinChars = ['|', '/', '-', '\\'];
    private double elapsedTime;

    void IGameObject.Update(double deltaTimee)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        
        if (state == GameState.WaitingForInput)
        {
            text.SetActive(true);
            elapsedTime = GameManager.Instance.GetElapsedTime();
            int animationIndex = (int)Math.Round(elapsedTime / 0.25);
            text.SetContent($"請選擇你的動作 (1-3) {spinChars[animationIndex % 4]}");
        }
        else
        {
            text.SetActive(false);
        }
    }

    void IGameObject.Render(StringBuilder frameBuffer)
    {
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}