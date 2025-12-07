//-----------------------------------------------------------------------
// <copyright file="FormalSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>FormalSymbol</summary>
//-----------------------------------------------------------------------

using Cool.Interpreter.Lib.Core.Syntax;

namespace Cool.Interpreter.Lib.Language.Symbols;

public class FormalSymbol
{
    public string Name { get; }
    public string Type { get; }
    public SourcePosition Location { get; }

    public FormalSymbol(string name, string type, SourcePosition location = default)
    {
        Name     = name;
        Type     = type;
        Location = location == default ? SourcePosition.None : location;
    }
}