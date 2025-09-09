using System.Text;
using Engine;

internal class SpinnerText : GameObject, ITextProvider
{
    [SerializeField]
    private string baseMessage = "請選擇你的動作 (1-3)";
    
    [SerializeField] 
    private double animationSpeed = 0.25;
    
    private readonly Text text = new();
    private readonly char[] spinChars = ['|', '/', '-', '\\'];
    private double elapsedTime;

    public override void Update(double deltaTime)
    {
        GameState state = GameManager.Instance!.GetCurrentState();
        
        if (state == GameState.WaitingForInput)
        {
            text.SetActive(true);
            elapsedTime = GameManager.Instance.GetElapsedTime();
            int animationIndex = (int)Math.Round(elapsedTime / animationSpeed);
            text.SetContent($"{baseMessage} {spinChars[animationIndex % 4]}");
        }
        else
        {
            text.SetActive(false);
        }
    }

    public override void Render(StringBuilder frameBuffer)
    {
    }

    public ITextRenderer GetTextRenderer()
    {
        return text;
    }
}