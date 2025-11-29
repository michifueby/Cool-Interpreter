//-----------------------------------------------------------------------
// <copyright file="DiagnosticSeverity.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>DiagnosticSeverity</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Diagnostics;

/// <summary>
/// Defines severity levels for diagnostics.
/// </summary>
public enum DiagnosticSeverity
{
    /// <summary>Internal error.</summary>
    InternalError = 0,
    /// <summary>Informational messages and hints.</summary>
    Info = 1,
    /// <summary>Warnings — do not prevent execution.</summary>
    Warning = 2,
    /// <summary>Errors — prevent successful compilation/interpretation.</summary>
    Error = 3
}