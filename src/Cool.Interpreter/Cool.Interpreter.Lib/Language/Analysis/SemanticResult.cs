//-----------------------------------------------------------------------
// <copyright file="SemanticResult.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SemanticResult</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents the complete result of semantic analysis for a Cool program.
/// </summary>
public class SemanticResult
{
    /// <summary>
    /// Gets the global symbol table containing all classes, methods, attributes, and types.
    /// Null if analysis failed due to critical errors.
    /// </summary>
    public SymbolTable? SymbolTable { get; }

    /// <summary>
    /// Gets a value indicating whether semantic analysis succeeded (no errors).
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets all semantic diagnostics (errors, warnings) produced during analysis.
    /// Never null.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticResult"/> class.
    /// </summary>
    /// <param name="symbolTable"></param>
    /// <param name="isSuccess"></param>
    /// <param name="diagnostics"></param>
    private SemanticResult(SymbolTable? symbolTable, bool isSuccess, IReadOnlyList<Diagnostic> diagnostics)
    {
        SymbolTable = symbolTable;
        IsSuccess = isSuccess;
        Diagnostics = diagnostics;
    }

    /// <summary>
    /// Creates a successful semantic result.
    /// </summary>
    public static SemanticResult Success(SymbolTable symbolTable, IReadOnlyList<Diagnostic> diagnostics)
        => new(symbolTable, true, diagnostics);

    /// <summary>
    /// Creates a failed semantic result (e.g., inheritance cycle, undefined type).
    /// </summary>
    public static SemanticResult Failure(IReadOnlyList<Diagnostic> diagnostics)
        => new(null, false, diagnostics);

    /// <summary>
    /// Converts the current semantic result to a string representation indicating the success or failure
    /// of the semantic analysis along with the count of diagnostics.
    /// </summary>
    /// <returns>
    /// A string describing whether the semantic analysis succeeded or failed, and the number of
    /// diagnostics, including errors if the analysis failed.
    /// </returns>
    public override string ToString()
        => IsSuccess
            ? $"Semantic analysis succeeded ({Diagnostics.Count} diagnostic(s))" 
            : $"Semantic analysis failed ({Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error)} errors)";
}