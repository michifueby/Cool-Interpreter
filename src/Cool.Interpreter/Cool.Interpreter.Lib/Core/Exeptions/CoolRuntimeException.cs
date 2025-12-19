//-----------------------------------------------------------------------
// <copyright file="CoolRuntimeException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolRuntimeException</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Exeptions;

using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a runtime exception that occurs during the interpretation of Cool programs.
/// This exception class is used to signal errors that are encountered during the execution phase
/// of Cool programs, such as invalid operations or boundary violations.
/// It provides detailed error information, including optional source location data,
/// to facilitate accurate debugging and high-quality error reporting.
/// </summary>
public sealed class CoolRuntimeException : Exception
{
    /// <summary>
    /// Gets the error code associated with this runtime exception.
    /// Used for precise error reporting and testing.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Represents a runtime exception that occurs during the interpretation of Cool programs.
    /// This exception provides detailed error information, including the source location
    /// for IDE-quality error reporting.
    /// </summary>
    public CoolRuntimeException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Represents a runtime exception that occurs during the interpretation of Cool programs.
    /// This exception provides detailed error information, including optional source location,
    /// facilitating precise error tracking and IDE-quality reporting.
    /// </summary>
    public CoolRuntimeException(string message, SourcePosition location, string? errorCode = null)
        : base(BuildMessage(message, location))
    {
        Location = location;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Represents a runtime exception that occurs during the interpretation of Cool programs.
    /// This exception provides detailed error information, including the source location
    /// for IDE-quality error reporting.
    /// </summary>
    public CoolRuntimeException(string message, Exception inner, string? errorCode = null)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// Gets the source code location where the runtime exception occurred.
    /// Provides details such as file path, line number, and column number,
    /// allowing precise identification of the error's origin in the source code.
    /// Can be null if no location is specified.
    /// </summary>
    public SourcePosition? Location { get; }

    /// <summary>
    /// Constructs a detailed error message by combining a user-friendly error description
    /// with source location information.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <param name="location">The position in the source code where the error occurred.</param>
    /// <returns>A formatted string combining the error message with source location details.</returns>
    private static string BuildMessage(string message, SourcePosition location)
        => $"{location}: runtime error: {message}";
}