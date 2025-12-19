//-----------------------------------------------------------------------
// <copyright file="CoolParserEngine.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolParserEngine</summary>
//-----------------------------------------------------------------------

using Antlr4.Runtime.Misc;

using System.Runtime.ExceptionServices;

namespace Cool.Interpreter.Lib.Language.Parsing;

using Antlr4.Runtime;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Orchestrates the ANTLR-generated lexer and parser to produce a clean AST.
/// This is the only class that knows about ANTLR.
/// </summary>
public class CoolParserEngine
{
    /// <summary>
    /// Parses the provided source code and generates a syntax tree along with diagnostics.
    /// </summary>
    /// <param name="sourceCode">The source code to parse. Must not be null.</param>
    /// <param name="sourceName">The name of the source, used for diagnostics. If not provided, defaults to "<unknown>".</param>
    /// <returns>A <see cref="ParseResult"/> containing the syntax tree and diagnostics.</returns>
    public ParseResult Parse(string sourceCode, string? sourceName = null)
    {
        ArgumentNullException.ThrowIfNull(sourceCode);

        // Normalize source name for diagnostics
        sourceName = string.IsNullOrWhiteSpace(sourceName)
            ? "<unknown>"
            : sourceName.Trim();

        var inputStream = new AntlrInputStream(sourceCode);
        var lexer = new CoolLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new CoolParser(tokenStream);

        // Collect diagnostics even on syntax errors 
        var diagnostics = new DiagnosticBag();
        var errorListener = new CoolErrorListener(sourceName, diagnostics);

        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(errorListener);
        parser.AddErrorListener(errorListener);

        // Enable rich error recovery (continue after errors)
        parser.ErrorHandler = new DefaultErrorStrategy();

        try
        {
            var result = RunParserWithIncreasedStack(parser, sourceName, diagnostics);

            return result;
        }
        catch (ParseCanceledException ex)
        {
            diagnostics.ReportError(
                location: new SourcePosition(sourceName, 1, 1),
                code: "COOL0001",
                message: "Parsing failed due to syntax error or ambiguous grammar (likely left recursion). " +
                         "Check your input or grammar rules.");

            return new ParseResult(null, diagnostics.Diagnostics);
        }
        catch (Exception ex) when (!(ex is ParseCanceledException))
        {
            // Real crashes (should never happen)
            diagnostics.ReportInternal(
                location: new SourcePosition(sourceName, 1, 1),
                code: "COOL0000",
                message: $"Internal parser error: {ex.Message}");

            return new ParseResult(null, diagnostics.Diagnostics);
        }
    }

    private ParseResult RunParserWithIncreasedStack(CoolParser parser, string sourceName, DiagnosticBag diagnostics)
    {
        // 4MB stack size to handle deep recursion
        const int StackSize = 4 * 1024 * 1024;
        
        ParseResult? result = null;
        Exception? caughtException = null;

        var thread = new Thread(() =>
        {
            try
            {
                var context = parser.program();
                var visitor = new AstBuilderVisitor(sourceName);
                var programNode = visitor.Visit(context) as ProgramNode;
                result = new ParseResult(programNode, diagnostics.Diagnostics);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }, StackSize);

        thread.Start();
        thread.Join();

        if (caughtException != null)
        {
            ExceptionDispatchInfo.Capture(caughtException).Throw();
        }

        return result ?? new ParseResult(null, diagnostics.Diagnostics);
    }
}