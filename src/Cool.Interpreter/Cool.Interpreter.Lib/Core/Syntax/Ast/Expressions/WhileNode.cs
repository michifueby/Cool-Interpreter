//-----------------------------------------------------------------------
// <copyright file="WhileNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>WhileNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a "while" loop construct in the abstract syntax tree of the Cool programming language.
/// Encapsulates a predicate (condition) and a body, which are both expressions.
/// The loop executes the body as long as the predicate evaluates to true.
/// </summary>
public class WhileNode : ExpressionNode
{
    /// <summary>
    /// Represents a "while" loop construct in the Cool programming language's abstract syntax tree.
    /// The WhileNode consists of a loop condition (predicate) and a loop body, both of which are expressions.
    /// The loop will execute the body repeatedly as long as the predicate evaluates to true.
    /// </summary>
    public WhileNode(ExpressionNode predicate, ExpressionNode body, SourcePosition location)
        : base(location)
    {
        Predicate = predicate;
        Body = body;
    }
    
    /// <summary>
    /// Gets the expression representing the condition for the "while" loop.
    /// The loop continues executing its body as long as this predicate evaluates to true.
    /// </summary>
    public ExpressionNode Predicate { get; }

    /// <summary>
    /// Gets the expression representing the body of the "while" loop.
    /// This body is executed repeatedly as long as the predicate of the loop evaluates to true.
    /// </summary>
    public ExpressionNode Body { get; }
    
    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface.
    /// Enables the visitor to process this <see cref="WhileNode"/> during traversal of the abstract syntax tree (AST).
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The implementation of the visitor pattern for this AST node.</param>
    /// <returns>The result of the visitor's operation on this <see cref="WhileNode"/>.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}