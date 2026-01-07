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
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Language.Symbols;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// First semantic pass: Registers all classes and validates the inheritance graph.
/// Detects: duplicate classes, undefined parents, inheritance cycles, Main class issues.
/// </summary>
public class InheritanceChecker
{
    /// <summary>
    /// Represents the symbol table used during the semantic analysis phase of the compiler.
    /// It holds information about all registered classes, including built-in and user-defined ones,
    /// for resolving class symbols, managing inheritance relationships, and detecting semantic errors.
    /// This symbol table is a central component for storing and retrieving class symbols throughout
    /// the compilation process, ensuring that all classes are correctly defined, their parent relationships
    /// are valid, and potential inheritance cycles are identified and reported.
    /// </summary>
    /// <summary>
    /// Represents the symbol table used during the semantic analysis phase of the compiler.
    /// It holds information about all registered classes, including built-in and user-defined ones.
    /// </summary>
    private SymbolTable _symbolTable;

    /// <summary>
    /// A collection of diagnostics used to record errors, warnings, and informational messages
    /// during the semantic analysis phase of the compiler.
    /// </summary>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// A collection that stores the set of all classes defined within a Cool program during the first
    /// semantic analysis pass. This set is used to detect duplicate class definitions, ensure that
    /// all parent classes are defined, and validate the inheritance graph.
    /// </summary>
    private readonly HashSet<string> _definedClasses = new(StringComparer.Ordinal);

    /// <summary>
    /// Maintains a mapping of class names to their respective parent class names
    /// within the inheritance graph of the language.
    /// </summary>
    private readonly Dictionary<string, string?> _classParentMap = new(StringComparer.Ordinal);

    /// <summary>
    /// Maintains a mapping of class names to their definition order (index in source file).
    /// Used to detect forward references in inheritance.
    /// </summary>
    private readonly Dictionary<string, int> _classDefinitionOrder = new(StringComparer.Ordinal);

    /// <summary>
    /// Represents the set of built-in classes that are predefined and essential for the
    /// language's runtime and type system.
    /// </summary>
    private static readonly ImmutableHashSet<string> BasicClasses =
        ImmutableHashSet.Create(StringComparer.Ordinal, "Object", "IO", "Int", "String", "Bool");

    public InheritanceChecker(SymbolTable symbolTable, DiagnosticBag diagnostics)
    {
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    /// <summary>
    /// Registers all classes defined in the given program...
    /// </summary>
    public SymbolTable RegisterClasses(ProgramNode program)
    {
        var currentTable = _symbolTable;  // starts with built-ins only
        int classIndex = 0;

        foreach (var classNode in program.Classes)
        {
            var className   = classNode.Name;
            // In Cool, all classes implicitly inherit from Object if no parent is specified
            var parentName  = classNode.InheritsFrom ?? "Object";
            var location    = classNode.Location;

            // Duplicate class?
            if (_definedClasses.Contains(className))
            {
                _diagnostics.ReportError(location, CoolErrorCodes.DuplicateClass,
                    $"Class '{className}' is already defined.");
                continue;
            }

            // Redefining built-in?
            if (BasicClasses.Contains(className))
            {
                _diagnostics.ReportError(location, CoolErrorCodes.RedefineBuiltin,
                    $"Cannot redefine built-in class '{className}'.");
                continue;
            }

            // Inheriting from primitive (Int/String/Bool)?
            if (parentName is not null &&
                BasicClasses.Contains(parentName) &&
                parentName != "Object" && parentName != "IO")
            {
                _diagnostics.ReportError(location, CoolErrorCodes.InheritFromPrimitive,
                    $"Class '{className}' cannot inherit from primitive type '{parentName}'.");
                continue;
            }

            // All checks passed — register the class
            _definedClasses.Add(className);
            _classParentMap[className] = parentName;
            _classDefinitionOrder[className] = classIndex;

            var classSymbol = new ClassSymbol(
                name: className,
                parentName: parentName,
                definition: classNode,
                location: location);

            // Populate methods and attributes from class features
            foreach (var feature in classNode.Features)
            {
                switch (feature)
                {
                    case MethodNode method:
                        if (classSymbol.HasMethod(method.Name))
                        {
                            _diagnostics.ReportError(method.Location, CoolErrorCodes.DuplicateMethod,
                                $"Method '{method.Name}' is multiply defined in class '{className}'.");
                            continue;
                        }
                        
                        var formals = method.Formals
                            .Select(f => new FormalSymbol(f.Name, f.TypeName))
                            .ToImmutableList();
                        var methodSymbol = MethodSymbol.Create(
                            method.Name,
                            method.ReturnTypeName,
                            formals,
                            method.Location);
                        classSymbol = classSymbol.WithMethod(methodSymbol);
                        break;

                    case AttributeNode attr:
                        if (classSymbol.HasAttribute(attr.Name))
                        {
                            _diagnostics.ReportError(attr.Location, CoolErrorCodes.DuplicateAttribute,
                                $"Attribute '{attr.Name}' is multiply defined in class '{className}'.");
                            continue;
                        }

                        var attrSymbol = new AttributeSymbol(
                            attr.Name,
                            attr.TypeName,
                            attr.Initializer,
                            attr.Location);
                        classSymbol = classSymbol.WithAttribute(attrSymbol);
                        break;
                }
            }

            currentTable = currentTable.WithClass(classSymbol);
            classIndex++;
        }

        // Check for missing Main class
        if (!_definedClasses.Contains("Main"))
        {
            _diagnostics.ReportError(SourcePosition.None, CoolErrorCodes.MissingMain,
                "Program must contain a class named 'Main'.");
        }

        // Update internal state so subsequent checks use the full table
        _symbolTable = currentTable;
        return currentTable;
    }

    /// <summary>
    /// Validates the inheritance relationships...
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
                    var classSym = _symbolTable.GetClass(className);
                    var location = classSym?.Definition?.Location ?? SourcePosition.None;
                    
                    _diagnostics.ReportError(location, CoolErrorCodes.UndefinedParent,
                        $"Class '{className}' inherits from undefined class '{parentName}'.");
                }
            }
        }
    }

    /// <summary>
    /// Detects cycles in the inheritance graph...
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

    private bool TryDetectCycle(string current, HashSet<string> visited, HashSet<string> stack)
    {
        visited.Add(current);
        stack.Add(current);

        if (_classParentMap.TryGetValue(current, out var parent) && parent is not null)
        {
            // If parent is undefined, we already reported it in EnsureAllParentsExist.
            if (!_definedClasses.Contains(parent) && !BasicClasses.Contains(parent))
            {
                 // Parent is not a known class, stop traversing this path.
                 stack.Remove(current);
                 return false;
            }

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
                
                // Use updated symbol table
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