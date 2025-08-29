// See https://aka.ms/new-console-template for more information
using CopilotDemo;

Console.WriteLine("Think of a number between 0 and 100, and I will guess it!");
Console.WriteLine("Answer with 'Y' if my guess is correct, 'N' if it's not.");
Console.WriteLine("If it's not correct, I'll ask if your number is higher or lower.");
Console.WriteLine();

var game = new NumberGuessingGame(0, 100);

while (!game.IsGameEnded)
{
    Console.WriteLine($"Is your number {game.CurrentGuess}? (Y/N)");
    string? response = Console.ReadLine();
    
    var guessResult = game.ProcessGuessResponse(response ?? "");
    
    if (guessResult == GuessResult.Correct)
    {
        Console.WriteLine($"Great! I guessed your number: {game.CurrentGuess}");
        Console.WriteLine("Thanks for playing!");
        break;
    }
    else if (guessResult == GuessResult.InvalidInput && response?.ToUpper().Trim() == "N")
    {
        Console.WriteLine("Is your number higher (H) or lower (L) than my guess?");
        string? direction = Console.ReadLine();
        
        var directionResult = game.ProcessDirectionResponse(direction ?? "");
        
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
    else if (guessResult == GuessResult.InvalidInput)
    {
        Console.WriteLine("Please enter 'Y' if my guess is correct or 'N' if it's not.");
    }
}

