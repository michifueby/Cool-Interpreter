//-----------------------------------------------------------------------
// <copyright file="ProgramNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ProgramNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// The root of a Cool program — Includes a list of classes.
/// </summary>
public sealed class ProgramNode : CoolSyntaxNode
{
    /// <summary>
    /// All classes in the program, in source order.
    /// Empty list = empty program.
    /// </summary>
    public IReadOnlyList<ClassNode> Classes { get; }

    /// <summary>
    /// Creates a new program with the given classes.
    /// </summary>
    public ProgramNode(IReadOnlyList<ClassNode> classes)
    {
        Classes = classes;
    }

    /// <summary>
    /// For debugging: show how many classes
    /// </summary>
    public override string ToString()
        => Classes.Count == 0 
            ? "Program (empty)" 
            : $"Program with {Classes.Count} class(es)";
}