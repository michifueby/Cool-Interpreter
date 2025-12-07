//-----------------------------------------------------------------------
// <copyright file="IdentifierExpressionNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>IdentifierExpressionNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents an identifier expression node in the Cool language's abstract syntax tree.
/// This node corresponds to identifiers in the source code and serves to encapsulate
/// their associated information such as name and location.
/// </summary>
public class IdentifierExpressionNode : ExpressionNode
{
    /// <summary>
    /// Represents an identifier expression node in the abstract syntax tree of the Cool language.
    /// This node is used for identifiers in the source code and stores information about the identifier,
    /// such as its name and its source code position.
    /// </summary>
    public IdentifierExpressionNode(string identifier, SourcePosition location)
        : base(location) => Identifier = identifier;
    
    /// <summary>
    /// Gets the name of the identifier represented by this node.
    /// This property holds the textual representation of the identifier
    /// as it appears in the source code. It serves as a key for variable
    /// lookups, function calls, and other identifier-related operations
    /// during interpretation or compilation.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface.
    /// This method allows the visitor to perform operations specific to the
    /// current <see cref="IdentifierExpressionNode"/> instance within the abstract syntax tree.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance that is operating on the syntax node.</param>
    /// <returns>A value of type <typeparamref name="T"/> produced by the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}