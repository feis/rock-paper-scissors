using System.Diagnostics;

namespace Engine;

internal class GameEngine
{
    private const int TargetFPS = 8;
    private readonly Stopwatch frameTimer = new();
    private readonly RenderingPipeline renderingPipeline = new();
    private readonly List<GameObject> gameObjects = new();
    
    public void Run()
    {
        Application.IsPlaying = true;
        
        SceneLoader.LoadScene("Scenes/game-scene.json", gameObjects);
        
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.Awake();
        }
        
        while (Application.IsPlaying)
        {
            double lastFrameTime = frameTimer.ElapsedMilliseconds / 1000.0;
            frameTimer.Restart();
            
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.isActive)
                {
                    gameObject.Update(lastFrameTime);
                }
            }
            
            renderingPipeline.Begin();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.isActive)
                {
                    gameObject.Render(renderingPipeline);
                }
            }
            renderingPipeline.End();
            WaitForFPS();
            renderingPipeline.SwapBuffers();
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
}