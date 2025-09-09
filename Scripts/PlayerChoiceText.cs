using Engine;

[Serializable]
internal class PlayerChoiceText : Component
{
    [SerializeField] 
    private Text text;

    public override void Awake()
    {
        text = GetComponent<Text>();
    }

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        var playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.Content = $"你剛選擇了: {GetChoiceName(playerChoice.Value)}";
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