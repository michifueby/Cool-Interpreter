//-----------------------------------------------------------------------
// <copyright file="ClassSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ClassSymbol</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

namespace Cool.Interpreter.Lib.Language.Symbols;



public sealed class ClassSymbol
{
    public string Name { get; }
    public string? ParentName { get; }
    
    public ClassNode Definition { get; }
    
    /// <summary>
    /// Source location of the class definition (used for error reporting).
    /// </summary>
    public SourcePosition Location { get; }

    public ImmutableDictionary<string, MethodSymbol> Methods { get; }
    public ImmutableDictionary<string, AttributeSymbol> Attributes { get; }

    // ──────────────────────────────────────────────────────────────────────
    // Public constructors
    // ──────────────────────────────────────────────────────────────────────
    public ClassSymbol(string name, string? parentName = null, SourcePosition location = default)
    {
        Name       = name       ?? throw new ArgumentNullException(nameof(name));
        ParentName = parentName;
        Location   = location == default ? SourcePosition.None : location;

        Methods    = ImmutableDictionary.Create<string, MethodSymbol>(StringComparer.Ordinal);
        Attributes = ImmutableDictionary.Create<string, AttributeSymbol>(StringComparer.Ordinal);
    }

    // Private constructor for WithX() methods
    private ClassSymbol(
        string name,
        string? parentName,
        SourcePosition location,
        ImmutableDictionary<string, MethodSymbol> methods,
        ImmutableDictionary<string, AttributeSymbol> attributes)
    {
        Name       = name;
        ParentName = parentName;
        Location   = location;
        Methods    = methods;
        Attributes = attributes;
    }

    // ──────────────────────────────────────────────────────────────────────
    // Immutable update methods (fluent API)
    // ──────────────────────────────────────────────────────────────────────
    public ClassSymbol WithMethod(MethodSymbol method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));
        if (Methods.ContainsKey(method.Name))
            throw new InvalidOperationException($"Method '{method.Name}' is already defined in class '{Name}'.");

        return new ClassSymbol(
            Name, ParentName, Location,
            Methods.SetItem(method.Name, method),
            Attributes);
    }

    public ClassSymbol WithAttribute(AttributeSymbol attribute)
    {
        if (attribute == null) throw new ArgumentNullException(nameof(attribute));
        if (Attributes.ContainsKey(attribute.Name))
            throw new InvalidOperationException($"Attribute '{attribute.Name}' is already defined in class '{Name}'.");

        return new ClassSymbol(
            Name, ParentName, Location,
            Methods,
            Attributes.SetItem(attribute.Name, attribute));
    }

    // ──────────────────────────────────────────────────────────────────────
    // Convenience helpers
    // ──────────────────────────────────────────────────────────────────────
    public bool HasMethod(string name) => Methods.ContainsKey(name);
    public bool HasAttribute(string name) => Attributes.ContainsKey(name);

    public override string ToString() => 
        $"class {Name}{(ParentName is null ? "" : $" inherits {ParentName}")} ({Methods.Count} methods, {Attributes.Count} attrs)";
}