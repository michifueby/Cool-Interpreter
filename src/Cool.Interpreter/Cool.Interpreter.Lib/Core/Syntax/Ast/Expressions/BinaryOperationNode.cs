//-----------------------------------------------------------------------
// <copyright file="BinaryOperationNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>BinaryOperationNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

using Cool.Interpreter.Lib.Core.Syntax.Operators;

/// <summary>
/// Represents a binary operation expression within the syntax tree.
/// </summary>
/// <remarks>
/// A binary operation consists of a left operand, a binary operator, and a right operand.
/// This node is part of the abstract syntax tree used to represent the structure of a COOL program.
/// </remarks>
public class BinaryOperationNode : ExpressionNode
{
    /// <summary>
    /// Represents a binary operation expression in the abstract syntax tree.
    /// This node encapsulates a left operand, a binary operator, and a right operand,
    /// and is used to model operations such as addition, subtraction, comparison, etc.
    /// </summary>
    public BinaryOperationNode(ExpressionNode left, BinaryOperator @operator,
        ExpressionNode right, SourcePosition location)
        : base(location)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
    
    /// <summary>
    /// Gets the left operand of the binary operation.
    /// </summary>
    /// <remarks>
    /// The left operand is an expression node within the abstract syntax tree (AST).
    /// It represents the value or sub-expression that appears on the left-hand side
    /// of the binary operator in the binary operation.
    /// </remarks>
    public ExpressionNode Left { get; }

    /// <summary>
    /// Gets the operator of the binary operation.
    /// </summary>
    /// <remarks>
    /// The operator defines the type of binary operation to be performed, such as addition,
    /// subtraction, multiplication, division, or relational comparisons (e.g., less than or equal).
    /// It is represented by a value from the <c>BinaryOperator</c> enumeration.
    /// </remarks>
    public BinaryOperator Operator { get; }

    /// <summary>
    /// Gets the right operand of the binary operation.
    /// </summary>
    /// <remarks>
    /// The right operand is an expression node within the abstract syntax tree (AST).
    /// It represents the value or sub-expression that appears on the right-hand side
    /// of the binary operator in the binary operation.
    /// </remarks>
    public ExpressionNode Right { get; }

    /// <summary>
    /// Accepts a visitor that conforms to the ICoolSyntaxVisitor interface and processes the current binary operation expression.
    /// This method supports the visitor pattern, allowing external operations to be performed on the binary operation node without
    /// modifying its structure.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance that will process the binary operation node.</param>
    /// <returns>The result produced by the visitor's operation.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}