using Engine;
using JetBrains.Annotations;

[UsedImplicitly]
internal class GameManager : Component
{
    public static GameManager? Instance;
    
    private readonly Random random = new();
    
    private GameChoice? playerChoice;
    private GameChoice computerChoice;
    private GameResult gameResult;
    private GameState currentState = GameState.GameStart;
    private double roundEndTime;
    private double gameEndTime;
    private double elapsedTime; 
    
    public GameState GetCurrentState() => currentState;
    public GameChoice? GetPlayerChoice() => playerChoice;
    public GameChoice GetComputerChoice() => computerChoice;
    public GameResult GetGameResult() => gameResult;
    public double GetElapsedTime() => elapsedTime;

    public GameManager()
    {
        Instance = this;
    }
    
    public override void Update(double deltaTime)
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
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    string input = keyInfo.KeyChar.ToString();

                    if (int.TryParse(input, out int choice) && choice is >= 1 and <= 3)
                    {
                        playerChoice = (GameChoice)choice;
                        SetComputerChoice();
                        gameResult = PlayRound(playerChoice.Value, computerChoice);
                        currentState = GameState.RoundCompleted;
                        roundEndTime = elapsedTime;
                    }
                }
                break;
            }
            case GameState.RoundCompleted:
                if (gameResult != GameResult.Draw)
                {
                    currentState = GameState.GameEnding;
                    gameEndTime = elapsedTime;
                }
                else
                {
                    currentState = GameState.DrawWaiting;
                }
                break;
                
            case GameState.DrawWaiting:
            {
                if (elapsedTime - roundEndTime >= 1)
                {
                    playerChoice = null;
                    currentState = GameState.WaitingForInput;
                }
                break;
            }
            case GameState.GameEnding:
            {
                if (elapsedTime - gameEndTime >= 1)
                {
                    currentState = GameState.Finished;
                    Application.IsPlaying = false;
                }
                break;
            }
            case GameState.Finished:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void SetComputerChoice()
    {
        computerChoice = (GameChoice)random.Next(1, 4);
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
}