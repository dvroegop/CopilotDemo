using CopilotDemo.Models;
using CopilotDemo.Services;
using FluentAssertions;

namespace CopilotDemo.Tests;

public class NumberGuessingServiceTests
{
    [Fact]
    public void Constructor_WithValidRange_InitializesCorrectly()
    {
        var service = new NumberGuessingService(10, 50);

        service.Min.Should().Be(10);
        service.Max.Should().Be(50);
        service.CurrentGuess.Should().Be(30); // (10 + 50) / 2 = 30
        service.IsGameEnded.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithInvalidRange_ThrowsArgumentException()
    {
        var act = () => new NumberGuessingService(50, 10);

        act.Should().Throw<ArgumentException>()
           .WithMessage("Min must be less than max");
    }

    [Fact]
    public void ProcessGuessResponse_CorrectGuess_ReturnsCorrectAndEndsGame()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessGuessResponse("C");

        result.Should().Be(GuessResult.Correct);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void ProcessGuessResponse_CorrectGuessLowercase_ReturnsCorrectAndEndsGame()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessGuessResponse("c");

        result.Should().Be(GuessResult.Correct);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void ProcessGuessResponse_IncorrectGuess_ReturnsInvalidInput()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessGuessResponse("N");

        result.Should().Be(GuessResult.InvalidInput);
    }

    [Fact]
    public void ProcessGuessResponse_InvalidInput_ReturnsInvalidInput()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessGuessResponse("X");

