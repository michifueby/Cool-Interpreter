//-----------------------------------------------------------------------
// <copyright file="CoolParserEngine.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolParserEngine</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Parsing;

/// <summary>
/// Orchestrates the ANTLR-generated lexer and parser to produce a clean AST.
/// This is the only class that knows about ANTLR.
/// </summary>
public class CoolParserEngine
{
    public ParseResult Parse(string sourceCode, string? sourceName = null)
    {
        throw new NotImplementedException();
    }
}