//-----------------------------------------------------------------------
// <copyright file="ClassSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ClassSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Represents a class symbol within the Cool programming language's symbol table.
/// A class symbol encapsulates metadata for a class, including its name, parent class name,
/// source location, methods, and attributes.
/// </summary>
public class ClassSymbol
{
    /// <summary>
    /// Gets the name of the class represented by this symbol.
    /// This property identifies the class within the symbol table and is used
    /// to reference the class in various operations, such as type checking and inheritance resolution.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the parent class for this class symbol.
    /// This property identifies the class from which the current class inherits, allowing for
    /// the resolution of inherited methods, attributes, and relationships within the symbol table.
    /// </summary>
    public string? ParentName { get; }

    /// <summary>
    /// Gets the AST (Abstract Syntax Tree) node that defines this class in the source code.
    /// This property represents the syntactic definition of the class, including any attributes
    /// or methods declared within it. It facilitates analysis and processing of the class
    /// structure during interpretation or compilation.
    /// </summary>
    public ClassNode? Definition { get; }

    /// <summary>
    /// Gets the source position of the class symbol within the original code.
    /// This property indicates the location of the class definition and is useful
    /// for error reporting, debugging, and analyzing the structure of the code.
    /// </summary>
    public SourcePosition Location { get; }

    /// <summary>
    /// Gets the collection of methods defined in this class.
    /// This property maps method names to their corresponding symbols, providing access
    /// to method metadata, such as return types and signatures. It ensures that method
    /// definitions are uniquely identified within the class.
    /// </summary>
    public ImmutableDictionary<string, MethodSymbol> Methods { get; }

    /// <summary>
    /// Gets the collection of attributes defined in the class represented by this symbol.
    /// Attributes are fields or properties associated with the class, allowing the storage of data specific to instances of the class.
    /// This property is immutable and maps attribute names to their corresponding symbols.
    /// </summary>
    public ImmutableDictionary<string, AttributeSymbol> Attributes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassSymbol"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parentName"></param>
    /// <param name="definition"></param>
    /// <param name="location"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ClassSymbol(
        string name,
        string? parentName,
        ClassNode definition,                      
        SourcePosition location)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ParentName = parentName;
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Location = location == default ? SourcePosition.None : location;
        Methods = ImmutableDictionary.Create<string, MethodSymbol>(StringComparer.Ordinal);
        Attributes = ImmutableDictionary.Create<string, AttributeSymbol>(StringComparer.Ordinal);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassSymbol"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parentName"></param>
    /// <param name="location"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ClassSymbol(string name, string? parentName = null, SourcePosition location = default)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ParentName = parentName;
        Definition = null;                         // explicitly allowed for built-ins
        Location = location == default ? SourcePosition.None : location;
        Methods = ImmutableDictionary.Create<string, MethodSymbol>(StringComparer.Ordinal);
        Attributes = ImmutableDictionary.Create<string, AttributeSymbol>(StringComparer.Ordinal);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassSymbol"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parentName"></param>
    /// <param name="definition"></param>
    /// <param name="location"></param>
    /// <param name="methods"></param>
    /// <param name="attributes"></param>
    private ClassSymbol(
        string name,
        string? parentName,
        ClassNode? definition,
        SourcePosition location,
        ImmutableDictionary<string, MethodSymbol> methods,
        ImmutableDictionary<string, AttributeSymbol> attributes)
    {
        Name = name;
        ParentName = parentName;
        Definition = definition;
        Location = location;
        Methods = methods;
        Attributes = attributes;
    }

    /// <summary>
    /// Creates a new <see cref="ClassSymbol"/> instance with the specified method added.
    /// </summary>
    /// <param name="method">The method to be added to the class symbol. Must not be null.</param>
    /// <returns>A new <see cref="ClassSymbol"/> instance with the given method included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="method"/> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a method with the same name as <paramref name="method"/> already exists in the class.
    /// </exception>
    public ClassSymbol WithMethod(MethodSymbol method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));
        if (Methods.ContainsKey(method.Name))
            throw new InvalidOperationException($"Method '{method.Name}' already defined in class '{Name}'.");

        return new ClassSymbol(Name, ParentName, Definition, Location,
            Methods.SetItem(method.Name, method), Attributes);
    }

    /// <summary>
    /// Returns a new instance of <see cref="ClassSymbol"/> with the specified attribute added.
    /// </summary>
    /// <param name="attribute">The attribute to add to the class symbol. Must not be null.</param>
    /// <returns>A new <see cref="ClassSymbol"/> instance with the added attribute.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="attribute"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when an attribute with the same name as the specified <paramref name="attribute"/>
    /// already exists in the class symbol.
    /// </exception>
    public ClassSymbol WithAttribute(AttributeSymbol attribute)
    {
        if (attribute == null) throw new ArgumentNullException(nameof(attribute));
        if (Attributes.ContainsKey(attribute.Name))
            throw new InvalidOperationException($"Attribute '{attribute.Name}' already defined in class '{Name}'.");

        return new ClassSymbol(Name, ParentName, Definition, Location,
            Methods, Attributes.SetItem(attribute.Name, attribute));
    }

    /// <summary>
    /// Determines whether the class contains a method with the specified name.
    /// </summary>
    /// <param name="name">The name of the method to check for.</param>
    /// <returns>True if the method exists in the class; otherwise, false.</returns>
    public bool HasMethod(string name) => Methods.ContainsKey(name);

    /// <summary>
    /// Determines whether the class contains an attribute with the specified name.
    /// </summary>
    /// <param name="name">The name of the attribute to check for.</param>
    /// <returns>True if the attribute exists; otherwise, false.</returns>
    public bool HasAttribute(string name) => Attributes.ContainsKey(name);

    /// <summary>
    /// Returns a string representation of the class symbol, including the class name,
    /// parent class (if any), and counts of methods and attributes.
    /// </summary>
    /// <returns>A string describing the class symbol.</returns>
    public override string ToString() =>
        $"class {Name}{(ParentName is null ? "" : $" inherits {ParentName}")} ({Methods.Count} methods, {Attributes.Count} attrs)";
}