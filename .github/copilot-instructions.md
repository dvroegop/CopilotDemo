# C# Console App Coding Guidelines — Adams Edition

> **DON’T PANIC.** Keep a towel handy. And `dotnet test`.

These guidelines are for a **small** C# console app that aspires to greatness, but is perfectly content with being a delightfully over‑engineered command‑line tool. Accuracy is encouraged, pomposity is mandatory, production‑readiness is optional.

---

## Table of Contents

1. Philosophy (or: Why 42 Is the Default Exit Code)
2. Repository & File Structure
3. Naming Conventions (Mostly Harmless)
4. Program Structure & Flow
5. Configuration & Secrets (Bring Your Own Towel)
6. Logging & Telemetry (Don’t Panic, Just Log)
7. Error Handling (Improbability‐Safe)
8. CLI Design (Human‑Readable, Vogon‑Avoidant)
9. Dependency Injection (Because Of Course)
10. Async & Cancellation (Hitch a Ride With `Task`)
11. Testing Patterns (Infinite Improbability Assertions)
12. Code Style, Analyzers & Linting
13. Git Hygiene (So Long, And Thanks For All The Branches)
14. Packaging & Releases (The Restaurant at the End of CI)
15. Appendix: Example Program Skeleton

---

## 1) Philosophy (or: Why 42 Is the Default Exit Code)

- **Be small, be clear, be funny.** If a choice arises between clever and clear, pick clear—but comment as if you’re auditioning for the Guide.
- **Fail loudly, exit politely.** Non‑zero exit codes are your way of saying “this didn’t go well, but we learned things.”
- **Treat the console as a UI.** Text is an interface; format it like you mean it.
- **Embrace the towel principle.** Keep essentials together: config, docs, and scripts travel as a set.

---

## 2) Repository & File Structure

```
./
├─ .editorconfig                  # Be nice to future you
├─ .gitignore
├─ README.md                      # Quickstart, usage, examples
├─ CONTRIBUTE.md                  # “Share and Enjoy”
├─ build/
│  ├─ pack.ps1                    # Build & pack
│  └─ version.ps1                 # Versioning spell
├─ src/
│  └─ Hitchhike.Cli/              # Main project (console)
│     ├─ Hitchhike.Cli.csproj
│     ├─ Program.cs               # Top‑level program
│     ├─ Composition/             # DI & wiring
│     │  └─ ServiceRegistration.cs
│     ├─ Commands/                # One class per verb
│     │  ├─ GuideCommand.cs
│     │  └─ TowelCommand.cs
│     ├─ Services/                # Domain-ish services
│     │  └─ ProbabilityDrive.cs
│     ├─ Models/                  # Records & DTOs
│     │  └─ HitchhikerOptions.cs
│     ├─ Infrastructure/          # File, HTTP, etc.
│     │  └─ VogonPoetryFilter.cs
│     └─ Utils/                   # Small helpers
│        └─ ConsoleStyler.cs
├─ tests/
│  ├─ Hitchhike.Cli.Tests/        # Unit tests (xUnit)
│  │  ├─ Hitchhike.Cli.Tests.csproj
│  │  └─ ProbabilityDriveTests.cs
│  └─ Hitchhike.Cli.Specs/        # Scenario tests (Verify)
│     └─ GuideCommand.Scenarios.cs
├─ samples/
│  └─ example-config.json
└─ .github/
   └─ workflows/
      └─ ci.yml                   # Build, test, package
```

**Rules of Thumb**

- Keep **one console project** (the thing users run) and push logic into services that are easy to test.
- **One command = one class**. If it grows tentacles, evolve into subcommands.
- Keep test projects mirroring `src` structure.

---

## 3) Naming Conventions (Mostly Harmless)

- **Projects**: `CompanyOrTheme.Product.Feature` → `Hitchhike.Cli`
- **Namespaces** mirror folders.
- **Classes**: `PascalCase` (`ProbabilityDrive`), **Interfaces**: `I`‑prefix (`IProbabilityDrive`).
- **Methods**: `PascalCase` (`Engage`), **parameters**: `camelCase`.
- **Async methods** end with `Async` (unless you are a time lord).
- **Files**: one public type per file, same name as type.
- **Constants**: `SCREAMING_SNAKE_CASE` only if they are truly cosmic.
- **Git branches**: `feat/…`, `fix/…`, `chore/…`, `docs/…`. Example: `feat/improbability-drive`.

**Reserved Words (for fun, not profit)**

- Avoid names like `Zaphod` unless the type actually has two heads.
- Exit codes: `0` = OK, `42` = “worked as intended but philosophically ambiguous”, others = specific failures.

---

## 4) Program Structure & Flow

Use **top‑level statements** or a small `Main` to bootstrap DI and command routing.

