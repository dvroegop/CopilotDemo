using CopilotDemo.Models;

namespace CopilotDemo.Services;

public interface INumberGuessingService
{
    int CurrentGuess { get; }
    bool IsGameEnded { get; }
    int Min { get; }
    int Max { get; }

    GuessResult ProcessGuessResponse(string response);
    GuessResult ProcessDirectionResponse(string direction);
    void Reset(int min = 0, int max = 100);
}