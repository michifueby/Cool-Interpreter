//-----------------------------------------------------------------------
// <copyright file="CoolInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolInterpreter</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
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
    /// The Cool source code to interpret and execute.
    /// </param>
    /// <param name="sourceName">
    /// The name of the source, used for diagnostic messages. If null, a default value of "<string>" will be used.
    /// </param>
    /// <returns>
    /// An <see cref="InterpretationResult"/> representing the outcome of the interpretation and execution process, including diagnostics, output, and any returned value from the Cool program.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="sourceCode"/> is null.
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
                returnedValue: null,
                diagnostics: parseResult.Diagnostics);
        }

        // Phase 2: Semantic Analysis
        var diagnostics = new DiagnosticBag();
        var semanticResult = _analyzer.Analyze(parseResult.SyntaxTree, diagnostics);

        if (!semanticResult.IsSuccess)
        {
            return InterpretationResult.Failure(
                output: string.Empty,
                returnedValue: null,
                diagnostics: semanticResult.Diagnostics);
        }

        // Phase 3: Execute
        // throw new NotImplementedException();
        
        // Temporary stub implementation for execution phase
        var output = "Execution phase is not yet implemented.";
        return InterpretationResult.Success(
            output: output,
            returnedValue: null);
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