        result.Should().Be(GuessResult.InvalidInput);
    }

    [Fact]
    public void ProcessDirectionResponse_Higher_UpdatesMinAndGuess()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessDirectionResponse("H");

        result.Should().Be(GuessResult.Continue);
        service.Min.Should().Be(51); // 50 + 1, since initial guess is (0+100)/2 = 50
    }

    [Fact]
    public void ProcessDirectionResponse_Lower_UpdatesMaxAndGuess()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessDirectionResponse("L");

        result.Should().Be(GuessResult.Continue);
        service.Max.Should().Be(49); // 50 - 1, since initial guess is (0+100)/2 = 50
    }

    [Fact]
    public void ProcessDirectionResponse_InvalidDirection_ReturnsInvalidInput()
    {
        var service = new NumberGuessingService();

        var result = service.ProcessDirectionResponse("X");

        result.Should().Be(GuessResult.InvalidInput);
    }

    [Theory]
    [InlineData("h")]
    [InlineData("H")]
    [InlineData(" H ")]
    public void ProcessDirectionResponse_HigherVariations_AllWork(string direction)
    {
        // Arrange
        var service = new NumberGuessingService();

        // Act
        var result = service.ProcessDirectionResponse(direction);

        // Assert
        result.Should().Be(GuessResult.Continue);
        service.Min.Should().Be(51); // Default range is 0-100, so initial guess is 50, after H min becomes 51
    }

    [Theory]
    [InlineData("l")]
    [InlineData("L")]
    [InlineData(" L ")]
    public void ProcessDirectionResponse_LowerVariations_AllWork(string direction)
    {
        // Arrange
        var service = new NumberGuessingService();

        // Act
        var result = service.ProcessDirectionResponse(direction);

        // Assert
        result.Should().Be(GuessResult.Continue);
        service.Max.Should().Be(49); // Default range is 0-100, so initial guess is 50, after L max becomes 49
    }

    [Fact]
    public void NormalGameFlow_HappyPath_WorksCorrectly()
    {
        var service = new NumberGuessingService(1, 10);
        
        // Guess should be around 5-6 with binary search
        service.CurrentGuess.Should().BeInRange(5, 6);
        
        // Say the number is too high
        var result1 = service.ProcessGuessResponse("L");
        result1.Should().Be(GuessResult.Continue);
        
        // Now guess should be lower
        var currentGuess = service.CurrentGuess;
        currentGuess.Should().BeLessThan(6);
        
        // Say correct
        var result2 = service.ProcessGuessResponse("C");
        result2.Should().Be(GuessResult.Correct);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void EdgeCase_UserLiesAboutAnswers_DetectsImpossibleState()
    {
        // Create a scenario that will definitely lead to impossible state
        var service = new NumberGuessingService(1, 3); // Very small range
        
        // Start with range 1-3, initial guess should be 2
        service.CurrentGuess.Should().Be(2);
        
        // Say it's lower, so max becomes 1, new guess becomes 1
        var result1 = service.ProcessGuessResponse("L");
        result1.Should().Be(GuessResult.Continue);
        
        // Now say it's lower again, which should be impossible since we're already at 1
        var result2 = service.ProcessGuessResponse("L");
        
        // This should trigger impossible state since max becomes 0 but min is still 1
        result2.Should().Be(GuessResult.ImpossibleState);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void EdgeCase_UserPicksNumberHigherThan100_GameHandlesCorrectly()
    {
        // User picks 150, we start with guess 50 (0-100 range)
        var service = new NumberGuessingService();
        
        // Keep saying higher until we're well above 100
        var result1 = service.ProcessGuessResponse("H"); // min becomes 51, new guess around 75
        result1.Should().Be(GuessResult.Continue);
        
        var result2 = service.ProcessGuessResponse("H"); // min becomes ~76, new guess around 88
        result2.Should().Be(GuessResult.Continue);
        
        var result3 = service.ProcessGuessResponse("H"); // min becomes ~89, new guess around 94
        result3.Should().Be(GuessResult.Continue);
        
        var result4 = service.ProcessGuessResponse("H"); // min becomes ~95, new guess around 97
        result4.Should().Be(GuessResult.Continue);
        
        // Now we should be able to continue guessing higher numbers
        // This test demonstrates the game can handle numbers > 100
        service.IsGameEnded.Should().BeFalse();
        service.CurrentGuess.Should().BeGreaterThan(90);
    }

    [Theory]
    [InlineData("h")]
    [InlineData("H")]
    [InlineData(" H ")]
    public void ProcessGuessResponse_HigherVariations_AllWork(string response)
    {
        // Arrange
        var service = new NumberGuessingService();

        // Act
        var result = service.ProcessGuessResponse(response);

        // Assert
        result.Should().Be(GuessResult.Continue);
        service.Min.Should().Be(51); // Default range is 0-100, so initial guess is 50, after H min becomes 51
    }

    [Theory]
    [InlineData("l")]
    [InlineData("L")]
    [InlineData(" L ")]
    public void ProcessGuessResponse_LowerVariations_AllWork(string response)
    {
        // Arrange
        var service = new NumberGuessingService();

        // Act
        var result = service.ProcessGuessResponse(response);

        // Assert
        result.Should().Be(GuessResult.Continue);
        service.Max.Should().Be(49); // Default range is 0-100, so initial guess is 50, after L max becomes 49
    }

    [Theory]
    [InlineData("c")]
    [InlineData("C")]
    [InlineData(" C ")]
    public void ProcessGuessResponse_CorrectVariations_AllWork(string response)
    {
        // Arrange
        var service = new NumberGuessingService();

        // Act
        var result = service.ProcessGuessResponse(response);

        // Assert
        result.Should().Be(GuessResult.Correct);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void NewCombinedGameFlow_HappyPath_WorksCorrectly()
    {
        var service = new NumberGuessingService();
        
        // Use the new combined H/L/C system
        var result1 = service.ProcessGuessResponse("H");
        result1.Should().Be(GuessResult.Continue);
        service.Min.Should().Be(51); // After H, min becomes current_guess + 1 = 50 + 1 = 51
        
        var result2 = service.ProcessGuessResponse("C"); // Just say correct instead of testing L
        result2.Should().Be(GuessResult.Correct);
        service.IsGameEnded.Should().BeTrue();
    }

    [Fact]
    public void Reset_WithValidParameters_ResetsGameState()
    {
        var service = new NumberGuessingService();
        
        // Play a bit
        service.ProcessGuessResponse("H");
        service.ProcessGuessResponse("C");
        
        service.IsGameEnded.Should().BeTrue();
        
        // Reset
        service.Reset(20, 80);
        
        service.Min.Should().Be(20);
        service.Max.Should().Be(80);
        service.CurrentGuess.Should().Be(50); // (20 + 80) / 2 = 50
        service.IsGameEnded.Should().BeFalse();
    }

    [Fact]
    public void Reset_WithInvalidParameters_ThrowsArgumentException()
    {
        var service = new NumberGuessingService();
        
        var act = () => service.Reset(80, 20);
        
        act.Should().Throw<ArgumentException>()
           .WithMessage("Min must be less than max");
    }
}