// See https://aka.ms/new-console-template for more information
using CopilotDemo;

Console.WriteLine("Think of a number between 0 and 100, and I will guess it!");
Console.WriteLine("Answer with 'C' if my guess is correct, 'L' if your number is lower, or 'H' if your number is higher.");
Console.WriteLine();

var game = new NumberGuessingGame(0, 100);

while (!game.IsGameEnded)
{
    Console.WriteLine($"Is my guess correct (C), or should it be lower (L) or higher (H)? My guess: {game.CurrentGuess}");
    string? response = Console.ReadLine();
    
    var guessResult = game.ProcessGuessResponse(response ?? "");
    
    if (guessResult == GuessResult.Correct)
    {
        Console.WriteLine($"Great! I guessed your number: {game.CurrentGuess}");
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
        else
        {
            Console.WriteLine("Please enter 'C' if correct, 'L' if your number is lower, or 'H' if your number is higher.");
        }
    }
}

