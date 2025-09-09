using System.Text;

internal class PromptText : IGameObject
{
    private readonly Text text = new("1:石頭, 2:布, 3:剪刀");

    void IGameObject.Update(Game game, StringBuilder frameBuffer)
    {
        GameState state = game.GetCurrentState();
        text.SetActive(state == GameState.WaitingForInput);
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}