using System.Text;

internal class Game
{
    private readonly Random random = new();
    private readonly TextVerticalGroup mainTextVerticalGroup = new();
    private readonly Text promptText;
    private readonly Text spinnerText;
    private readonly Text playerChoiceText;
    private readonly Text computerChoiceText;
    private readonly Text resultText;
    
    private GameChoice? playerChoice;
    private GameChoice computerChoice;
    private GameResult gameResult;
    private GameState currentState = GameState.GameStart;
    private double roundEndTime;
    private double gameEndTime;
    private int animationIndex;
    private double elapsedTime;
    
    public Game()
    {
        promptText = new Text("1:石頭, 2:布, 3:剪刀");
        spinnerText = new Text("");
        playerChoiceText = new Text("");
        computerChoiceText = new Text("");
        resultText = new Text("");
        
        mainTextVerticalGroup.Add(promptText);
        mainTextVerticalGroup.Add(spinnerText);
        mainTextVerticalGroup.Add(playerChoiceText);
        mainTextVerticalGroup.Add(computerChoiceText);
        mainTextVerticalGroup.Add(resultText);
    }
    
    public bool IsFinished => currentState == GameState.Finished;

    public void Update(double deltaTime, StringBuilder frameBuffer)
    {
        elapsedTime += deltaTime;
        frameBuffer.Clear();
        
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
                        computerChoice = GetComputerChoice();
                        gameResult = PlayRound(playerChoice.Value, computerChoice);
                        currentState = GameState.RoundCompleted;
                        roundEndTime = elapsedTime;
                    }
                }
                animationIndex = (int)Math.Round(elapsedTime / 0.25);
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
        
        UpdateTextElements();
        mainTextVerticalGroup.Render(frameBuffer);
    }
    
    private void UpdateTextElements()
    {
        switch (currentState)
        {
            case GameState.WaitingForInput:
                promptText.SetActive(true);
                spinnerText.SetActive(true);
                playerChoiceText.SetActive(false);
                computerChoiceText.SetActive(false);
                resultText.SetActive(false);
                
                char[] spinChars = ['|', '/', '-', '\\'];
                spinnerText.SetContent($"請選擇你的動作 (1-3) {spinChars[animationIndex % 4]}");
                break;
                
            case GameState.RoundCompleted:
            case GameState.DrawWaiting:
            case GameState.GameEnding:
                promptText.SetActive(false);
                spinnerText.SetActive(false);
                playerChoiceText.SetActive(true);
                computerChoiceText.SetActive(true);
                resultText.SetActive(true);
                
                if (playerChoice.HasValue)
                {
                    playerChoiceText.SetContent($"你剛選擇了: {GetChoiceName(playerChoice.Value)}");
                    computerChoiceText.SetContent($"電腦選擇了: {GetChoiceName(computerChoice)}");
                    resultText.SetContent(GetResultMessage(gameResult));
                }
                break;

            case GameState.GameStart:
            case GameState.Finished:
            default:
                promptText.SetActive(false);
                spinnerText.SetActive(false);
                playerChoiceText.SetActive(false);
                computerChoiceText.SetActive(false);
                resultText.SetActive(false);
                break;
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
}
