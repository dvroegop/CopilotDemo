# CopilotDemo
Een demo van Github Copilot

## Number Guessing Game

A console application that demonstrates proper C# coding guidelines and architecture patterns.

### Usage

```bash
dotnet run --project src/CopilotDemo
```

### CLI Options

- `--help` - Display help and usage information
- `--version` - Show version information  
- `--verbosity` or `-v` - Set verbosity level (quiet|normal|detailed|diagnostic)

### Examples

```bash
# Play the game with normal verbosity
dotnet run --project src/CopilotDemo

# Play with detailed logging
dotnet run --project src/CopilotDemo -- --verbosity detailed

# Show help
dotnet run --project src/CopilotDemo -- --help
```

### Architecture

This project follows C# Console App coding guidelines with:

- **Dependency Injection** using Microsoft.Extensions.DependencyInjection
- **Command Pattern** for separating CLI concerns from business logic  
- **Async/Cancellation** support with proper CTRL-C handling
- **Proper Error Handling** with meaningful exit codes
- **Structured Logging** with configurable verbosity levels
- **CLI Argument Parsing** using System.CommandLine

### Project Structure

```
src/CopilotDemo/
├── Commands/           # Command implementations
├── Services/           # Business logic services  
├── Models/             # Data models and DTOs
├── Composition/        # Dependency injection setup
└── Program.cs          # Application entry point

tests/CopilotDemo.Tests/
└── *Tests.cs           # Unit tests
```

### Building and Testing

```bash
# Build the solution
dotnet build

# Run all tests  
dotnet test

# Run the application
dotnet run --project src/CopilotDemo
```
