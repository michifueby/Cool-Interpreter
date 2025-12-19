//-----------------------------------------------------------------------
// <copyright file="CoolInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolInterpreter</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Evaluation;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Analysis;
using Cool.Interpreter.Lib.Language.Parsing;

/// <summary>
/// Executes Cool programs by directly interpreting the Cool program.
/// </summary>
public class CoolInterpreter : IInterpreter
{
    /// <summary>
    /// An instance of the <see cref="CoolParserEngine"/> class
    /// responsible for orchestrating the ANTLR-generated lexer and parser
    /// to produce a clean AST (Abstract Syntax Tree) for interpreting Cool programs.
    /// </summary>
    private readonly CoolParserEngine _parser = new();

    /// <summary>
    /// An instance of the <see cref="CoolSemanticAnalyzer"/> class used to perform semantic analysis
    /// on Cool programs. It carries out tasks such as building an inheritance graph,
    /// populating the symbol table, conducting type checking, and detecting semantic errors
    /// during interpretation.
    /// </summary>
    private readonly CoolSemanticAnalyzer _analyzer = new();

    /// <summary>
    /// Interprets and executes a Cool program from the provided source code.
    /// </summary>
    /// <param name="sourceCode">
    /// The source code of the Cool program to be executed.
    /// </param>
    /// <param name="sourceName">
    /// An optional identifier for the source of the code, such as a file name. Defaults to "<string>" if not specified.
    /// </param>
    /// <returns>
    /// An <see cref="InterpretationResult"/> representing the result of the program execution, including diagnostics and output.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided source code is null.
    /// </exception>
    /// <exception cref="NotImplementedException">
    /// Thrown when the execution phase of the interpreter has not been implemented.
    /// </exception>
    public InterpretationResult Run(string sourceCode, string? sourceName = null)
    {
        ArgumentNullException.ThrowIfNull(sourceCode);

        sourceName ??= "<string>";

        // Phase 1: Parse
        var parseResult = _parser.Parse(sourceCode, sourceName);
        if (parseResult.SyntaxTree is null)
        {
            return InterpretationResult.Failure(
                output: string.Empty,
                diagnostics: [..parseResult.Diagnostics],
                returnedValue: null);
        }
        else if (parseResult.HasErrors)
        {
            return InterpretationResult.Failure(
                output: string.Empty,
                diagnostics: [..parseResult.Diagnostics],
                returnedValue: null);
        }

        // Phase 2: Semantic Analysis
        var diagnostics = new DiagnosticBag();
        var semanticResult = _analyzer.Analyze(parseResult.SyntaxTree, diagnostics);

        if (!semanticResult.IsSuccess)
        {
            return InterpretationResult.Failure(
                output: string.Empty,
                diagnostics: [..semanticResult.Diagnostics],
                returnedValue: null);
        }
        
        var symbolTable = semanticResult.SymbolTable
                          ?? throw new InvalidOperationException("Semantic analyzer returned null SymbolTable");
        
        // Phase 3: Build runtime environment with captured I/O
        var outputBuffer = new StringWriter();
        var runtimeEnv = new CoolRuntimeEnvironment(symbolTable)
            .WithOutput(outputBuffer)
            .WithInput(new StringReader("")); // no input by default
        
        CoolObject? returnedValue = null;
        DiagnosticBag executionDiagnostics = new();

        try
        {
            var evaluator = new CoolEvaluator(runtimeEnv);
            returnedValue = evaluator.Evaluate(parseResult.SyntaxTree);
        }
        catch (CoolRuntimeException ex)
        {
            var code = ex.ErrorCode;

            // Fallback for legacy exceptions without specific codes
            if (code is null)
            {
                if (ex.Message.Contains("division by zero")) code = CoolErrorCodes.DivisionByZero;
                else if (ex.Message.Contains("substr")) code = CoolErrorCodes.SubstrOutOfRange;
                else if (ex.Message.Contains("abort")) code = CoolErrorCodes.AbortCalled;
                else code = CoolErrorCodes.RuntimeError;
            }

            executionDiagnostics.ReportError(
                ex.Location ?? SourcePosition.None,
                code,
                ex.Message);
        }
        catch (Exception ex)
        {
            // This should never happen — it's a bug in the interpreter
            executionDiagnostics.ReportError(
                SourcePosition.None,
                CoolErrorCodes.InternalInterpreterError,
                $"Internal interpreter error: {ex.ToString()}");
        }
        
        // Phase 3: Return result
        if (executionDiagnostics.HasErrors)
        {
            // Combine parsing diagnostics with execution diagnostics
            var allDiagnostics = parseResult.Diagnostics
                .Concat(executionDiagnostics.Diagnostics)
                .ToImmutableArray();
            
            return InterpretationResult.Failure(
                output: string.Empty,
                diagnostics: allDiagnostics,
                returnedValue: null);
        }

        string finalOutput = outputBuffer.ToString();
        string resultString = returnedValue switch
        {
            CoolVoid => string.Empty,
            null     => "null",
            _        => returnedValue.ToString()
        };

        return InterpretationResult.Success(
            output: finalOutput,
            returnedValue: resultString);
    }

    /// <summary>
    /// Parses the provided Cool program source code and generates a parse result including the syntax tree and diagnostics.
    /// </summary>
    /// <param name="sourceCode">
    /// The source code of the Cool program to be parsed. Cannot be null.
    /// </param>
    /// <param name="sourceName">
    /// An optional identifier for the source of the code, such as a file name. Defaults to "<string>" if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="ParseResult"/> object containing the syntax tree and diagnostics generated during parsing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided source code is null.
    /// </exception>
    public ParseResult TestParsing(string sourceCode, string? sourceName = null)
    {
        ArgumentNullException.ThrowIfNull(sourceCode);

        sourceName ??= "<string>";

        return _parser.Parse(sourceCode, sourceName);
    }

    /// <summary>
    /// Parses and performs semantic analysis on the provided Cool program source code.
    /// This method is intended for testing semantic analysis independently.
    /// </summary>
    /// <param name="sourceCode">
    /// The source code of the Cool program to be analyzed. Cannot be null.
    /// </param>
    /// <param name="sourceName">
    /// An optional identifier for the source of the code, such as a file name. Defaults to "&lt;string&gt;" if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="SemanticResult"/> object containing the symbol table (if successful) and diagnostics generated during analysis.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided source code is null.
    /// </exception>
    public SemanticResult TestSemantics(string sourceCode, string? sourceName = null)
    {
        ArgumentNullException.ThrowIfNull(sourceCode);

        sourceName ??= "<string>";

        // First parse the source code
        var parseResult = _parser.Parse(sourceCode, sourceName);
        
        // If parsing failed, return failure with parse diagnostics
        if (parseResult.SyntaxTree is null || parseResult.HasErrors)
        {
            return SemanticResult.Failure(parseResult.Diagnostics.ToList());
        }

        // Perform semantic analysis
        var diagnostics = new DiagnosticBag();
        return _analyzer.Analyze(parseResult.SyntaxTree, diagnostics);
    }

    /// <summary>
    /// Interprets and executes a Cool program from a specified file.
    /// </summary>
    /// <param name="filePath">
    /// The path of the file containing the Cool program to be executed.
    /// </param>
    /// <returns>
    /// An <see cref="InterpretationResult"/> representing the result of the program execution.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the file path is null, empty, or consists only of whitespace.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the file at the specified path does not exist.
    /// </exception>
    public InterpretationResult RunFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cool source file not found: {filePath}");

        var sourceCode = File.ReadAllText(filePath);
        return Run(sourceCode, filePath);
    }
}