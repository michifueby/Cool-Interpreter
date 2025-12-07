//-----------------------------------------------------------------------
// <copyright file="CoolInt.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolInt</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

/// <summary>
/// Represents an integer type in the Cool programming language.
/// The `CoolInt` class wraps an integer value and extends the `CoolObject` base class,
/// thereby allowing it to integrate seamlessly within the Cool runtime environment.
/// This class provides functionality to convert the wrapped integer value to a string,
/// compare two `CoolInt` objects for equality, and access its integer value.
/// </summary>
public class CoolInt : CoolObject
{
    /// <summary>
    /// Represents a built-in class for the 'Int' type in the Cool programming language.
    /// </summary>
    public CoolInt(int value) : base(PredefinedClasses.Int) => Value = value;

    /// <summary>
    /// Gets the integer value associated with the current instance of the <see cref="CoolInt"/> class.
    /// This property represents the underlying value of the 'Int' type in the Cool programming language.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Converts the encapsulated integer value to its string representation.
    /// </summary>
    /// <returns>
    /// A string that represents the integer value stored in the current instance of CoolInt.
    /// </returns>
    public override string AsString() => Value.ToString();

    /// <summary>
    /// Converts the wrapped integer value contained within the `CoolInt` instance to its string representation.
    /// </summary>
    /// <returns>
    /// A string representation of the integer value contained in this `CoolInt` instance.
    /// </returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current `CoolInt` instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current `CoolInt` instance.</param>
    /// <returns>
    /// `true` if the specified object is a `CoolInt` and its wrapped integer value is equal to
    /// the wrapped integer value of the current instance; otherwise, `false`.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is CoolInt i && Value == i.Value; // Int uses value equality (spec: equality on basic types is value-based)

    /// <summary>
    /// Returns a hash code for the current CoolInt instance.
    /// The hash code is derived from the integer value contained in the CoolInt object.
    /// </summary>
    /// <returns>An integer representing the hash code of the CoolInt instance.</returns>
    public override int GetHashCode() => Value.GetHashCode();
}