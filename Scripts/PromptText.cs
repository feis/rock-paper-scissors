using System.Text;
using Engine;

internal class PromptText : GameObject, ITextProvider
{
    [SerializeField]
    private string promptMessage = "1:石頭, 2:布, 3:剪刀";
    
    private Text? text;

    public override void Update(double deltaTime)
    {
        text ??= new Text(promptMessage);
        
        GameState state = GameManager.Instance!.GetCurrentState();
        text.SetActive(state == GameState.WaitingForInput);
    }

    public override void Render(StringBuilder frameBuffer)
    {
    }

    public ITextRenderer GetTextRenderer()
    {
        text ??= new Text(promptMessage);
        return text;
    }
}