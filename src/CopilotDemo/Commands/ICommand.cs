namespace CopilotDemo.Commands;

public interface ICommand
{
    Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
}