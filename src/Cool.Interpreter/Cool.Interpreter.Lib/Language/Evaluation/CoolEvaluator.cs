//-----------------------------------------------------------------------
// <copyright file="CoolEvaluator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolEvaluator</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using System;
using System.Linq;
using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Core.Syntax.Operators;
using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents an evaluator for executing Cool language programs.
/// The <c>CoolEvaluator</c> class implements the <c>ICoolSyntaxVisitor</c> interface to traverse
/// and evaluate abstract syntax tree (AST) nodes, producing corresponding <c>CoolObject</c> results.
/// </summary>
/// <remarks>
/// This class works within the context of the Cool runtime environment and supports the evaluation
/// of various Cool language constructs, such as literals, expressions, and program structures.
/// </remarks>
public class CoolEvaluator : ICoolSyntaxVisitor<CoolObject>
{
    /// <summary>
    /// Represents the runtime environment used by the evaluator to store global state,
    /// manage the symbol table, and provide access to runtime classes and objects.
    /// This variable is used internally by the CoolEvaluator to facilitate the evaluation
    /// of Cool programs, including object creation and method dispatch.
    /// </summary>
    private readonly CoolRuntimeEnvironment _runtime;

    /// <summary>
    /// Represents the current evaluation environment in which variables, bindings, and
    /// the invoking object (self) are tracked during the execution of a Cool program.
    /// This variable is used to manage scope and ensure proper context for evaluating
    /// expressions, resolving identifiers, and handling assignments.
    /// </summary>
    private Environment _env;

    /// <summary>
    /// Defines methods to evaluate and interpret a Cool language syntax tree, producing corresponding <see cref="CoolObject"/> instances.
    /// Implements the <see cref="ICoolSyntaxVisitor{CoolObject}"/> interface to visit various syntax nodes and execute their logic.
    /// </summary>
    /// <remarks>
    /// This class operates as the core evaluator in the Cool language interpreter, handling runtime environments
    /// and dispatch mechanisms for evaluating programs and expressions.
    /// </remarks>
    public CoolEvaluator(CoolRuntimeEnvironment runtime) 
        => _runtime = runtime;

    public CoolObject Evaluate(ProgramNode program)
    {
        // 1. Find Main class symbol
        var mainClassSymbol = _runtime.SymbolTable.GetClass("Main")
                              ?? throw new CoolRuntimeException("No class Main found");

        // 2. Find the main() method (must have 0 parameters)
        var mainMethodNode = mainClassSymbol.Definition!.Features
                                 .OfType<MethodNode>()
                                 .FirstOrDefault(m => m.Name == "main" && m.Formals.Count == 0)
                             ?? throw new CoolRuntimeException("main() method not found or has parameters");

        // 3. Convert symbol → runtime class
        var mainRuntimeClass = RuntimeClassFactory.FromSymbol(mainClassSymbol, _runtime);

        // 4. Create the Main object
        var mainObject = ObjectFactory.Create(mainRuntimeClass, _runtime);

        // 5. Set up environment with self = mainObject
        _env = Environment.Empty.WithSelf(mainObject);

        // 6. Execute the body of main() 
        return mainMethodNode.Body.Accept(this);
    }

    /// <summary>
    /// Evaluates a given expression node within a specified environment and returns the resulting <see cref="CoolObject"/>.
    /// Temporarily changes the current environment to the supplied one for the duration of the evaluation,
    /// reverting back to the original environment once the evaluation is complete.
    /// </summary>
    /// <param name="expr">The expression node to be evaluated.</param>
    /// <param name="newEnv">The new environment context in which the expression will be evaluated.</param>
    /// <returns>The <see cref="CoolObject"/> resulting from the evaluation of the expression.</returns>
    private CoolObject StartEvaluation(ExpressionNode expr, Environment newEnv)
    {
        var old = _env;
        _env = newEnv;

        try
        {
            return expr.Accept(this);
        }
        finally
        {
            _env = old;
        }
    }

    /// <summary>
    /// Evaluates the given <see cref="IntegerLiteralNode"/> by creating and returning a <see cref="CoolInt"/> object
    /// with the integer value represented by the node.
    /// </summary>
    /// <param name="node">The <see cref="IntegerLiteralNode"/> containing the integer value to be evaluated.</param>
    /// <returns>A <see cref="CoolInt"/> object representing the integer value encapsulated in the provided <see cref="IntegerLiteralNode"/>.</returns>
    public CoolObject Visit(IntegerLiteralNode node) 
        => new CoolInt(node.Value);

