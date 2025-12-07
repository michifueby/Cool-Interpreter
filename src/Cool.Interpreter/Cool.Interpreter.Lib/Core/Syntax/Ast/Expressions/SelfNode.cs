//-----------------------------------------------------------------------
// <copyright file="SelfNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SelfNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a reference to the current instance (`self`) within the Cool programming language.
/// This node is used in the Abstract Syntax Tree (AST) for constructs that involve the `self`
/// reference, allowing access to the current object.
/// </summary>
public class SelfNode : ExpressionNode
{
    /// <summary>
    /// Represents a reference to the current instance (`self`) within the Cool programming language.
    /// This class is part of the Abstract Syntax Tree (AST) and is used to model constructs that
    /// explicitly refer to the current object.
    /// </summary>
    public SelfNode(SourcePosition location) : base(location)
    {
    }

    /// <summary>
    /// Accepts a visitor implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// to process or transform this syntax node.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance to process this node.</param>
    /// <returns>A result of type <typeparamref name="T"/> produced by the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}