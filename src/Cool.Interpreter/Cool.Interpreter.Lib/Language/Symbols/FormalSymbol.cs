//-----------------------------------------------------------------------
// <copyright file="FormalSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>FormalSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents a formal parameter symbol in the Cool language.
/// </summary>
/// <remarks>
/// A formal symbol encapsulates the name and type information for a parameter,
/// typically used in method or function declarations.
/// </remarks>
public class FormalSymbol(string name, string type)
{
    /// <summary>
    /// Gets the name of the formal symbol. This represents the identifier associated with the symbol.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the type associated with the formal symbol. This represents the data type of the parameter in the context of a method or function declaration.
    /// </summary>
    public string Type { get; } = type;
}