using CopilotDemo.Commands;
using CopilotDemo.Composition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection()
    .AddLogging(builder => builder.AddSimpleConsole(o => o.SingleLine = true))
    .AddGameServices()
    .BuildServiceProvider();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    var command = services.GetRequiredService<NumberGuessingCommand>();
    return await command.ExecuteAsync(cts.Token);
}
catch (OperationCanceledException)
{
    return 130;
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger?.LogError(ex, "Unexpected error");
    Console.Error.WriteLine("Something improbable occurred. Try --diagnostic for more details.");
    return 1;
}