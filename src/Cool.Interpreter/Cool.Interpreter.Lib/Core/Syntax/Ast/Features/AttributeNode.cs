//-----------------------------------------------------------------------
// <copyright file="AttributeNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>AttributeNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Features;

using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an attribute node within the abstract syntax tree (AST) of the Cool language.
/// </summary>
/// <remarks>
/// An attribute in the Cool language is a class-level declaration of a variable with an optional
/// initializer expression. This class contains information related to the attribute's type,
/// name, and its defined order within the class source.
/// </remarks>
public class AttributeNode : FeatureNode
{
    /// <summary>
    /// Represents an attribute node in the abstract syntax tree (AST) of the programming language.
    /// It defines an attribute, including its type, optional initializer, and source order.
    /// </summary>
    public AttributeNode(
        string name,
        string typeName,
        ExpressionNode? initializer,
        int sourceOrder,
        SourcePosition location) : base(name, location)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        Initializer = initializer;
        SourceOrder = sourceOrder;
    }
    
    /// <summary>
    /// Gets the declared type of the attribute as a string.
    /// </summary>
    /// <remarks>
    /// This property represents the static type associated with the attribute as declared in the Cool program.
    /// The type can either be a concrete type name or "SELF_TYPE" to indicate the class where the attribute is defined.
    /// </remarks>
    public string TypeName { get; }

    /// <summary>
    /// Gets the optional initializer expression for the attribute.
    /// </summary>
    /// <remarks>
    /// This property holds the expression used to initialize the value of the attribute, if one is defined.
    /// If no initializer is specified, the property will be null. The initializer expression is evaluated
    /// during object construction or as specified by the program semantics.
    /// </remarks>
    public ExpressionNode? Initializer { get; }

    /// <summary>
    /// Gets the order in which the attribute is declared within the source code of the Cool class.
    /// </summary>
    /// <remarks>
    /// This property represents the sequence number of the attribute as it appears in the class definition.
    /// It is used to determine the order of attributes when traversing or processing Cool class declarations.
    /// </remarks>
    public int SourceOrder { get; } // needed for correct init order (§5)

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface and processes this attribute node.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance to process this node.</param>
    /// <returns>The result of the visitor's operation, determined by its implementation.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}