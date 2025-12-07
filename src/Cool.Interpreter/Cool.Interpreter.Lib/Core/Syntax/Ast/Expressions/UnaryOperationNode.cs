//-----------------------------------------------------------------------
// <copyright file="UnaryOperationNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>UnaryOperationNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

using Cool.Interpreter.Lib.Core.Syntax.Operators;

/// <summary>
/// Represents a unary operation node in the abstract syntax tree.
/// A unary operation consists of a single operand and a unary operator.
/// </summary>
public class UnaryOperationNode : ExpressionNode
{
    /// <summary>
    /// Gets the unary operator used in the unary operation.
    /// </summary>
    /// <remarks>
    /// The <c>Operator</c> property specifies the type of unary operation to be applied,
    /// such as negation (<c>UnaryOperator.Negate</c>) or logical negation (<c>UnaryOperator.Not</c>).
    /// </remarks>
    public UnaryOperator Operator { get; }

    /// <summary>
    /// Gets the operand for the unary operation.
    /// </summary>
    /// <remarks>
    /// The <c>Operand</c> property represents the expression to which the unary operator,
    /// specified by the <c>Operator</c> property, is applied during evaluation.
    /// </remarks>
    public ExpressionNode Operand { get; }

    /// <summary>
    /// Represents a unary operation node in the abstract syntax tree.
    /// A unary operation involves a single operand and a unary operator.
    /// </summary>
    public UnaryOperationNode(UnaryOperator @operator, ExpressionNode operand,
        SourcePosition location)
        : base(location)
    {
        Operator = @operator;
        Operand = operand;
    }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and processes this <see cref="UnaryOperationNode"/> instance.
    /// </summary>
    /// <typeparam name="T">The return type produced by the visitor.</typeparam>
    /// <param name="visitor">An object implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface.</param>
    /// <returns>The result of the visitor's operation on this node.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}