using CopilotDemo.Models;
using CopilotDemo.Services;
using Microsoft.Extensions.Logging;

namespace CopilotDemo.Commands;

public sealed class NumberGuessingCommand : ICommand
{
    private readonly INumberGuessingService guessingService;
    private readonly ILogger<NumberGuessingCommand> logger;

    public NumberGuessingCommand(
        INumberGuessingService guessingService,
        ILogger<NumberGuessingCommand> logger)
    {
        this.guessingService = guessingService;
        this.logger = logger;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine("Think of a number between 0 and 100, and I will guess it!");
            Console.WriteLine("Answer with 'C' if my guess is correct, 'L' if your number is lower, or 'H' if your number is higher.");
            Console.WriteLine();

            while (!this.guessingService.IsGameEnded && !cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"Is my guess correct (C), or should it be lower (L) or higher (H)? My guess: {this.guessingService.CurrentGuess}");
                string? response = Console.ReadLine();

                var guessResult = this.guessingService.ProcessGuessResponse(response ?? string.Empty);

                if (guessResult == GuessResult.Correct)
                {
                    Console.WriteLine($"Great! I guessed your number: {this.guessingService.CurrentGuess}");
                    Console.WriteLine("Thanks for playing!");
                    break;
                }
                else if (guessResult == GuessResult.Continue)
                {
                    // Continue with the game loop
                    continue;
                }
                else if (guessResult == GuessResult.ImpossibleState)
                {
                    Console.WriteLine("It seems there might be an error - are you sure about your responses?");
                    break;
                }
                else if (guessResult == GuessResult.InvalidInput)
                {
                    // Check if it's the old N response that needs direction
                    if (response?.ToUpper().Trim() == "N")
                    {
                        Console.WriteLine("Is your number higher (H) or lower (L) than my guess?");
                        string? direction = Console.ReadLine();

                        var directionResult = this.guessingService.ProcessDirectionResponse(direction ?? string.Empty);

                        if (directionResult == GuessResult.Continue)
                        {
                            // Continue with the game loop
                            continue;
                        }
                        else if (directionResult == GuessResult.ImpossibleState)
                        {
                            Console.WriteLine("It seems there might be an error - are you sure about your responses?");
                            break;
                        }
                        else if (directionResult == GuessResult.InvalidInput)
                        {
                            Console.WriteLine("Please enter 'H' for higher or 'L' for lower.");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter 'C' if correct, 'L' if your number is lower, or 'H' if your number is higher.");
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Game cancelled by user.");
                return 130; // CTRL-C exit code as per guidelines
            }

            return 0; // Success
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Game cancelled by user.");
            return 130; // CTRL-C exit code as per guidelines
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error during number guessing game");
            Console.Error.WriteLine("Something improbable occurred. Please try again.");
            return 1; // General error
        }
    }
}