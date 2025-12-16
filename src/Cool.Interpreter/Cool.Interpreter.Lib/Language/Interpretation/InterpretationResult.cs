//-----------------------------------------------------------------------
// <copyright file="InterpretationResult.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>InterpretationResult</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

using Cool.Interpreter.Lib.Language.Classes;
using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Diagnostics;

/// <summary>
/// Represents the complete result of executing a Cool program via <see cref="CoolInterpreter"/>.
/// This is the one and only return type from the public facade.
/// </summary>
public class InterpretationResult
{
    public string Output { get; }
    public CoolObject? ReturnedValue { get; }
    
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    
    public bool IsSuccess => Diagnostics.All(d => d.Severity is not (DiagnosticSeverity.Error or DiagnosticSeverity.InternalError));

    private InterpretationResult(string output, CoolObject? returnedValue, IReadOnlyList<Diagnostic> diagnostics)
    {
        Output = output ?? string.Empty;
        ReturnedValue = returnedValue;
        Diagnostics = diagnostics ?? ImmutableArray<Diagnostic>.Empty;
    }

    /// <summary>
    /// Creates a new <see cref="InterpretationResult"/> instance representing a successful execution of a Cool program.
    /// </summary>
    /// <param name="output">The output produced by the interpreter during execution.</param>
    /// <param name="returnedValue">The primary return value of the program execution.</param>
    /// <returns>A new instance of <see cref="InterpretationResult"/> with no diagnostics and marked as successful.</returns>
    public static InterpretationResult Success(string output, CoolObject? returnedValue)
        => new(output, returnedValue, ImmutableArray<Diagnostic>.Empty);

    /// <summary>
    /// Creates a new <see cref="InterpretationResult"/> instance representing a failed execution of a Cool program.
    /// </summary>
    /// <param name="output">The output produced by the interpreter during execution, if any.</param>
    /// <param name="returnedValue">The primary return value of the program execution, if applicable.</param>
    /// <param name="diagnostics">A list of diagnostics generated during execution, representing errors or warnings.</param>
    /// <returns>A new instance of <see cref="InterpretationResult"/> with the provided diagnostics and marked as failed.</returns>
    public static InterpretationResult Failure(string output, CoolObject? returnedValue,
        IReadOnlyList<Diagnostic> diagnostics)
        => new(output, returnedValue, diagnostics);

    public override string ToString()
    {
        if (!IsSuccess)
            return $"Execution failed with {Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error)} error(s)";

        return ReturnedValue is null // Missing --> Add is CoolNull !!!
            ? $"Execution completed. Output length: {Output.Length} character(s)."
            : $"Execution completed → {ReturnedValue}\nOutput:\n{Output}";
    }
}