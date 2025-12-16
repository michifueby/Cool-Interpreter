//-----------------------------------------------------------------------
// <copyright file="RuntimeClassFactory.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>RuntimeClassFactory</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using System.Collections.Immutable;
using System.Linq;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Provides functionality to convert a <c>ClassSymbol</c> (semantic phase representation)
/// into a <c>CoolClass</c> (runtime phase representation).
/// This is used during runtime evaluation of the Cool programming language to create
/// runtime class objects from their semantic descriptions.
/// </summary>
public static class RuntimeClassFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="CoolClass"/> from the provided <see cref="ClassSymbol"/> and runtime environment.
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

        // Otherwise, it's a user-defined class — build it from the symbol

        // Resolve parent recursively
        CoolClass? parent = symbol.ParentName switch
        {
            null or "Object" => PredefinedClasses.Object,  // Main inherits from Object implicitly
            var p => FromSymbol(env.SymbolTable.GetClass(p)!, env)
        };

        // Extract methods and attributes from the AST definition
        var methods = symbol.Definition.Features
            .OfType<MethodNode>()
            .ToImmutableDictionary(m => m.Name, m => m);

        var attributes = symbol.Definition.Features
            .OfType<AttributeNode>()
            .ToImmutableDictionary(a => a.Name, a => a);

        return new CoolClass(
            name: symbol.Name,
            parent: parent,
            methods: methods,
            attributes: attributes);
    }
}