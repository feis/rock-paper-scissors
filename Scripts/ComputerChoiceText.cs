using Engine;
using JetBrains.Annotations;

[UsedImplicitly]
internal class ComputerChoiceText : Component
{
    [SerializeField] private Text text;
    
    public override void Awake()
    {
        text = GetComponent<Text>();
    }

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        GameChoice computerChoice = GameManager.Instance.GetComputerChoice();
        GameChoice? playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.Content = $"電腦選擇了: {GetChoiceName(computerChoice)}";
        }
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

    public override void Render(RenderingPipeline rp)
    {
        text.Render(rp);
    }
}