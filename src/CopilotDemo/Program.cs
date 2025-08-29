using CopilotDemo.Commands;
using CopilotDemo.Composition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

var rootCommand = new RootCommand("A number guessing game where the computer tries to guess your number!");

var verbosityOption = new Option<string>(
    aliases: ["--verbosity", "-v"],
    description: "Set the verbosity level (quiet|normal|detailed|diagnostic)")
{
    IsRequired = false
};
verbosityOption.SetDefaultValue("normal");

rootCommand.AddOption(verbosityOption);

rootCommand.SetHandler(async (string verbosity) =>
{
    var logLevel = verbosity.ToLower() switch
    {
        "quiet" => LogLevel.Error,
        "detailed" => LogLevel.Debug,
        "diagnostic" => LogLevel.Trace,
        _ => LogLevel.Information
    };

    var services = new ServiceCollection()
        .AddLogging(builder => builder
            .AddSimpleConsole(o => o.SingleLine = true)
            .SetMinimumLevel(logLevel))
        .AddGameServices()
        .BuildServiceProvider();

    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

    try
    {
        var command = services.GetRequiredService<NumberGuessingCommand>();
        Environment.ExitCode = await command.ExecuteAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        Environment.ExitCode = 130;
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Unexpected error");
        Console.Error.WriteLine("Something improbable occurred. Try --verbosity diagnostic for more details.");
        Environment.ExitCode = 1;
    }
}, verbosityOption);

return await rootCommand.InvokeAsync(args);