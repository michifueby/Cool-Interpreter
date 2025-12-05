# Cool-Interpreter

## Concept
```mermaid

flowchart TD
    %% ========================= INPUT =========================
    A[("
    **Source Code**
    cool files
    ")] --> InterpreterFacade

    subgraph Facade ["Facade"]
        direction TB
        InterpreterFacade[CoolInterpreter

        The only class users import]
        
        InterpreterFacade --> Analyzer
        InterpreterFacade --> RuntimeEngine
    end

    subgraph Frontend ["Parsing & Semantic Analysis (Internal)"]
         direction TB
        Analyzer[CoolCodeAnalyzer
        • Coordinates parsing + analysis
        • Used only by CoolInterpreter]

        Analyzer --> Parser

        Parser[CoolParserEngine
        ANTLR wrapper]

        Parser --> Builder
        Builder[AstBuilderVisitor
        ANTLR → Clean AST]

        Builder --> AST[ProgramNode, ClassNode
        IfNode, LetNode, DispatchNode...
        Hand-written, pure C# AST]

        AST --> Semantic
        Semantic[CoolSemanticAnalyzer
        • Inheritance graph
        • Symbol table
        • Type checking
        • SELF_TYPE resolution]

        Semantic --> SemanticResult[SemanticResult
        Symbol table + Diagnostics]
    end

    %% ========================= RUNTIME (Hidden) =========================
    subgraph Runtime ["Runtime – Interpretation (Internal)"]
        direction TB
        RuntimeEngine[ExecutionEngine
        Tree-walk interpreter
        Visitor pattern]

        RuntimeEngine --> Env[CoolEnvironment
        Scopes, self, let bindings]

        RuntimeEngine --> Obj[CoolObject + CoolClass
        Instances, inheritance, dispatch]

        RuntimeEngine --> Val[CoolValue
        Int, String, Bool, Object, Void]

        RuntimeEngine --> Builtins[Built-in Classes
        IO.out_string, String_concat
        Object.abort, type_name]
    end

    %% ========================= DECISION & OUTPUT =========================
    InterpreterFacade --> Decision{Can Execute?}

    Decision -->|Yes| Runtime
    Decision -->|No| DiagOutput

    Runtime --> SuccessOutput[("
    **Program Output**
    Hello from Cool!
    Factorial = 120
    Success
    ")]

    DiagOutput["
    **Diagnostics**
    Hello.cool(12,8): error COOL0001: Undeclared identifier 'x'
    Main.cool(5,3): warning COOLW9001: Unused variable 'temp'
    "]

    SuccessOutput --> FinalOutput
    DiagOutput --> FinalOutput

    FinalOutput["
    **Final Result**
    InterpretationResult
    • Success / Failed
    • Value (optional)
    • Full diagnostics list
    • AST + Symbol table (for IDEs)
    "]

    %% ========================= STYLING =========================
    classDef facade fill:#e6f3ff,stroke:#1890ff,stroke-width:3px,color:#1e3a8a,font-weight:bold
    classDef internal fill:#f9f99,stroke:#666,stroke-dasharray: 6 6
    classDef output fill:#f0df4,stroke:#22c55e,stroke-width:2px
    classDef error fill:#feff2,stroke:#ef4444,stroke-width:2px

    class InterpreterFacade facade
    class Analyzer,Parser,Builder,AST,Semantic,SemanticResult,RuntimeEngine,Env,Obj,Val,Builtins internal
    class SuccessOutput output
    class DiagOutput,FinalOutput error

```

## Facade Usage
```csharp
var interpreter = new CoolInterpreter();

var result = interpreter.RunFile("examples/hello.cool");
Console.WriteLine(result.Output);

var result2 = interpreter.Run("""
    class Main inherits IO {
        main(): Object {
            out_string("Hello, Cool!\n")
        };
    };
    """);
```