    /// <summary>
    /// Evaluates the given <see cref="StringLiteralNode"/> by creating and returning a <see cref="CoolString"/> object
    /// encapsulating the string value represented by the node.
    /// </summary>
    /// <param name="node">The <see cref="StringLiteralNode"/> containing the string value to be evaluated.</param>
    /// <returns>A <see cref="CoolString"/> object representing the string value encapsulated in the specified <see cref="StringLiteralNode"/>.</returns>
    public CoolObject Visit(StringLiteralNode node) 
        => new CoolString(node.Value);

    /// <summary>
    /// Evaluates the given <see cref="BoolLiteralNode"/> by returning the corresponding <see cref="CoolBool"/> object.
    /// </summary>
    /// <param name="node">The <see cref="BoolLiteralNode"/> containing the boolean value to be evaluated.</param>
    /// <returns>A <see cref="CoolBool"/> object representing the boolean value encapsulated in the provided <see cref="BoolLiteralNode"/>.</returns>
    public CoolObject Visit(BoolLiteralNode node) 
        => node.Value ? CoolBool.True : CoolBool.False;

    /// <summary>
    /// Evaluates the given <see cref="SelfNode"/> and returns the current context's <see cref="CoolObject"/>
    /// representing the "self" reference.
    /// </summary>
    /// <param name="node">The <see cref="SelfNode"/> representing the "self" expression in the syntax tree.</param>
    /// <returns>The <see cref="CoolObject"/> that represents the "self" instance in the current execution context.</returns>
    public CoolObject Visit(SelfNode node) 
        => _env.Self;

    /// <summary>
    /// Evaluates a given <see cref="NoExpressionNode"/> and provides its corresponding <see cref="CoolObject"/>
    /// representation, which is a predefined instance of <see cref="CoolVoid"/>.
    /// </summary>
    /// <param name="node">The <see cref="NoExpressionNode"/> to be evaluated, representing an absence of a specific expression in the AST.</param>
    /// <returns>The singleton instance of <see cref="CoolVoid"/>, indicating the lack of a meaningful expression result.</returns>
    public CoolObject Visit(NoExpressionNode node) 
        => CoolVoid.Value;

    /// <summary>
    /// Evaluates the given <see cref="IdentifierExpressionNode"/> by retrieving the corresponding
    /// <see cref="CoolObject"/> from the environment using the identifier provided in the node.
    /// </summary>
    /// <param name="node">The <see cref="IdentifierExpressionNode"/> containing the identifier for which
    /// the corresponding value will be looked up in the environment.</param>
    /// <returns>The <see cref="CoolObject"/> associated with the identifier in the provided
    /// <see cref="IdentifierExpressionNode"/>.</returns>
    public CoolObject Visit(IdentifierExpressionNode node) 
        => _env.Lookup(node.Identifier);

    /// <summary>
    /// Evaluates the given <see cref="AssignNode"/> by executing its expression and updating the corresponding object's attribute in the environment.
    /// </summary>
    /// <param name="node">The <see cref="AssignNode"/> containing the identifier of the attribute to be assigned and the expression to evaluate for its value.</param>
    /// <returns>The evaluated <see cref="CoolObject"/> resulting from the expression of the given <see cref="AssignNode"/>.</returns>
    /// <exception cref="CoolRuntimeException">Thrown when the assignment is attempted outside an object context or the current object context is invalid.</exception>
    public CoolObject Visit(AssignNode node)
    {
        var value = node.Expression.Accept(this);

        if (_env.Self is not CoolUserObject selfObj)
            throw new CoolRuntimeException("assignment outside object context");

        var newSelf = selfObj.WithAttribute(node.Identifier, value);
        _env = _env.WithSelf(newSelf);

        return value; // Cool spec 6
    }

