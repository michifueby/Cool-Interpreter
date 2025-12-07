//-----------------------------------------------------------------------
// <copyright file="CaseNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CaseNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a "case" expression node in the Cool Abstract Syntax Tree (AST).
/// Used to model a "case" construct in the Cool programming language,
/// which supports pattern matching and control flow based on branches.
/// </summary>
public class CaseNode : ExpressionNode
{
    /// <summary>
    /// Represents the expression being evaluated in the "case" construct.
    /// This property holds the main expression whose value is matched against
    /// the branches in the "case" statement of the Cool programming language.
    /// </summary>
    public ExpressionNode Scrutinee { get; }

    /// <summary>
    /// Represents the collection of branches in a "case" expression in the Cool programming language.
    /// Each branch specifies a condition and its corresponding action to be executed
    /// if the condition matches the result of the expression being evaluated.
    /// </summary>
    public IReadOnlyList<CaseBranchNode> Branches { get; }

    /// <summary>
    /// Represents a node in the Cool Abstract Syntax Tree (AST) for a "case" expression.
    /// A "case" expression evaluates a scrutinee expression and then matches it
    /// against a set of branches for pattern-based control flow.
    /// </summary>
    public CaseNode(ExpressionNode scrutinee, IReadOnlyList<CaseBranchNode> branches,
        SourcePosition location)
        : base(location)
    {
        Scrutinee = scrutinee;
        Branches = branches;
    }

    /// <summary>
    /// Accepts a visitor of the Cool Abstract Syntax Tree (AST) and allows it to
    /// process this "case" expression node. This method enables visiting logic to
    /// be defined externally from the node structure, promoting separation of concerns.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the visitor after processing this node.</typeparam>
    /// <param name="visitor">An implementation of the <see cref="ICoolSyntaxVisitor{T}" />
    /// interface that defines the visiting logic for this node.</param>
    /// <returns>A value of type <typeparamref name="T"/> resulting from the visitor's
    /// processing of this "case" expression node.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}