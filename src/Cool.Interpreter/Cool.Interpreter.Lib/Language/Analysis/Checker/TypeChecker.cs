//-----------------------------------------------------------------------
// <copyright file="TypeChecker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>TypeChecker</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis.Checker;

using System.Collections.Immutable;
using Core.Diagnostics;
using Core.Syntax;
using Core.Syntax.Ast;
using Core.Syntax.Ast.Expressions;
using Core.Syntax.Ast.Features;
using Core.Syntax.Operators;
using Symbols;

/// <summary>
/// Provides the functionality for semantic type checking in the Cool programming language.
/// It validates type consistency and ensures that a Cool program adheres to type rules.
/// </summary>
public class TypeChecker
{
    /// <summary>
    /// Represents the symbol table used for managing and accessing class symbols
    /// during the type-checking process. This table provides essential information
    /// to ensure semantic correctness of the Cool program, including class definitions,
    /// their methods, and attributes.
    /// </summary>
    /// <remarks>
    /// The symbol table facilitates the resolution of types, methods, and class hierarchy
    /// by indexing class information. It serves as a core component of the type-checking
    /// phase, enabling analysis of the program's semantic structure and rules.
    /// </remarks>
    private readonly SymbolTable _symbols;

    /// <summary>
    /// Holds a collection of diagnostics generated during the semantic type-checking process.
    /// These diagnostics report errors, warnings, or informational messages related to the
    /// analysis of a Cool program's structure and type consistency.
    /// </summary>
    /// <remarks>
    /// This field is used to record issues identified during type checking, such as type
    /// mismatches, undefined variables, and violations of the language's type rules. It
    /// facilitates error tracking and helps ensure that the Cool program adheres to
    /// semantic correctness.
    /// </remarks>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// Tracks the name of the class that is currently being analyzed during the type-checking process.
    /// This information is essential for resolving `self` type references and for ensuring that
    /// type rules related to the current class context are correctly applied.
    /// </summary>
    /// <remarks>
    /// The current class context is updated as the type checker traverses different class definitions
    /// in the Cool program. It plays a vital role in scope resolution and method or attribute type
    /// verification within the class being analyzed.
    /// </remarks>
    private string _currentClass = "Object"; // type of 'self'

    /// <summary>
    /// Represents the current mapping of local variable names to their declared types within
    /// the scope of a method during the type-checking process. This dictionary is updated
    /// dynamically as the scope changes, such as when entering or exiting methods.
    /// </summary>
    /// <remarks>
    /// This variable helps to maintain the correct scope for local variables, ensuring their
    /// types are tracked and validated during semantic analysis. By resetting or restoring
    /// the mapping when transitioning between different scopes, it facilitates accurate
    /// type checking for variable usage.
    /// </remarks>
    private ImmutableDictionary<string, string> _localVariables = ImmutableDictionary<string, string>.Empty;

