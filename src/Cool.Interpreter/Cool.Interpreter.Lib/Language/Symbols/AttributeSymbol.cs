//-----------------------------------------------------------------------
// <copyright file="AttributeSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>AttributeSymbol</summary>
//-----------------------------------------------------------------------

using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;

namespace Cool.Interpreter.Lib.Language.Symbols;

public sealed class AttributeSymbol
{
    public string Name { get; }
    public string Type { get; }
    public ExpressionNode? Initializer { get; }  // Important for later!
    public SourcePosition Location { get; }

    public AttributeSymbol(string name, string type, ExpressionNode? initializer = null, SourcePosition location = default)
    {
        Name        = name;
        Type        = type;
        Initializer = initializer;
        Location    = location == default ? SourcePosition.None : location;
    }
}