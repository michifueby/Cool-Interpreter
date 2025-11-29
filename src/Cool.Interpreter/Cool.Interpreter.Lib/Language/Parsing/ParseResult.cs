//-----------------------------------------------------------------------
// <copyright file="ParseResult.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ParseResult</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Parsing;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents the result of parsing Cool source code.
/// Contains the syntax tree (if successful) and all diagnostics (errors, warnings).
/// </summary>
/// <remarks>
/// This class is intentionally simple and immutable after construction.
/// Used by CoolInterpreter.Parse() and returned to the user.
/// Follows the same design used by Rust Analyzer, Deno, and TypeScript.
/// </remarks>
public class ParseResult
{
    /// <summary>
    /// Gets the root of the parsed syntax tree.
    /// Null if parsing failed completely (e.g., unrecoverable syntax error).
    /// </summary>
    public ProgramNode? SyntaxTree { get; }

    /// <summary>
    /// Gets all diagnostics produced during parsing (syntax errors, lexer issues, etc.).
    /// Never null. Empty list means no issues.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics { get; }

    /// <summary>
    /// Gets a value indicating whether parsing succeeded and produced a valid syntax tree.
    /// </summary>
    public bool IsSuccess => SyntaxTree is not null;

    /// <summary>
    /// Gets a value indicating whether any errors occurred during parsing.
    /// </summary>
    public bool HasErrors => Diagnostics.Count > 0 && Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Creates a successful parse result.
    /// </summary>
    public static ParseResult Success(ProgramNode syntaxTree, IReadOnlyList<Diagnostic> diagnostics)
        => new(syntaxTree, diagnostics);

    /// <summary>
    /// Creates a failed parse result (syntax errors prevented AST construction).
    /// </summary>
    public static ParseResult Failure(IReadOnlyList<Diagnostic> diagnostics)
        => new(null, diagnostics);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseResult"/> class.
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="diagnostics"></param>
    public ParseResult(ProgramNode? syntaxTree, IReadOnlyList<Diagnostic> diagnostics)
    {
        SyntaxTree = syntaxTree;
        Diagnostics = diagnostics;
    }

    /// <summary>
    /// Returns a short, human-readable summary.
    /// Used in debugging.
    /// </summary>
    public override string ToString()
    {
        if (IsSuccess)
            return $"Parse succeeded ({Diagnostics.Count} diagnostic(s))";

        var errorCount = Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error);
        return $"Parse failed ({errorCount} error(s))";
    }
}