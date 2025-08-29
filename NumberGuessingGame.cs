namespace CopilotDemo;

public enum GuessResult
{
    Correct,
    InvalidInput,
    ImpossibleState,
    Continue // Game continues with new guess
}

public class NumberGuessingGame
{
    private int _min;
    private int _max;
    private int _currentGuess;
    private bool _gameEnded;

    public NumberGuessingGame(int min = 0, int max = 100)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max");
        
        _min = min;
        _max = max;
        _currentGuess = 64; // Start with 64 as specified in original code
        _gameEnded = false;
    }

    public int CurrentGuess => _currentGuess;
    public bool IsGameEnded => _gameEnded;
    public int Min => _min;
    public int Max => _max;

    public GuessResult ProcessGuessResponse(string response)
    {
        if (_gameEnded)
            return GuessResult.Correct;

        var normalizedResponse = response?.ToUpper().Trim();
        
        if (normalizedResponse == "Y")
        {
            _gameEnded = true;
            return GuessResult.Correct;
        }
        else if (normalizedResponse == "N")
        {
            return GuessResult.InvalidInput; // Need direction for next guess
        }
        else
        {
            return GuessResult.InvalidInput;
        }
    }

    public GuessResult ProcessDirectionResponse(string direction)
    {
        if (_gameEnded)
            return GuessResult.Correct;

        var normalizedDirection = direction?.ToUpper().Trim();
        
        if (normalizedDirection == "H")
        {
            _min = _currentGuess + 1;
            return UpdateGuessAndCheckState();
        }
        else if (normalizedDirection == "L")
        {
            _max = _currentGuess - 1;
            return UpdateGuessAndCheckState();
        }
        else
        {
            return GuessResult.InvalidInput;
        }
    }

    private GuessResult UpdateGuessAndCheckState()
    {
        // Check if we've narrowed it down impossibly
        if (_min > _max)
        {
            _gameEnded = true;
            return GuessResult.ImpossibleState;
        }
        
        // Calculate new guess using binary search
        _currentGuess = (_min + _max) / 2;
        
        return GuessResult.Continue;
    }

    public void Reset(int min = 0, int max = 100)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max");
            
        _min = min;
        _max = max;
        _currentGuess = 64;
        _gameEnded = false;
    }
}