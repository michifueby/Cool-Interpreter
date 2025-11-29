//-----------------------------------------------------------------------
// <copyright file="CoolClass.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolClass</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents a Cool class in the Cool programming language.
/// This class is immutable and encapsulates the structural hierarchy
/// and behavior of a Cool class during program execution,
/// including its name, parent class, methods, and attributes.
/// </summary>
public class CoolClass
{
    /// <summary>
    /// Gets the name of the Cool class.
    /// This property represents the identifier of the class,
    /// used to distinguish it from other classes within the
    /// Cool programming language hierarchy.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the parent of the Cool class.
    /// This property represents the parent class from which the current
    /// Cool class inherits, forming the hierarchical structure of the Cool
    /// programming language. If the class does not have a parent, this property
    /// will be null, indicating it is a root class within the hierarchy.
    /// </summary>
    public CoolClass? Parent { get; }

    /// <summary>
    /// Gets the collection of methods defined in the Cool class.
    /// Each method is represented as a key-value pair where the key is the method's name
    /// and the value is the corresponding <see cref="MethodNode"/>.
    /// This property enables the lookup of methods by their identifiers
    /// and defines the behavior associated with the class.
    /// </summary>
    public ImmutableDictionary<string, MethodNode> Methods { get; }

    /// <summary>
    /// Gets the attributes of the Cool class.
    /// This property contains a collection of attributes associated
    /// with the class, represented as an immutable dictionary where
    /// the key is the attribute name and the value is an
    /// <c>AttributeNode</c> describing the attribute's structure.
    /// </summary>
    public ImmutableDictionary<string, AttributeNode> Attributes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoolClass"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <param name="methods"></param>
    /// <param name="attributes"></param>
    public CoolClass(string name, CoolClass? parent,
        ImmutableDictionary<string, MethodNode> methods,
        ImmutableDictionary<string, AttributeNode> attributes)
    {
        Name = name;
        Parent = parent;
        Methods = methods;
        Attributes = attributes;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="CoolClass"/> instance.
    /// </summary>
    /// <returns>The name of the Cool class.</returns>
    public override string ToString() => Name;
}