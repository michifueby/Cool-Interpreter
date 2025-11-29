//-----------------------------------------------------------------------
// <copyright file="PredefinedClasses.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>PredefinedClasses</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax.Ast;

/// <summary>
/// Provides a collection of predefined classes in the Cool programming language.
/// These classes include core language components such as `Object`, `Int`, `String`, `Bool`, and `IO`.
/// The predefined classes serve as the foundational building blocks for the Cool language,
/// offering a base hierarchy and common functionality for user-defined classes.
/// </summary>
public static class PredefinedClasses
{
    /// <summary>
    /// Represents the base predefined class `Object` in the Cool programming language.
    /// This class serves as the root of the class hierarchy in Cool, providing default behavior
    /// and acting as the parent class for all other classes unless explicitly specified otherwise.
    /// </summary>
    public static readonly CoolClass Object = new("Object", null,
        ImmutableDictionary<string, MethodNode>.Empty,
        ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined class `Int` in the Cool programming language.
    /// This class is used for handling integer values and provides basic functionality
    /// for numerical operations within the language. It inherits from the `Object` class,
    /// thereby integrating into the core object hierarchy of Cool.
    /// </summary>
    public static readonly CoolClass Int = new("Int", Object,
        ImmutableDictionary<string, MethodNode>.Empty,
        ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined class `String` in the Cool programming language.
    /// This class provides functionality for handling sequences of characters
    /// and includes operations such as string manipulation and comparison.
    /// It inherits common behavior from the `Object` class and acts as the default
    /// type for string-related operations.
    /// </summary>
    public static readonly CoolClass String = new("String", Object,
        ImmutableDictionary<string, MethodNode>.Empty,
        ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined class `Bool` in the Cool programming language.
    /// This class is a subclass of `Object` and is used to encapsulate the boolean values
    /// true and false, providing basic behavior and structure for boolean operations in the language.
    /// </summary>
    public static readonly CoolClass Bool = new("Bool", Object,
        ImmutableDictionary<string, MethodNode>.Empty,
        ImmutableDictionary<string, AttributeNode>.Empty);

    /// <summary>
    /// Represents the predefined class `IO` in the Cool programming language.
    /// This class provides methods for input and output operations, enabling interaction
    /// with external sources such as console or files. It inherits default behavior from
    /// the `Object` class and expands its functionality with specialized input/output features.
    /// </summary>
    public static readonly CoolClass Io = new("Io", Object,
        ImmutableDictionary<string, MethodNode>.Empty,
        ImmutableDictionary<string, AttributeNode>.Empty);
}