using System.Text;

internal class Game
{
    private readonly Random random = new();
    private readonly List<IGameObject> gameObjects = new();
    
    private GameChoice? playerChoice;
    private GameChoice computerChoice;
    private GameResult gameResult;
    private GameState currentState = GameState.GameStart;
    private double roundEndTime;
    private double gameEndTime;
    private double elapsedTime;
    
    public Game()
    {
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
    
    public bool IsFinished => currentState == GameState.Finished;
    
    public GameState GetCurrentState() => currentState;
    public GameChoice? GetPlayerChoice() => playerChoice;
    public GameChoice GetComputerChoice() => computerChoice;
    public GameResult GetGameResult() => gameResult;
    public double GetElapsedTime() => elapsedTime;

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
                }
                break;
            }
            case GameState.Finished:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        foreach (IGameObject gameObject in gameObjects)
        {
            gameObject.Update(this, frameBuffer);
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
