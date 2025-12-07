//-----------------------------------------------------------------------
// <copyright file="DispatchNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>DispatchNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a node in the abstract syntax tree that models a method dispatch (method call) expression.
/// </summary>
public class DispatchNode : ExpressionNode
{
    /// <summary>
    /// Represents a node in the abstract syntax tree (AST) for a method dispatch (method call) expression.
    /// This node enables modeling the invocation of a method, optionally specifying a caller,
    /// a static type, a method name, and a collection of arguments.
    /// </summary>
    public DispatchNode(ExpressionNode caller, string? staticTypeName,
        string methodName, IReadOnlyList<ExpressionNode> arguments, SourcePosition location)
        : base(location)
    {
        Caller = caller;
        StaticTypeName = staticTypeName;
        MethodName = methodName;
        Arguments = arguments;
    }
    
    /// <summary>
    /// Gets the expression node representing the caller object of the method dispatch.
    /// This is the object on which the method is invoked.
    /// </summary>
    public ExpressionNode Caller { get; }

    /// <summary>
    /// Gets the name of the static type to be used for method dispatch resolution.
    /// If specified, this type is used to determine the method being invoked,
    /// overriding the dynamic type of the caller object.
    /// </summary>
    public string? StaticTypeName { get; } // For e@T.f()

    /// <summary>
    /// Gets the name of the method to be dispatched in the method call expression.
    /// This represents the identifier of the method being invoked.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the list of argument expressions passed to the method during its dispatch.
    /// These expressions represent the values or computations supplied as input parameters
    /// when invoking the method.
    /// </summary>
    public IReadOnlyList<ExpressionNode> Arguments { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface.
    /// This method facilitates the implementation of the Visitor design pattern,
    /// enabling external operations to be performed on this node without modifying its structure.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the visitor.</typeparam>
    /// <param name="visitor">The visitor instance implementing the logic specific to this node type.</param>
    /// <returns>The result of visiting this node, which is determined by the logic within the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}