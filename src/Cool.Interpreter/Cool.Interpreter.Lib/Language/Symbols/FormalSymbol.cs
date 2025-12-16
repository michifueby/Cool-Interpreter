//-----------------------------------------------------------------------
// <copyright file="FormalSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>FormalSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a formal parameter (symbol) in the context of a method or function definition in the Cool programming language.
/// </summary>
/// <remarks>
/// A formal parameter symbol encapsulates information about a method or function parameter, including its name, type, and source position in the code.
/// This symbol is utilized primarily for semantic analysis and type checking in the interpreter.
/// </remarks>
public class FormalSymbol
{
    /// <summary>
    /// Gets the name of the formal parameter (symbol).
    /// </summary>
    /// <remarks>
    /// The name represents the identifier used to refer to the parameter within the method or function definition.
    /// It is used during semantic analysis to map input arguments to their respective symbols.
    /// This property is immutable after the object is created.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Gets the type associated with the formal parameter (symbol).
    /// </summary>
    /// <remarks>
    /// Represents the declared type of the parameter in the Cool programming language.
    /// This property is used in semantic analysis and type checking to ensure type consistency
    /// in method or function invocations. The value is typically determined at the point of symbol creation
    /// and remains immutable thereafter.
    /// </remarks>
    public string Type { get; }

    /// <summary>
    /// Gets the source code location of the formal parameter (symbol).
    /// </summary>
    /// <remarks>
    /// The location provides detailed information about the position in the source code where the parameter is defined,
    /// including the file path, line number, and column number. This is useful for error reporting, debugging, and
    /// semantic analysis during interpretation or compilation.
    /// This property is immutable and represents precise source code positioning.
    /// </remarks>
    public SourcePosition Location { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormalSymbol"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="location"></param>
    public FormalSymbol(string name, string type, SourcePosition location = default)
    {
        Name     = name;
        Type     = type;
        Location = location == default ? SourcePosition.None : location;
    }
}