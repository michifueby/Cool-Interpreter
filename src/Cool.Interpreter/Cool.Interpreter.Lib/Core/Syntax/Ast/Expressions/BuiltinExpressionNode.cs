namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

/// <summary>
/// Represents a node in the Cool Abstract Syntax Tree (AST) that corresponds to a built-in expression.
/// A built-in expression is a pre-defined construct in the language that may have special semantics
/// or evaluation rules not defined through user code.
/// </summary>
public class BuiltinExpressionNode : ExpressionNode
{
    /// <summary>
    /// Gets the name of the built-in expression represented by this node.
    /// The name uniquely identifies the specific built-in construct in the Cool language syntax.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Represents a node in the abstract syntax tree (AST) corresponding to a built-in expression in the Cool programming language.
    /// </summary>
    /// <remarks>
    /// Built-in expressions are pre-defined constructs in Cool that provide functionality
    /// or semantics directly supported by the language. These expressions may not be implemented
    /// by the user and instead are treated as inherent features of the language.
    /// </remarks>
    public BuiltinExpressionNode(string name) : base(SourcePosition.None) => Name = name;

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface and
    /// delegates the processing logic for the current <see cref="BuiltinExpressionNode"/> instance.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">The visitor instance implementing the logic to process this node.</param>
    /// <returns>The result of processing the current node through the visitor.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);
}