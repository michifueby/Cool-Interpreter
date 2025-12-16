//-----------------------------------------------------------------------
// <copyright file="InheritanceChecker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>InheritanceChecker</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis.Checker;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Language.Symbols;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// First semantic pass: Registers all classes and validates the inheritance graph.
/// Detects: duplicate classes, undefined parents, inheritance cycles, Main class issues.
/// </summary>
public class InheritanceChecker
{
    /// <summary>
    /// Represents the symbol table used to manage class symbols during semantic analysis.
    /// </summary>
    /// <remarks>
    /// The symbol table is utilized to store and retrieve class symbols, ensuring that
    /// each class is uniquely identified by its name. It plays a crucial role in operations
    /// such as validating inheritance hierarchies, checking for duplicate class definitions,
    /// and ensuring the existence of parent classes during semantic analysis.
    /// </remarks>
    private readonly SymbolTable _symbolTable;

    /// <summary>
    /// Collects and logs diagnostic messages generated during the semantic analysis process.
    /// </summary>
    /// <remarks>
    /// This variable is used to record errors, warnings, and informational messages
    /// encountered while performing tasks such as class registration and inheritance graph validation.
    /// It helps identify issues like duplicate class definitions, undefined parent classes,
    /// inheritance cycles, and other semantic violations.
    /// </remarks>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// Stores the set of all class names that have been defined during the semantic analysis.
    /// </summary>
    /// <remarks>
    /// This collection is used to track the classes encountered during the registration phase.
    /// It helps in detecting duplicate class definitions, ensuring the uniqueness of class names,
    /// and validating the existence of required classes, such as the "Main" class. This set also
    /// serves as a reference for verifying parent-child relationships in the inheritance graph
    /// during subsequent analysis phases.
    /// </remarks>
    private readonly HashSet<string> _definedClasses = new();

    /// <summary>
    /// Maps each class name to its direct parent class name within the inheritance graph.
    /// </summary>
    /// <remarks>
    /// This mapping is foundational for validating the inheritance hierarchy during semantic analysis.
    /// It is used to detect issues such as undefined parent classes, inheritance cycles, and to ensure
    /// proper class relationships. The key in the dictionary represents the class name, and the value
    /// is the name of the parent class, or null if the class does not explicitly inherit from another.
    /// </remarks>
    private readonly Dictionary<string, string?> _classParentMap = new();

    public InheritanceChecker(SymbolTable symbolTable, DiagnosticBag diagnostics)
    {
        _symbolTable = symbolTable;
        _diagnostics = diagnostics;
    }

    /// <summary>
    /// Registers all classes from the AST and builds the inheritance map.
    /// </summary>
    public void RegisterClasses(ProgramNode program)
    {
        foreach (var classNode in program.Classes)
        {
            var className = classNode.Name;
            var parentName = classNode.InheritsFrom;
            
            // 1. Detect duplicate class definition
            if (_definedClasses.Contains(className))
            {
                _diagnostics.ReportError(
                    classNode.Location,
                    "COOL0101",
                    $"Class '{className}' is already defined.");
                continue;
            }

            // 2. Disallow redefining basic classes
            if (IsBasicClass(className))
            {
                _diagnostics.ReportError(
                    classNode.Location,
                    "COOL0102",
                    $"Cannot redefine built-in class '{className}'.");
                continue;
            }

            // 3. Validate parent exists (later)
            

            // Register
            _definedClasses.Add(className);
            _classParentMap[className] = parentName;

            var classSymbol = new ClassSymbol(className, parentName);
            _symbolTable.AddClass(classSymbol);
        }

        // 4. Ensure Main class exists
        if (!_definedClasses.Contains("Main"))
        {
            _diagnostics.ReportError(
                SourcePosition.None,
                "COOL0103",
                "Program must contain a class 'Main'.");
        }
    }

    /// <summary>
    /// Validates the full inheritance graph: undefined parents and cycles.
    /// </summary>
    public void CheckInheritanceGraph()
    {
        // 1. Check for undefined parents
        foreach (var (className, parentName) in _classParentMap)
        {
            if (parentName is null)
                continue;

            if (!_definedClasses.Contains(parentName) && !IsBasicClass(parentName))
            {
                _diagnostics.ReportError(
                    SourcePosition.None,
                    "COOL0104",
                    $"Class '{className}' inherits from undefined class '{parentName}'.");
            }
        }
        // 2. Check for inheritance cycles
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();
        foreach (var className in _definedClasses.Where(className => DetectCycle(className, visited, recursionStack)))
        {
            _diagnostics.ReportError(
                SourcePosition.None,
                "COOL0105",
                $"Inheritance cycle detected involving class '{className}'.");
        }
    }

    private bool DetectCycle(string className, HashSet<string> visited, HashSet<string> recursionStack)
    {
        if (recursionStack.Contains(className))
            return true;

        if (!visited.Add(className))
            return false;

        recursionStack.Add(className);

        if (_classParentMap.TryGetValue(className, out var parentName) && parentName is not null)
        {
            if (DetectCycle(parentName, visited, recursionStack))
                return true;
        }

        recursionStack.Remove(className);
        return false;
    }

    private static bool IsBasicClass(string name) =>
        name is "Object" or "IO" or "Int" or "String" or "Bool";
}