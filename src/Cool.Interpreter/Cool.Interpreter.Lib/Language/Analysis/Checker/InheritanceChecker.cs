//-----------------------------------------------------------------------
// <copyright file="InheritanceChecker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>InheritanceChecker</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis.Checker;

using System.Collections.Immutable;
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
    /// Represents the table used to store and manage symbols for classes in the semantic analysis phase.
    /// This symbol table is used to track information about classes, such as their definitions and
    /// inheritance relationships, during the first semantic pass of the program analysis. It enables
    /// validation of class definitions, detection of duplicate class names, and resolution of inheritance
    /// relationships. It also facilitates error reporting for issues such as undefined parent classes
    /// and inheritance cycles.
    /// </summary>
    private readonly SymbolTable _symbolTable;

    /// <summary>
    /// Stores and manages diagnostic messages, including errors, warnings, and informational messages,
    /// encountered during semantic analysis. This diagnostic bag is used to report issues such as
    /// duplicate class declarations, invalid inheritance relationships, or missing required classes (e.g., Main).
    /// It ensures that all detected problems are collected in a consistent manner for later retrieval
    /// and presentation to the user.
    /// </summary>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// Represents the collection of all class names defined in the program during the first semantic pass.
    /// This set is used to track and validate user-defined classes, ensuring they are uniquely named,
    /// not redefined, and correctly referenced in the inheritance hierarchy.
    /// It aids in detecting errors such as duplicate class definitions, missing Main class, and invalid
    /// inheritance relationships.
    /// </summary>
    private readonly HashSet<string> _definedClasses = new(StringComparer.Ordinal);

    /// <summary>
    /// Represents a mapping between class names and their parent class names in the inheritance graph.
    /// Each entry in the map associates a class with its parent class, if defined. If a class does not
    /// explicitly inherit from another class, its parent value may be null. This structure is used to
    /// validate inheritance relationships, detect undefined parent classes, ensure proper hierarchy,
    /// and identify inheritance cycles during the semantic analysis phase of a COOL program.
    /// </summary>
    private readonly Dictionary<string, string?> _classParentMap = new(StringComparer.Ordinal);

    /// <summary>
    /// Represents the set of built-in classes that are inherently available in the language.
    /// These classes are considered fundamental and include "Object", "IO", "Int", "String",
    /// and "Bool". This set is used during semantic analysis to enforce rules such as
    /// preventing redefinition of built-in classes, disallowing inheritance from primitive
    /// types except for specific cases, and validating the inheritance hierarchy.
    /// </summary>
    private static readonly ImmutableHashSet<string> BasicClasses =
        ImmutableHashSet.Create(StringComparer.Ordinal, "Object", "IO", "Int", "String", "Bool");

    /// <summary>
    /// Performs checking and validation of inheritance relationships in the
    /// provided abstract syntax tree. The class is responsible for building
    /// an inheritance graph and detecting potential issues such as inheritance
    /// cycles or invalid inheritance chains.
    /// </summary>
    public InheritanceChecker(SymbolTable symbolTable, DiagnosticBag diagnostics)
    {
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    /// <summary>
    /// Registers all classes defined in the program and validates their inheritance relationships.
    /// It checks for duplicate class definitions, redefinitions of built-in classes, invalid
    /// inheritance from primitive types, and ensures the presence of a "Main" class.
    /// </summary>
    /// <param name="program">The root node of the program's abstract syntax tree, containing
    /// all class definitions to be registered and validated.</param>
    public void RegisterClasses(ProgramNode program)
    {
        foreach (var classNode in program.Classes)
        {
            var className = classNode.Name;
            var parentName = classNode.InheritsFrom;
            var location = classNode.Location;

            if (_definedClasses.Contains(className))
            {
                _diagnostics.ReportError(location, CoolErrorCodes.DuplicateClass, $"Class '{className}' is already defined.");
                continue;
            }

            if (BasicClasses.Contains(className))
            {
                _diagnostics.ReportError(location, CoolErrorCodes.RedefineBuiltin, $"Cannot redefine built-in class '{className}'.");
                continue;
            }

            if (parentName is not null && BasicClasses.Contains(parentName) && parentName != "Object" && parentName != "IO")
            {
                _diagnostics.ReportError(location, CoolErrorCodes.InheritFromPrimitive, $"Class '{className}' cannot inherit from primitive type '{parentName}'.");
                continue;
            }

            _definedClasses.Add(className);
            _classParentMap[className] = parentName;

            // Save the ClassNode so we can get location later
            var classSymbol = new ClassSymbol(className, parentName);
            _symbolTable.WithClass(classSymbol);
        }

        if (!_definedClasses.Contains("Main"))
        {
            _diagnostics.ReportError(SourcePosition.None, CoolErrorCodes.MissingMain, "Program must contain a class named 'Main'.");
        }
    }

    /// <summary>
    /// Validates the inheritance relationships in the program by ensuring that all parent classes
    /// exist and detecting inheritance cycles. This method is a critical part of semantic analysis
    /// and helps to identify structural issues in the inheritance hierarchy before further
    /// processing occurs.
    /// </summary>
    public void CheckInheritanceGraph()
    {
        this.EnsureAllParentsExist();
        this.DetectInheritanceCycles();
    }

    /// <summary>
    /// Ensures that all parent classes referenced in the inheritance graph exist.
    /// Validates that each class's parent is either a defined user class or a built-in class.
    /// If an undefined parent is found, reports an error using the diagnostic system.
    /// </summary>
    private void EnsureAllParentsExist()
    {
        foreach (var className in _definedClasses)
        {
            if (_classParentMap.TryGetValue(className, out var parentName) && parentName is not null)
            {
                bool parentExists = _definedClasses.Contains(parentName) || BasicClasses.Contains(parentName);
                if (!parentExists)
                {
                    var location = _symbolTable.GetClass(className)?.Definition?.Location ?? SourcePosition.None;
                    _diagnostics.ReportError(location, CoolErrorCodes.UndefinedParent,
                        $"Class '{className}' inherits from undefined class '{parentName}'.");
                }
            }
        }
    }

    /// <summary>
    /// Detects cycles in the inheritance graph of the defined classes.
    /// This method ensures there are no circular inheritance relationships
    /// by traversing the inheritance hierarchy and identifying cycles.
    /// If a cycle is found, it records the error in the diagnostics bag and stops further processing.
    /// </summary>
    private void DetectInheritanceCycles()
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var recursionStack = new HashSet<string>(StringComparer.Ordinal);

        foreach (var className in _definedClasses)
        {
            if (!visited.Contains(className))
            {
                if (TryDetectCycle(className, visited, recursionStack))
                    return; // Stop after first cycle
            }
        }
    }

    /// <summary>
    /// Attempts to detect an inheritance cycle starting from the specified class in the inheritance graph.
    /// This method performs a depth-first traversal of the graph, checking for cycles by maintaining
    /// a recursion stack to track the current traversal path.
    /// </summary>
    /// <param name="current">The name of the class being analyzed for potential cycles.</param>
    /// <param name="visited">A set of classes that have already been visited during the traversal.</param>
    /// <param name="stack">A stack used to track the current path of the recursion and detect cycles.</param>
    /// <returns>
    /// Returns true if an inheritance cycle is detected; otherwise, returns false.
    /// </returns>
    private bool TryDetectCycle(string current, HashSet<string> visited, HashSet<string> stack)
    {
        visited.Add(current);
        stack.Add(current);

        if (_classParentMap.TryGetValue(current, out var parent) && parent is not null)
        {
            if (stack.Contains(parent))
            {
                var cycle = new List<string>();
                var node = current;
                do
                {
                    cycle.Add(node);
                    node = _classParentMap[node]!;
                } while (node != parent);
                cycle.Add(parent);
                cycle.Reverse();

                var path = string.Join(" to ", cycle);
                var location = _symbolTable.GetClass(current)?.Definition?.Location ?? SourcePosition.None;

                _diagnostics.ReportError(location, CoolErrorCodes.InheritanceCycle, $"Inheritance cycle detected: {path}");
                return true;
            }

            if (!visited.Contains(parent))
            {
                if (TryDetectCycle(parent, visited, stack))
                    return true;
            }
        }

        stack.Remove(current);
        return false;
    }
}