//-----------------------------------------------------------------------
// <copyright file="CoolVoid.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolVoid</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

/// <summary>
/// Represents the singleton instance of the "void" value in the Cool programming language.
/// This class is used to express the absence of value, similar to the concept of null or Unit in some languages.
/// It provides a default textual representation of "void" and is used internally in language feature implementations.
/// </summary>
public class CoolVoid : CoolObject
{
    /// <summary>
    /// Represents the singleton instance of the "void" value in the Cool programming language.
    /// This is a special class inheriting from <see cref="CoolObject"/>, designed to express the absence of a value.
    /// The "void" instance acts as a placeholder for operations where no value is intended to exist,
    /// similar to null in other programming languages.
    /// </summary>
    public CoolVoid() : base(PredefinedClasses.Object) { }

    /// <summary>
    /// Represents a singleton instance of the "void" value in the Cool programming language.
    /// This static field is used to signify the absence of a value, similar to null or Unit in other languages.
    /// It ensures a consistent representation of "void" across the language's implementation and evaluations.
    /// </summary>
    public static readonly CoolVoid Value = new();

    /// <summary>
    /// Provides the string representation of the "void" value in the Cool programming language.
    /// This overrides the base implementation to always return the literal "void",
    /// reflecting the absence of a value.
    /// </summary>
    /// <returns>A string representing the "void" value, which is always "void".</returns>
    public override string AsString() 
        => "void";

    /// <summary>
    /// Returns a string representation of the current CoolVoid instance.
    /// This override provides a textual description of "void," which represents the absence of a value in the Cool programming language.
    /// </summary>
    /// <returns>A string literal "void".</returns>
    public override string ToString() 
        => "void";
}