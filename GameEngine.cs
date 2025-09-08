using System.Diagnostics;
using System.Text;

internal class GameEngine
{
    private readonly Game game = new();
    private const int TargetFPS = 8;
    private readonly Stopwatch frameTimer = new();
    private readonly StringBuilder frameBuffer = new();
    
    public void Run()
    {
        while (!game.IsFinished)
        {
            double lastFrameTime = frameTimer.ElapsedMilliseconds / 1000.0;
            frameTimer.Restart();
            game.Update(lastFrameTime, frameBuffer);
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
    
    private void SwapBuffer(string output)
    {
        if (!game.IsFinished)
        {
            Console.Clear();
        }
        Console.Write(output);
    }
}