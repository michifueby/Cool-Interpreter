//-----------------------------------------------------------------------
// <copyright file="CoolSyntaxNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolSyntaxNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Abstract base class for all nodes in the Cool Abstract Syntax Tree (AST).
/// Represents a single construct in Cool source code (class, expression, feature, etc.).
/// </summary>
public abstract class CoolSyntaxNode
{
    /// <summary>
    /// Represents the base class for constructing types of nodes in the Cool Abstract Syntax Tree (AST).
    /// Provides a common interface for various Cool language constructs such as classes, expressions,
    /// features, and other core structural components within Cool source code.
    /// </summary>
    protected CoolSyntaxNode(SourcePosition location)
        => Location = location;
    
    /// <summary>
    /// Source location of this node — used for diagnostics and debugging.
    /// Never null — defaults to SourcePosition.None for synthetic nodes.
    /// </summary>
    public SourcePosition Location { get; }

    /// <summary>
    /// Accepts a visitor that processes this syntax node and returns a result of the specified type.
    /// This method implements the Visitor design pattern, allowing the visitor to traverse
    /// and act upon specific syntax node types while maintaining separation of concerns.
    /// </summary>
    /// <typeparam name="T">The type of result produced by the visitor's processing.</typeparam>
    /// <param name="visitor">The visitor instance that performs specific logic on nodes of the syntax tree.</param>
    /// <returns>The result produced by the visitor after processing this syntax node.</returns>
    public abstract T Accept<T>(ICoolSyntaxVisitor<T> visitor);
}