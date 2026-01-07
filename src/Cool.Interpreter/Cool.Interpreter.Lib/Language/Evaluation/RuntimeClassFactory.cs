//-----------------------------------------------------------------------
// <copyright file="RuntimeClassFactory.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>RuntimeClassFactory</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Core.Syntax.Ast.Features;
using Classes;
using Symbols;

/// <summary>
/// Provides functionality to convert a <c>ClassSymbol</c> (semantic phase representation)
/// into a <c>CoolClass</c> (runtime phase representation).
/// This is used during runtime evaluation of the Cool programming language to create
/// runtime class objects from their semantic descriptions.
/// </summary>
public static class RuntimeClassFactory
{
    /// <summary>
    /// Cache for user-defined runtime classes to avoid repeated creation.
    /// Key is the class name, value is the constructed CoolClass.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CoolClass> _classCache = new();

    /// <summary>
    /// Clears the runtime class cache. Should be called between interpreter runs
    /// to ensure clean state.
    /// </summary>
    public static void ClearCache() => _classCache.Clear();

    /// <summary>
    /// Creates a new instance of <see cref="CoolClass"/> from the provided <see cref="ClassSymbol"/> and runtime environment.
    /// Uses caching to avoid repeated creation of the same runtime class.
    /// </summary>
    /// <param name="symbol">The <see cref="ClassSymbol"/> representing the class to be instantiated.</param>
    /// <param name="env">The <see cref="CoolRuntimeEnvironment"/> providing the runtime context.</param>
    /// <returns>A fully constructed <see cref="CoolClass"/> with its parents, methods, and attributes resolved.</returns>
    public static CoolClass FromSymbol(ClassSymbol symbol, CoolRuntimeEnvironment env)
    {
        // Fast path: if it's a predefined built-in class, return the shared instance
        if (PredefinedClasses.ByName.TryGetValue(symbol.Name, out var predefinedClass))
        {
            return predefinedClass;
        }

        // Check cache first to avoid repeated class construction
        if (_classCache.TryGetValue(symbol.Name, out var cachedClass))
        {
            return cachedClass;
        }

        // Otherwise, it's a user-defined class — build it from the symbol

        // Resolve parent recursively
        var parent = symbol.ParentName switch
        {
            null or "Object" => PredefinedClasses.Object,  // Main inherits from Object implicitly
            var p => FromSymbol(env.SymbolTable.GetClass(p)!, env)
        };

        // Extract methods and attributes from the AST definition
        var methods = symbol.Definition?.Features
            .OfType<MethodNode>()
            .ToImmutableDictionary(m => m.Name, m => m);

        var attributes = symbol.Definition?.Features
            .OfType<AttributeNode>()
            .ToImmutableDictionary(a => a.Name, a => a);

        if (methods == null || attributes == null)
        {
            throw new Core.Exceptions.CoolRuntimeException(
                $"Class '{symbol.Name}' is missing method or attribute definitions.",
                Core.Syntax.SourcePosition.None);
        }
        var newClass = new CoolClass(
            name: symbol.Name,
            parent: parent,
            methods: methods,
            attributes: attributes);

        // Cache and return the new class
        _classCache.TryAdd(symbol.Name, newClass);
        return newClass;
    }
}