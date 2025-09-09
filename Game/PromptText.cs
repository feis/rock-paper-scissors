using System.Text;

namespace Game;

internal class PromptText : IGameObject
{
    private readonly Text text = new("1:石頭, 2:布, 3:剪刀");

    void IGameObject.Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        text.SetActive(state == GameState.WaitingForInput);
    }

    void IGameObject.Render(StringBuilder frameBuffer)
    {
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}