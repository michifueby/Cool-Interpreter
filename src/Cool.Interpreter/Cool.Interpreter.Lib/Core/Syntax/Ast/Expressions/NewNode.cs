//-----------------------------------------------------------------------
// <copyright file="NewNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>NewNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a syntactic node corresponding to the creation of a new object in the Cool language.
/// This node encapsulates the type name of the object to be instantiated and its source location in the code.
/// </summary>
public class NewNode : ExpressionNode
{
    /// <summary>
    /// Represents a syntactic node for object instantiation in the Cool programming language.
    /// This class models a `new` expression in the abstract syntax tree, which is used to create
    /// a new instance of a specified type during execution.
    /// </summary>
    public NewNode(string typeName, SourcePosition location)
        : base(location) => TypeName = typeName;
    
    /// <summary>
    /// Gets the name of the type associated with the object to be instantiated in the Cool language.
    /// This property represents the class identifier of the object being created using the "new" expression.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Accepts a visitor that processes the current syntactic node in the abstract syntax tree.
    /// This method facilitates the implementation of the visitor pattern, enabling behavior to be defined
    /// for this specific node type without modifying the node itself.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor object implementing logic for processing this node.</param>
    /// <returns>The result of the visitor's operation on this node.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}