//-----------------------------------------------------------------------
// <copyright file="CoolSemanticAnalyzer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolSemanticAnalyzer</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Language.Analysis.Checker;
using Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Performs full semantic analysis on a Cool program according to the official specification.
/// </summary>
/// <remarks>
/// Responsibilities:
/// • Build inheritance graph and detect cycles
/// • Populate symbol table (classes, attributes, methods)
/// • Type checking and SELF_TYPE resolution
/// • Report semantic errors (redefined classes, override errors, etc.)
/// </remarks>
public class CoolSemanticAnalyzer : ISemanticAnalyzer
{
    /// <summary>
    /// Analyzes the given syntax tree for semantic correctness, performs class registration, inheritance checking,
    /// and type checking. Returns a semantic analysis result containing diagnostics and generated symbol information.
    /// </summary>
    /// <param name="syntaxTree">The root node of the abstract syntax tree representing the program.</param>
    /// <param name="diagnostics">A diagnostic bag used to collect errors and warnings encountered during analysis.</param>
    /// <returns>A <see cref="SemanticResult"/> object containing the analysis results including diagnostics and symbols.</returns>
    public SemanticResult Analyze(ProgramNode syntaxTree, DiagnosticBag diagnostics)
    {
        ArgumentNullException.ThrowIfNull(syntaxTree);
        ArgumentNullException.ThrowIfNull(diagnostics);

        // Start with the built-in classes already registered
        var symbolTable = SymbolTable.Empty;

        // Phase 1: Register user-defined classes + basic validation
        var inheritanceChecker = new InheritanceChecker(symbolTable, diagnostics);
        inheritanceChecker.RegisterClasses(syntaxTree);

        // Early exit if basic errors (duplicate class, redefine builtin, etc.)
        if (diagnostics.HasErrors)
        {
            return SemanticResult.Failure(diagnostics.Diagnostics);
        }

        // Phase2: Build full inheritance graph and detect cycles
        inheritanceChecker.CheckInheritanceGraph();       // reports COOL0106 if cycle found

        if (diagnostics.HasErrors)
        {
            return SemanticResult.Failure(diagnostics.Diagnostics);
        }

        // Phase3: Full type checking (attributes, methods, expressions)
        var typeChecker = new TypeChecker(symbolTable, diagnostics);
        typeChecker.Check(syntaxTree);

        // Final result
        return diagnostics.HasErrors
            ? SemanticResult.Failure(diagnostics.Diagnostics)
            : SemanticResult.Success(symbolTable, diagnostics.Diagnostics);
    }
}