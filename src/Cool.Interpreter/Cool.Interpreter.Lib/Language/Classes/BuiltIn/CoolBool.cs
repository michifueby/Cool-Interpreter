//-----------------------------------------------------------------------
// <copyright file="CoolBool.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolBool</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

/// <summary>
/// Represents the boolean type within the Cool programming language runtime.
/// This class provides the ability to encapsulate and manipulate boolean values
/// as objects, consistent with the design principle that everything in Cool
/// is an object.
/// </summary>
/// <remarks>
/// The CoolBool class includes predefined instances for the boolean values
/// `true` and `false`. These instances are used to ensure consistency
/// and memory efficiency when dealing with boolean operations in the runtime.
/// It includes methods to provide consistent string representations and
/// equality comparisons for boolean values.
/// </remarks>
public class CoolBool : CoolObject
{
    /// <summary>
    /// Represents a "Bool" type object in the Cool language, which is a wrapper around a boolean value.
    /// </summary>
    public CoolBool(bool value) : base(PredefinedClasses.Bool) => Value = value;

    /// <summary>
    /// Gets the boolean value encapsulated within the current CoolBool instance.
    /// </summary>
    /// <remarks>
    /// This property provides access to the underlying primitive boolean value
    /// of the CoolBool object, allowing operations or comparisons consistent
    /// with the Cool language runtime semantics.
    /// The Value property is read-only and is set at the time of object construction.
    /// Use this property to retrieve the actual boolean state represented by
    /// the CoolBool instance.
    /// </remarks>
    public bool Value { get; }

    /// <summary>
    /// Represents the predefined boolean constant `true` for the Cool language runtime.
    /// </summary>
    /// <remarks>
    /// This static, read-only field provides a singleton instance of the `CoolBool` class
    /// representing the boolean value `true`. It is intended to be used wherever a `true`
    /// boolean literal is needed in the context of the Cool language. This avoids repeated
    /// object construction and ensures consistent representation of the `true` value.
    /// </remarks>
    public static readonly CoolBool True  = new(true);

    /// <summary>
    /// Represents the predefined constant False for the CoolBool type.
    /// </summary>
    /// <remarks>
    /// This static field is used to denote the boolean value `false` within
    /// the Cool language runtime. It serves as the canonical instance
    /// representing the `false` state for objects of type CoolBool.
    /// Use this field to express the `false` literal value in scenarios involving
    /// comparisons, logical operations, or default value assignments
    /// associated with CoolBool instances.
    /// </remarks>
    public static readonly CoolBool False = new(false);

    /// <summary>
    /// Converts the boolean value represented by this CoolBool instance into its string representation ("true" or "false").
    /// </summary>
    /// <returns>A string representation of the boolean value.</returns>
    public override string AsString() => Value ? "true" : "false";

    /// <summary>
    /// Returns a string representation of the boolean value wrapped by this CoolBool instance.
    /// </summary>
    /// <returns>
    /// A string "true" if the value is true; otherwise, "false".
    /// </returns>
    public override string ToString() => AsString();

    /// <summary>
    /// Determines whether the specified object is equal to the current CoolBool instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current CoolBool instance.</param>
    /// <returns>
    /// true if the specified object is a CoolBool with the same boolean value; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj) => obj is CoolBool b && Value == b.Value;

    /// <summary>
    /// Returns the hash code for the current instance based on its encapsulated boolean value.
    /// </summary>
    /// <returns>
    /// An integer representing the hash code of the boolean value encapsulated by this CoolBool instance.
    /// </returns>
    public override int GetHashCode() => Value.GetHashCode();
}