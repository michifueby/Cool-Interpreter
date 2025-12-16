//-----------------------------------------------------------------------
// <copyright file="TypeChecker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>TypeChecker</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Analysis.Checker;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Core.Syntax.Operators;
using Cool.Interpreter.Lib.Language.Symbols;

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
                case AttributeNode attr: CheckAttribute(attr); 
                    break;
                
                case MethodNode method: CheckMethod(method); 
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
        if (attr.Initializer is null) 
            return;

        var declaredType = attr.TypeName;
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
        // Build scope: formals only (self is implicit)
        var scope = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
        foreach (var formal in method.Formals)
            scope[formal.Name] = formal.TypeName;

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
        NewNode n                   => n.TypeName == "SELF_TYPE" ? _currentClass : n.TypeName,
        
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
        var receiverType = GetExpressionType(node.Caller);
        var staticType = node.StaticTypeName;

        if (string.IsNullOrEmpty(staticType))
            return GetMethodReturnType(receiverType, node.MethodName, node.Arguments, node.Location);
        
        if (!IsTypeCompatible(receiverType, staticType))
            _diagnostics.ReportError(node.Location, CoolErrorCodes.StaticDispatchTypeError, $"Receiver of type {receiverType} cannot be used as {staticType}");

        return GetMethodReturnType(staticType, node.MethodName, node.Arguments, node.Location);
    }

    /// <summary>
    /// Determines the return type of a specified method within a given class, verifying its existence,
    /// checking method arguments, and resolving the actual return type or reporting errors if necessary.
    /// </summary>
    /// <param name="className">The name of the class containing the method to be checked.</param>
    /// <param name="methodName">The name of the method whose return type is determined.</param>
    /// <param name="args">The list of argument expressions passed to the method.</param>
    /// <param name="pos">The source position indicating where the method call occurs in the program.</param>
    /// <returns>The return type of the method, or "Object" if the method is not found or an error is encountered.</returns>
    private string GetMethodReturnType(string className, string methodName,
        IReadOnlyList<ExpressionNode> args, SourcePosition pos)
    {
        var cls = FindClassWithMethod(className, methodName, args.Count);
        if (cls is null)
        {
            _diagnostics.ReportError(pos, CoolErrorCodes.StaticDispatchTypeError, $"Method '{methodName}' not found in class {className} or its ancestors");
            return "Object";
        }

        var method = cls.Methods[methodName];
        CheckMethodArguments(method, args, pos);

        return method.ReturnType == "SELF_TYPE" ? className : method.ReturnType;
    }

    /// <summary>
    /// Searches for a class in the symbol table that contains a method with the specified name and arity
    /// (number of parameters). The search considers the inheritance hierarchy of the class.
    /// </summary>
    /// <param name="className">The name of the class to begin the search from.</param>
    /// <param name="methodName">The name of the method to look for.</param>
    /// <param name="arity">The number of parameters the method should have.</param>
    /// <returns>The class symbol containing the method if found; otherwise, null.</returns>
    private ClassSymbol? FindClassWithMethod(string className, string methodName, int arity)
    {
        var cls = _symbols.GetClass(className);
        while (cls != null)
        {
            if (cls.Methods.TryGetValue(methodName, out var m) && m.Formals.Count == arity)
                return cls;
            cls = cls.ParentName is null ? null : _symbols.GetClass(cls.ParentName);
        }
        return null;
    }

    /// <summary>
    /// Validates the arguments supplied to a method call by checking their types against the formal parameters
    /// defined in the method declaration. Reports an error if the types are incompatible.
    /// </summary>
    /// <param name="method">The method symbol containing the formal parameter definitions.</param>
    /// <param name="args">A list of expression nodes representing the arguments passed to the method call.</param>
    /// <param name="pos">The source position indicating where the method call occurs in the syntax tree.</param>
    private void CheckMethodArguments(MethodSymbol method, IReadOnlyList<ExpressionNode> args, SourcePosition pos)
    {
        for (int i = 0; i < args.Count; i++)
        {
            var argType = GetExpressionType(args[i]);
            var paramType = method.Formals[i].Type;
            if (!IsTypeCompatible(argType, paramType))
                _diagnostics.ReportError(args[i].Location, CoolErrorCodes.WrongNumberOfArguments, $"Argument {i+1}: expected {paramType}, got {argType}");
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
    /// Determines and returns the type of a "let" expression, including its bindings and body, in the syntax tree of a COOL program.
    /// </summary>
    /// <param name="node">The <see cref="LetNode"/> instance representing the "let" expression to be analyzed.</param>
    /// <returns>A string representing the type of the "let" expression's body, which ultimately determines the type of the entire "let" expression.</returns>
    private string GetTypeOfLet(LetNode node)
    {
        var previousLocals = _localVariables;
        var builder = _localVariables.ToBuilder();

        foreach (var binding in node.Bindings)
        {
            string declaredType = binding.TypeName;           // may be null if omitted
            string? initializerType = null;

            if (binding.Initializer is not null)
            {
                initializerType = GetExpressionType(binding.Initializer);

                if (declaredType is not null && !IsTypeCompatible(initializerType, declaredType))
                {
                    _diagnostics.ReportError(binding.Location,
                        CoolErrorCodes.LetBindingTypeMismatch,
                        $"Let variable '{binding.Identifier}' declared as '{declaredType}', " +
                        $"but initialized with expression of type '{initializerType}'");
                }
            }

            // Determine the actual type of the binding
            string bindingType = declaredType
                                 ?? initializerType
                                 ?? DeterminateDefaultTypeForDeclaredOrError(binding); // only if declared

            // If no type and no initializer → error (not allowed in Cool)
            if (declaredType is null && initializerType is null)
            {
                _diagnostics.ReportError(binding.Location,
                    CoolErrorCodes.LetNoTypeNoInit,
                    $"Let variable '{binding.Identifier}' has no type declaration and no initializer");
                bindingType = "Object"; // recover
            }

            // Add to scope
            builder[binding.Identifier] = bindingType;
        }

        _localVariables = builder.ToImmutable();

        // Type of let = type of body
        string bodyType = GetExpressionType(node.Body);

        // Restore scope
        _localVariables = previousLocals;

        return bodyType;
    }
    
    private string DeterminateDefaultTypeForDeclaredOrError(LetBindingNode binding)
    {
        if (binding.TypeName is not null)
            return DeterminateDefaultType(binding.TypeName);

        // Should not reach here — already checked above
        return "Object";
    }

    /// <summary>
    /// Determines the resulting type of a "case" expression by evaluating all branches of the
    /// "case" construct, ensuring type consistency and computing the least common ancestor
    /// of branch result types.
    /// </summary>
    /// <param name="node">The <see cref="CaseNode"/> representing the "case" expression to analyze.</param>
    /// <returns>The resulting type of the "case" expression. Returns "Object" if no branches are defined.</returns>
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
    /// Infers and returns the semantic type of a binary operation in a COOL program based on the operator
    /// and the types of the left and right operands.
    /// </summary>
    /// <param name="node">The binary operation node representing the operation within the syntax tree.</param>
    /// <returns>
    /// The string representation of the resolved type for the binary operation. This can include "Int",
    /// "Bool", or "Object" based on the operator and operand types.
    /// </returns>
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
            
            _ => this.ReportAndRecover(node.Location, CoolErrorCodes.InvalidBinaryOperation, $"Operator '{node.Operator}' not supported on types '{left}' and '{right}'")
        };
    }

    /// <summary>
    /// Determines the type of a unary operation by evaluating the type of its operand
    /// and considering the specific operator applied.
    /// </summary>
    /// <param name="node">The unary operation node containing the operator and its operand expression.</param>
    /// <returns>The deduced type of the unary operation. If the operator is not applicable to the operand type, returns "Object" and reports an error.</returns>
    private string GetTypeOfUnary(UnaryOperationNode node)
    {
        var type = GetExpressionType(node.Operand);
        return node.Operator switch
        {
            UnaryOperator.Negate when type == "Int" => "Int",
            
            UnaryOperator.Not when type == "Bool" => "Bool",
            
            _ => this.ReportAndRecover(node.Location, CoolErrorCodes.InvalidUnaryOperation, $"Operator '{node.Operator}' not supported on type '{type}'")
        };
    }

    /// <summary>
    /// Reports a diagnostic error for the given source position and error details,
    /// and provides a recovery type to maintain semantic analysis consistency.
    /// </summary>
    /// <param name="pos">The source position in the program where the error occurred.</param>
    /// <param name="code">The error code associated with the diagnostic.</param>
    /// <param name="message">The error message describing the diagnostic issue.</param>
    /// <returns>The default recovery type, which is always "Object".</returns>
    private string ReportAndRecover(SourcePosition pos, string code, string message)
    {
        _diagnostics.ReportError(pos, code, message);
        return "Object";
    }

    /// <summary>
    /// Retrieves the type of a given variable within the context of the current class and local scope.
    /// Reports an error if the variable is undefined or inaccessible in the given location.
    /// </summary>
    /// <param name="name">The name of the variable whose type is to be determined.</param>
    /// <param name="location">The source code position of the variable occurrence for error reporting.</param>
    /// <returns>The type of the variable as a string, or "Object" if the variable is undefined.</returns>
    private string GetVariableType(string name, SourcePosition location)
    {
        if (_localVariables.TryGetValue(name, out var type)) return type;
        if (name == "self") return _currentClass;

        var cls = _symbols.GetClass(_currentClass);
        while (cls != null)
        {
            if (cls.Attributes.TryGetValue(name, out var attr))
                return attr.Type;
            cls = cls.ParentName is null ? null : _symbols.GetClass(cls.ParentName);
        }

        _diagnostics.ReportError(location, CoolErrorCodes.UndefinedVariable, $"Undefined variable '{name}'");
        return "Object";
    }

    /// <summary>
    /// Determines whether a given subtype is compatible with a specified supertype within the context
    /// of the class hierarchy in the COOL language.
    /// This is used for ensuring type correctness when analyzing expressions and assignments.
    /// </summary>
    /// <param name="subtype">The name of the subtype being checked for compatibility. This can include "SELF_TYPE".</param>
    /// <param name="supertype">The name of the supertype against which compatibility is being evaluated. This can include "SELF_TYPE".</param>
    /// <returns>
    /// Returns true if the subtype is compatible with the supertype (i.e., the subtype is the same as
    /// or inherits from the supertype). Returns false otherwise. If either type is null, the method will also return false.
    /// </returns>
    private bool IsTypeCompatible(string? subtype, string? supertype)
    {
        if (subtype == supertype) return true;
        if (subtype is null || supertype is null) return false;
        if (subtype == "SELF_TYPE") subtype = _currentClass;
        if (supertype == "SELF_TYPE") supertype = _currentClass;

        var current = _symbols.GetClass(subtype);
        while (current != null)
        {
            if (current.Name == supertype) return true;
            current = current.ParentName is null ? null : _symbols.GetClass(current.ParentName);
        }
        return false;
    }

    /// <summary>
    /// Determines the least common ancestor (join) of two types in the type hierarchy.
    /// This is used to find a common type when evaluating expressions with multiple branches,
    /// such as in conditional or case statements in the COOL language.
    /// </summary>
    /// <param name="type1">The name of the first type in the type comparison.</param>
    /// <param name="type2">The name of the second type in the type comparison.</param>
    /// <returns>The name of the least common ancestor type if the types are compatible,
    /// or "Object" if no compatibility exists between the provided types.</returns>
    private string JoinTypes(string type1, string type2) =>
        IsTypeCompatible(type1, type2) ? type2 :
        IsTypeCompatible(type2, type1) ? type1 : "Object";

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
}