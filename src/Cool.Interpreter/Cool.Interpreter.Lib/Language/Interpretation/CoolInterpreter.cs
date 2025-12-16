//-----------------------------------------------------------------------
// <copyright file="CoolInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolInterpreter</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

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
        catch (CoolRuntimeException ex) when (ex.Message.Contains("division by zero"))
        {
            executionDiagnostics.ReportError(
                SourcePosition.None,
                ex.Message,
                CoolErrorCodes.DivisionByZero);
        }
        catch (CoolRuntimeException ex) when (ex.Message.Contains("substr"))
        {
            executionDiagnostics.ReportError(
                SourcePosition.None,
                ex.Message,
                CoolErrorCodes.SubstrOutOfRange);
        }
        catch (CoolRuntimeException ex) when (ex.Message.Contains("abort"))
        {
            executionDiagnostics.ReportError(
                SourcePosition.None,
                ex.Message,
                CoolErrorCodes.AbortCalled);
        }
        catch (CoolRuntimeException ex)
        {
            // Generic runtime error from user code
            executionDiagnostics.ReportError(
                SourcePosition.None,
                ex.Message,
                CoolErrorCodes.RuntimeError);
        }
        catch (Exception ex)
        {
            // This should never happen — it's a bug in the interpreter
            executionDiagnostics.ReportError(
                SourcePosition.None,
                $"Internal interpreter error: {ex.Message}",
                CoolErrorCodes.InternalInterpreterError);
        }
        
        // Phase 3: Return result
        if (executionDiagnostics.HasErrors)
        {
            return InterpretationResult.Failure(
                output: string.Empty,
                diagnostics: [..parseResult.Diagnostics],
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