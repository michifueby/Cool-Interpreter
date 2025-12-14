//-----------------------------------------------------------------------
// <copyright file="SymbolTable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SymbolTable</summary>
//-----------------------------------------------------------------------

using Cool.Interpreter.Lib.Core.Syntax;

namespace Cool.Interpreter.Lib.Language.Symbols;

using System.Collections.Immutable;

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
public sealed class SymbolTable
{
    /// <summary>
    /// The empty symbol table — contains only built-in classes (Object, IO, Int, String, Bool).
    /// </summary>
    public static readonly SymbolTable Empty = CreateWithBuiltins();

    /// <summary>
    /// All classes currently registered (including built-ins).
    /// </summary>
    public ImmutableDictionary<string, ClassSymbol> Classes { get; }

    private SymbolTable(ImmutableDictionary<string, ClassSymbol> classes)
    {
        Classes = classes;
    }

    /// <summary>
    /// Creates a new symbol table with an additional class.
    /// Throws if the class already exists (use WithClassOrUpdate for overrides).
    /// </summary>
    public SymbolTable WithClass(ClassSymbol classSymbol)
    {
        if (classSymbol is null) throw new ArgumentNullException(nameof(classSymbol));
        if (Classes.ContainsKey(classSymbol.Name))
            throw new InvalidOperationException($"Class '{classSymbol.Name}' is already defined.");

        return new SymbolTable(
            Classes.SetItem(classSymbol.Name, classSymbol));
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
        TryGetClass(name)
        ?? throw new KeyNotFoundException($"Class '{name}' not found in symbol table.");

    /// <summary>
    /// Checks if a class exists in the table.
    /// </summary>
    public bool HasClass(string name) => Classes.ContainsKey(name);

    // --------------------------------------------------------------------
    // Private: Build the initial table with built-in classes
    // --------------------------------------------------------------------
    private static SymbolTable CreateWithBuiltins()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, ClassSymbol>(StringComparer.Ordinal);

        // Built-in classes — these are part of every Cool program
        AddBuiltin(builder, "Object");
        AddBuiltin(builder, "IO", "Object");
        AddBuiltin(builder, "Int", "Object");
        AddBuiltin(builder, "String", "Object");
        AddBuiltin(builder, "Bool", "Object");

        return new SymbolTable(builder.ToImmutable());
    }

    private static void AddBuiltin(ImmutableDictionary<string, ClassSymbol>.Builder builder, string name, string? parent = null)
    {
        var symbol = new ClassSymbol(name, parent, SourcePosition.None);
        builder.Add(name, symbol);
    }
}