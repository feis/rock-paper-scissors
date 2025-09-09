using System.Text;

internal class SpinnerText : IGameObject
{
    private readonly Text text = new();
    private readonly char[] spinChars = ['|', '/', '-', '\\'];
    private double elapsedTime;

    void IGameObject.Update(Game game, StringBuilder frameBuffer)
    {
        GameState state = game.GetCurrentState();
        
        if (state == GameState.WaitingForInput)
        {
            text.SetActive(true);
            elapsedTime = game.GetElapsedTime();
            int animationIndex = (int)Math.Round(elapsedTime / 0.25);
            text.SetContent($"請選擇你的動作 (1-3) {spinChars[animationIndex % 4]}");
        }
        else
        {
            text.SetActive(false);
        }
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}