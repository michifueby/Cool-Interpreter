//-----------------------------------------------------------------------
// <copyright file="BuiltinExpressionNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>BuiltinExpressionNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a node in the Cool Abstract Syntax Tree (AST) that corresponds to a built-in expression.
/// A built-in expression is a pre-defined construct in the language that may have special semantics
/// or evaluation rules not defined through user code.
/// </summary>
public class BuiltinExpressionNode : ExpressionNode
{
    /// <summary>
    /// Represents a node in the abstract syntax tree (AST) corresponding to a built-in expression in the Cool programming language.
    /// </summary>
    /// <remarks>
    /// Built-in expressions are pre-defined constructs in Cool that provide functionality
    /// or semantics directly supported by the language. These expressions may not be implemented
    /// by the user and instead are treated as inherent features of the language.
    /// </remarks>
    public BuiltinExpressionNode(string name, IReadOnlyList<ExpressionNode> arguments, SourcePosition position)
        : base(position)
    {
        Name = name;
        Arguments = arguments;
    }
    
    /// <summary>
    /// Gets the name of the built-in expression represented by this node.
    /// The name uniquely identifies the specific built-in construct in the Cool language syntax.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the list of arguments associated with the built-in expression node.
    /// Each argument is represented as an expression node and corresponds to the inputs
    /// provided to the built-in construct during its evaluation.
    /// </summary>
    public IReadOnlyList<ExpressionNode> Arguments { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface and
    /// delegates the processing logic for the current <see cref="BuiltinExpressionNode"/> instance.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance implementing the logic to process this node.</param>
    /// <returns>The result of processing the current node through the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}