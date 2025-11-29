//-----------------------------------------------------------------------
// <copyright file="ISemanticAnalyzer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ISemanticAnalyzer</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Defines a contract for performing semantic analysis on a Cool program's abstract syntax tree (AST).
/// </summary>
public interface ISemanticAnalyzer
{
    /// <summary>
    /// Analyzes a parsed Cool program and returns semantic information and diagnostics.
    /// </summary>
    /// <param name="syntaxTree">The root of the parsed AST. Must not be null.</param>
    /// <param name="diagnostics">Bag to collect semantic errors/warnings.</param>
    /// <returns>A <see cref="SemanticResult"/> containing symbol table and analysis success state.</returns>
    SemanticResult Analyze(ProgramNode syntaxTree, DiagnosticBag diagnostics);
}