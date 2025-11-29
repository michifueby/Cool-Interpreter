//-----------------------------------------------------------------------
// <copyright file="CoolErrorListener.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolErrorListener</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Diagnostics;

using Antlr4.Runtime;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a listener that handles syntax and lexer errors encountered during the
/// parsing or lexical analysis phases of execution. This listener collects diagnostic
/// information about the errors and records them in a diagnostic bag for further processing.
/// </summary>
public class CoolErrorListener : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
{
    /// <summary>
    /// Represents the name of the source file or input being processed. This value is used to
    /// associate syntax and lexer errors with their originating source, enabling detailed diagnostic
    /// reporting for errors encountered during analysis or parsing.
    /// </summary>
    private readonly string _sourceName;

    /// <summary>
    /// Stores diagnostic information, such as errors, warnings, and other messages,
    /// encountered during the lexical and syntax analysis phases. This collection is used
    /// to track and report issues related to the parsing process, enabling detailed
    /// error handling and debugging.
    /// </summary>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoolErrorListener"/> class.
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="diagnostics"></param>
    public CoolErrorListener(string sourceName, DiagnosticBag diagnostics)
    {
        _sourceName   = sourceName;
        _diagnostics  = diagnostics;
    }

    /// <summary>
    /// Reports a syntax error encountered during parsing or lexing and adds a diagnostic entry.
    /// </summary>
    /// <param name="output">The output stream where the error can be logged.</param>
    /// <param name="recognizer">The source parser or lexer where the error originated.</param>
    /// <param name="offendingSymbol">The offending symbol that caused the syntax error.</param>
    /// <param name="line">The line number where the error occurred.</param>
    /// <param name="charPositionInLine">The character position in the line where the error occurred.</param>
    /// <param name="msg">The error message describing the syntax issue.</param>
    /// <param name="e">An optional exception that provides additional context about the error.</param>
    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol,
        int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var location = new SourcePosition(_sourceName, line, charPositionInLine + 1);
        var diagnostic = Diagnostic.Error(
            location: location,
            code: "COOL0001",
            message: $"Lexer error: {msg}");

        _diagnostics.Add(diagnostic);
    }

    /// <summary>
    /// Reports a syntax error encountered during parsing or lexical analysis
    /// and adds a diagnostic entry to the diagnostics bag.
    /// </summary>
    /// <param name="output">The output stream where the error message will be logged.</param>
    /// <param name="recognizer">The parser or lexer where the error originated.</param>
    /// <param name="offendingSymbol">The integer representation of the offending symbol causing the syntax error.</param>
    /// <param name="line">The line number where the syntax error occurred.</param>
    /// <param name="charPositionInLine">The character position in the line where the error occurred.</param>
    /// <param name="msg">The error message that describes the encountered issue.</param>
    /// <param name="e">An optional exception that provides detailed context about the syntax error.</param>
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol,
        int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var location = new SourcePosition(_sourceName, line, charPositionInLine + 1);
        var diagnostic = Diagnostic.Error(
            location: location,
            code: "COOL0002",
            message: $"Syntax error: {msg}");

        _diagnostics.Add(diagnostic);
    }
}