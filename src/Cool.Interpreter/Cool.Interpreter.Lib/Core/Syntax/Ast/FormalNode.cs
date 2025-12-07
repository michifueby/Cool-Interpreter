//-----------------------------------------------------------------------
// <copyright file="FormalNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>FormalNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents a formal parameter node in the abstract syntax tree (AST) of the Cool programming language.
/// A formal parameter defines the name and type of a parameter required by a method or a function.
/// </summary>
public class FormalNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents a formal parameter in the syntax tree, including its name, type, and source position.
    /// </summary>
    public FormalNode(string name, string typeName, SourcePosition location)
        : base(location)
    {
        Name = name;
        TypeName = typeName;
    }
    
    /// <summary>
    /// Gets the name of the formal parameter defined in the abstract syntax tree (AST).
    /// This property represents the identifier name associated with the parameter
    /// when it is declared in a method or function.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type name of the formal parameter defined in the abstract syntax tree (AST).
    /// This property specifies the data type associated with the parameter as required by
    /// the method or function it belongs to.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Accepts a visitor that performs an operation on this formal parameter node.
    /// </summary>
    /// <param name="visitor">The visitor implementation that performs the operation on this node.</param>
    /// <typeparam name="T">The type of the result produced by the visitor.</typeparam>
    /// <returns>The result of the visitor's operation.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}