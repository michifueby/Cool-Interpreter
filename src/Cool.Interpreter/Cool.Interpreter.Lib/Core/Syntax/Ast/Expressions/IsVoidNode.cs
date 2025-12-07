//-----------------------------------------------------------------------
// <copyright file="IsVoidNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>IsVoidNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a node in the abstract syntax tree (AST) for the "isvoid" operation in the Cool language.
/// The "isvoid" operation evaluates whether the provided expression evaluates to a void object.
/// </summary>
public class IsVoidNode : ExpressionNode
{
    /// <summary>
    /// Represents a node in the abstract syntax tree (AST) for the "isvoid" operation in the Cool language.
    /// The "isvoid" operation evaluates whether an expression results in a void object.
    /// </summary>
    public IsVoidNode(ExpressionNode expression, SourcePosition location)
        : base(location) => Expression = expression;
    
    /// <summary>
    /// Gets the expression associated with the "isvoid" operation.
    /// This expression is evaluated to determine if it results in a void object.
    /// </summary>
    public ExpressionNode Expression { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and invokes the corresponding visit method for the current node type.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's visit method.</typeparam>
    /// <param name="visitor">The visitor instance that will process this node.</param>
    /// <returns>The result of invoking the visitor's visit method for this node type.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}