    /// <summary>
    /// Represents a semantic type checker that evaluates the types and ensures type correctness
    /// in a given syntax tree of a COOL language program.
    /// </summary>
    public TypeChecker(SymbolTable symbols, DiagnosticBag diagnostics)
    {
        _symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    /// <summary>
    /// Performs semantic type checking on the given program node to ensure type correctness
    /// according to the rules of the Cool programming language.
    /// </summary>
    /// <param name="program">The root node of the syntax tree representing the Cool program to be type-checked.</param>
    public void Check(ProgramNode program)
    {
        foreach (var classNode in program.Classes)
            CheckClass(classNode);
    }

    /// <summary>
    /// Performs semantic type checking on a given class node in the Cool program.
    /// Processes the features (attributes and methods) of the class to ensure type correctness,
    /// updating and restoring the current class and local variable context as necessary.
    /// </summary>
    /// <param name="classNode">The class node to be type-checked.</param>
    private void CheckClass(ClassNode classNode)
    {
        var previousClass = _currentClass;
        var previousLocals = _localVariables;

        _currentClass = classNode.Name;
        _localVariables = ImmutableDictionary<string, string>.Empty;

        foreach (var feature in classNode.Features)
        {
            switch (feature)
            {
                case AttributeNode attr: 
                    CheckAttribute(attr); 
                    break;
                
                case MethodNode method: 
                    CheckMethod(method); 
                    break;
            }
        }

        _currentClass = previousClass;
        _localVariables = previousLocals;
    }

    /// <summary>
    /// Validates an attribute feature by analyzing its initializer's type against the declared type
    /// to ensure type compatibility. Reports errors if type mismatches are found.
    /// </summary>
    /// <param name="attr">The attribute to be checked, which includes the declared type and optional initializer.</param>
    private void CheckAttribute(AttributeNode attr)
    {
        EnsureTypeExists(attr.TypeName, attr.Location);

        // Check for illegal shadowing (attribute defined in parent)
        var cls = _symbols.TryGetClass(_currentClass);
        var parentName = cls?.ParentName;
        while (parentName != null)
        {
            var parent = _symbols.TryGetClass(parentName);
            if (parent == null) break; 
            
            if (parent.Attributes.ContainsKey(attr.Name))
            {
                _diagnostics.ReportError(attr.Location, CoolErrorCodes.DuplicateAttribute,
                    $"Attribute '{attr.Name}' is an attribute of an inherited class.");
                return; // Stop checking to avoid cascading errors
            }
            parentName = parent.ParentName;
        }

        if (attr.Initializer is null) 
            return;

        var declaredType = attr.TypeName; // TypeName is validated above
        var actualType = GetExpressionType(attr.Initializer);

        if (!IsTypeCompatible(actualType, declaredType))
            _diagnostics.ReportError(attr.Location, CoolErrorCodes.TypeMismatchInAttributeInit, $"Attribute '{attr.Name}' is {declaredType}, but initializer has type {actualType}");
    }

    /// <summary>
    /// Analyzes the semantic correctness of a method within the context of type checking
    /// by validating its body, formal parameters, and return type consistency.
    /// </summary>
    /// <param name="method">The method to be evaluated, containing its formal parameters, body, and declared return type.</param>
    private void CheckMethod(MethodNode method)
    {
        EnsureTypeExists(method.ReturnTypeName, method.Location);

        // Check for valid override
        var cls = _symbols.TryGetClass(_currentClass);
        var parentName = cls?.ParentName;
        while (parentName != null)
        {
            var parent = _symbols.TryGetClass(parentName);
            if (parent == null) break;

            if (parent.Methods.TryGetValue(method.Name, out var parentMethod))
            {
                // Check formal parameter count match
                if (method.Formals.Count != parentMethod.Formals.Count)
                {
                    _diagnostics.ReportError(method.Location, CoolErrorCodes.MethodOverrideParamCountMismatch,
                        $"In redefined method '{method.Name}', parameter count {method.Formals.Count} is different from original parameter count {parentMethod.Formals.Count}.");
                }
                else
                {
                     // Check parameter types match
                     for (var i = 0; i < method.Formals.Count; i++)
                     {
                         if (method.Formals[i].TypeName != parentMethod.Formals[i].Type)
                         {
                             _diagnostics.ReportError(method.Location, CoolErrorCodes.MethodOverrideParamTypeMismatch,
                                 $"In redefined method '{method.Name}', parameter type {method.Formals[i].TypeName} is different from original type {parentMethod.Formals[i].Type}.");
                         }
                     }
                }
                
                // COOL allows method overrides with different return types
                // No return type checking is performed for overridden methods
                
                break; // Found the nearest override, stop checking up
            }
            parentName = parent.ParentName;
        }


        // Build scope: formals only (self is implicit)
        var scope = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
        foreach (var formal in method.Formals)
        {
            EnsureTypeExists(formal.TypeName, method.Location); // Or formal location if available
            scope[formal.Name] = formal.TypeName;
        }

        var previousLocals = _localVariables;
        _localVariables = scope.ToImmutable();

        var bodyType = GetExpressionType(method.Body);
        var returnType = method.ReturnTypeName == "SELF_TYPE" ? _currentClass : method.ReturnTypeName;

        if (!IsTypeCompatible(bodyType, returnType))
            _diagnostics.ReportError(method.Location, CoolErrorCodes.MethodReturnTypeMismatch,
                $"Method '{method.Name}' should return {returnType}, but body returns {bodyType}");

        _localVariables = previousLocals;
    }

    /// <summary>
    /// Verifies that the specified type is defined in the symbol table.
    /// Reports an error if the type is unknown.
    /// </summary>
    private void EnsureTypeExists(string typeName, SourcePosition location)
    {
        switch (typeName)
        {
            case "SELF_TYPE":  // Internal type?
            case "prim_slot":
                return;
        }

        if (_symbols.TryGetClass(typeName) is null)
        {
            _diagnostics.ReportError(location, CoolErrorCodes.UndefinedType, $"Type '{typeName}' is not defined.");
        }
    }

    /// <summary>
    /// Determines and returns the type of a given expression node within the context of the COOL language.
    /// </summary>
    /// <param name="expr">The expression node for which the type is to be determined.</param>
    /// <returns>A string representing the determined type of the provided expression node.</returns>
    private string GetExpressionType(ExpressionNode expr) 
        => expr switch
    {
        IntegerLiteralNode          => "Int",
        StringLiteralNode           => "String",
        BoolLiteralNode             => "Bool",
        SelfNode                    => _currentClass,
        IdentifierExpressionNode id => GetVariableType(id.Identifier, id.Location),
        NewNode n                   => GetTypeOfNew(n),
        
        AssignNode a                => GetTypeOfAssign(a),
        DispatchNode d              => GetTypeOfDispatch(d),
        IfNode i                    => GetTypeOfIf(i),
        WhileNode                   => "Object",
        BlockNode b                 => GetTypeOfBlock(b),
        LetNode l                   => GetTypeOfLet(l),
        CaseNode c                  => GetTypeOfCase(c),
        IsVoidNode                  => "Bool",

        BinaryOperationNode op      => GetTypeOfBinary(op),
        UnaryOperationNode op       => GetTypeOfUnary(op), 

        _ => "Object"
    };

    private string GetTypeOfNew(NewNode node)
    {
        EnsureTypeExists(node.TypeName, node.Location);
        return node.TypeName == "SELF_TYPE" ? _currentClass : node.TypeName;
    }

    /// <summary>
    /// Determines the type of an assignment expression by validating the compatibility
    /// between the type of the assigned expression and the type of the variable being assigned to.
    /// </summary>
    /// <param name="node">The assignment node containing the target variable, the expression being assigned, and the source position in the syntax tree.</param>
    /// <returns>The type of the assigned expression if the assignment is valid; otherwise, reports a type error.</returns>
    private string GetTypeOfAssign(AssignNode node)
    {
        var valueType = GetExpressionType(node.Expression);
        var variableType = GetVariableType(node.Identifier, node.Location);

        if (!IsTypeCompatible(valueType, variableType))
            _diagnostics.ReportError(node.Location, CoolErrorCodes.AssignToWrongType,$"Cannot assign {valueType} to variable of type {variableType}");

        return valueType;
    }

    /// <summary>
    /// Determines the static return type of a method dispatch (method call) expression in the Cool language,
    /// ensuring type correctness by evaluating the receiver's type, the method's signature, and the types
    /// of the provided arguments.
    /// </summary>
    /// <param name="node">The dispatch node representing the method call, including the caller,
    /// method name, and arguments.</param>
    /// <returns>The static type of the method dispatch as a string, which may be specific to the method's return
    /// type or the static class context in case of special types like "SELF_TYPE".</returns>
    private string GetTypeOfDispatch(DispatchNode node)
    {
        string dispatchType;  // The type from which we start method lookup

        if (node.Caller == null)
        {
            // Implicit self dispatch: f(args) ≡ self.f(args)
            dispatchType = _currentClass;  // e.g., "Main"
        }
        else
        {
            // Explicit dispatch: e.f(args)
            dispatchType = GetExpressionType(node.Caller);
        }

        // Static dispatch: e@T.f(args)
        var lookupType = node.StaticTypeName ?? dispatchType;

        // Optional: check conformance for static dispatch
        if (node.StaticTypeName == null)
            return GetMethodReturnType(lookupType, node.MethodName, node.Arguments, node.Location, dispatchType);
        EnsureTypeExists(node.StaticTypeName, node.Location);
        if (!IsTypeCompatible(dispatchType, node.StaticTypeName))
        {
            _diagnostics.ReportError(node.Location, CoolErrorCodes.StaticDispatchTypeError,
                $"Receiver of type '{dispatchType}' does not conform to static type '{node.StaticTypeName}'");
            // Recovery: still use the static type for lookup
        }

        // Note: dispatchType is passed for SELF_TYPE resolution - even with static dispatch,
        // SELF_TYPE resolves to the type of the caller expression, not the static dispatch type.
        return GetMethodReturnType(lookupType, node.MethodName, node.Arguments, node.Location, dispatchType);
    }

    /// <summary>
    /// Determines the return type of a specified method within a given class, verifying its existence,
    /// checking method arguments, and resolving the actual return type or reporting errors if necessary.
    /// </summary>
    /// <param name="className">The class in which to look up the method</param>
    /// <param name="methodName">The name of the method</param>
    /// <param name="args">The arguments to the method</param>
    /// <param name="pos">The source location for error reporting</param>
    /// <param name="selfTypeResolution">The type to use when resolving SELF_TYPE (typically the receiver's type)</param>
    private string GetMethodReturnType(string className, string methodName,
        IReadOnlyList<ExpressionNode> args, SourcePosition pos, string? selfTypeResolution = null)
    {
        var cls = FindClassWithMethod(className, methodName);
        if (cls is null)
        {
            _diagnostics.ReportError(pos, CoolErrorCodes.UndefinedMethod, $"Method '{methodName}' not found in class {className} or its ancestors");
            return "Object";
        }
        
        // This is safe because FindClassWithMethod keeps returning non-null only if method exists
        var method = cls.Methods[methodName];
        CheckMethodArguments(method, args, pos);

        // When resolving SELF_TYPE, use selfTypeResolution if provided (for static dispatch),
        // otherwise fall back to className (for regular dispatch)
        var typeForSelfType = selfTypeResolution ?? className;
        return method.ReturnType == "SELF_TYPE" ? typeForSelfType : method.ReturnType;
    }

    /// <summary>
    /// Searches for a class in the symbol table that contains a method with the specified name.
    /// The search considers the inheritance hierarchy of the class.
    /// </summary>
    private ClassSymbol? FindClassWithMethod(string className, string methodName)
    {
        var cls = _symbols.TryGetClass(className);
        while (cls != null)
        {
            if (cls.Methods.ContainsKey(methodName))
                return cls;
            cls = cls.ParentName is null ? null : _symbols.TryGetClass(cls.ParentName);
        }
        return null;
    }

    /// <summary>
    /// Validates the arguments supplied to a method call by checking their count and types against
    /// the formal parameters defined in the method declaration. Reports error if count or types are incompatible.
    /// </summary>
    private void CheckMethodArguments(MethodSymbol method, IReadOnlyList<ExpressionNode> args, SourcePosition pos)
    {
        if (method.Formals.Count != args.Count)
        {
             _diagnostics.ReportError(pos, CoolErrorCodes.WrongNumberOfArguments, 
                 $"Method '{method.Name}' expects {method.Formals.Count} arguments, but {args.Count} were provided.");
             // Stop validation here to avoid index out of range
             return; 
        }

        for (var i = 0; i < args.Count; i++)
        {
            var argType = GetExpressionType(args[i]);
            var paramType = method.Formals[i].Type;
            if (!IsTypeCompatible(argType, paramType))
                _diagnostics.ReportError(args[i].Location, CoolErrorCodes.ArgumentTypeMismatch, $"Argument {i+1}: expected {paramType}, got {argType}");
        }
    }

    /// <summary>
    /// Determines the inferred type of an `if` expression in the COOL language.
    /// Validates that the predicate of the `if` expression is of type `Bool`.
    /// Combines the types of the `then` and `else` branches to compute the result type.
    /// </summary>
    /// <param name="node">The `IfNode` representing the `if` expression to type-check.</param>
    /// <returns>
    /// The inferred type of the `if` expression, determined as the least common ancestor
    /// of the types of the `then` and `else` branches in the class hierarchy.
    /// </returns>
    private string GetTypeOfIf(IfNode node)
    {
        var condType = GetExpressionType(node.Predicate);
        if (condType != "Bool")
            _diagnostics.ReportError(node.Predicate.Location, CoolErrorCodes.IfPredicateNotBool, $"If condition must be Bool, got {condType}");

        var thenType = GetExpressionType(node.ThenBranch);
        var elseType = GetExpressionType(node.ElseBranch);
        return JoinTypes(thenType, elseType);
    }

    /// <summary>
    /// Determines the type of a block expression by evaluating the type
    /// of the last expression in the block. If the block is empty,
    /// it defaults to "Object".
    /// </summary>
    /// <param name="node">The block node containing a sequence of expressions
    /// whose type is to be evaluated.</param>
    /// <returns>The type of the last expression in the block, or "Object" if the block is empty.</returns>
    private string GetTypeOfBlock(BlockNode node)
        => node.Expressions.LastOrDefault() is { } e ? GetExpressionType(e) : "Object";

    /// <summary>
    /// Infers and returns the semantic type of a binary operation in a COOL program based on the operator
    /// and the types of the left and right operands.
    /// </summary>
    private string GetTypeOfBinary(BinaryOperationNode node)
    {
        var left = GetExpressionType(node.Left);
        var right = GetExpressionType(node.Right);

        return node.Operator switch
        {
            BinaryOperator.Plus or BinaryOperator.Minus or
            BinaryOperator.Multiply or BinaryOperator.Divide
                when left == "Int" && right == "Int" => "Int",

            BinaryOperator.LessThan or BinaryOperator.LessThanOrEqual
                when left == "Int" && right == "Int" => "Bool",

            BinaryOperator.Equal => "Bool", 
            
            _ => ReportAndRecover(node.Location, CoolErrorCodes.InvalidBinaryOperation, $"Operator '{node.Operator}' not supported on types '{left}' and '{right}'")
        };
    }

    /// <summary>
    /// Determines the type of a unary operation by evaluating the type of its operand
    /// and considering the specific operator applied.
    /// </summary>
    private string GetTypeOfUnary(UnaryOperationNode node)
    {
        var type = GetExpressionType(node.Operand);
        return node.Operator switch
        {
            UnaryOperator.Negate when type == "Int" => "Int",
            
            UnaryOperator.Not when type == "Bool" => "Bool",
            
            _ => ReportAndRecover(node.Location, CoolErrorCodes.InvalidUnaryOperation, $"Operator '{node.Operator}' not supported on type '{type}'")
        };
    }

    /// <summary>
    /// Reports a diagnostic error for the given source position and error details,
    /// and provides a recovery type to maintain semantic analysis consistency.
    /// </summary>
    private string ReportAndRecover(SourcePosition pos, string code, string message)
    {
        _diagnostics.ReportError(pos, code, message);
        return "Object";
    }

    /// <summary>
    /// Retrieves the type of a given variable within the context of the current class and local scope.
    /// In Cool, if an identifier is not found as a variable, it may be a zero-argument method call.
    /// Reports an error if the variable is undefined and no method with that name exists.
    /// </summary>
    private string GetVariableType(string name, SourcePosition location)
    {
        if (_localVariables.TryGetValue(name, out var type)) return type;
        if (name == "self") return _currentClass;

        var cls = _symbols.TryGetClass(_currentClass);
        while (cls != null)
        {
            if (cls.Attributes.TryGetValue(name, out var attr))
                return attr.Type;
            cls = cls.ParentName is null ? null : _symbols.TryGetClass(cls.ParentName);
        }

        // In Cool, an identifier without parentheses may also be a zero-argument method call.
        // This is syntactic sugar for self.methodName().
        var clsWithMethod = FindClassWithMethod(_currentClass, name);
        if (clsWithMethod != null)
        {
            var method = clsWithMethod.Methods[name];
            // Zero-argument method call - return the method's return type
            return method.ReturnType == "SELF_TYPE" ? _currentClass : method.ReturnType;
        }

        _diagnostics.ReportError(location, CoolErrorCodes.UndefinedVariable, $"Undefined variable '{name}'");
        return "Object";
    }

    /// <summary>
    /// Determines whether a given subtype is compatible with a specified supertype within the context
    /// of the class hierarchy in the COOL language.
    /// Returns true if the subtype is compatible with the supertype.
    /// </summary>
    private bool IsTypeCompatible(string? subtype, string? supertype)
    {
        if (subtype == supertype) return true;
        if (subtype is null || supertype is null) return false;
        if (subtype == "SELF_TYPE") subtype = _currentClass;
        if (supertype == "SELF_TYPE") supertype = _currentClass;

        var current = _symbols.TryGetClass(subtype);
        while (current != null)
        {
            if (current.Name == supertype) return true;
            current = current.ParentName is null ? null : _symbols.TryGetClass(current.ParentName);
        }
        return false;
    }

    /// <summary>
    /// Determines and returns the type of a "let" expression, including its bindings and body, in the syntax tree of a COOL program.
    /// </summary>
    /// <param name="node">The <see cref="LetNode"/> instance representing the "let" expression to be analyzed.</param>
    /// <returns>A string representing the type of the "let" expression's body, which ultimately determines the type of the entire "let" expression.</returns>
    /// <summary>
    /// Determines and returns the type of a "let" expression, including its bindings and body, in the syntax tree of a COOL language program.
    /// </summary>
    private string GetTypeOfLet(LetNode node)
    {
        var previousLocals = _localVariables;
        // Don't bulk-copy yet, we need to respect scope order
        // But since _localVariables is immutable, we just update it in the loop
        
        foreach (var binding in node.Bindings)
        {
            if (binding is null) continue;
            
            var declaredType = binding.TypeName;  // ?? "Object";

            if (binding.Initializer is not null)
            {
                // Verify initializer using CURRENT scope (includes previous let-bindings)
                var initializationType = GetExpressionType(binding.Initializer);

                if (!IsTypeCompatible(initializationType, declaredType))
                {
                    _diagnostics.ReportError(binding.Location,
                        CoolErrorCodes.LetBindingTypeMismatch,
                        $"Let binding '{binding.Identifier}' of type {declaredType} cannot be initialized with type {initializationType}");
                }
            }
            
            // Update scope for NEXT binding and BODY
            _localVariables = _localVariables.SetItem(binding.Identifier, declaredType);
        }

        // Type of let = type of body
        var bodyType = GetExpressionType(node.Body);

        // Restore scope
        _localVariables = previousLocals;

        return bodyType;
    }
    
    /// <summary>
    /// Determines the resulting type of a "case" expression by evaluating all branches...
    /// </summary>
    private string GetTypeOfCase(CaseNode node)
    {
        GetExpressionType(node.Scrutinee); 

        string? resultType = null;
        foreach (var branch in node.Branches)
        {
            var old = _localVariables;
            _localVariables = _localVariables.SetItem(branch.Identifier, branch.TypeName);
            var branchType = GetExpressionType(branch.Body);
            resultType = resultType is null ? branchType : JoinTypes(resultType, branchType);
            _localVariables = old;
        }
        return resultType ?? "Object";
    }

    /// <summary>
    /// Determines the least common ancestor (LCA) of two types in the inheritance hierarchy.
    /// </summary>
    private string JoinTypes(string type1, string type2)
    {
        if (type1 == type2) return type1;
        if (type1 == "SELF_TYPE") type1 = _currentClass;
        if (type2 == "SELF_TYPE") type2 = _currentClass;

        // Get inheritance chain for type1
        var chain1 = new HashSet<string>(StringComparer.Ordinal);
        var curr = type1;
        while (curr != null)
        {
            chain1.Add(curr);
            var cls = _symbols.GetClass(curr);
            curr = cls?.ParentName;
        }

        // Walk up type2 until we find a match
        curr = type2;
        while (curr != null)
        {
            if (chain1.Contains(curr)) return curr;
            var cls = _symbols.GetClass(curr);
            curr = cls?.ParentName;
        }

        return "Object"; // Fallback, though Object should have been found
    }

    /// <summary>
    /// Determines the default type for a given Cool language type name.
    /// For primitive types such as Int, String, or Bool, it returns the type name.
    /// For other types, the default type is Object.
    /// </summary>
    /// <param name="typeName">The name of the type to evaluate for a default type.</param>
    /// <returns>The default type corresponding to the given type name.</returns>
    private string DeterminateDefaultType(string typeName)
    {
        return typeName switch
        {
            "Int"    => "Int",
            "String" => "String",
            "Bool"   => "Bool",
            _        => "Object"
        };
    }
    private bool IsBuiltInClass(string? className)
    {
        return className is "Object" or "IO" or "Int" or "String" or "Bool";
    }
}
