using System.Text;

internal class Game
{
    private readonly Random random = new();
    private GameChoice? playerChoice;
    private GameChoice computerChoice;
    private GameResult gameResult;
    private GameState currentState = GameState.GameStart;
    private double roundEndTime;
    private double gameEndTime;
    private int animationIndex;
    private double elapsedTime;
    
    public bool IsFinished => currentState == GameState.Finished;

    public void Update(double deltaTime, StringBuilder frameBuffer)
    {
        elapsedTime += deltaTime;

        switch (currentState)
        {
            case GameState.GameStart:
                currentState = GameState.WaitingForInput;
                break;
            
            case GameState.WaitingForInput:
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    string input = keyInfo.KeyChar.ToString();

                    if (int.TryParse(input, out int choice) && choice is >= 1 and <= 3)
                    {
                        playerChoice = (GameChoice)choice;
                        computerChoice = GetComputerChoice();
                        gameResult = PlayRound(playerChoice.Value, computerChoice);
                        currentState = GameState.RoundCompleted;
                        roundEndTime = elapsedTime;
                    }
                }
                animationIndex = (int)Math.Round(elapsedTime / 0.25);
                
                RenderWaitingForInput(frameBuffer);
                break;
            }
            case GameState.RoundCompleted:
                if (gameResult != GameResult.Draw)
                {
                    currentState = GameState.GameEnding;
                    gameEndTime = elapsedTime;
                    RenderRoundResult(frameBuffer);
                }
                else
                {
                    currentState = GameState.DrawWaiting;
                    RenderRoundResult(frameBuffer);
                }
                break;
                
            case GameState.DrawWaiting:
            {
                if (elapsedTime - roundEndTime >= 1)
                {
                    playerChoice = null;
                    currentState = GameState.WaitingForInput;
                    break;
                }
                
                RenderRoundResult(frameBuffer);
                break;
            }
            case GameState.GameEnding:
            {
                if (elapsedTime - gameEndTime >= 1)
                {
                    currentState = GameState.Finished;
                    break;
                }
                
                RenderRoundResult(frameBuffer);
                break;
            }
            case GameState.Finished:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private GameChoice GetComputerChoice()
    {
        return (GameChoice)random.Next(1, 4);
    }

    private static GameResult PlayRound(GameChoice playerChoice, GameChoice computerChoice)
    {
        if (playerChoice == computerChoice)
        {
            return GameResult.Draw;
        }

        bool playerWins = (playerChoice == GameChoice.Rock && computerChoice == GameChoice.Scissors) ||
                         (playerChoice == GameChoice.Paper && computerChoice == GameChoice.Rock) ||
                         (playerChoice == GameChoice.Scissors && computerChoice == GameChoice.Paper);

        return playerWins ? GameResult.Win : GameResult.Lose;
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
    
    private void RenderWaitingForInput(StringBuilder frameBuffer)
    {
        frameBuffer.AppendLine("1:石頭, 2:布, 3:剪刀");
        char[] spinChars = ['|', '/', '-', '\\'];
        frameBuffer.Append($"請選擇你的動作 (1-3) {spinChars[animationIndex % 4]}");
    }
    
    private void RenderRoundResult(StringBuilder frameBuffer)
    {
        frameBuffer.Append($"你剛選擇了: {GetChoiceName(playerChoice!.Value)}");
        frameBuffer.AppendLine();
        frameBuffer.AppendLine($"電腦選擇了: {GetChoiceName(computerChoice)}");
        frameBuffer.AppendLine(GetResultMessage(gameResult));
    }
}
