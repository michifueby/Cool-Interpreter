//-----------------------------------------------------------------------
// <copyright file="CoolErrorCodes.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolErrorCodes</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Diagnostics;

/// <summary>
/// Provides a collection of error codes used for diagnostic purposes
/// within the Cool programming language interpreter.
/// </summary>
public static class CoolErrorCodes
{
    /// <summary>
    /// Represents the error code "COOL0101" used to indicate
    /// that a class with the same name has been defined more than once
    /// within the same context in the Cool programming language.
    /// </summary>
    public const string DuplicateClass = "COOL0101";

    /// <summary>
    /// Represents the error code "COOL0102" used to indicate
    /// an attempt to redefine a built-in class in the Cool programming language,
    /// which is not allowed by the language specifications.
    /// </summary>
    public const string RedefineBuiltin = "COOL0102";

    /// <summary>
    /// Represents the error code "COOL0103" used to indicate
    /// that a required class named "Main" is missing in the program.
    /// </summary>
    public const string MissingMain = "COOL0103";

    /// <summary>
    /// Represents the error code "COOL0104" indicating that a class in the Cool programming language
    /// attempts to inherit from a primitive type that is not allowed to act as a parent, such as
    /// "Int", "String", or other similar built-in types.
    /// </summary>
    public const string InheritFromPrimitive = "COOL0104";

    /// <summary>
    /// Represents the error code "COOL0105" used to indicate
    /// that a class inherits from an undefined parent class
    /// in the Cool programming language.
    /// </summary>
    public const string UndefinedParent = "COOL0105";

    /// <summary>
    /// Represents the error code "COOL0106" used to indicate
    /// the detection of an inheritance cycle within the class hierarchy
    /// in the Cool programming language.
    /// </summary>
    public const string InheritanceCycle = "COOL0106";

    /// <summary>
    /// Represents the error code "COOL0201" used to indicate
    /// that an attribute's initializer has a type mismatch, where
    /// the declared type of the attribute differs from the type
    /// of the initializer expression in the Cool programming language.
    /// </summary>
    public const string TypeMismatchInAttributeInit = "COOL0201"; // Attribute 'x' : T, but initializer has type S

    /// <summary>
    /// Represents the error code "COOL0202" used to indicate
    /// a mismatch between the declared return type of a method
    /// and the actual type of the value returned by its body
    /// in the Cool programming language.
    /// </summary>
    public const string MethodReturnTypeMismatch = "COOL0202"; // Method 'f' should return T, but body has type S

    /// <summary>
    /// Represents the error code "COOL0210" used to indicate
    /// that a value of one type is being assigned to a variable
    /// of an incompatible or incorrect type in the Cool programming language.
    /// </summary>
    public const string AssignToWrongType = "COOL0210"; // Cannot assign type S to variable of type T

    /// <summary>
    /// Represents the error code "COOL0211" used to indicate
    /// that an identifier is being used without being defined
    /// in the current context in the Cool programming language.
    /// </summary>
    public const string UndefinedVariable = "COOL0211"; // Undefined identifier 'x'

    /// <summary>
    /// Represents the error code "COOL0212" used to indicate
    /// that a method with the specified name is not defined
    /// within the declared class or its ancestors in the Cool programming language.
    /// </summary>
    public const string UndefinedMethod = "COOL0212"; // Method 'f' not found in class T

    /// <summary>
    /// Represents the error code "COOL0213" used to indicate
    /// that a method was invoked with an incorrect number of arguments,
    /// where the method expected a specific number but received a different count.
    /// </summary>
    public const string WrongNumberOfArguments = "COOL0213"; // Method 'f' expects N args, got M

    /// <summary>
    /// Represents the error code "COOL0214" used to indicate
    /// that an argument provided to a method does not match the expected type,
    /// specifying the argument index, expected type, and received type.
    /// </summary>
    public const string ArgumentTypeMismatch = "COOL0214"; // Argument N: expected T, got S

