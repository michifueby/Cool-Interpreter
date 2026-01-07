//-----------------------------------------------------------------------
// <copyright file="MethodNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>MethodNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Features;

using Expressions;

/// <summary>
/// Represents a method declaration within the program's syntax tree.
/// A MethodNode contains the method's name, its formal parameters,
/// its return type, and the expression that comprises its body.
/// This class enables analysis, transformation, and interpretation
/// of method-related constructs in the AST (Abstract Syntax Tree).
/// </summary>
public class MethodNode : FeatureNode
{
    /// <summary>
    /// Represents a method definition node in the abstract syntax tree (AST).
    /// This node models a method, including its name, parameters (formals),
    /// return type, body, and location in the source code.
    /// </summary>
    public MethodNode(string name, IReadOnlyList<FormalNode> formals,
        string returnTypeName, ExpressionNode body, SourcePosition location)
        : base(name, location)
    {
        Formals = formals;
        ReturnTypeName = returnTypeName;
        Body = body;
    }
    
    /// <summary>
    /// Gets the list of formal parameters for the method.
    /// This property provides an immutable collection of <see cref="FormalNode"/> objects,
    /// each representing a single parameter declared in the method.
    /// Formal parameters define the inputs required by the method and include
    /// their names and types.
    /// </summary>
    public IReadOnlyList<FormalNode> Formals { get; }

    /// <summary>
    /// Gets the name of the return type for the method.
    /// This property represents the type of value the method is expected to return.
    /// Return type names are typically used during semantic analysis in the
    /// interpretation or compilation of the program.
    /// </summary>
    public string ReturnTypeName { get; }

    /// <summary>
    /// Gets the expression that defines the body of the method.
    /// This property represents the implementation of the method,
    /// encapsulating its logic within an <see cref="ExpressionNode"/>.
    /// The body of the method is executed when the method is invoked,
    /// and it defines the operations to be performed and the value to be returned, if any.
    /// </summary>
    public ExpressionNode Body { get; }

    /// <summary>
    /// Accepts a visitor implementing the ICoolSyntaxVisitor interface,
    /// allowing external logic to be applied to the MethodNode instance
    /// based on the visitor's implementation.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance processing this MethodNode.</param>
    /// <returns>The result of the visitor's operation on this MethodNode.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}