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
public class InterpretationResult
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

    /// <summary>
    /// Represents the result of interpreting a Cool program.
    /// It encapsulates information about whether the interpretation succeeded,
    /// the program's output, any returned value, and a set of diagnostics.
    /// </summary>
    private InterpretationResult(
        bool isSuccess,
        string output,
        string returnedValue,
        ImmutableArray<Diagnostic> diagnostics)
    {
        IsSuccess = isSuccess;
        Output        = output        ?? string.Empty;
        ReturnedValue = returnedValue ?? string.Empty;
        Diagnostics   = diagnostics.IsDefault ? ImmutableArray<Diagnostic>.Empty : diagnostics;
    }

    /// <summary>
    /// Creates a successful interpretation result indicating that the interpretation completed successfully.
    /// It includes the output produced by the program, an optional returned value, and any diagnostics generated during execution.
    /// </summary>
    /// <param name="output">The output text produced by the interpretation process.</param>
    /// <param name="returnedValue">The optional value returned by the program, if applicable.</param>
    /// <param name="diagnostics">A collection of diagnostics generated during the interpretation process, if any.</param>
    /// <returns>A new instance of <see cref="InterpretationResult"/> representing a successful interpretation.</returns>
    public static InterpretationResult Success(
        string output,
        string? returnedValue = null,
        ImmutableArray<Diagnostic>? diagnostics = null) =>
        new(
            isSuccess: true,
            output: output ?? string.Empty,
            returnedValue: returnedValue ?? string.Empty,
            diagnostics: diagnostics ?? ImmutableArray<Diagnostic>.Empty);

    /// <summary>
    /// Creates a new instance of <see cref="InterpretationResult"/> representing a failure in interpreting a program.
    /// It encapsulates diagnostic information and optional output and return value.
    /// </summary>
    /// <param name="output">The textual output generated up to the point of failure.</param>
    /// <param name="diagnostics">A collection of diagnostic information detailing the issues encountered.</param>
    /// <param name="returnedValue">An optional value returned by the program, which may be null in case of failure.</param>
    /// <returns>An <see cref="InterpretationResult"/> instance representing the failure.</returns>
    public static InterpretationResult Failure(
        string output,
        ImmutableArray<Diagnostic> diagnostics,
        string? returnedValue = null) =>
        new(
            isSuccess: false,
            output: output ?? string.Empty,
            returnedValue: returnedValue ?? string.Empty,
            diagnostics: diagnostics);

    /// <summary>
    /// Creates a failure result of interpreting a Cool program, encapsulating the output
    /// and a single diagnostic message indicating the reason for the failure.
    /// </summary>
    /// <param name="output">The program's output generated prior to the failure.</param>
    /// <param name="diagnostic">The diagnostic conveying details of the failure.</param>
    /// <returns>An instance of <see cref="InterpretationResult"/> representing the failure state.</returns>
    public static InterpretationResult Failure(string output, Diagnostic diagnostic) =>
        Failure(output, ImmutableArray.Create(diagnostic));

    /// <summary>
    /// Creates a failure result for the interpretation of a Cool program.
    /// This method allows specifying multiple diagnostic messages that describe the reasons for failure.
    /// </summary>
    /// <param name="output">The output produced during the failed interpretation.</param>
    /// <param name="diagnostics">An array of diagnostic messages providing details about the failure.</param>
    /// <returns>An <see cref="InterpretationResult"/> indicating a failure.</returns>
    public static InterpretationResult Failure(string output, params Diagnostic[] diagnostics) =>
        Failure(output, diagnostics.ToImmutableArray());

    /// <summary>
    /// True if there are any diagnostics with a severity level of Error.
    /// Indicates the presence of errors during the interpretation process.
    /// </summary>
    public bool HasErrors => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// True if there are diagnostics with a severity level of Warning.
    /// </summary>
    public bool HasWarnings => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Warning);

    /// <summary>
    /// Converts the result of program interpretation into a human-readable string representation.
    /// The string provides detailed information about the execution outcome, including
    /// success or failure status, the number of errors (if any), and any program output
    /// or returned values.
    /// </summary>
    /// <returns>
    /// A string describing the interpretation result, including execution status,
    /// diagnostic information, output, and returned values.
    /// </returns>
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