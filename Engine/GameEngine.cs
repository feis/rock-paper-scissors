using System.Diagnostics;
using System.Text;

namespace Engine;

internal class GameEngine
{
    private const int TargetFPS = 8;
    private readonly Stopwatch frameTimer = new();
    private readonly StringBuilder frameBuffer = new();
    private readonly List<GameObject> gameObjects = new();
    
    public void Run()
    {
        Application.IsPlaying = true;
        
        SceneLoader.LoadScene("Scenes/game-scene.json", gameObjects);
        
        while (Application.IsPlaying)
        {
            double lastFrameTime = frameTimer.ElapsedMilliseconds / 1000.0;
            frameTimer.Restart();
            
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(lastFrameTime);
            }
            
            frameBuffer.Clear();

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Render(frameBuffer);
            }

            WaitForFPS();
            SwapBuffer(frameBuffer.ToString());
        }
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