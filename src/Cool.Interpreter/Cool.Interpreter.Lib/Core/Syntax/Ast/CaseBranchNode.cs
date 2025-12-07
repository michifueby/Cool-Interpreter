//-----------------------------------------------------------------------
// <copyright file="CaseBranchNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CaseBranchNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a branch in the `case` expression of the Cool programming language.
/// Each branch includes an identifier, a type name for type-checking purposes,
/// and an associated expression body that will execute when the branch is chosen.
/// </summary>
public class CaseBranchNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents a branch within a `case` expression in the Cool programming language.
    /// A `CaseBranchNode` contains an identifier for pattern matching, a type name
    /// for type-checking the branch, and an expression body that is executed if this
    /// branch is selected during evaluation.
    /// </summary>
    public CaseBranchNode(string identifier, string typeName,
        ExpressionNode body, SourcePosition location)
        : base(location)
    {
        Identifier = identifier;
        TypeName = typeName;
        Body = body;
    }
    
    /// <summary>
    /// Gets the identifier of the case branch variable.
    /// This identifier represents the variable name assigned to the value
    /// being matched in the context of this branch. It is used for
    /// evaluation and scope resolution within the expression body of the branch.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the type name associated with this branch in the `case` expression.
    /// This type is used for type-checking and determines whether this branch
    /// will be chosen during the evaluation of the `case` expression.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the body expression of the case branch.
    /// This expression represents the code to be executed when this branch is selected
    /// during the evaluation of a `case` expression in the Cool programming language.
    /// </summary>
    public ExpressionNode Body { get; }

    /// <summary>
    /// Accepts a visitor implementing the `ICoolSyntaxVisitor` interface to perform an operation
    /// based on the type of this node. It is part of the visitor pattern implementation
    /// used for traversing the Cool Abstract Syntax Tree (AST).
    /// </summary>
    /// <param name="visitor">An instance of a class implementing the `ICoolSyntaxVisitor` interface.</param>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <returns>The result of the visitor's operation, determined by the visitor's implementation.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}