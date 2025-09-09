using Engine;

[Serializable]
internal class SpinnerText : Component
{
    [SerializeField] 
    private double animationSpeed = 0.25;

    [SerializeField] private Text? text;
    
    private readonly char[] spinChars = ['|', '/', '-', '\\'];

    private string baseMessage;
    
    public override void Awake()
    {
        text = GetComponent<Text>(); 
        baseMessage = text!.Content;
    }

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        
        if (state == GameState.WaitingForInput)
        {
            text!.SetActive(true);
            double elapsedTime = GameManager.Instance.GetElapsedTime();
            int animationIndex = (int)Math.Round(elapsedTime / animationSpeed);
            text.Content = $"{baseMessage} {spinChars[animationIndex % 4]}";
        }
        else
        {
            text!.SetActive(false);
        }
    }

    public override void Render(RenderingPipeline rp)
    {
    }
}