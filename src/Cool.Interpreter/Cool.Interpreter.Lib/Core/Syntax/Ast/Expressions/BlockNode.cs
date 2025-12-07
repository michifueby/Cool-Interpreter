//-----------------------------------------------------------------------
// <copyright file="BlockNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>BlockNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a block expression in the Cool Abstract Syntax Tree (AST).
/// A block is a sequence of expressions where the result of the block
/// is the result of the last expression in the sequence.
/// </summary>
public class BlockNode : ExpressionNode
{
    /// <summary>
    /// Represents a block node in the Abstract Syntax Tree (AST),
    /// which is a sequence of expressions enclosed within the node.
    /// The value of the block is determined by the last expression in the sequence.
    /// </summary>
    public BlockNode(IReadOnlyList<ExpressionNode> expressions, SourcePosition location)
        : base(location)
        => Expressions = expressions;
    
    /// <summary>
    /// Gets the sequence of expressions contained within the block.
    /// Each expression in the sequence is evaluated in order, and the result of the block
    /// is determined by the evaluation of the last expression in the sequence.
    /// </summary>
    public IReadOnlyList<ExpressionNode> Expressions { get; }

    /// <summary>
    /// Accepts a visitor that conforms to the ICoolSyntaxVisitor interface
    /// and dispatches the current BlockNode instance to the corresponding
    /// Visit method, enabling operations to be performed on the node.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's Visit operation.</typeparam>
    /// <param name="visitor">An instance of a class that implements ICoolSyntaxVisitor.
    /// The visitor defines the operation to be performed on this BlockNode.</param>
    /// <returns>The result of the visitor's Visit operation applied to this BlockNode.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}