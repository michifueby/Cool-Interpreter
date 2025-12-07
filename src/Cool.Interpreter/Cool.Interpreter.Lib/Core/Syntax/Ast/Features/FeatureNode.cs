//-----------------------------------------------------------------------
// <copyright file="FeatureNode.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>FeatureNode</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast.Features;

/// <summary>
/// Represents a feature node in the Cool abstract syntax tree (AST).
/// Serves as an abstract base class for specific feature types such as
/// attributes and methods within a class definition.
/// </summary>
/// <remarks>
/// A feature node uniquely identifies an element of functionality within
/// a class, such as a field or a method, and includes metadata such as
/// location and name.
/// </remarks>
public abstract class FeatureNode : CoolSyntaxNode
{
    /// <summary>
    /// Gets the name associated with this feature node.
    /// </summary>
    /// <remarks>
    /// The name represents the identifier of the feature, such as an attribute name,
    /// method name, or similar element within the concept of a class or program construct.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Abstract base class representing a feature node in the Cool abstract syntax tree (AST).
    /// </summary>
    /// <remarks>
    /// A feature node models specific elements of a class, such as attributes
    /// (fields) or methods (functions). This class holds common metadata and
    /// behaviors shared across all feature types in the AST.
    /// </remarks>
    protected FeatureNode(string name, SourcePosition location)
        : base(location) => Name = name;
}