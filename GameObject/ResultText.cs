using System.Text;

internal class ResultText : IGameObject
{
    private readonly Text text = new();

    void IGameObject.Update(Game game, StringBuilder frameBuffer)
    {
        GameState state = game.GetCurrentState();
        GameResult gameResult = game.GetGameResult();
        GameChoice? playerChoice = game.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.SetContent(GetResultMessage(gameResult));
        }
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