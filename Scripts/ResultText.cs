using Engine;

[Serializable]
internal class ResultText : Component
{
    [SerializeField] private Text text;

    public override void Awake()
    {
        text = GetComponent<Text>();
    }

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        GameResult gameResult = GameManager.Instance.GetGameResult();
        GameChoice? playerChoice = GameManager.Instance.GetPlayerChoice();
        
        bool shouldShow = state is GameState.RoundCompleted or GameState.DrawWaiting or GameState.GameEnding;
        
        text.SetActive(shouldShow);
        
        if (shouldShow && playerChoice.HasValue)
        {
            text.Content = GetResultMessage(gameResult);
        }
    }

    public override void Render(RenderingPipeline rp)
    {
        text.Render(rp);
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
}