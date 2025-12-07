//-----------------------------------------------------------------------
// <copyright file="LetNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>LetNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a "let" expression node in the abstract syntax tree (AST) of the Cool language.
/// This node is used to define a scope-limited sequence of variable bindings followed
/// by an expression that utilizes those bindings.
/// </summary>
public class LetNode : ExpressionNode
{
    /// <summary>
    /// Represents a "let" node in the abstract syntax tree (AST) for the Cool programming language.
    /// This node encapsulates a block of variable bindings, restricting their scope to the context
    /// of a subsequent expression.
    /// </summary>
    public LetNode(IReadOnlyList<LetBindingNode> bindings,
        ExpressionNode body, SourcePosition location)
        : base(location)
    {
        Bindings = bindings;
        Body = body;
    }
    
    /// <summary>
    /// Gets the collection of variable bindings in a "let" expression.
    /// Each binding represents a variable defined in the scope of the "let" expression,
    /// including its identifier, type, and optional initializer.
    /// </summary>
    public IReadOnlyList<LetBindingNode> Bindings { get; }

    /// <summary>
    /// Gets the body expression of the "let" expression.
    /// This represents the main computation or sub-expression that utilizes
    /// the variables defined in the associated bindings of the "let" expression.
    /// </summary>
    public ExpressionNode Body { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface.
    /// This method enables double-dispatch, allowing the visitor to process this
    /// specific node type (`LetNode`) within the abstract syntax tree.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance implementing operations for the Cool syntax tree.</param>
    /// <returns>The result of the visitor's operation on this node.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}