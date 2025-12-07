//-----------------------------------------------------------------------
// <copyright file="ExpressionNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ExpressionNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an abstract syntax tree node for an expression in the Cool language.
/// This serves as the base class for all specific types of expressions in the syntax tree.
/// </summary>
public abstract class ExpressionNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents the base class for all expression nodes in the syntax tree.
    /// </summary>
    /// <remarks>
    /// An expression node is a component of the abstract syntax tree (AST) that
    /// represents an expression in the source code. This class serves as the base
    /// class for all specific types of expressions. Each derived class must define
    /// the specific behavior or structure for its corresponding expression type.
    /// </remarks>
    /// <param name="location">
    /// Specifies the source location of the expression node, which identifies
    /// the position of the expression in the source code for diagnostic purposes.
    /// </param>
    protected ExpressionNode(SourcePosition location) : base(location)
    {
    }
}