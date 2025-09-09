using System.Text;

namespace Game;

internal class PlayerChoiceText : IGameObject
{
    private readonly Text text = new();

    void IGameObject.Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        var playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.SetContent($"你剛選擇了: {GetChoiceName(playerChoice.Value)}");
        }
    }

    void IGameObject.Render(StringBuilder frameBuffer)
    {
    }

    private static string GetChoiceName(GameChoice choice)
    {
        return choice switch
        {
            GameChoice.Rock => "石頭",
            GameChoice.Paper => "布",
            GameChoice.Scissors => "剪刀",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}