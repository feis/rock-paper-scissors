using System.Text;

namespace Game;

internal class ComputerChoiceText : IGameObject
{
    private readonly Text text = new();
    void IGameObject.Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        GameChoice computerChoice = GameManager.Instance.GetComputerChoice();
        GameChoice? playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.SetContent($"電腦選擇了: {GetChoiceName(computerChoice)}");
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