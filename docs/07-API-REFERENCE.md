# API Reference

## Table of Contents
1. [Public API](#public-api)
2. [CoolInterpreter](#coolinterpreter)
3. [InterpretationResult](#interpretationresult)
4. [Testing API](#testing-api)
5. [Extension Points](#extension-points)

---

## Public API

The COOL Interpreter provides a clean, minimal public API through the **Facade pattern**. Users interact primarily with the `CoolInterpreter` class.

### Namespace Structure

```
Cool.Interpreter.Lib
├── Language.Interpretation    # Public API
│   ├── CoolInterpreter        # Main entry point
│   ├── IInterpreter           # Interface
│   └── InterpretationResult   # Result type
└── Core.Diagnostics           # Public diagnostic types
    └── Diagnostic             # Error/warning information
```

---

## CoolInterpreter

### Class Declaration

```csharp
namespace Cool.Interpreter.Lib.Language.Interpretation;

public class CoolInterpreter : IInterpreter
```

### Constructor

```csharp
public CoolInterpreter()
```

Creates a new interpreter instance with default configuration.

**Example:**
```csharp
var interpreter = new CoolInterpreter();
```

---

### Run Method

#### Run from String

```csharp
public InterpretationResult Run(string sourceCode, string? sourceName = null)
```

Interprets COOL source code and returns the execution result.

**Parameters:**
- `sourceCode` (string): The COOL program source code
- `sourceName` (string?, optional): Name of the source (e.g., file name) for error reporting. Defaults to `"<string>"`

**Returns:** `InterpretationResult`

**Throws:** `ArgumentNullException` if `sourceCode` is null

**Example:**
```csharp
var interpreter = new CoolInterpreter();

var result = interpreter.Run("""
    class Main inherits IO {
        main(): Object {
            out_string("Hello, COOL!\n")
        };
    };
    """);

if (result.IsSuccess)
{
    Console.WriteLine($"Output: {result.Output}");
}
else
{
    foreach (var diagnostic in result.Diagnostics)
    {
        Console.WriteLine(diagnostic);
    }
}
```

#### Run from File

```csharp
public InterpretationResult RunFile(string filePath)
```

Reads and interprets a COOL program from a file.

**Parameters:**
- `filePath` (string): Path to the .cool file

**Returns:** `InterpretationResult`

**Throws:** 
- `ArgumentNullException` if `filePath` is null
- `FileNotFoundException` if file doesn't exist
- `IOException` if file cannot be read

**Example:**
```csharp
var interpreter = new CoolInterpreter();
var result = interpreter.RunFile("examples/hello.cool");

Console.WriteLine(result.Output);
```

---

### Testing Methods

These methods are provided for unit testing individual phases:

#### TestParsing

```csharp
public ParseResult TestParsing(string sourceCode, string? sourceName = null)
```

Tests only the parsing phase without semantic analysis or execution.

**Parameters:**
- `sourceCode` (string): COOL source code to parse
- `sourceName` (string?, optional): Source identifier for diagnostics

**Returns:** `ParseResult` containing AST and parse diagnostics

**Example:**
```csharp
var result = interpreter.TestParsing(sourceCode);

if (!result.HasErrors)
{
    Console.WriteLine("Parsing successful!");
    Console.WriteLine($"Classes found: {result.SyntaxTree.Classes.Count}");
}
```

#### TestSemantics

```csharp
public SemanticResult TestSemantics(string sourceCode, string? sourceName = null)
```

Tests parsing and semantic analysis without execution.

**Parameters:**
- `sourceCode` (string): COOL source code to analyze
- `sourceName` (string?, optional): Source identifier for diagnostics

**Returns:** `SemanticResult` containing symbol table and semantic diagnostics

**Example:**
```csharp
var result = interpreter.TestSemantics(sourceCode);

if (result.IsSuccess)
{
    Console.WriteLine("Semantic analysis passed!");
    Console.WriteLine($"Classes defined: {result.SymbolTable.Classes.Count}");
}
```

---

## InterpretationResult

### Class Declaration

```csharp
namespace Cool.Interpreter.Lib.Language.Interpretation;

public class InterpretationResult
```

Represents the complete result of interpreting a COOL program.

### Properties

#### IsSuccess

```csharp
public bool IsSuccess { get; }
```

Indicates whether the interpretation completed successfully without errors.

**Example:**
```csharp
if (result.IsSuccess)
{
    // Process successful execution
}
else
{
    // Handle errors
}
```

#### Output

```csharp
public string Output { get; }
```

Captured output from I/O operations (`out_string`, `out_int`).

**Example:**
```csharp
Console.WriteLine($"Program output:\n{result.Output}");
```

#### ReturnedValue

```csharp
public CoolObject? ReturnedValue { get; }
```

The value returned by the `main()` method. `null` if execution failed.

**Example:**
```csharp
if (result.ReturnedValue is CoolInt intValue)
{
    Console.WriteLine($"Returned: {intValue.Value}");
}
```

#### Diagnostics

```csharp
public ImmutableArray<Diagnostic> Diagnostics { get; }
```

Collection of all errors, warnings, and informational messages generated during interpretation.

**Example:**
```csharp
foreach (var diagnostic in result.Diagnostics)
{
    Console.WriteLine($"{diagnostic.Severity}: {diagnostic.Message}");
}
```

#### HasErrors

```csharp
public bool HasErrors => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
```

Indicates whether any errors occurred (convenience property).

---

### Factory Methods

#### Success

```csharp
public static InterpretationResult Success(
    string output, 
    ImmutableArray<Diagnostic> diagnostics, 
    CoolObject? returnedValue)
```

Creates a successful interpretation result.

#### Failure

```csharp
public static InterpretationResult Failure(
    string output, 
    ImmutableArray<Diagnostic> diagnostics, 
    CoolObject? returnedValue)
```

Creates a failed interpretation result.

---

## Diagnostic

### Class Declaration

```csharp
namespace Cool.Interpreter.Lib.Core.Diagnostics;

public class Diagnostic
```

Represents a single error, warning, or informational message.

### Properties

#### Location

```csharp
public SourcePosition Location { get; }
```

Source location where the diagnostic was generated.

#### Severity

```csharp
public DiagnosticSeverity Severity { get; }
```

Severity level: `Error`, `Warning`, `Info`, or `Internal`.

#### Code

```csharp
public string Code { get; }
```

Diagnostic code (e.g., `"COOL0102"`).

#### Message

```csharp
public string Message { get; }
```

Human-readable description of the diagnostic.

### Methods

#### ToString

```csharp
public override string ToString()
```

Formats the diagnostic as: `<file>(<line>,<col>): <severity> <code>: <message>`

**Example Output:**
```
hello.cool(12,8): error COOL0001: Syntax error: missing ';'
```

---

## ParseResult

### Class Declaration

```csharp
namespace Cool.Interpreter.Lib.Language.Parsing;

public class ParseResult
```

Result of the parsing phase.

### Properties

```csharp
public ProgramNode? SyntaxTree { get; }
public ImmutableArray<Diagnostic> Diagnostics { get; }
public bool HasErrors { get; }
```

---

## SemanticResult

### Class Declaration

```csharp
namespace Cool.Interpreter.Lib.Language.Analysis;

public class SemanticResult
```

Result of semantic analysis.

### Properties

```csharp
public bool IsSuccess { get; }
public SymbolTable? SymbolTable { get; }
public ImmutableArray<Diagnostic> Diagnostics { get; }
```

---

## Usage Examples

### Basic Execution

```csharp
using Cool.Interpreter.Lib.Language.Interpretation;

var interpreter = new CoolInterpreter();

var result = interpreter.Run("""
    class Main {
        main(): Int { 42 };
    };
    """);

Console.WriteLine($"Success: {result.IsSuccess}");
Console.WriteLine($"Returned: {result.ReturnedValue}");
```

### Error Handling

```csharp
var result = interpreter.Run(sourceCode);

if (!result.IsSuccess)
{
    Console.WriteLine("Compilation/execution failed:");
    
    foreach (var diagnostic in result.Diagnostics)
    {
        var color = diagnostic.Severity == DiagnosticSeverity.Error 
            ? ConsoleColor.Red 
            : ConsoleColor.Yellow;
        
        Console.ForegroundColor = color;
        Console.WriteLine(diagnostic);
        Console.ResetColor();
    }
}
```

### I/O Redirection

```csharp
var sourceCode = """
    class Main inherits IO {
        main(): Object {
            {
                out_string("Enter your name: ");
                let name: String <- in_string() in
                    out_string("Hello, ").out_string(name).out_string("!\n");
            }
        };
    };
    """;

// Create a custom runtime environment with redirected I/O
var interpreter = new CoolInterpreter();
var result = interpreter.Run(sourceCode);

// Output is captured in result.Output
Console.WriteLine(result.Output);
```

### Accessing Symbol Table

```csharp
var semanticResult = interpreter.TestSemantics(sourceCode);

if (semanticResult.IsSuccess)
{
    var symbolTable = semanticResult.SymbolTable;
    
    foreach (var classSymbol in symbolTable.Classes.Values)
    {
        Console.WriteLine($"Class: {classSymbol.Name}");
        Console.WriteLine($"  Parent: {classSymbol.ParentName ?? "Object"}");
        Console.WriteLine($"  Methods: {classSymbol.Methods.Count}");
        Console.WriteLine($"  Attributes: {classSymbol.Attributes.Count}");
    }
}
```

### Inspecting AST

```csharp
var parseResult = interpreter.TestParsing(sourceCode);

if (!parseResult.HasErrors && parseResult.SyntaxTree != null)
{
    var program = parseResult.SyntaxTree;
    
    foreach (var classNode in program.Classes)
    {
        Console.WriteLine($"Class {classNode.Name}");
        
        foreach (var feature in classNode.Features)
        {
            if (feature is MethodNode method)
            {
                Console.WriteLine($"  Method: {method.Name}");
                Console.WriteLine($"    Returns: {method.ReturnType}");
                Console.WriteLine($"    Parameters: {method.Formals.Count}");
            }
            else if (feature is AttributeNode attribute)
            {
                Console.WriteLine($"  Attribute: {attribute.Name}: {attribute.Type}");
            }
        }
    }
}
```

---

## Extension Points

### Custom Error Listeners

While not exposed in the public API, advanced users can extend the interpreter by:

1. **Custom Diagnostic Handlers:**
```csharp
// Filter or transform diagnostics
var result = interpreter.Run(sourceCode);
var errors = result.Diagnostics
    .Where(d => d.Severity == DiagnosticSeverity.Error)
    .OrderBy(d => d.Location.Line);
```

2. **Wrapping the Interpreter:**
```csharp
public class CoolInterpreterWithLogging : IInterpreter
{
    private readonly CoolInterpreter _inner = new();
    private readonly ILogger _logger;
    
    public InterpretationResult Run(string sourceCode, string? sourceName = null)
    {
        _logger.LogInformation($"Interpreting {sourceName}");
        
        var result = _inner.Run(sourceCode, sourceName);
        
        if (result.HasErrors)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                _logger.LogError(diagnostic.ToString());
            }
        }
        
        return result;
    }
}
```

---

## Thread Safety

**Important:** `CoolInterpreter` instances are **NOT thread-safe**. Each thread should create its own interpreter instance.

**Safe Usage:**
```csharp
// Option 1: One interpreter per thread
Parallel.ForEach(files, file => {
    var interpreter = new CoolInterpreter();  // New instance
    var result = interpreter.RunFile(file);
    // Process result
});

// Option 2: Lock around interpreter usage
var interpreter = new CoolInterpreter();
var lockObj = new object();

Parallel.ForEach(files, file => {
    lock (lockObj)
    {
        var result = interpreter.RunFile(file);
        // Process result
    }
});
```

---

## Performance Considerations

### Memory Usage

- AST and symbol table are immutable and can be reused
- Each `Run()` creates a new runtime environment
- Large programs may require significant heap space

### Execution Speed

- Tree-walk interpretation (not bytecode compilation)
- Suitable for small to medium programs
- Recursive algorithms may be slow for deep recursion

### Optimization Tips

```csharp
// Reuse interpreter instance for multiple runs
var interpreter = new CoolInterpreter();

foreach (var file in files)
{
    var result = interpreter.RunFile(file);  // Reuses parser
    // Process result
}

// For testing, use phase-specific methods
var semanticResult = interpreter.TestSemantics(code);  // Faster than Run()
```

---

## Version Compatibility

**Current Version:** 1.0.0

**API Stability:**
- ✅ Stable: `CoolInterpreter`, `InterpretationResult`, `Diagnostic`
- ⚠️ Internal: AST nodes, symbol table, evaluator (may change)

**Breaking Changes Policy:**
- Major version changes may break API
- Minor/patch versions maintain backward compatibility

---

**Next:** Continue to [08-IMPLEMENTATION-DETAILS.md](08-IMPLEMENTATION-DETAILS.md) for implementation specifics.