    /// <summary>
    /// Creates a new instance of a CoolObject based on the given type from a NewNode.
    /// </summary>
    /// <param name="node">The NewNode containing the type name for which a new CoolObject instance needs to be created.
    /// Valid type names include "Int", "String", "Bool", "IO", "Object", and custom user-defined types.</param>
    /// <returns>A CoolObject instance corresponding to the specified type name. For the built-in types, it returns:
    /// <see cref="CoolInt.Zero"/> for "Int", <see cref="CoolString.Empty"/> for "String", and <see cref="CoolBool.False"/> for "Bool".
    /// For user-defined types, it creates and returns a new instance using the object factory.</returns>
    public CoolObject Visit(NewNode node) => node.TypeName switch
    {
        "Int" => CoolInt.Zero,
        
        "String" => CoolString.Empty,
        
        "Bool"   => CoolBool.False,
        
        "IO"     => _runtime.Io,
        
        "Object" => _runtime.ObjectRoot,
        
        _ => ObjectFactory.Create(RuntimeClassFactory.FromSymbol(_runtime.SymbolTable.GetClass(node.TypeName), _runtime), _runtime)
    };

    /// <summary>
    /// Evaluates the given <see cref="DispatchNode"/> by invoking the specified method on the receiver object
    /// with the provided arguments, and returns the result of the method call.
    /// </summary>
    /// <param name="node">The <see cref="DispatchNode"/> representing the method dispatch, including the receiver, the method name,
    /// the arguments, and an optional static type for method resolution.</param>
    /// <returns>A <see cref="CoolObject"/> that represents the result of the method invocation on the receiver.</returns>
    /// <exception cref="CoolRuntimeException">Thrown if the method specified in the <see cref="DispatchNode"/> is not found
    /// or if an error occurs during method invocation.</exception>
    public CoolObject Visit(DispatchNode node)
    {
        // 1. Evaluate the receiver (the object on which the method is called)
        CoolObject receiver = node.Caller.Accept(this)
                              ?? throw new CoolRuntimeException("Dispatch receiver evaluated to void");

        // 2. Evaluate all actual arguments
        CoolObject[] actualArgs = node.Arguments
            .Select(arg => arg.Accept(this)
                           ?? throw new CoolRuntimeException("Argument evaluated to void"))
            .ToArray();

        // 3. Determine the starting class for method lookup
        CoolClass lookupStartClass = node.StaticTypeName is null
            ? receiver.Class                                                  // Dynamic dispatch: start from runtime class
            : ResolveStaticDispatchClass(node.StaticTypeName);                 // Static dispatch: use explicit type

        // 4. Find the method in the inheritance chain
        MethodNode? method = FindMethodInHierarchy(lookupStartClass, node.MethodName, actualArgs.Length)
                             ?? throw new CoolRuntimeException(
                                 $"Method '{node.MethodName}' with {actualArgs.Length} argument(s) not found in class '{lookupStartClass.Name}' or its ancestors");

        // 5. Build the activation frame:
        //    - self = receiver
        //    - bind formals to actual arguments
        Environment activationFrame = Environment.Empty.WithSelf(receiver);

        for (int i = 0; i < method.Formals.Count; i++)
        {
            string formalName = method.Formals[i].Name;
            CoolObject argValue = actualArgs[i];
            activationFrame = activationFrame.WithLocal(formalName, argValue);
        }

        // 6. Execute the method body in the new frame
        return StartEvaluation(method.Body, activationFrame);
    }

    /// <summary>
    /// Evaluates the given <see cref="IfNode"/> by processing its predicate condition and
    /// executing the appropriate branch based on the result of the evaluation.
    /// </summary>
    /// <param name="node">The <see cref="IfNode"/> containing the predicate expression, then-branch, and else-branch.</param>
    /// <returns>A <see cref="CoolObject"/> resulting from the execution of either the then-branch or the else-branch,
    /// depending on the evaluation of the predicate condition.</returns>
    /// <exception cref="CoolRuntimeException">Thrown if the predicate condition does not evaluate to a <see cref="CoolBool"/> object.</exception>
    public CoolObject Visit(IfNode node)
    {
        var cond = node.Predicate.Accept(this) as CoolBool
                   ?? throw new CoolRuntimeException("if predicate must be Bool");
        
        return cond.Value ? node.ThenBranch.Accept(this) : node.ElseBranch.Accept(this);
    }

    /// <summary>
    /// Evaluates a <see cref="WhileNode"/> by executing its body repeatedly as long as the predicate evaluates to a <see cref="CoolBool"/>
    /// with a true value, and halts when the predicate evaluates to false.
    /// </summary>
    /// <param name="node">The <see cref="WhileNode"/> containing the predicate and body for the while loop.</param>
    /// <returns>A <see cref="CoolVoid"/> object, as the while loop does not produce a meaningful return value.</returns>
    /// <exception cref="CoolRuntimeException">Thrown if the predicate of the <see cref="WhileNode"/> does not evaluate to a <see cref="CoolBool"/>.</exception>
    public CoolObject Visit(WhileNode node)
    {
        while (true)
        {
            var cond = node.Predicate.Accept(this) as CoolBool
                       ?? throw new CoolRuntimeException("while predicate must be Bool");
            
            if (!cond.Value) break;
            node.Body.Accept(this);
        }
        
        return CoolVoid.Value;
    }

