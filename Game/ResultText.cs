using System.Text;

namespace Game;

internal class ResultText : IGameObject
{
    private readonly Text text = new();

    void IGameObject.Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        GameResult gameResult = GameManager.Instance.GetGameResult();
        GameChoice? playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.SetContent(GetResultMessage(gameResult));
        }
    }

    void IGameObject.Render(StringBuilder frameBuffer)
    {
    }

    private static string GetResultMessage(GameResult result)
    {
        return result switch
        {
            GameResult.Win => "你贏了！",
            GameResult.Lose => "你輸了！",
            GameResult.Draw => "平手！",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}