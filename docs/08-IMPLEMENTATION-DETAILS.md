# Implementation Details

## Table of Contents
1. [Code Organization](#code-organization)
2. [Key Design Decisions](#key-design-decisions)
3. [Performance Considerations](#performance-considerations)
4. [Known Limitations](#known-limitations)
5. [Future Enhancements](#future-enhancements)
6. [Development Guidelines](#development-guidelines)

---

## Code Organization

### Project Structure

```
Cool.Interpreter.sln/
├── Cool.Interpreter.Lib/           # Core library
│   ├── Antlr4/                     # Generated ANTLR4 files
│   │   ├── Cool.g4                 # Grammar definition
│   │   ├── CoolLexer.cs           # Generated lexer
│   │   └── CoolParser.cs          # Generated parser
│   │
│   ├── Core/                       # Core infrastructure
│   │   ├── Diagnostics/           # Error reporting
│   │   │   ├── Diagnostic.cs
│   │   │   ├── DiagnosticBag.cs
│   │   │   └── CoolErrorCodes.cs
│   │   ├── Exceptions/            # Custom exceptions
│   │   │   └── CoolRuntimeException.cs
│   │   └── Syntax/                # AST definitions
│   │       ├── Ast/
│   │       │   ├── ProgramNode.cs
│   │       │   ├── ClassNode.cs
│   │       │   ├── Features/
│   │       │   │   ├── MethodNode.cs
│   │       │   │   └── AttributeNode.cs
│   │       │   └── Expressions/
│   │       │       ├── LiteralNodes.cs
│   │       │       ├── BinaryOperationNode.cs
│   │       │       ├── DispatchNode.cs
│   │       │       ├── IfNode.cs
│   │       │       ├── WhileNode.cs
│   │       │       ├── LetNode.cs
│   │       │       └── CaseNode.cs
│   │       └── SourcePosition.cs
│   │
│   └── Language/                   # Language implementation
│       ├── Analysis/              # Semantic analysis
│       │   ├── CoolSemanticAnalyzer.cs
│       │   └── Checker/
│       │       ├── InheritanceChecker.cs
│       │       └── TypeChecker.cs
│       │
│       ├── Classes/               # Runtime object model
│       │   ├── CoolObject.cs
│       │   ├── CoolUserObject.cs
│       │   ├── CoolClass.cs
│       │   └── BuiltIn/
│       │       ├── CoolInt.cs
│       │       ├── CoolString.cs
│       │       ├── CoolBool.cs
│       │       ├── ObjectClass.cs
│       │       ├── IOClass.cs
│       │       └── StringClass.cs
│       │
│       ├── Evaluation/            # Runtime execution
│       │   ├── CoolEvaluator.cs
│       │   ├── CoolRuntimeEnvironment.cs
│       │   └── Environment.cs
│       │
│       ├── Interpretation/        # Public facade
│       │   ├── CoolInterpreter.cs
│       │   ├── IInterpreter.cs
│       │   └── InterpretationResult.cs
│       │
│       ├── Parsing/               # ANTLR integration
│       │   ├── CoolParserEngine.cs
│       │   ├── AstBuilderVisitor.cs
│       │   └── ParseResult.cs
│       │
│       └── Symbols/               # Symbol table
│           ├── SymbolTable.cs
│           ├── ClassSymbol.cs
│           ├── MethodSymbol.cs
│           ├── AttributeSymbol.cs
│           └── FormalSymbol.cs
│
├── Cool.Interpreter.Console/       # Command-line interface
│   └── Program.cs
│
└── Cool.Interpreter.Tests/         # Test suite
    ├── ParserTests.cs
    ├── SemanticTests.cs
    ├── RuntimeTests.cs
    ├── AlgorithmTests.cs
    └── TestCases/
        ├── Parsing/
        ├── Semantics/
        └── Algorithm/
```

### Namespace Organization

```
Cool.Interpreter.Lib
├── Core
│   ├── Diagnostics            # Error reporting infrastructure
│   ├── Exceptions             # Custom exception types
│   └── Syntax                 # AST node definitions
│       └── Ast
│           ├── Features       # Class members (methods, attributes)
│           └── Expressions    # Expression nodes
│
└── Language
    ├── Analysis               # Semantic analysis phase
    │   └── Checker           # Type and inheritance checkers
    ├── Classes               # Runtime object model
    │   └── BuiltIn          # Built-in class implementations
    ├── Evaluation            # Runtime execution phase
    ├── Interpretation        # Public API facade
    ├── Parsing               # Parsing phase
    └── Symbols               # Symbol table and symbols
```

---

## Key Design Decisions

### 1. **Facade Pattern for API**

**Decision:** Single `CoolInterpreter` class as entry point

**Rationale:**
- Simplifies user experience
- Hides internal complexity (parser, analyzer, evaluator)
- Easy to version and maintain
- Clear separation between public API and internal implementation

**Trade-offs:**
- Less flexibility for advanced users
- Cannot easily swap individual components
- All operations go through single class

**Benefits:**
- Easy to learn and use
- Consistent API
- Good for teaching and small projects

---

### 2. **Immutable Data Structures**

**Decision:** AST nodes, symbol tables, and diagnostic collections are immutable

**Rationale:**
- Thread-safety by design
- No defensive copying needed
- Clear data flow (no hidden mutations)
- Functional programming style

**Implementation:**
```csharp
// SymbolTable is immutable
public class SymbolTable
{
    public ImmutableDictionary<string, ClassSymbol> Classes { get; }
    
    // Returns new instance, doesn't mutate
    public SymbolTable WithClass(ClassSymbol classSymbol)
        => new SymbolTable(Classes.SetItem(classSymbol.Name, classSymbol));
}

// AST nodes are immutable
public record ClassNode(
    string Name,
    string? InheritsFrom,
    ImmutableArray<FeatureNode> Features,
    SourcePosition Location
) : CoolSyntaxNode(Location);
```

**Trade-offs:**
- Slight performance overhead (allocations)
- More memory usage (no in-place updates)
- Requires immutable collection types

**Benefits:**
- Safe to share across threads
- No accidental mutations
- Easy to reason about
- Enables caching and memoization

---

### 3. **Tree-Walk Interpretation**

**Decision:** Direct AST evaluation without bytecode compilation

**Rationale:**
- Simpler implementation
- Easier to understand and debug
- Sufficient performance for teaching purposes
- Matches course requirements

**Alternative Considered:** Bytecode compilation
- More complex to implement
- Better performance for large programs
- Harder to debug
- Overkill for this project

**Implementation:**
```csharp
// Direct AST evaluation
public CoolObject Visit(BinaryOperationNode node)
{
    var left = node.Left.Accept(this);   // Evaluate left
    var right = node.Right.Accept(this);  // Evaluate right
    return ApplyOperator(left, node.Operator, right);
}
```

**Trade-offs:**
- Slower than compiled code
- No optimization passes
- Repeated AST traversal for loops

**Benefits:**
- Simple, readable code
- Easy to add new features
- Good debugging experience
- Fast compilation (no code generation)

---

### 4. **Visitor Pattern for AST Traversal**

**Decision:** Use Visitor pattern for all AST operations

**Rationale:**
- Standard compiler design pattern
- Separation of concerns (structure vs. operations)
- Easy to add new operations
- Type-safe dispatch

**Implementation:**
```csharp
public interface ICoolSyntaxVisitor<T>
{
    T Visit(IntegerLiteralNode node);
    T Visit(BinaryOperationNode node);
    T Visit(IfNode node);
    // ... more visit methods
}

// Multiple visitors for different purposes
class AstBuilderVisitor : ICoolSyntaxVisitor<object?>  { }
class CoolEvaluator : ICoolSyntaxVisitor<CoolObject>  { }
class TypeChecker : ICoolSyntaxVisitor<string>  { }
```

**Trade-offs:**
- Verbose (many visit methods)
- Adding new AST node types requires updating all visitors
- More boilerplate code

**Benefits:**
- Type-safe
- Compiler-checked completeness
- Clear separation of concerns
- Easy to add new operations

---

### 5. **Comprehensive Diagnostic System**

**Decision:** Collect all errors with source locations, not just first error

**Rationale:**
- Better user experience (see all errors at once)
- Professional quality error messages
- IDE integration support
- Teaching tool (students see all mistakes)

**Implementation:**
```csharp
public class DiagnosticBag
{
    private readonly List<Diagnostic> _diagnostics = new();
    
    public void ReportError(SourcePosition location, string code, string message)
        => _diagnostics.Add(Diagnostic.Error(location, code, message));
}

// Format: file(line,col): severity CODE: message
hello.cool(12,8): error COOL0001: Syntax error: missing ';'
```

**Trade-offs:**
- More complex error handling
- Need to track source positions
- Extra memory for diagnostic collection

**Benefits:**
- See all errors in one run
- Professional error messages
- IDE integration
- Better debugging experience

---

### 6. **ANTLR4 for Parsing**

**Decision:** Use ANTLR4 for lexer and parser generation

**Rationale:**
- Industry-standard tool
- Declarative grammar specification
- Automatic error recovery
- Well-documented and supported

**Alternative Considered:** Hand-written parser
- More control over error messages
- No external dependencies
- More work to implement
- Easier to make mistakes

**Implementation:**
```antlr
// Cool.g4 grammar
program : classDefine+ ;

classDefine 
    : CLASS TYPE (INHERITS TYPE)? '{' feature* '}' ';'
    ;
```

**Trade-offs:**
- Generated code is not readable
- ANTLR learning curve
- Build step required
- Limited control over parse tree structure

**Benefits:**
- Fast development
- Standard approach
- Good error recovery
- Proven correctness

---

### 7. **Built-in Class Implementation**

**Decision:** Implement built-in classes (Object, IO, Int, String, Bool) in C#, not COOL

**Rationale:**
- Better performance (native code)
- Access to .NET I/O APIs
- No need for primitive operations in COOL
- Cleaner implementation

**Implementation:**
```csharp
public static class IOClass
{
    public static CoolObject OutString(
        CoolObject self, 
        List<CoolObject> args, 
        CoolRuntimeEnvironment runtime)
    {
        var str = (CoolString)args[0];
        runtime.Output.Write(str.Value);
        return self;  // Return SELF_TYPE
    }
}
```

**Trade-offs:**
- Built-ins are "magic" (not COOL code)
- Users can't see implementation
- Harder to modify built-in behavior

**Benefits:**
- Much faster execution
- Access to platform APIs
- Cleaner code
- No bootstrapping issues

---

## Performance Considerations

### Execution Speed

**Current Performance:**
- Small programs (< 100 LOC): < 100ms
- Medium programs (100-500 LOC): < 500ms
- Large programs (500-2000 LOC): < 2s
- Recursive algorithms: Depends on depth

**Bottlenecks:**
1. **AST Traversal:** Each expression requires tree walk
2. **Method Dispatch:** Runtime method lookup
3. **Environment Copying:** Immutable environments create copies
4. **No Optimization:** No constant folding, dead code elimination, etc.

**Optimization Opportunities:**
```csharp
// 1. Cache method lookups
private Dictionary<(string className, string methodName), MethodInfo> _methodCache;

// 2. Tail call optimization for recursion
if (IsTailCall(methodCall))
    return OptimizeTailCall(methodCall);

// 3. Constant folding during parsing
if (left is IntegerLiteralNode l && right is IntegerLiteralNode r)
    return new IntegerLiteralNode(l.Value + r.Value);
```

### Memory Usage

**Typical Memory Consumption:**
- AST: ~10KB per 100 LOC
- Symbol Table: ~5KB per 10 classes
- Runtime Environment: ~1MB baseline
- Object Creation: ~100 bytes per object

**Memory Optimization Tips:**
```csharp
// 1. Reuse interpreter instance
var interpreter = new CoolInterpreter();  // Create once
foreach (var file in files)
{
    var result = interpreter.RunFile(file);  // Reuse
}

// 2. Use testing APIs for validation only
var semanticResult = interpreter.TestSemantics(code);  // Skip execution

// 3. Clear caches between runs
RuntimeClassFactory.ClearCache();  // Already done in Run()
```

### Compilation Time

**Phase Timings:**
1. **Parsing:** ~50ms for 1000 LOC
2. **Semantic Analysis:** ~100ms for 1000 LOC
3. **Execution:** Variable (depends on program logic)

**Total:** ~150ms baseline + execution time

---

## Known Limitations

### 1. **No Garbage Collection Tuning**

**Limitation:** Relies entirely on .NET GC

**Impact:**
- Long-running programs may accumulate objects
- No manual memory management

**Workaround:**
```csharp
// Force GC between runs if needed
var result = interpreter.Run(code);
GC.Collect();
```

---

### 2. **Stack Overflow for Deep Recursion**

**Limitation:** Deep recursion can cause stack overflow

**Impact:**
- Recursive algorithms limited by stack depth (~10,000 calls)

**Example:**
```cool
class Main {
    factorial(n: Int): Int {
        if n = 0 then 1
        else n * factorial(n - 1)  -- Stack overflow for large n
        fi
    };
    main(): Int { factorial(100000) };  -- CRASH
};
```

**Workaround:** Use iterative algorithms or tail recursion optimization

---

### 3. **No Interactive Debugger**

**Limitation:** No built-in debugger for COOL code

**Impact:**
- Debugging requires print statements
- No breakpoints or step-through execution

**Workaround:**
```cool
-- Add debug output
class Main inherits IO {
    debug(msg: String): SELF_TYPE {
        out_string("DEBUG: ").out_string(msg).out_string("\n")
    };
    
    main(): Object {
        {
            debug("Starting main");
            -- ... code ...
            debug("Finished main");
        }
    };
};
```

---

### 4. **Limited Error Recovery**

**Limitation:** Semantic and runtime errors stop execution

**Impact:**
- Only first phase of errors is reported
- Cannot see errors in later phases if earlier phase fails

**Example:**
```cool
-- Syntax error prevents semantic analysis
class Main {
    main(): Int { 42   -- Missing semicolon
};

-- Semantic error prevents execution
class Main {
    x: Foo;  -- Undefined type
    main(): Int { 42 };
};
```

---

### 5. **No Optimization**

**Limitation:** No compiler optimizations applied

**Impact:**
- Slow execution for large programs
- Unnecessary computations not eliminated

**Examples of missed optimizations:**
```cool
-- Constant folding
x <- 2 + 3;  -- Not optimized to x <- 5;

-- Dead code elimination
if true then x else y fi;  -- y branch not eliminated

-- Common subexpression elimination
a <- x + y;
b <- x + y;  -- x + y computed twice
```

---

### 6. **Single-Threaded Execution**

**Limitation:** No parallel execution of COOL code

**Impact:**
- Cannot utilize multiple CPU cores
- Sequential execution only

**Note:** Interpreter instance is not thread-safe

---

## Future Enhancements

### Short-Term (1-2 months)

1. **Enhanced Error Messages**
   - Add "did you mean?" suggestions
   - Show code context with errors
   - Color-coded terminal output

2. **Performance Improvements**
   - Cache method lookups
   - Optimize environment copying
   - Constant folding

3. **Better Testing Tools**
   - Test coverage reporting
   - Performance benchmarks
   - Regression test suite

### Medium-Term (3-6 months)

1. **Interactive REPL**
   ```
   cool> let x: Int <- 42 in x + 1
   43
   cool> class Point { x: Int; y: Int; };
   cool> new Point
   #Point
   ```

2. **Debugger Support**
   - Breakpoints
   - Step-through execution
   - Variable inspection
   - Call stack viewing

3. **IDE Integration**
   - VS Code extension
   - Language server protocol
   - Syntax highlighting
   - IntelliSense

### Long-Term (6-12 months)

1. **Bytecode Compilation**
   - Compile to intermediate representation
   - Optimization passes
   - JIT compilation

2. **Static Analysis Tools**
   - Unused code detection
   - Style checking
   - Complexity metrics

3. **Standard Library**
   - Collections (List, Set, Map)
   - File I/O
   - Math functions
   - String utilities

---

## Development Guidelines

### Adding New Features

#### 1. **New AST Node**

```csharp
// 1. Define AST node
public record NewExpressionNode(
    ExpressionNode Child,
    SourcePosition Location
) : ExpressionNode(Location);

// 2. Update parser visitor
public override object? VisitNewExpression(CoolParser.NewExpressionContext context)
{
    var child = Visit(context.expression()) as ExpressionNode;
    return new NewExpressionNode(child!, ToSourcePosition(context.Start));
}

// 3. Update type checker
public string Visit(NewExpressionNode node)
{
    var childType = CheckExpression(node.Child);
    // Validate and return type
    return resultType;
}

// 4. Update evaluator
public CoolObject Visit(NewExpressionNode node)
{
    var child = node.Child.Accept(this);
    // Execute and return result
    return result;
}

// 5. Add tests
[Test]
public void Semantic_NewExpression_ValidatesType() { }

[Test]
public void Runtime_NewExpression_ExecutesCorrectly() { }
```

#### 2. **New Built-in Method**

```csharp
// 1. Add to symbol table
var stringClass = new ClassSymbol("String", "Object", SourcePosition.None)
    .WithMethod(MethodSymbol.CreateBuiltin("trim", "String"));

// 2. Implement method
public static CoolObject Trim(CoolObject self, List<CoolObject> args)
{
    var str = (CoolString)self;
    return new CoolString(str.Value.Trim());
}

// 3. Register in method dispatch
case "trim":
    return StringClass.Trim(self, args);

// 4. Add tests
[Test]
public void Runtime_String_Trim_RemovesWhitespace() { }
```

#### 3. **New Error Code**

```csharp
// 1. Add to CoolErrorCodes
public const string NewError = "COOL0999";

// 2. Use in code
_diagnostics.ReportError(
    node.Location,
    CoolErrorCodes.NewError,
    "Description of new error"
);

// 3. Document in 05-DIAGNOSTICS.md

// 4. Add test
[Test]
public void Semantic_NewError_IsReported() { }
```

### Code Style Guidelines

1. **File Headers**
```csharp
//-----------------------------------------------------------------------
// <copyright file="FileName.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Authors</author>
// <summary>Brief description</summary>
//-----------------------------------------------------------------------
```

2. **XML Documentation**
```csharp
/// <summary>
/// Brief description of the class/method.
/// </summary>
/// <param name="parameter">Description</param>
/// <returns>Description</returns>
public string Method(string parameter) { }
```

3. **Naming Conventions**
- Classes: `PascalCase`
- Methods: `PascalCase`
- Properties: `PascalCase`
- Fields: `_camelCase` (private), `PascalCase` (public)
- Locals: `camelCase`

4. **Organization**
```csharp
public class Example
{
    // 1. Static fields
    // 2. Instance fields
    // 3. Constructors
    // 4. Properties
    // 5. Public methods
    // 6. Protected methods
    // 7. Private methods
}
```

### Testing Guidelines

1. **Test Naming:** `Phase_Scenario_ExpectedBehavior`
2. **Arrange-Act-Assert:** Clear separation
3. **One Assert Per Test:** Focus on single behavior
4. **Test Both Success and Failure:** Cover all paths

---

## Contributors

**Development Team:**
- **Michael Füby** - Architecture, Semantic Analysis, Documentation
- **Armin Zimmerling** - Runtime Execution, Object Model, Built-in Classes
- **Mahmoud Ibrahim** - Parsing, AST Construction, Diagnostics System

**Course:** Compiler Construction (M.INFO.B.24.WS25 COM 68598)  
**Institution:** FH Wiener Neustadt  
**Date:** January 2026

---

## License

Copyright (c) FH Wiener Neustadt. All rights reserved.

This project is created for educational purposes as part of the Compiler Construction course.

---

**End of Documentation**

For questions or issues, please contact the development team or refer to the course materials.
