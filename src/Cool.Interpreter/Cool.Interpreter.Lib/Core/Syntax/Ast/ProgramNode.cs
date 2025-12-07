//-----------------------------------------------------------------------
// <copyright file="ProgramNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ProgramNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents the root node of a Cool language abstract syntax tree (AST).
/// </summary>
/// <remarks>
/// A program in the Cool language is represented as a collection of class nodes.
/// This class serves as the container for all class declarations within a specific program.
/// </remarks>
public class ProgramNode : CoolSyntaxNode
{
    /// <summary>
    /// Represents the root node of an abstract syntax tree (AST) for a Cool program.
    /// </summary>
    /// <remarks>
    /// A program in the Cool language is defined as a collection of class declarations.
    /// The <see cref="ProgramNode"/> serves as a container for all class nodes within a program,
    /// enabling the hierarchical representation of the Cool language structure.
    /// </remarks>
    public ProgramNode(IReadOnlyList<ClassNode> classes, SourcePosition location = default)
        : base(location) => Classes = classes;
    
    /// <summary>
    /// Gets the collection of all class declarations within the Cool program represented by this syntax tree.
    /// </summary>
    /// <remarks>
    /// The list contains all the class nodes defined in the Cool program, providing access to their declarations,
    /// features, and other metadata.
    /// </remarks>
    public IReadOnlyList<ClassNode> Classes { get; }

    /// <summary>
    /// Accepts a visitor that implements the <see cref="ICoolSyntaxVisitor{T}"/> interface
    /// and processes the current <see cref="ProgramNode"/> instance.
    /// </summary>
    /// <typeparam name="T">The return type of the visitor's operation.</typeparam>
    /// <param name="visitor">An implementation of <see cref="ICoolSyntaxVisitor{T}"/> that will process this node.</param>
    /// <returns>The result of the visitor's operation, defined by the type <typeparamref name="T"/>.</returns>
    public override T Accept<T>(ICoolSyntaxVisitor<T> visitor)
        => visitor.Visit(this);

    /// <summary>
    /// Converts the <see cref="ProgramNode"/> instance to its string representation.
    /// </summary>
    /// <returns>
    /// A string indicating the state of the program. If the program contains no classes,
    /// the string "Program (empty)" is returned. Otherwise, it returns a string
    /// specifying the number of classes in the program.
    /// </returns>
    public override string ToString()
        => Classes.Count == 0 ? "Program (empty)" : $"Program with {Classes.Count} class(es)";
}