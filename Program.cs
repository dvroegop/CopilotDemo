// See https://aka.ms/new-console-template for more information
Console.WriteLine("Think of a number between 0 and 100, and I will guess it!");
Console.WriteLine("Answer with 'Y' if my guess is correct, 'N' if it's not.");
Console.WriteLine("If it's not correct, I'll ask if your number is higher or lower.");
Console.WriteLine();

int min = 0;
int max = 100;
int guess = 64; // Start with 64 as specified

while (true)
{
    Console.WriteLine($"Is your number {guess}? (Y/N)");
    string? response = Console.ReadLine()?.ToUpper().Trim();
    
    if (response == "Y")
    {
        Console.WriteLine($"Great! I guessed your number: {guess}");
        Console.WriteLine("Thanks for playing!");
        break;
    }
    else if (response == "N")
    {
        Console.WriteLine("Is your number higher (H) or lower (L) than my guess?");
        string? direction = Console.ReadLine()?.ToUpper().Trim();
        
        if (direction == "H")
        {
            min = guess + 1;
        }
        else if (direction == "L")
        {
            max = guess - 1;
        }
        else
        {
            Console.WriteLine("Please enter 'H' for higher or 'L' for lower.");
            continue;
        }
        
        // Calculate new guess using binary search
        guess = (min + max) / 2;
        
        // Check if we've narrowed it down impossibly
        if (min > max)
        {
            Console.WriteLine("It seems there might be an error - are you sure about your responses?");
            break;
        }
    }
    else
    {
        Console.WriteLine("Please enter 'Y' if my guess is correct or 'N' if it's not.");
    }
}