    /// <summary>
    /// Evaluates the provided <see cref="BlockNode"/> by executing each expression in the block and returning the result of the last evaluated expression.
    /// </summary>
    /// <param name="node">The <see cref="BlockNode"/> containing the sequence of expressions to be evaluated.</param>
    /// <returns>A <see cref="CoolObject"/> representing the result of evaluating the last expression in the block.</returns>
    public CoolObject Visit(BlockNode node)
    {
        CoolObject last = CoolVoid.Value;

        foreach (var e in node.Expressions)
            last = e.Accept(this);
        
        return last;
    }

    /// <summary>
    /// Evaluates the given <see cref="LetNode"/> by processing its bindings and evaluating its body in the updated environment.
    /// </summary>
    /// <param name="node">The <see cref="LetNode"/> containing the variable bindings and the body of the let expression to evaluate.</param>
    /// <returns>A <see cref="CoolObject"/> representing the result of evaluating the body of the let expression within the context of the updated environment.</returns>
    public CoolObject Visit(LetNode node)
    {
        var frame = _env;

        foreach (var b in node.Bindings)
        {
            var value = b.Initializer is null
                ? DefaultValue(b.TypeName)
                : b.Initializer.Accept(this);
            frame = frame.WithLocal(b.Identifier, value);
        }
        
        return StartEvaluation(node.Body, frame);
    }

    /// <summary>
    /// Evaluates the given <see cref="CaseNode"/> by matching the provided scrutinee against
    /// the branches defined in the case expression and returning the result of the first matching branch.
    /// </summary>
    /// <param name="node">The <see cref="CaseNode"/> representing the case expression to evaluate.</param>
    /// <returns>A <see cref="CoolObject"/> resulting from the evaluation of the matching branch body.</returns>
    /// <exception cref="CoolRuntimeException">
    /// Thrown when no branch in the case expression matches the scrutinee.
    /// </exception>
    public CoolObject Visit(CaseNode node)
    {
        var value = node.Scrutinee.Accept(this);

        foreach (var branch in node.Branches)
        {
            if (Conforms(value.Class.Name, branch.TypeName, _runtime.SymbolTable))
                return StartEvaluation(branch.Body, _env.WithLocal(branch.Identifier, value));
        }
        
        throw new CoolRuntimeException("case: no matching branch");
    }

    /// <summary>
    /// Evaluates the given <see cref="IsVoidNode"/> by determining whether the expression it encapsulates evaluates to a <see cref="CoolVoid"/>.
    /// </summary>
    /// <param name="node">The <see cref="IsVoidNode"/> containing the expression to evaluate.</param>
    /// <returns>A <see cref="CoolBool"/> object that is <see cref="CoolBool.True"/> if the expression evaluates to a <see cref="CoolVoid"/>, otherwise <see cref="CoolBool.False"/>.</returns>
    public CoolObject Visit(IsVoidNode node) =>
        node.Expression.Accept(this) is CoolVoid ? CoolBool.True : CoolBool.False;

