//-----------------------------------------------------------------------
// <copyright file="InterpretationResult.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>InterpretationResult</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

using System.Collections.Immutable;
using Core.Diagnostics;

/// <summary>
/// Represents the complete result of executing a Cool program via <see cref="CoolInterpreter"/>.
/// This is the one and only return type from the public facade.
/// </summary>
public sealed class InterpretationResult
{
    /// <summary>
    /// True if the program executed successfully (no errors).
    /// Warnings do not make it false.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Everything written via out_string() / out_int().
    /// Never null.
    /// </summary>
    public string Output { get; }

    /// <summary>
    /// String representation of the value returned by main().
    /// Empty string if void/null. Never null.
    /// </summary>
    public string ReturnedValue { get; }

    /// <summary>
    /// All diagnostics from parsing, semantic analysis, and runtime.
    /// Never null.
    /// </summary>
    public ImmutableArray<Diagnostic> Diagnostics { get; }

    // Private constructor — forces use of factory methods
    private InterpretationResult(
        bool isSuccess,
        string output,
        string returnedValue,
        ImmutableArray<Diagnostic> diagnostics)
    {
        IsSuccess     = isSuccess;
        Output        = output        ?? string.Empty;
        ReturnedValue = returnedValue ?? string.Empty;
        Diagnostics   = diagnostics.IsDefault ? ImmutableArray<Diagnostic>.Empty : diagnostics;
    }

    // --------------------------------------------------------------------
    // Factory methods
    // --------------------------------------------------------------------
    public static InterpretationResult Success(
        string output,
        string? returnedValue = null,
        ImmutableArray<Diagnostic>? diagnostics = null) =>
        new(
            isSuccess: true,
            output: output ?? string.Empty,
            returnedValue: returnedValue ?? string.Empty,
            diagnostics: diagnostics ?? ImmutableArray<Diagnostic>.Empty);

    public static InterpretationResult Failure(
        string output,
        ImmutableArray<Diagnostic> diagnostics,
        string? returnedValue = null) =>
        new(
            isSuccess: false,
            output: output ?? string.Empty,
            returnedValue: returnedValue ?? string.Empty,
            diagnostics: diagnostics);

    // Convenience overloads
    public static InterpretationResult Failure(string output, Diagnostic diagnostic) =>
        Failure(output, ImmutableArray.Create(diagnostic));

    public static InterpretationResult Failure(string output, params Diagnostic[] diagnostics) =>
        Failure(output, diagnostics.ToImmutableArray());

    // --------------------------------------------------------------------
    // Helpful properties
    // --------------------------------------------------------------------
    public bool HasErrors   => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
    public bool HasWarnings => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Warning);

    // --------------------------------------------------------------------
    // Beautiful ToString()
    // --------------------------------------------------------------------
    public override string ToString()
    {
        if (!IsSuccess || HasErrors)
        {
            var errorCount = Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error);
            return errorCount == 1
                ? "Execution failed with 1 error."
                : $"Execution failed with {errorCount} errors.";
        }

        var hasOutput = !string.IsNullOrWhiteSpace(Output);
        var hasValue  = !string.IsNullOrEmpty(ReturnedValue);

        if (!hasOutput && !hasValue)
            return "Execution completed successfully (no output).";

        if (!hasValue)
            return $"Execution completed.\nOutput:\n{Output.TrimEnd()}";

        return !hasOutput 
            ? $"Execution completed → {ReturnedValue}" 
            : $"Execution completed → {ReturnedValue}\nOutput:\n{Output.TrimEnd()}";
    }
}