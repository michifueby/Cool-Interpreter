//-----------------------------------------------------------------------
// <copyright file="StringLiteralNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>StringLiteralNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a string literal in the Cool language's abstract syntax tree (AST).
/// This class is a specific type of expression node that encapsulates a string literal value defined in the source code.
/// </summary>
public class StringLiteralNode : ExpressionNode
{
    /// <summary>
    /// Represents a string literal in the Cool language's abstract syntax tree (AST).
    /// This node encapsulates the value of the string literal as defined in the source code
    /// and its corresponding location within the source file.
    /// </summary>
    public StringLiteralNode(string value, SourcePosition location)
        : base(location) => Value = value;
    
    /// <summary>
    /// Gets the string literal value represented by this node in the abstract syntax tree.
    /// This value corresponds to the string defined in the source code and is encapsulated
    /// as part of the StringLiteralNode for further processing or evaluation.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface for processing this node.
    /// This method follows the visitor pattern, allowing external operations to be defined for this specific type of AST node.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the visitor after processing this node.</typeparam>
    /// <param name="visitor">An object implementing the <see cref="ICoolSyntaxVisitor{T}"/> interface used to visit this node.</param>
    /// <returns>The result of the visitor's operation on this node, of type <typeparamref name="T"/>.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}