//-----------------------------------------------------------------------
// <copyright file="BoolLiteralNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>BoolLiteralNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a boolean literal node in the Cool Abstract Syntax Tree (AST).
/// This node encapsulates a boolean value and its location within the source code.
/// </summary>
public class BoolLiteralNode : ExpressionNode
{
    /// <summary>
    /// Represents a boolean literal node in the Cool Abstract Syntax Tree (AST).
    /// This node is used to encapsulate a boolean value (true or false) and its
    /// corresponding source code location information.
    /// </summary>
    public BoolLiteralNode(bool value, SourcePosition location)
        : base(location) => Value = value;
    
    /// <summary>
    /// Gets the encapsulated boolean value represented by this <see cref="BoolLiteralNode"/>.
    /// </summary>
    /// <value>
    /// A <see cref="bool"/> indicating the literal value (true or false) stored in this node.
    /// </value>
    public bool Value { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface,
    /// allowing external operations to be performed on this Boolean literal node.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance defining the operation to be performed on this node.</param>
    /// <returns>The result produced by the visitor's operation.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}