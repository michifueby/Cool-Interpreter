//-----------------------------------------------------------------------
// <copyright file="SymbolTable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SymbolTable</summary>
//-----------------------------------------------------------------------

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
    /// <summary>
    /// A dictionary that serves as the internal storage for class symbols within the symbol table.
    /// </summary>
    /// <remarks>
    /// Used to store instances of <see cref="ClassSymbol"/> where the key is the class name and
    /// the value is the corresponding <see cref="ClassSymbol"/> object. This collection allows
    /// efficient retrieval and management of class symbols during semantic analysis.
    /// </remarks>
    private readonly Dictionary<string, ClassSymbol> _classes = new();

    /// <summary>
    /// An immutable collection of class symbols managed by the symbol table.
    /// </summary>
    /// <remarks>
    /// Provides read-only access to the internal dictionary of class symbols. The keys represent
    /// class names, and the values are the corresponding <see cref="ClassSymbol"/> instances.
    /// This property is used to inspect and retrieve information about the classes stored
    /// within the symbol table, ensuring data integrity by preventing modification.
    /// </remarks>
    public IReadOnlyDictionary<string, ClassSymbol> Classes => _classes;

    /// <summary>
    /// Adds a new class symbol to the symbol table. If the class already exists in the table,
    /// it will be replaced with the provided class symbol.
    /// </summary>
    /// <param name="classSymbol">The class symbol to be added to the symbol table. This defines the class structure
    /// and its relationships with other classes.</param>
    public void AddClass(ClassSymbol classSymbol)
        => _classes[classSymbol.Name] = classSymbol;

    /// <summary>
    /// Retrieves a class symbol from the symbol table by its name.
    /// </summary>
    /// <param name="name">The name of the class to retrieve.</param>
    /// <returns>
    /// The class symbol associated with the specified name if it exists in the symbol table;
    /// otherwise, null if no class symbol with the given name is found.
    /// </returns>
    public ClassSymbol? GetClass(string name)
        => _classes.TryGetValue(name, out var cls) ? cls : null;
}