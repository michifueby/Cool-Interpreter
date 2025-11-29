//-----------------------------------------------------------------------
// <copyright file="IInterpreter.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>IInterpreter</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Interpretation;

/// <summary>
/// Defines the interface for an interpreter capable of executing programs.
/// Implementations of this interface should provide the necessary functionality
/// to interpret and execute the semantics of the program supplied to them.
/// </summary>
public interface IInterpreter
{
    /// <summary>
    /// Executes the provided source code and interprets its semantics.
    /// </summary>
    /// <param name="sourceCode">The source code to be interpreted as a string.</param>
    /// <param name="sourceName">An optional name for the source code, used for referencing or error reporting.</param>
    /// <returns>An <see cref="InterpretationResult"/> containing the result of the execution.</returns>
    InterpretationResult Run(string sourceCode, string? sourceName = null);

    /// <summary>
    /// Executes the program contained in the specified file and interprets its semantics.
    /// </summary>
    /// <param name="filePath">The path to the file containing the source code to be executed.</param>
    /// <returns>An <see cref="InterpretationResult"/> containing the result of the execution.</returns>
    InterpretationResult RunFile(string filePath);
}