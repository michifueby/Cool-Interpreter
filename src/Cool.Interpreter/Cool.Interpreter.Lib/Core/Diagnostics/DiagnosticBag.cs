//-----------------------------------------------------------------------
// <copyright file="DiagnosticBag.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>DiagnosticBag</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Diagnostics;

using System.Collections.Generic;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Bag that collects diagnostics during compilation/interpretation.
/// </summary>
public class DiagnosticBag
{
    /// <summary>
    /// Stores a collection of diagnostic messages generated during compilation or interpretation.
    /// </summary>
    private readonly List<Diagnostic> _diagnostics = new();

    /// <summary>
    /// Gets all collected diagnostics as a read-only list.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    /// <summary>
    /// True if there is at least one error.
    /// </summary>
    public bool HasErrors => _diagnostics.Exists(d => d.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Adds a diagnostic.
    /// </summary>
    public void Add(Diagnostic diagnostic)
        => _diagnostics.Add(diagnostic);

    /// <summary>
    /// Adds multiple diagnostics.
    /// </summary>
    public void AddRange(IEnumerable<Diagnostic> diagnostics)
        => _diagnostics.AddRange(diagnostics);

    /// <summary>
    /// Clears all diagnostics.
    /// </summary>
    public void Clear()
        => _diagnostics.Clear();

    /// <summary>
    /// Reports an error diagnostic with the specified location, code, and message.
    /// </summary>
    /// <param name="location">The position in the source code where the error occurred.</param>
    /// <param name="code">A string representing the error code.</param>
    /// <param name="message">A descriptive message for the error.</param>
    public void ReportError(SourcePosition location, string code, string message)
        => Add(Diagnostic.Error(location, code, message));

    /// <summary>
    /// Adds a warning diagnostic.
    /// </summary>
    /// <param name="location">The location in the source code associated with this diagnostic.</param>
    /// <param name="code">The diagnostic code representing the type of warning.</param>
    /// <param name="message">A descriptive message about the warning.</param>
    public void ReportWarning(SourcePosition location, string code, string message)
        => Add(Diagnostic.Warning(location, code, message));

    /// <summary>
    /// Adds an informational diagnostic.
    /// </summary>
    /// <param name="location">The source position where the diagnostic occurred.</param>
    /// <param name="code">The code associated with the diagnostic.</param>
    /// <param name="message">The message describing the diagnostic.</param>
    public void ReportInfo(SourcePosition location, string code, string message)
        => Add(Diagnostic.Info(location, code, message));
    
    /// <summary>
    /// Adds an informational diagnostic.
    /// </summary>
    /// <param name="location">The source position where the diagnostic occurred.</param>
    /// <param name="code">The code associated with the diagnostic.</param>
    /// <param name="message">The message describing the diagnostic.</param>
    public void ReportInternal(SourcePosition location, string code, string message)
        => Add(Diagnostic.Internal(location, code, message));
}