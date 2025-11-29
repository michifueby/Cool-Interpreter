//-----------------------------------------------------------------------
// <copyright file="ClassNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ClassNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents a class node within the Cool Abstract Syntax Tree (AST).
/// Encapsulates the definition of a Cool class, including its name,
/// its optional inheritance specification, its features, and its location within the source code.
/// </summary>
public class ClassNode : CoolSyntaxNode
{
    /// <summary>
    /// Gets the name of the class represented by this <see cref="ClassNode"/> instance.
    /// This value corresponds to the identifier used to define the class within the Cool program.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the parent class from which this <see cref="ClassNode"/> instance inherits.
    /// Returns <c>null</c> if the class does not explicitly inherit from another class.
    /// </summary>
    public string? InheritsFrom { get; }

    /// <summary>
    /// Gets the collection of features defined within this <see cref="ClassNode"/> instance.
    /// Features represent the structural and behavioral components of the class, including
    /// attributes and methods.
    /// </summary>
    public IReadOnlyList<FeatureNode> Features { get; }

    /// <summary>
    /// Gets the location of this <see cref="ClassNode"/> instance within the source code.
    /// Represents the position in the file, including line and column information,
    /// where the class definition is declared.
    /// </summary>
    public SourcePosition Location { get; }  

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassNode"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="inheritsFrom"></param>
    /// <param name="features"></param>
    /// <param name="location"></param>
    public ClassNode(string name, string? inheritsFrom, 
        IReadOnlyList<FeatureNode> features, 
        SourcePosition location)
    {
        Name = name;
        InheritsFrom = inheritsFrom;
        Features = features;
        Location = location;
    }
}