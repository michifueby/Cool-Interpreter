//-----------------------------------------------------------------------
// <copyright file="AttributeSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>AttributeSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an attribute symbol in the Cool programming language.
/// This class encapsulates metadata about an attribute, including its
/// name, type, optional initializer, and source code location.
/// </summary>
public class AttributeSymbol
{
    /// <summary>
    /// Gets the name of the attribute or class symbol.
    /// Represents the unique identifier or label for an element,
    /// such as an attribute within a class or a class itself.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type associated with the attribute or symbol.
    /// Represents the data type or classification of an element, such as
    /// the type of an attribute within a class in the Cool programming language.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the optional initializer expression for the attribute.
    /// Specifies an expression assigned to the attribute at the time of its definition.
    /// If no initializer is provided, the value is null.
    /// </summary>
    public ExpressionNode? Initializer { get; }

    /// <summary>
    /// Gets the source code location associated with the attribute.
    /// Represents the position in the source file where the attribute is defined,
    /// including file path, line, and column details.
    /// </summary>
    public SourcePosition Location { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSymbol"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="initializer"></param>
    /// <param name="location"></param>
    public AttributeSymbol(string name, string type, ExpressionNode? initializer = null, SourcePosition location = default)
    {
        Name        = name;
        Type        = type;
        Initializer = initializer;
        Location    = location == default ? SourcePosition.None : location;
    }
}