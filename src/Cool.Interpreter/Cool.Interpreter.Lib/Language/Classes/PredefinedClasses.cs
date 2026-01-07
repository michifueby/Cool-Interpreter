//-----------------------------------------------------------------------
// <copyright file="PredefinedClasses.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>PredefinedClasses</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes;

using System.Collections.Immutable;
using Core.Syntax.Ast;
using Core.Syntax;
using Core.Syntax.Ast.Expressions;
using Core.Syntax.Ast.Features;

/// <summary>
/// Provides a collection of predefined classes in the Cool programming language.
/// These classes include core language components such as `Object`, `Int`, `String`, `Bool`, and `IO`.
/// The predefined classes serve as the foundational building blocks for the Cool language,
/// offering a base hierarchy and common functionality for user-defined classes.
/// </summary>
/// <summary>
/// Contains the five predefined basic classes of the Cool language.
/// These classes are part of the language itself – they always exist, cannot be redefined,
/// and are known at compile-time. This static class follows the best-practice pattern
/// used by Roslyn, Java, and all serious compiler implementations.
/// </summary>
public static class PredefinedClasses
{
    /// <summary>
    /// Represents the root object class in the inheritance hierarchy of the Cool language's predefined classes.
    /// </summary>
    public static readonly CoolClass Object = new(
        name: "Object",
        parent: null,
        methods: CreateObjectMethods(),
        attributes: ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined integer class in the Cool language,
    /// used to model integer values and their associated operations.
    /// </summary>
    public static readonly CoolClass Int = new(
        name: "Int",
        parent: Object,
        methods: ImmutableDictionary<string, MethodNode>.Empty,
        attributes: ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined String class in the Cool language, which inherits from the Object class.
    /// Provides methods for string manipulation, such as calculating the length, concatenating strings,
    /// and extracting substrings.
    /// </summary>
    public static readonly CoolClass String = new(
        name: "String",
        parent: Object,
        methods: CreateStringMethods(),
        attributes: ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined Boolean class in the Cool language.
    /// This class is used to define values that are either true or false,
    /// enabling conditional logic and Boolean operations within the language.
    /// </summary>
    public static readonly CoolClass Bool = new(
        name: "Bool",
        parent: Object,
        methods: ImmutableDictionary<string, MethodNode>.Empty,
        attributes: ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined IO class in the Cool language, which provides methods
    /// for basic input and output operations.
    /// </summary>
    public static readonly CoolClass Io = new(
        name: "IO",
        parent: Object,
        methods: CreateIoMethods(),
        attributes: ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Provides a read-only mapping of predefined class names to their corresponding <see cref="CoolClass"/> instances.
    /// </summary>
    public static IReadOnlyDictionary<string, CoolClass> ByName { get; } = // O(1) lookup – heavily used in semantic analysis and interpreter
        new Dictionary<string, CoolClass>(StringComparer.Ordinal)
        {
            ["Object"] = Object,
            ["Int"]    = Int,
            ["String"] = String,
            ["Bool"]   = Bool,
            ["IO"]     = Io
        }.AsReadOnly();

    /// <summary>
    /// Creates and returns a collection of predefined methods for the `Object` class in the Cool language.
    /// These methods serve as built-in functionalities for the `Object` class, which is the root of all
    /// class hierarchies in Cool. Predefined methods include:
    /// - `abort`: Halts the execution of the program.
    /// - `type_name`: Returns the name of the class as a string.
    /// - `copy`: Creates a shallow copy of the object.
    /// </summary>
    /// <returns>
    /// An immutable dictionary where the keys are the names of the predefined methods
    /// and the values are their corresponding `MethodNode` representations.
    /// </returns>
    private static ImmutableDictionary<string, MethodNode> CreateObjectMethods() =>
        ImmutableDictionary<string, MethodNode>.Empty
            .Add("abort",     BuiltinMethod("abort",     "Object"))
            .Add("type_name", BuiltinMethod("type_name", "String"))
            .Add("copy",      BuiltinMethod("copy",      "SELF_TYPE"));

    /// <summary>
    /// Creates and returns a collection of predefined methods for the `String` class in the Cool language.
    /// These methods provide essential string manipulation functionalities. The predefined methods include:
    /// - `length`: Returns the length of the string as an `Int`.
    /// - `concat`: Concatenates the string with another string and returns the resulting `String`.
    /// - `substr`: Extracts a substring from the string starting at a specified index and with a specified length, returning the resultant `String`.
    /// </summary>
    /// <returns>
    /// An immutable dictionary where the keys are the names of the predefined methods
    /// and the values are their corresponding `MethodNode` representations.
    /// </returns>
    private static ImmutableDictionary<string, MethodNode> CreateStringMethods() =>
        ImmutableDictionary<string, MethodNode>.Empty
            .Add("length", BuiltinMethod("length", "Int"))
            .Add("concat", BuiltinMethod("concat", "String", ("s", "String")))
            .Add("substr", BuiltinMethod("substr", "String", ("i", "Int"), ("l", "Int")));

    /// <summary>
    /// Creates and returns a collection of predefined methods for the `IO` class in the Cool language.
    /// The `IO` class provides methods for performing basic input/output operations such as reading
    /// and writing strings and integers. Predefined methods include:
    /// - `out_string`: Outputs a string to the standard output.
    /// - `out_int`: Outputs an integer to the standard output.
    /// - `in_string`: Reads a string input from the standard input.
    /// - `in_int`: Reads an integer input from the standard input.
    /// </summary>
    /// <returns>
    /// An immutable dictionary where the keys are the names of the predefined methods
    /// and the values are their corresponding `MethodNode` representations.
    /// </returns>
    private static ImmutableDictionary<string, MethodNode> CreateIoMethods() =>
        ImmutableDictionary<string, MethodNode>.Empty
            .Add("out_string", BuiltinMethod("out_string", "SELF_TYPE", ("x", "String")))
            .Add("out_int",    BuiltinMethod("out_int",    "SELF_TYPE", ("x", "Int")))
            .Add("in_string",  BuiltinMethod("in_string",  "String"))
            .Add("in_int",     BuiltinMethod("in_int",     "Int"));

    /// <summary>
    /// Creates a predefined method for a class in the Cool language. The method is represented by its
    /// name, return type, and a collection of formal parameters. The returned method node contains
    /// a body that the interpreter recognizes as a built-in functionality.
    /// </summary>
    /// <param name="name">
    /// The name of the predefined method (e.g., "abort", "type_name").
    /// </param>
    /// <param name="returnType">
    /// The return type of the method (e.g., "String", "Int").
    /// </param>
    /// <param name="parameters">
    /// A collection of parameters where each parameter is specified as a tuple containing
    /// the parameter's name and type (e.g., ("s", "String")).
    /// </param>
    /// <returns>
    /// A `MethodNode` representing the predefined method with its associated name, return type, parameters,
    /// and built-in functionality body.
    /// </returns>
    private static MethodNode BuiltinMethod(
        string name,
        string returnType,
        params (string Name, string Type)[] parameters)
    {
        var formals = parameters
            .Select(p => new FormalNode(p.Name, p.Type, SourcePosition.None))
            .ToImmutableArray();

        // The body is a BuiltinExpressionNode that carries:
        //   • the builtin name
        //   • references to the formal parameters as arguments
        var arguments = formals
            .Select(f => new IdentifierExpressionNode(f.Name, SourcePosition.None))
            .ToArray();

        var body = new BuiltinExpressionNode(name, arguments, SourcePosition.None);

        return new MethodNode(name, formals, returnType, body, SourcePosition.None);
    }
}