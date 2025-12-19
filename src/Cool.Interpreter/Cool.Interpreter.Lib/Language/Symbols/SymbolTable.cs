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

        // Built-in classes with their methods — no AST definition available (and not needed)
        
        // Object: abort(), type_name(), copy()
        var objectClass = new ClassSymbol("Object", null, SourcePosition.None)
            .WithMethod(MethodSymbol.CreateBuiltin("abort", "Object"))
            .WithMethod(MethodSymbol.CreateBuiltin("type_name", "String"))
            .WithMethod(MethodSymbol.CreateBuiltin("copy", "SELF_TYPE"));
        builder.Add("Object", objectClass);

        // IO: out_string(x), out_int(x), in_string(), in_int()
        var ioClass = new ClassSymbol("IO", "Object", SourcePosition.None)
            .WithMethod(MethodSymbol.CreateBuiltin("out_string", "SELF_TYPE", ("x", "String")))
            .WithMethod(MethodSymbol.CreateBuiltin("out_int", "SELF_TYPE", ("x", "Int")))
            .WithMethod(MethodSymbol.CreateBuiltin("in_string", "String"))
            .WithMethod(MethodSymbol.CreateBuiltin("in_int", "Int"));
        builder.Add("IO", ioClass);

        // Int: no additional methods beyond Object
        var intClass = new ClassSymbol("Int", "Object", SourcePosition.None);
        builder.Add("Int", intClass);

        // String: length(), concat(s), substr(i, l)
        var stringClass = new ClassSymbol("String", "Object", SourcePosition.None)
            .WithMethod(MethodSymbol.CreateBuiltin("length", "Int"))
            .WithMethod(MethodSymbol.CreateBuiltin("concat", "String", ("s", "String")))
            .WithMethod(MethodSymbol.CreateBuiltin("substr", "String", ("i", "Int"), ("l", "Int")));
        builder.Add("String", stringClass);

        // Bool: no additional methods beyond Object
        var boolClass = new ClassSymbol("Bool", "Object", SourcePosition.None);
        builder.Add("Bool", boolClass);

        return new SymbolTable(builder.ToImmutable());
    }
}