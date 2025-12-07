//-----------------------------------------------------------------------
// <copyright file="LetBindingNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>LetBindingNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a single binding in a "let" expression within the Cool programming language's Abstract Syntax Tree (AST).
/// A binding connects an identifier to a type and optionally includes an initial value (initializer).
/// </summary>
public class LetBindingNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents a single binding in a "let" expression within the Cool programming language's Abstract Syntax Tree (AST).
    /// A binding connects an identifier to a type and optionally includes an initial value (initializer).
    /// </summary>
    public LetBindingNode(string identifier, string typeName,
        ExpressionNode? initializer, SourcePosition location)
        : base(location)
    {
        Identifier = identifier;
        TypeName = typeName;
        Initializer = initializer;
    }
    
    /// <summary>
    /// Gets the identifier for the let binding in the Cool programming language's Abstract Syntax Tree (AST).
    /// The identifier serves as the name for the variable being introduced in the let expression.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the type name associated with the binding in a "let" expression within the Cool programming language's Abstract Syntax Tree (AST).
    /// The type name specifies the expected type of the variable being introduced in the binding.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the initializer expression for the let binding in the Cool programming language's Abstract Syntax Tree (AST).
    /// The initializer defines an optional initial value for the variable being introduced in the let expression.
    /// If no initializer is provided, the variable is set to a default value based on its type.
    /// </summary>
    public ExpressionNode? Initializer { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and processes the current <see cref="LetBindingNode"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance responsible for processing this LetBindingNode.</param>
    /// <returns>The result of the visitor's processing logic tailored to this LetBindingNode.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}