//-----------------------------------------------------------------------
// <copyright file="AssignNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>AssignNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an assignment expression in the Cool abstract syntax tree (AST).
/// An assignment expression consists of an identifier and an associated expression
/// whose evaluated result is assigned to that identifier.
/// </summary>
public class AssignNode : ExpressionNode
{
    /// <summary>
    /// Represents an assignment node in the abstract syntax tree (AST) of the Cool programming language.
    /// This node encapsulates the process of assigning the result of an expression to a specified identifier.
    /// </summary>
    public AssignNode(string identifier, ExpressionNode expression, SourcePosition location)
        : base(location)
    {
        Identifier = identifier;
        Expression = expression;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the expression associated with an assignment operation in the abstract syntax tree (AST).
    /// This property represents the right-hand side of the assignment
    /// where the value to be assigned is stored or computed.
    /// </summary>
    public ExpressionNode Expression { get; }

    /// <summary>
    /// Accepts a visitor implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface for traversing
    /// or processing the current node in the Cool abstract syntax tree (AST).
    /// This method dispatches the current instance to the visitor's appropriate method for handling
    /// the specific node type.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance responsible for processing the current node.</param>
    /// <returns>The result produced by the visitor after processing this node.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}