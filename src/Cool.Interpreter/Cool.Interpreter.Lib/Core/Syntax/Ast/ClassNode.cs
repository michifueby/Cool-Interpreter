//-----------------------------------------------------------------------
// <copyright file="ClassNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ClassNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;

/// <summary>
/// Represents a class node within the Cool Abstract Syntax Tree (AST).
/// Encapsulates the definition of a Cool class, including its name,
/// its optional inheritance specification, its features, and its location within the source code.
/// </summary>
public class ClassNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents a class node in the Cool programming language's Abstract Syntax Tree (AST).
    /// Encapsulates the structure and metadata of a Cool class, including its name,
    /// potential inheritance, and associated feature definitions.
    /// </summary>
    public ClassNode(string name, string? inheritsFrom,
        IReadOnlyList<FeatureNode> features, SourcePosition location)
        : base(location)
    {
        Name = name;
        InheritsFrom = inheritsFrom;
        Features = features;
    }
    
    /// <summary>
    /// Gets the name of the class represented by the current class node.
    /// This property is used to identify the class in the Cool Abstract Syntax Tree (AST),
    /// as well as in various operations such as inheritance validation, symbol registration, and runtime evaluation.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the parent class from which the current class derives, if specified.
    /// This property represents the optional inheritance relationship for a class in the Cool Abstract Syntax Tree (AST).
    /// It is used in operations such as inheritance validation, hierarchy resolution, and symbol registration.
    /// </summary>
    public string? InheritsFrom { get; }

    /// <summary>
    /// Gets the collection of features defined within the current class node.
    /// Features represent the functional components of a class, such as attributes and methods,
    /// and provide the building blocks for the class's behavior and structure.
    /// </summary>
    public IReadOnlyList<FeatureNode> Features { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface, allowing
    /// the implementation of custom behavior for the current <see cref="ClassNode"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the visitor.</typeparam>
    /// <param name="visitor">An instance of a class implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface.</param>
    /// <returns>The result produced by the visitor after processing the current <see cref="ClassNode"/>.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}