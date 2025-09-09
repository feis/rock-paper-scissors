using Engine;

[Serializable]
internal class PromptText : Component
{
    private Text text;

    public override void Awake()
    {
        text = GetComponent<Text>();
    }

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        text.SetActive(state == GameState.WaitingForInput);
    }

    public override void Render(RenderingPipeline rp)
    {
        text.Render(rp);
    }
}