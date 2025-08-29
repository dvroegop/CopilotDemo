using CopilotDemo.Models;

namespace CopilotDemo.Services;

public sealed class NumberGuessingService : INumberGuessingService
{
    private int min;
    private int max;
    private int currentGuess;
    private bool gameEnded;

    public NumberGuessingService(int min = 0, int max = 100)
    {
        if (min >= max)
        {
            throw new ArgumentException("Min must be less than max");
        }

        this.min = min;
        this.max = max;
        this.currentGuess = (min + max) / 2; // Use binary search from the beginning
        this.gameEnded = false;
    }

    public int CurrentGuess => this.currentGuess;

    public bool IsGameEnded => this.gameEnded;

    public int Min => this.min;

    public int Max => this.max;

    public GuessResult ProcessGuessResponse(string response)
    {
        if (this.gameEnded)
        {
            return GuessResult.Correct;
        }

        var normalizedResponse = response?.ToUpper().Trim();

        // Handle new combined input: C (correct), L (lower), H (higher)
        if (normalizedResponse == "C")
        {
            this.gameEnded = true;
            return GuessResult.Correct;
        }
        else if (normalizedResponse == "L")
        {
            this.max = this.currentGuess - 1;
            return this.UpdateGuessAndCheckState();
        }
        else if (normalizedResponse == "H")
        {
            this.min = this.currentGuess + 1;
            return this.UpdateGuessAndCheckState();
        }
        // Keep backward compatibility for existing Y/N responses
        else if (normalizedResponse == "Y")
        {
            this.gameEnded = true;
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
        if (this.gameEnded)
        {
            return GuessResult.Correct;
        }

        var normalizedDirection = direction?.ToUpper().Trim();

        if (normalizedDirection == "H")
        {
            this.min = this.currentGuess + 1;
            return this.UpdateGuessAndCheckState();
        }
        else if (normalizedDirection == "L")
        {
            this.max = this.currentGuess - 1;
            return this.UpdateGuessAndCheckState();
        }
        else
        {
            return GuessResult.InvalidInput;
        }
    }

    public void Reset(int min = 0, int max = 100)
    {
        if (min >= max)
        {
            throw new ArgumentException("Min must be less than max");
        }

        this.min = min;
        this.max = max;
        this.currentGuess = (min + max) / 2;
        this.gameEnded = false;
    }

    private GuessResult UpdateGuessAndCheckState()
    {
        // Check if we've narrowed it down impossibly
        if (this.min > this.max)
        {
            this.gameEnded = true;
            return GuessResult.ImpossibleState;
        }

        // Calculate new guess using binary search
        this.currentGuess = (this.min + this.max) / 2;

        return GuessResult.Continue;
    }
}