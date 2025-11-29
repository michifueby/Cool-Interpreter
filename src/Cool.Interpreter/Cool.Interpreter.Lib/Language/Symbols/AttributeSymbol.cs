//-----------------------------------------------------------------------
// <copyright file="AttributeSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>AttributeSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents an attribute symbol in the context of a programming language's symbol table.
/// An attribute is defined by its name and type, and is typically tied to a class or object.
/// </summary>
public class AttributeSymbol(string name, string type)
{
    /// <summary>
    /// Gets the name of the attribute represented by this symbol.
    /// The name uniquely identifies the attribute within its context.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the type of the attribute represented by this symbol.
    /// The type defines the data type or classification of the attribute within its context.
    /// </summary>
    public string Type { get; } = type;
}