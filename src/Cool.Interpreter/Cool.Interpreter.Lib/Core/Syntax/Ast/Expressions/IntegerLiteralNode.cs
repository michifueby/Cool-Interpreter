//-----------------------------------------------------------------------
// <copyright file="IntegerLiteralNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>IntegerLiteralNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an integer literal in the Cool language.
/// This node is part of the Abstract Syntax Tree (AST) and encapsulates
/// an integer value along with its source code position.
/// </summary>
public class IntegerLiteralNode : ExpressionNode
{
    /// <summary>
    /// Represents an integer literal node in the Cool Abstract Syntax Tree (AST).
    /// This node encapsulates an integer value and its source position within the code.
    /// </summary>
    public IntegerLiteralNode(int value, SourcePosition location)
        : base(location) => Value = value;
    
    /// <summary>
    /// Gets the integer value represented by this node.
    /// This property provides access to the literal integer
    /// value encapsulated within the IntegerLiteralNode as part
    /// of the Abstract Syntax Tree (AST).
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and allows the visitor to perform an operation specific to this node type.
    /// </summary>
    /// <typeparam name="T">The return type of the operation to be performed by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance that will process this node.</param>
    /// <returns>The result of the operation performed by the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}