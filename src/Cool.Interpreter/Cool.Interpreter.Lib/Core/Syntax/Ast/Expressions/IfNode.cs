//-----------------------------------------------------------------------
// <copyright file="IfNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>IfNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a conditional expression in the abstract syntax tree of the Cool language.
/// This node captures the structure of an "if-then-else" statement, where a predicate
/// expression determines which branch (then or else) will be evaluated at runtime.
/// </summary>
public class IfNode : ExpressionNode
{
    /// <summary>
    /// Represents a conditional expression node in the abstract syntax tree for the Cool language.
    /// Encapsulates the structure of an "if-then-else" expression, which evaluates the predicate
    /// to determine whether to execute the "then" or "else" branch at runtime.
    /// </summary>
    public IfNode(ExpressionNode predicate, ExpressionNode thenBranch,
        ExpressionNode elseBranch, SourcePosition location)
        : base(location)
    {
        Predicate = predicate;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }
    
    /// <summary>
    /// Gets the predicate expression of the "if-then-else" conditional statement.
    /// This is a Boolean expression that determines which branch (then or else)
    /// will be executed during runtime evaluation of the conditional.
    /// </summary>
    public ExpressionNode Predicate { get; }

    /// <summary>
    /// Gets the expression representing the "then" branch of the "if-then-else" conditional statement.
    /// This branch is evaluated and its result returned when the predicate expression evaluates to true.
    /// </summary>
    public ExpressionNode ThenBranch { get; }

    /// <summary>
    /// Gets the expression representing the "else" branch of the "if-then-else" conditional statement.
    /// This branch is evaluated and executed only when the predicate expression evaluates to false.
    /// </summary>
    public ExpressionNode ElseBranch { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface,
    /// enabling traversal or processing of this node in the abstract syntax tree.
    /// The visitor can perform an operation and optionally return a result
    /// by evaluating this conditional expression node.
    /// </summary>
    /// <typeparam name="T">
    /// The return type of the operation performed by the visitor on this node.
    /// </typeparam>
    /// <param name="visitor">
    /// The visitor that processes this conditional expression node in the AST.
    /// </param>
    /// <returns>
    /// The result of the operation performed by the visitor.
    /// </returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}