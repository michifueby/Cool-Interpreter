//-----------------------------------------------------------------------
// <copyright file="SymbolTable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SymbolTable</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;

namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents a table that stores and manages class symbols within a symbol table for semantic analysis.
/// </summary>
/// <remarks>
/// Provides functionality to add new class symbols and retrieve existing ones.
/// The symbol table ensures that each class is uniquely identified by its name.
/// </remarks>
public class SymbolTable
{
    public static readonly SymbolTable Empty = new();

    public ImmutableDictionary<string, ClassSymbol> Classes { get; }

    public SymbolTable()
    {
        Classes = ImmutableDictionary.Create<string, ClassSymbol>(StringComparer.Ordinal);
    }

    private SymbolTable(ImmutableDictionary<string, ClassSymbol> classes)
    {
        Classes = classes;
    }

    public SymbolTable WithClass(ClassSymbol classSymbol)
    {
        if (classSymbol == null) throw new ArgumentNullException(nameof(classSymbol));
        return new SymbolTable(Classes.SetItem(classSymbol.Name, classSymbol));
    }

    public ClassSymbol? TryGetClass(string name) =>
        Classes.TryGetValue(name, out var cls) ? cls : null;

    public ClassSymbol GetClass(string name) =>
        TryGetClass(name) ?? throw new KeyNotFoundException($"Class '{name}' not found in symbol table.");
}