```csharp
using Hitchhike.Cli.Composition;
using Hitchhike.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddLogging()
    .AddCommandServices()
    .BuildServiceProvider();

return await services
    .GetRequiredService<CommandRouter>()
    .RouteAsync(args, CancellationToken.None);
```

**Command Router**

- Parse `args` → map to `ICommand` implementation.
- Each command implements `Task<int> ExecuteAsync(CancellationToken)`.
- Shared options live in a `HitchhikerOptions` record.

**Separation of Concerns**

- `Program.cs` knows *wiring*.
- `Commands/*` knows *CLI semantics*.
- `Services/*` knows *domain-ish logic*.
- `Infrastructure/*` knows about *the universe* (file system, HTTP, environment).

---

## 5) Configuration & Secrets (Bring Your Own Towel)

- Use `IConfiguration` with providers in this order: `appsettings.json` → `user‑secrets` (dev) → env vars → `--option` flags.
- **Never** commit secrets. User secrets or environment variables are your towel.
- Example `appsettings.json`:

```json
{
  "Hitchhike": {
    "DefaultPlanet": "Magrathea",
    "AvoidVogonPoetry": true
  }
}
```

- Expose strongly typed options via `IOptions<HitchhikerOptions>`.

---

## 6) Logging & Telemetry (Don’t Panic, Just Log)

- Use `Microsoft.Extensions.Logging`—keep levels sensible:
  - `Trace`/`Debug`: narrative asides.
  - `Information`: milestones (“Engaging Improbability Drive”).
  - `Warning`: improbable but survivable.
  - `Error`: tea has spilled.
  - `Critical`: someone read Vogon poetry aloud.
- Prefer **structured logging**: `logger.LogInformation("Planet {Planet} visited", planetName);`
- Allow `--verbosity` / `-v` flags (`quiet`, `normal`, `detailed`, `diagnostic`).

---

## 7) Error Handling (Improbability‑Safe)

- Exceptions either **bubble** (to top‑level handler) or are **handled with meaning** (wrap, enrich, and rethrow).
- Top‑level handler prints a friendly message (no stack trace unless `--diagnostic`).
- Map exceptions to exit codes. Example:
  - `ArgumentException` → `2`
  - `OperationCanceledException` → `130` (CTRL‑C)
  - `NotImplementedException` → `42` (philosophically ambiguous)

Example top‑level guard:

```csharp
try { /* run app */ }
catch (OperationCanceledException) { return 130; }
catch (Exception ex) {
    logger.LogError(ex, "Unexpected error");
    Console.Error.WriteLine("Something improbable occurred. Try --diagnostic.");
    return 1;
}
```

---

## 8) CLI Design (Human‑Readable, Vogon‑Avoidant)

- Support `--help` and `--version`. If you don’t, may your tea always be cold.
- Prefer **verbs**: `guide`, `towel`, `drive engage`.
- Options use `--long-names` with short aliases `-t`.
- Provide **examples** in help output.
- Use a proven parser (e.g., `System.CommandLine`). If not, document your brave decision.

Example help (abbreviated):

```
Hitchhike CLI
Usage:
  hitchhike guide [--planet <name>] [--avoid-vogon]
  hitchhike towel --pack <item1,item2,...>

Options:
  -p, --planet           Planet to consult the Guide about
  -a, --avoid-vogon      Filter out Vogon poetry (recommended)
  -v, --verbosity        quiet|normal|detailed|diagnostic
  -h, --help             Display help
```

---

## 9) Dependency Injection (Because Of Course)

- Use `Microsoft.Extensions.DependencyInjection`.
- Register interfaces for all services, even if mocking them feels like befriending a Ravenous Bugblatter Beast.
- Composition root (`Composition/ServiceRegistration.cs`) owns lifetimes.

```csharp
public static class ServiceRegistration
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<CommandRouter>()
            .AddTransient<IProbabilityDrive, ProbabilityDrive>()
            .AddTransient<IConsoleStyler, ConsoleStyler>()
            .AddTransient<GuideCommand>()
            .AddTransient<TowelCommand>();
    }
}
```

---

## 10) Async & Cancellation (Hitch a Ride With `Task`)

- Commands expose `ExecuteAsync(CancellationToken)` and honor cancellation.
- Use `await using` and `await foreach` where it saves boilerplate and dignity.
- Wire CTRL‑C: `Console.CancelKeyPress` → `CancellationTokenSource.Cancel()`.

---

## 11) Testing Patterns (Infinite Improbability Assertions)

**Frameworks & Tools**

- **xUnit** (tests), **FluentAssertions** (assertions), **Verify** (snapshot tests), **Bogus** (fake data), **coverlet** (coverage), **JustMock/Moq** (pick your poison).

**Patterns**

- **AAA**: Arrange–Act–Assert. No extra A’s unless you’re screaming.
- **Property‑based tests** (e.g., random inputs remain Non‑Vogon). Use FsCheck or NBomber for scenarios.
- **Golden Master** for CLI output: snapshot stdout/stderr and verify.
- **Characterization tests** before refactors (document the current weirdness).
- **Contract tests** for services that talk to the universe (file system, HTTP) using small fakes.

