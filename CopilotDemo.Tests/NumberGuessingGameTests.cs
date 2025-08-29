using CopilotDemo;

namespace CopilotDemo.Tests;

public class NumberGuessingGameTests
{
    [Fact]
    public void Constructor_WithValidRange_InitializesCorrectly()
    {
        // Arrange & Act
        var game = new NumberGuessingGame(0, 100);

        // Assert
        Assert.Equal(0, game.Min);
        Assert.Equal(100, game.Max);
        Assert.Equal(64, game.CurrentGuess);
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void Constructor_WithInvalidRange_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new NumberGuessingGame(100, 0));
        Assert.Throws<ArgumentException>(() => new NumberGuessingGame(50, 50));
    }

    [Fact]
    public void ProcessGuessResponse_CorrectGuess_ReturnsCorrectAndEndsGame()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("Y");

        // Assert
        Assert.Equal(GuessResult.Correct, result);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_CorrectGuessLowercase_ReturnsCorrectAndEndsGame()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("y");

        // Assert
        Assert.Equal(GuessResult.Correct, result);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_IncorrectGuess_ReturnsInvalidInput()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("N");

        // Assert
        Assert.Equal(GuessResult.InvalidInput, result);
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_InvalidInput_ReturnsInvalidInput()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("X");

        // Assert
        Assert.Equal(GuessResult.InvalidInput, result);
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void ProcessDirectionResponse_Higher_UpdatesMinAndGuess()
    {
        // Arrange
        var game = new NumberGuessingGame();
        var initialGuess = game.CurrentGuess; // 64

        // Act
        var result = game.ProcessDirectionResponse("H");

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(65, game.Min); // min should be guess + 1
        Assert.True(game.CurrentGuess != initialGuess); // guess should change
    }

    [Fact]
    public void ProcessDirectionResponse_Lower_UpdatesMaxAndGuess()
    {
        // Arrange
        var game = new NumberGuessingGame();
        var initialGuess = game.CurrentGuess; // 64

        // Act
        var result = game.ProcessDirectionResponse("L");

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(63, game.Max); // max should be guess - 1
        Assert.True(game.CurrentGuess != initialGuess); // guess should change
    }

    [Fact]
    public void ProcessDirectionResponse_InvalidDirection_ReturnsInvalidInput()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessDirectionResponse("X");

        // Assert
        Assert.Equal(GuessResult.InvalidInput, result);
    }

    [Theory]
    [InlineData("h")]
    [InlineData("H")]
    [InlineData(" H ")]
    public void ProcessDirectionResponse_HigherVariations_AllWork(string direction)
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessDirectionResponse(direction);

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(65, game.Min);
    }

    [Theory]
    [InlineData("l")]
    [InlineData("L")]
    [InlineData(" L ")]
    public void ProcessDirectionResponse_LowerVariations_AllWork(string direction)
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessDirectionResponse(direction);

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(63, game.Max);
    }

    [Fact]
    public void NormalGameFlow_HappyPath_WorksCorrectly()
    {
        // Arrange
        var game = new NumberGuessingGame(0, 100);
        // Let's simulate the user thinking of number 25
        
        // Act & Assert
        // Initial guess: 64
        Assert.Equal(64, game.CurrentGuess);
        
        // User says "lower" - number is lower than 64
        var result1 = game.ProcessDirectionResponse("L");
        Assert.Equal(GuessResult.Continue, result1);
        // max becomes 63, new guess = (0 + 63) / 2 = 31
        Assert.Equal(31, game.CurrentGuess);
        
        // User says "lower" again - number is lower than 31
        var result2 = game.ProcessDirectionResponse("L");
        Assert.Equal(GuessResult.Continue, result2);
        // max becomes 30, new guess = (0 + 30) / 2 = 15
        Assert.Equal(15, game.CurrentGuess);
        
        // User says "higher" - number is higher than 15
        var result3 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result3);
        // min becomes 16, new guess = (16 + 30) / 2 = 23
        Assert.Equal(23, game.CurrentGuess);
        
        // User says "higher" - number is higher than 23
        var result4 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result4);
        // min becomes 24, new guess = (24 + 30) / 2 = 27
        Assert.Equal(27, game.CurrentGuess);
        
        // User says "lower" - number is lower than 27
        var result5 = game.ProcessDirectionResponse("L");
        Assert.Equal(GuessResult.Continue, result5);
        // max becomes 26, new guess = (24 + 26) / 2 = 25
        Assert.Equal(25, game.CurrentGuess);
        
        // User confirms correct guess
        var result6 = game.ProcessGuessResponse("Y");
        Assert.Equal(GuessResult.Correct, result6);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void EdgeCase_UserLiesAboutAnswers_DetectsImpossibleState()
    {
        // Arrange - User thinks of 50 but lies about responses
        var game = new NumberGuessingGame(0, 100);
        
        // Act & Assert
        // Initial guess: 64, user says "higher" (lie - 50 is lower)
        var result1 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result1);
        
        // New guess: 82, user says "higher" again (lie)
        var result2 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result2);
        
        // New guess: 91, user says "higher" again (lie)
        var result3 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result3);
        
        // New guess: 95, user says "higher" again (lie)
        var result4 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result4);
        
        // New guess: 97, user says "higher" again (lie)
        var result5 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result5);
        
        // New guess: 99, user says "higher" again (lie)
        var result6 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result6);
        
        // New guess: 100, user says "lower" (now telling truth but too late)
        var result7 = game.ProcessDirectionResponse("L");
        Assert.Equal(GuessResult.ImpossibleState, result7);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void EdgeCase_UserPicksNumberHigherThan100_GameHandlesCorrectly()
    {
        // Arrange - Test with extended range to accommodate numbers > 100
        var game = new NumberGuessingGame(0, 200);
        
        // Act - Simulate user thinking of 150
        // Initial guess: 64, user says "higher"
        var result1 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Continue, result1);
        
        // Keep going higher until we reach the area around 150
        var result2 = game.ProcessDirectionResponse("H"); // around 132
        Assert.Equal(GuessResult.Continue, result2);
        
        var result3 = game.ProcessDirectionResponse("H"); // around 166
        Assert.Equal(GuessResult.Continue, result3);
        
        var result4 = game.ProcessDirectionResponse("L"); // around 149
        Assert.Equal(GuessResult.Continue, result4);
        
        var result5 = game.ProcessDirectionResponse("H"); // around 157
        Assert.Equal(GuessResult.Continue, result5);
        
        var result6 = game.ProcessDirectionResponse("L"); // around 153
        Assert.Equal(GuessResult.Continue, result6);
        
        var result7 = game.ProcessDirectionResponse("L"); // around 150
        Assert.Equal(GuessResult.Continue, result7);
        
        // Eventually we should be able to guess correctly
        // This test demonstrates the game can handle numbers > 100
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void Reset_ResetsGameToInitialState()
    {
        // Arrange
        var game = new NumberGuessingGame();
        game.ProcessDirectionResponse("H"); // Change some state
        
        // Act
        game.Reset();
        
        // Assert
        Assert.Equal(0, game.Min);
        Assert.Equal(100, game.Max);
        Assert.Equal(64, game.CurrentGuess);
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void Reset_WithNewRange_UpdatesRange()
    {
        // Arrange
        var game = new NumberGuessingGame();
        
        // Act
        game.Reset(10, 50);
        
        // Assert
        Assert.Equal(10, game.Min);
        Assert.Equal(50, game.Max);
        Assert.Equal(64, game.CurrentGuess); // Still starts with 64
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void AfterGameEnds_SubsequentCallsReturnCorrect()
    {
        // Arrange
        var game = new NumberGuessingGame();
        game.ProcessGuessResponse("Y"); // End the game
        
        // Act & Assert
        var result1 = game.ProcessGuessResponse("N");
        Assert.Equal(GuessResult.Correct, result1);
        
        var result2 = game.ProcessDirectionResponse("H");
        Assert.Equal(GuessResult.Correct, result2);
    }

    // New tests for the combined C/L/H functionality
    [Fact]
    public void ProcessGuessResponse_CorrectGuessWithC_ReturnsCorrectAndEndsGame()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("C");

        // Assert
        Assert.Equal(GuessResult.Correct, result);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_CorrectGuessWithLowerC_ReturnsCorrectAndEndsGame()
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse("c");

        // Assert
        Assert.Equal(GuessResult.Correct, result);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_HigherWithH_UpdatesMinAndContinues()
    {
        // Arrange
        var game = new NumberGuessingGame();
        var initialGuess = game.CurrentGuess; // 64

        // Act
        var result = game.ProcessGuessResponse("H");

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(65, game.Min); // min should be guess + 1
        Assert.True(game.CurrentGuess != initialGuess); // guess should change
        Assert.False(game.IsGameEnded);
    }

    [Fact]
    public void ProcessGuessResponse_LowerWithL_UpdatesMaxAndContinues()
    {
        // Arrange
        var game = new NumberGuessingGame();
        var initialGuess = game.CurrentGuess; // 64

        // Act
        var result = game.ProcessGuessResponse("L");

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(63, game.Max); // max should be guess - 1
        Assert.True(game.CurrentGuess != initialGuess); // guess should change
        Assert.False(game.IsGameEnded);
    }

    [Theory]
    [InlineData("h")]
    [InlineData("H")]
    [InlineData(" H ")]
    public void ProcessGuessResponse_HigherVariations_AllWork(string response)
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse(response);

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(65, game.Min);
    }

    [Theory]
    [InlineData("l")]
    [InlineData("L")]
    [InlineData(" L ")]
    public void ProcessGuessResponse_LowerVariations_AllWork(string response)
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse(response);

        // Assert
        Assert.Equal(GuessResult.Continue, result);
        Assert.Equal(63, game.Max);
    }

    [Theory]
    [InlineData("c")]
    [InlineData("C")]
    [InlineData(" C ")]
    public void ProcessGuessResponse_CorrectVariations_AllWork(string response)
    {
        // Arrange
        var game = new NumberGuessingGame();

        // Act
        var result = game.ProcessGuessResponse(response);

        // Assert
        Assert.Equal(GuessResult.Correct, result);
        Assert.True(game.IsGameEnded);
    }

    [Fact]
    public void NewCombinedGameFlow_HappyPath_WorksCorrectly()
    {
        // Arrange
        var game = new NumberGuessingGame(0, 100);
        // Let's simulate the user thinking of number 25
        
        // Act & Assert
        // Initial guess: 64
        Assert.Equal(64, game.CurrentGuess);
        
        // User says "L" - number is lower than 64
        var result1 = game.ProcessGuessResponse("L");
        Assert.Equal(GuessResult.Continue, result1);
        // max becomes 63, new guess = (0 + 63) / 2 = 31
        Assert.Equal(31, game.CurrentGuess);
        
        // User says "L" again - number is lower than 31
        var result2 = game.ProcessGuessResponse("L");
        Assert.Equal(GuessResult.Continue, result2);
        // max becomes 30, new guess = (0 + 30) / 2 = 15
        Assert.Equal(15, game.CurrentGuess);
        
        // User says "H" - number is higher than 15
        var result3 = game.ProcessGuessResponse("H");
        Assert.Equal(GuessResult.Continue, result3);
        // min becomes 16, new guess = (16 + 30) / 2 = 23
        Assert.Equal(23, game.CurrentGuess);
        
        // User says "H" - number is higher than 23
        var result4 = game.ProcessGuessResponse("H");
        Assert.Equal(GuessResult.Continue, result4);
        // min becomes 24, new guess = (24 + 30) / 2 = 27
        Assert.Equal(27, game.CurrentGuess);
        
        // User says "L" - number is lower than 27
        var result5 = game.ProcessGuessResponse("L");
        Assert.Equal(GuessResult.Continue, result5);
        // max becomes 26, new guess = (24 + 26) / 2 = 25
        Assert.Equal(25, game.CurrentGuess);
        
        // User confirms correct guess with "C"
        var result6 = game.ProcessGuessResponse("C");
        Assert.Equal(GuessResult.Correct, result6);
        Assert.True(game.IsGameEnded);
    }
}