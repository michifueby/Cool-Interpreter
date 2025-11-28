//-----------------------------------------------------------------------
// <copyright file="CoolCodeAnalyzer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolCodeAnalyzer</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.CodeAnalysis;

/// <summary>
/// Represents the complete result of parsing and semantic analysis of a Cool program.
/// This is the primary return type used by <see cref="CoolCodeAnalyzer"/>. 
/// </summary>
public class CoolCodeAnalyzer
{
    public AnalysisResult Analyze(string sourceCode, string? sourceName = null)
    {
        throw new NotImplementedException();
    }
}