//-----------------------------------------------------------------------
// <copyright file="NoExpressionNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>NoExpressionNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an expression node in the abstract syntax tree that explicitly indicates
/// the absence of any meaningful expression. This node is used as a placeholder
/// in scenarios where an expression is syntactically required but not provided.
/// </summary>
public class NoExpressionNode : ExpressionNode
{
    /// <summary>
    /// Represents an expression node indicating the absence of a meaningful expression
    /// in the abstract syntax tree. This class serves as a placeholder for cases where
    /// an expression is syntactically required but not provided.
    /// </summary>
    public NoExpressionNode(SourcePosition location) : base(location)
    {
    }

    /// <summary>
    /// Accepts a visitor implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and allows it to process this node within the abstract syntax tree.
    /// The specific visit method invoked depends on the dynamic type of the node.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result returned by the visitor after processing this node.
    /// </typeparam>
    /// <param name="visitor">
    /// The visitor responsible for performing operations on this node.
    /// </param>
    /// <returns>
    /// The result of the visitor's operation on this node, which is of type <typeparamref name="T"/>.
    /// </returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}