    /// <summary>
    /// Represents the error code "COOL0220" which indicates that an "if" statement
    /// has a predicate that is not of type Bool in the Cool programming language.
    /// </summary>
    public const string IfPredicateNotBool = "COOL0220"; // If predicate must be Bool, got T

    /// <summary>
    /// Represents the error code "COOL0221" used to indicate that the predicate
    /// in a while loop must evaluate to a boolean type, but an incompatible type
    /// was provided instead in the context of the Cool programming language.
    /// </summary>
    public const string WhilePredicateNotBool = "COOL0221"; // While predicate must be Bool, got T

    /// <summary>
    /// Represents the error code "COOL0230" used to indicate
    /// a type mismatch in the initialization of a variable
    /// within a "let" binding in the Cool programming language.
    /// </summary>
    public const string LetBindingTypeMismatch = "COOL0230"; // Let binding 'x' : T initialized with S

    /// <summary>
    /// Represents the error code "COOL0231" used to indicate
    /// that no branch in a case expression matches the type
    /// of the scrutinee in the Cool programming language.
    /// </summary>
    public const string CaseNoBranchMatches = "COOL0231"; // No case branch matches scrutinee type (future)

    /// <summary>
    /// Represents the error code "COOL0240" used to indicate an invalid or unsupported binary operation
    /// between two types in the Cool programming language, such as when an operator is applied to incompatible types.
    /// </summary>
    public const string InvalidBinaryOperation = "COOL0240"; // Operator + not supported between T and S

    /// <summary>
    /// Represents the error code "COOL0241" used to indicate
    /// that an invalid unary operation has been attempted, such as
    /// applying an operator that is not applicable to the given type
    /// in the Cool programming language.
    /// </summary>
    public const string InvalidUnaryOperation = "COOL0241"; // Operator ~ not applicable to T

    /// <summary>
    /// Represents the error code "COOL0250" used to indicate
    /// that a static dispatch is attempted with a receiver whose type
    /// is incompatible with the specified static type in the Cool programming language.
    /// </summary>
    public const string StaticDispatchTypeError = "COOL0250"; // Receiver of type T cannot be used as static type S

    /// <summary>
    /// Represents the error code "COOL0260" used to indicate
    /// the use of an undefined type within the Cool programming language.
    /// </summary>
    public const string UndefinedType = "COOL0260"; // Use of undefined type 'T'

    /// <summary>
    /// Represents the error code "COOL0261" used to indicate
    /// that the SELF_TYPE construct has been misused outside of a valid class context
    /// in the Cool programming language.
    /// </summary>
    public const string SelfTypeMisuse = "COOL0261"; // Invalid use of SELF_TYPE outside class context
    
    /// <summary>
    /// Runtime error: division by zero occurred.
    /// </summary>
    public const string DivisionByZero = "COOL0301";

    /// <summary>
    /// Runtime error: substr() called with invalid index or length.
    /// </summary>
    public const string SubstrOutOfRange = "COOL0302";

    /// <summary>
    /// Runtime error: abort() was explicitly called.
    /// </summary>
    public const string AbortCalled = "COOL0303";

    /// <summary>
    /// Runtime error: attribute access on non-object (e.g. Int.x).
    /// </summary>
    public const string AttributeAccessOnPrimitive = "COOL0304";

    /// <summary>
    /// Runtime error: assignment to a non-existent attribute.
    /// </summary>
    public const string AssignToUndefinedAttribute = "COOL0305";

    /// <summary>
    /// Runtime error: dispatch on void (e.g. x.f() where x is void).
    /// </summary>
    public const string DispatchOnVoid = "COOL0306";

    /// <summary>
    /// Runtime error: internal interpreter bug — should never happen in correct programs.
    /// </summary>
    public const string InternalInterpreterError = "COOL0399";

    /// <summary>
    /// Catch-all runtime error for unexpected exceptions during evaluation.
    /// Used when a user exception (e.g. from CoolRuntimeException) has no specific code.
    /// </summary>
    public const string RuntimeError = "COOL0398";
}