    /// <summary>
    /// Evaluates the specified <see cref="BinaryOperationNode"/> by performing the binary operation defined
    /// by the operator and operand values in the node, returning the result as a <see cref="CoolObject"/>.
    /// </summary>
    /// <param name="node">The <see cref="BinaryOperationNode"/> representing the binary operation to evaluate,
    /// including the operator and its left and right operands.</param>
    /// <returns>A <see cref="CoolObject"/> encapsulating the result of the binary operation.
    /// The result is determined based on the operation type and the operand values.</returns>
    /// <exception cref="CoolRuntimeException">Thrown if an invalid operator or operand combination is encountered,
    /// or if attempting to divide by zero during division operations.</exception>
    public CoolObject Visit(BinaryOperationNode node)
    {
        var l = node.Left.Accept(this);
        var r = node.Right.Accept(this);

        return (node.Operator, l, r) switch
        {
            (BinaryOperator.Plus,  CoolInt a, CoolInt b) => new CoolInt(a.Value + b.Value),
            
            (BinaryOperator.Minus, CoolInt a, CoolInt b) => new CoolInt(a.Value - b.Value),
            
            (BinaryOperator.Multiply, CoolInt a, CoolInt b) => new CoolInt(a.Value * b.Value),
            
            (BinaryOperator.Divide, CoolInt a, CoolInt b) =>
                b.Value == 0 ? throw new CoolRuntimeException("Division by zero")
                             : new CoolInt(a.Value / b.Value),
            
            (BinaryOperator.LessThan, CoolInt a, CoolInt b) => CoolBool.From(a.Value < b.Value),
            
            (BinaryOperator.LessThanOrEqual, CoolInt a, CoolInt b) => CoolBool.From(a.Value <= b.Value),
            
            (BinaryOperator.Equal, var a, var b) => CoolBool.From(RuntimeEquals(a, b)),
            
            _ => throw new CoolRuntimeException($"Invalid binary operator {node.Operator}")
        };
    }

    /// <summary>
    /// Processes a <see cref="UnaryOperationNode"/> by evaluating its operand and applying the specified unary operator.
    /// </summary>
    /// <param name="node">The <see cref="UnaryOperationNode"/> representing the unary operation to be evaluated.</param>
    /// <returns>A <see cref="CoolObject"/> that represents the result of applying the unary operator to the operand.</returns>
    /// <exception cref="CoolRuntimeException">
    /// Thrown when the operation is applied to an operand of an incompatible type.
    /// </exception>
    /// <exception cref="NotImplementedException">
    /// Thrown when the unary operator is not supported.
    /// </exception>
    public CoolObject Visit(UnaryOperationNode node)
    {
        var op = node.Operand.Accept(this);
        return node.Operator switch
        {
            UnaryOperator.Negate => op is CoolInt i
                ? new CoolInt(-i.Value)
                                   : throw new CoolRuntimeException("~ only on Int"),
            
            UnaryOperator.Not => op is CoolBool b ? CoolBool.From(!b.Value)
                                 : throw new CoolRuntimeException("not only on Bool"),
            
            _ => throw new NotImplementedException()
        };
    }

    public CoolObject Visit(BuiltinExpressionNode node)
    {
        var self = _env.Self;
        var args = node.Arguments.Select(a => a.Accept(this)).ToArray();

        return node.Name switch
        {
            "abort"      => throw new CoolRuntimeException("Object.abort()"),
            "type_name"  => new CoolString(self.Class.Name),
            "copy"       => self.Copy(),

            "out_string" when InheritsFromIo(self) && args.Length == 1 && args[0] is CoolString s =>
                _runtime.Io.OutString(s),

            "out_int" when InheritsFromIo(self) && args.Length == 1 && args[0] is CoolInt i =>
                _runtime.Io.OutInt(i),

            "in_string" when InheritsFromIo(self) => _runtime.Io.InString(),
            "in_int" when InheritsFromIo(self) => _runtime.Io.InInt(),

            "length" when self is CoolString s => new CoolInt(s.Value.Length),

            "concat" when self is CoolString s && args.Length == 1 && args[0] is CoolString o =>
                new CoolString(s.Value + o.Value),

            "substr" when self is CoolString s && args.Length == 2 =>
                Substr(s, args[0] as CoolInt, args[1] as CoolInt),

            _ => throw new CoolRuntimeException($"Unknown builtin {node.Name}")
        };
    }

    /// <summary>
    /// Checks whether the given CoolObject inherits from IO (directly or indirectly).
    /// </summary>
    private static bool InheritsFromIo(CoolObject obj)
    {
        if (obj is CoolIo) return true;
        var cls = obj.Class;
        while (cls != null)
        {
            if (cls.Name == "IO") return true;
            cls = cls.Parent;
        }
        return false;
    }

    // Declaration nodes – never reached at runtime
    public CoolObject Visit(ProgramNode _) => CoolVoid.Value;
    public CoolObject Visit(ClassNode _) => CoolVoid.Value;
    public CoolObject Visit(AttributeNode _) => CoolVoid.Value;
    public CoolObject Visit(MethodNode _) => CoolVoid.Value;
    public CoolObject Visit(FormalNode _) => CoolVoid.Value;
    public CoolObject Visit(FeatureNode _) => CoolVoid.Value;
    public CoolObject Visit(LetBindingNode _) => CoolVoid.Value;
    public CoolObject Visit(CaseBranchNode _) => CoolVoid.Value;

