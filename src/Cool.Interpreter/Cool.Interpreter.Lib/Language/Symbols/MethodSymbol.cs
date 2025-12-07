//-----------------------------------------------------------------------
// <copyright file="MethodSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>MethodSymbol</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax;

namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents a method symbol in the Cool language.
/// </summary>
/// <remarks>
/// This class encapsulates the name, return type, and formal parameters of a method.
/// It is used to define and store metadata about methods within a class.
/// </remarks>
public sealed class MethodSymbol
{
    public string Name { get; }
    public string ReturnType { get; }
    public ImmutableList<FormalSymbol> Formals { get; }
    public SourcePosition Location { get; }

    public MethodSymbol(string name, string returnType, SourcePosition location = default)
    {
        Name       = name;
        ReturnType = returnType;
        Formals    = ImmutableList<FormalSymbol>.Empty;
        Location   = location == default ? SourcePosition.None : location;
    }

    private MethodSymbol(
        string name,
        string returnType,
        ImmutableList<FormalSymbol> formals,
        SourcePosition location)
    {
        Name       = name;
        ReturnType = returnType;
        Formals    = formals;
        Location   = location;
    }

    public MethodSymbol AddFormal(string name, string type) =>
        new(Name, ReturnType, Formals.Add(new FormalSymbol(name, type)), Location);
}