//-----------------------------------------------------------------------
// <copyright file="Diagnostic.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Diagnostic</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Diagnostics;

using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a single diagnostic message produced during parsing, semantic analysis,
/// or interpretation of Cool source code.
/// </summary>
public class Diagnostic
{
    /// <summary>
    /// Gets the severity of the diagnostic.
    /// </summary>
    public DiagnosticSeverity Severity { get; }

    /// <summary>
    /// Gets the unique diagnostic code (e.g., COOL0001, COOL1004, COOLW9001).
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable message. May contain {0}, {1} placeholders.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the source location where the diagnostic occurred.
    /// </summary>
    public SourcePosition Location { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Diagnostic"/> class.
    /// </summary>
    /// <param name="severity"></param>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="location"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private Diagnostic(
        DiagnosticSeverity severity,
        string code,
        string message,
        SourcePosition? location)
    {
        Severity = severity;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Location = location ?? SourcePosition.None;
    }

    /// <summary>
    /// Creates a new <see cref="Diagnostic"/> instance with an error severity level.
    /// </summary>
    /// <param name="location">The source position where the error occurred.</param>
    /// <param name="code">The diagnostic code associated with the error.</param>
    /// <param name="message">The error message providing details about the issue.</param>
    /// <returns>A <see cref="Diagnostic"/> object representing the error.</returns>
    public static Diagnostic Error(SourcePosition location, string code, string message)
        => new(DiagnosticSeverity.Error, code, message, location);

    /// <summary>
    /// Creates a diagnostic message with a severity level of Warning.
    /// </summary>
    /// <param name="location">The source location associated with the diagnostic.</param>
    /// <param name="code">The diagnostic code representing the type of warning.</param>
    /// <param name="message">The message describing the warning.</param>
    /// <returns>A new instance of the <see cref="Diagnostic"/> class with a severity of Warning.</returns>
    public static Diagnostic Warning(SourcePosition location, string code, string message)
        => new(DiagnosticSeverity.Warning, code, message, location);

    /// <summary>
    /// Creates a diagnostic instance with an informational severity.
    /// </summary>
    /// <param name="location">The source position where the diagnostic is located.</param>
    /// <param name="code">The unique code identifying the diagnostic.</param>
    /// <param name="message">The message describing the diagnostic.</param>
    /// <returns>A <see cref="Diagnostic"/> instance representing an informational message.</returns>
    public static Diagnostic Info(SourcePosition location, string code, string message)
        => new(DiagnosticSeverity.Info, code, message, location);

    /// <summary>
    /// Creates a diagnostic message with a severity of "Internal".
    /// </summary>
    /// <param name="location">The source position where the diagnostic occurred.</param>
    /// <param name="code">The code identifying the diagnostic message.</param>
    /// <param name="message">The diagnostic message description.</param>
    /// <returns>A new instance of the <see cref="Diagnostic"/> class representing an internal diagnostic.</returns>
    public static Diagnostic Internal(SourcePosition location, string code, string message)
        => new(DiagnosticSeverity.Info, code, message, location);

    /// <summary>
    /// Returns a complete string suitable for console or IDE output.
    /// Example: Hello.cool(12,8): error COOL0001: Undeclared identifier 'x'
    /// </summary>
    public override string ToString()
    {
        var severityText = Severity switch
        {
            DiagnosticSeverity.Error => "error",
            DiagnosticSeverity.Warning => "warning",
            DiagnosticSeverity.Info => "info",
            _ => Severity.ToString().ToLowerInvariant()
        };

        return $"{Location}: {severityText} {Code}: {this.Message}";
    }
}