    /// <summary>
    /// Returns the default value for a given Cool type as a CoolObject.
    /// </summary>
    /// <param name="type">The name of the Cool type for which the default value is required.
    /// Valid options include "Int", "String", "Bool", and others.</param>
    /// <returns>The default CoolObject corresponding to the specified type.
    /// For "Int", it returns <see cref="CoolInt.Zero"/>; for "String", it returns <see cref="CoolString.Empty"/>;
    /// for "Bool", it returns <see cref="CoolBool.False"/>; for other types, it returns <see cref="CoolVoid.Value"/>.</returns>
    private static CoolObject DefaultValue(string type) => type switch
    {
        "Int" => CoolInt.Zero,
        "String" => CoolString.Empty,
        "Bool" => CoolBool.False,
        _ => CoolVoid.Value
    };

    /// <summary>
    /// Retrieves a substring from a given CoolString based on the specified start index and length.
    /// </summary>
    /// <param name="s">The CoolString from which the substring will be extracted.</param>
    /// <param name="i">The starting index for the substring, represented as a CoolInt.</param>
    /// <param name="len">The length of the substring to extract, represented as a CoolInt.</param>
    /// <returns>A new CoolString containing the specified substring.</returns>
    /// <exception cref="CoolRuntimeException">
    /// Thrown when the start index or length is invalid, or when the specified range exceeds the bounds of the original string.
    /// </exception>
    private static CoolString Substr(CoolString s, CoolInt i, CoolInt len)
    {
        if (i == null || len == null || i.Value < 0 || len.Value < 0 || i.Value + len.Value > s.Value.Length)
            throw new CoolRuntimeException("String.substr out of range");
        
        return new CoolString(s.Value.Substring(i.Value, len.Value));
    }

    /// <summary>
    /// Evaluates whether two CoolObject instances are equal based on their type and value.
    /// </summary>
    /// <param name="a">The first CoolObject instance to compare.</param>
    /// <param name="b">The second CoolObject instance to compare.</param>
    /// <returns>True if the two CoolObject instances are considered equal; otherwise, false.</returns>
    private static bool RuntimeEquals(CoolObject a, CoolObject b) =>
        a is CoolInt ai && b is CoolInt bi ? ai.Value == bi.Value :
        a is CoolString sa && b is CoolString sb ? sa.Value == sb.Value :
        a is CoolBool ba && b is CoolBool bb ? ba.Value == bb.Value :
        ReferenceEquals(a, b);
    
    /// <summary>
    /// Searches for a method by name and arity in the class hierarchy, starting from the given class
    /// and walking up the inheritance chain.
    /// </summary>
    /// <returns>The MethodNode if found; otherwise null.</returns>
    private MethodNode? FindMethodInHierarchy(CoolClass startClass, string methodName, int arity)
    {
        CoolClass? current = startClass;

        while (current is not null)
        {
            if (current.Methods.TryGetValue(methodName, out MethodNode? method) &&
                method.Formals.Count == arity)
            {
                return method;
            }

            current = current.Parent;
        }

        return null;
    }
    
    /// <summary>
    /// Resolves a class by name for static dispatch. Throws if the class is not found.
    /// </summary>
    private CoolClass ResolveStaticDispatchClass(string typeName)
    {
        var classSymbol = _runtime.SymbolTable.TryGetClass(typeName);
        if (classSymbol is null)
            throw new CoolRuntimeException($"Static dispatch type '{typeName}' not found");

        return RuntimeClassFactory.FromSymbol(classSymbol, _runtime);
    }

    /// <summary>
    /// Determines whether a class, represented by its name, conforms to another class in the inheritance hierarchy.
    /// </summary>
    /// <param name="actual">The name of the class to check for conformity.</param>
    /// <param name="expected">The name of the class to check against as the expected type.</param>
    /// <param name="st">The symbol table used to retrieve class information and evaluate inheritance relationships.</param>
    /// <returns>True if the actual class conforms to the expected class; otherwise, false.</returns>
    private static bool Conforms(string actual, string expected, SymbolTable st)
    {
        if (actual == expected) return true;
        var cls = st.GetClass(actual);
        while (cls?.ParentName is not null)
        {
            cls = st.GetClass(cls.ParentName);
            if (cls?.Name == expected) return true;
        }
        return false;
    }
}