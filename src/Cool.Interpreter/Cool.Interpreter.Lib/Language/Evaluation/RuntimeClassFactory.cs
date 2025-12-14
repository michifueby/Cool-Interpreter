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
        // Resolve parent recursively (null for Object, otherwise recurse)
        CoolClass? parent = symbol.ParentName switch
        {
            null or "Object" =>  null,
            var p => FromSymbol(env.SymbolTable.GetClass(p)!, env)
        };

        if (parent is null)
            return null;

        // Extract MethodNodes from the AST stored in ClassSymbol.Definition
        var methods = symbol.Definition.Features
            .OfType<MethodNode>()
            .ToImmutableDictionary(
                method => method.Name,           // key
                method => method);               // value (MethodNode itself)

        // Extract AttributeNodes
        var attributes = symbol.Definition.Features
            .OfType<AttributeNode>()
            .ToImmutableDictionary(
                attr => attr.Name,
                attr => attr);

        return new CoolClass(
            name: symbol.Name,
            parent: parent,
            methods: methods,
            attributes: attributes);
    }
}