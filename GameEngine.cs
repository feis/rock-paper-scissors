using System.Diagnostics;
using System.Text;
using Game;

internal static class Application
{
    public static bool IsPlaying;
}

internal class GameEngine
{
    private const int TargetFPS = 8;
    private readonly Stopwatch frameTimer = new();
    private readonly StringBuilder frameBuffer = new();
    private readonly List<IGameObject> gameObjects = new();
    
    public void Run()
    {
        Application.IsPlaying = true;
        
        LoadScene(gameObjects);
        
        while (Application.IsPlaying)
        {
            double lastFrameTime = frameTimer.ElapsedMilliseconds / 1000.0;
            frameTimer.Restart();
            
            foreach (IGameObject gameObject in gameObjects)
            {
                gameObject.Update(lastFrameTime);
            }
            
            frameBuffer.Clear();

            foreach (IGameObject gameObject in gameObjects)
            {
                gameObject.Render(frameBuffer);
            }

            WaitForFPS();
            SwapBuffer(frameBuffer.ToString());
        }
    }

    private static void LoadScene(List<IGameObject> gameObjects)
    {
        gameObjects.Clear();
        GameManager gameManager = new();
        gameObjects.Add(gameManager);

        PromptText promptText = new();
        SpinnerText spinnerText = new();
        PlayerChoiceText playerChoiceText = new();
        ComputerChoiceText computerChoiceText = new();
        ResultText resultText = new();

        gameObjects.Add(promptText);
        gameObjects.Add(spinnerText);
        gameObjects.Add(playerChoiceText);
        gameObjects.Add(computerChoiceText);
        gameObjects.Add(resultText);

        TextVerticalGroup textVerticalGroup = new();

        textVerticalGroup.Add(promptText.GetTextRenderer());
        textVerticalGroup.Add(spinnerText.GetTextRenderer());
        textVerticalGroup.Add(playerChoiceText.GetTextRenderer());
        textVerticalGroup.Add(computerChoiceText.GetTextRenderer());
        textVerticalGroup.Add(resultText.GetTextRenderer());

        gameObjects.Add(textVerticalGroup);
    }

    private void WaitForFPS()
    {
        const int targetFrameTimeInMilliseconds = 1000 / TargetFPS;
        int elapsedTime = (int)frameTimer.ElapsedMilliseconds;
        int remainingTime = targetFrameTimeInMilliseconds - elapsedTime;
        
        if (remainingTime > 0)
        {
            Thread.Sleep(remainingTime);
        }
    }
    
    private static void SwapBuffer(string output)
    {
        if (Application.IsPlaying)
        {
            Console.Clear();
        }
        Console.Write(output);
    }
}