**Examples**

```csharp
[Fact]
public async Task Engage_ShouldIncreaseProbability()
{
    var drive = new ProbabilityDrive();
    var before = drive.Probability;

    await drive.EngageAsync();

    drive.Probability.Should().BeGreaterThan(before);
}
```

**Test Layout**

```
/tests
  /Hitchhike.Cli.Tests
    ProbabilityDriveTests.cs
    TowelCommandTests.cs
  /Hitchhike.Cli.Specs
    GuideCommand.Scenarios.cs   # snapshot/verify style
```

**Coverage**

- Aim for **meaningful** coverage; 100% is for robots and bureaucrats.
- Guard rails: build fails under 70% *line* coverage in CI.

---

## 12) Code Style, Analyzers & Linting

- Enable **nullable** and **treat warnings as errors** in `.csproj` (live a little).
- Add **.editorconfig** with rules for imports, newlines, and quality of life.
- Add analyzers: `Microsoft.CodeAnalysis.NetAnalyzers`, `StyleCop.Analyzers` (configure to be merely stern, not Vogon).
- Prefer ``, `` for tiny DTOs if appropriate.
- Use `` when the type is obvious, be explicit when it’s not.

`Directory.Build.props` snippet:

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="*" PrivateAssets="all" />
  <PackageReference Include="StyleCop.Analyzers" Version="*" PrivateAssets="all" />
</ItemGroup>
```

---

## 13) Git Hygiene (So Long, And Thanks For All The Branches)

- **One feature per branch**; open small PRs; write commit messages like haikus if you must, but keep the first line under 72 chars.
- Use **Conventional Commits**: `feat: add improbability drive`.
- Protect `main` with CI—no red builds on `main` (that way lies poetry).

---

## 14) Packaging & Releases (The Restaurant at the End of CI)

- CI: build → test → pack → (optional) publish a self‑contained single‑file.
- Provide a `` that prints semantic version and commit hash.
- Changelog: keep a `CHANGELOG.md` with short, human entries.

Minimal `ci.yml` outline:

```yaml
name: ci
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '9.0.x' }
      - run: dotnet restore
      - run: dotnet build --configuration Release --nologo
      - run: dotnet test  --configuration Release --collect:"XPlat Code Coverage"
      - run: dotnet pack  --configuration Release -o artifacts
```

---

## 15) Appendix: Example Program Skeleton

``

```csharp
using Hitchhike.Cli.Composition;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddLogging(builder => builder.AddSimpleConsole(o => o.SingleLine = true))
    .AddCommandServices()
    .BuildServiceProvider();

var router = services.GetRequiredService<CommandRouter>();
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

return await router.RouteAsync(args, cts.Token);
```

``

```csharp
public sealed class GuideCommand : ICommand
{
    private readonly IProbabilityDrive _drive;
    private readonly IConsoleStyler _styler;

    public GuideCommand(IProbabilityDrive drive, IConsoleStyler styler)
        => (_drive, _styler) = (drive, styler);

    public async Task<int> ExecuteAsync(CancellationToken ct)
    {
        _styler.Header("THE HITCHHIKER'S GUIDE TO THE CONSOLE");
        await _drive.EngageAsync(ct);
        Console.WriteLine($"Probability: {_drive.Probability:P2}");
        Console.WriteLine("Tip: Always know where your towel is.");
        return 0; // success, and a nice cup of tea
    }
}
```

``

```csharp
public interface IProbabilityDrive
{
    double Probability { get; }
    Task EngageAsync(CancellationToken ct = default);
}

public sealed class ProbabilityDrive : IProbabilityDrive
{
    private static readonly Random _rng = new();
    public double Probability { get; private set; } = 0.000001;

    public Task EngageAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Probability = Math.Min(1.0, Probability * (1 + _rng.NextDouble() * 4242));
        return Task.CompletedTask;
    }
}
```

``

```csharp
public interface IConsoleStyler
{
    void Header(string text);
}

public sealed class ConsoleStyler : IConsoleStyler
{
    public void Header(string text)
    {
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', text.Length));
    }
}
```

``

```csharp
public class ProbabilityDriveTests
{
    [Fact]
    public async Task Engage_ShouldIncreaseProbability()
    {
        var drive = new ProbabilityDrive();
        var before = drive.Probability;

        await drive.EngageAsync();

        drive.Probability.Should().BeGreaterThan(before);
    }
}
```

---

## Final Notes (Towels, Always Towels)

- Include a `--dry-run` for anything destructive.
- Print helpful errors; link to docs in README.
- Add an `examples/` section so users can copy‑paste their way to glory.
- And remember: if at first you don’t succeed, the universe was simply being *highly* improbable today.

