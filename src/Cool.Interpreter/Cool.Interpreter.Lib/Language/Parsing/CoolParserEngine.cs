//-----------------------------------------------------------------------
// <copyright file="CoolParserEngine.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolParserEngine</summary>
//-----------------------------------------------------------------------

using Antlr4.Runtime;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax;

namespace Cool.Interpreter.Lib.Language.Parsing;

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
        parser.ErrorHandler = new BailErrorStrategy(); 

        try
        {
            var programContext = parser.program();

            // Only build AST if there were no fatal lexer/parser errors
            if (diagnostics.HasErrors)
            {
                return new ParseResult(
                    syntaxTree: null,
                    diagnostics: diagnostics.ToReadOnly());
            }

            var visitor = new AstBuilderVisitor(diagnostics);
            var syntaxTree = visitor.Visit(programContext) as ProgramNode;

            return new ParseResult(
                syntaxTree: syntaxTree,
                diagnostics: diagnostics.ToReadOnly());
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // This should never happen — but if visitor crashes, report it gracefully
            diagnostics.ReportInternal(
                location: new SourcePosition(sourceName, 1, 1),"COOL0000",
                message: $"Internal parser error: {ex.Message}");

            return new ParseResult(
                syntaxTree: null,
                diagnostics: diagnostics.ToReadOnly());
        }
    }
}