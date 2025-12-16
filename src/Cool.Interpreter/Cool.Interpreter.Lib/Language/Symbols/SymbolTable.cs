//-----------------------------------------------------------------------
// <copyright file="SymbolTable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SymbolTable</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a table that stores and manages class symbols within a symbol table for semantic analysis.
/// </summary>
/// <remarks>
/// Provides functionality to add new class symbols and retrieve existing ones.
/// The symbol table ensures that each class is uniquely identified by its name.
/// </remarks>
/// <summary>
/// Immutable symbol table containing class definitions.
/// Thread-safe, pure, and designed for functional-style semantic analysis.
/// </summary>
public class SymbolTable
{
    /// <summary>
    /// The empty symbol table — contains only built-in classes (Object, IO, Int, String, Bool).
    /// </summary>
    public static readonly SymbolTable Empty = CreateWithBuiltins();

    /// <summary>
    /// All classes currently registered (including built-ins).
    /// </summary>
    public ImmutableDictionary<string, ClassSymbol> Classes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolTable"/> class.
    /// </summary>
    /// <param name="classes"></param>
    private SymbolTable(ImmutableDictionary<string, ClassSymbol> classes)
    {
        Classes = classes;
    }

    /// <summary>
    /// Creates a new symbol table with an additional class.
    /// Throws if the class already exists.
    /// </summary>
    public SymbolTable WithClass(ClassSymbol classSymbol)
    {
        ArgumentNullException.ThrowIfNull(classSymbol);

        return Classes.ContainsKey(classSymbol.Name) ? throw new InvalidOperationException($"Class '{classSymbol.Name}' is already defined.") : new SymbolTable(Classes.SetItem(classSymbol.Name, classSymbol));
    }

    /// <summary>
    /// Gets a class symbol by name. Returns null if not found.
    /// </summary>
    public ClassSymbol? TryGetClass(string name) =>
        Classes.TryGetValue(name, out var cls) ? cls : null;

    /// <summary>
    /// Gets a class symbol by name. Throws if not found.
    /// </summary>
    public ClassSymbol GetClass(string name) =>
        TryGetClass(name) ?? throw new KeyNotFoundException($"Class '{name}' not found in symbol table.");

    // --------------------------------------------------------------------
    // Private: Build the initial table with built-in classes
    // --------------------------------------------------------------------
    private static SymbolTable CreateWithBuiltins()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, ClassSymbol>(StringComparer.Ordinal);

        // Built-in classes — no AST definition available (and not needed)
        AddBuiltin(builder, "Object");
        AddBuiltin(builder, "IO", "Object");
        AddBuiltin(builder, "Int", "Object");
        AddBuiltin(builder, "String", "Object");
        AddBuiltin(builder, "Bool", "Object");

        return new SymbolTable(builder.ToImmutable());
    }

    /// <summary>
    /// Adds a built-in class to the given symbol table builder.
    /// </summary>
    /// <param name="builder">The builder for the immutable dictionary of classes.</param>
    /// <param name="name">The name of the built-in class to add.</param>
    /// <param name="parent">The name of the parent class for the built-in class, or null if there is no parent.</param>
    private static void AddBuiltin(
        ImmutableDictionary<string, ClassSymbol>.Builder builder,
        string name,
        string? parent = null)
    {
        // Use the parameterless constructor (which now delegates to the main one with Definition = null!)
        // This is safe for built-ins because RuntimeClassFactory will short-circuit them anyway.
        var symbol = new ClassSymbol(name, parent, SourcePosition.None);
        builder.Add(name, symbol);
    }
}