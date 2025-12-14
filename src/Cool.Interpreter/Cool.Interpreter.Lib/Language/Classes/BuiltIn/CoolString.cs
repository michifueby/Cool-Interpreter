//-----------------------------------------------------------------------
// <copyright file="CoolString.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolString</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

using Cool.Interpreter.Lib.Core.Exeptions;

/// <summary>
/// Represents a string object in the Cool programming language.
/// This class provides operations for basic string manipulation
/// such as concatenation, substring extraction, and length retrieval.
/// </summary>
public class CoolString : CoolObject
{
    /// <summary>
    /// Represents a string in the Cool Programming Language. This class extends the functionality of
    /// the base CoolObject. Provides methods for string manipulation, including length calculation,
    /// concatenation, and substring extraction.
    /// </summary>
    public CoolString(string value) : base(PredefinedClasses.String)
        => Value = value ?? "";
    
    /// <summary>
    /// Gets the underlying string value of the CoolString object.
    /// This property provides access to the raw string data encapsulated
    /// within the CoolString instance.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Retrieves the length of the current string instance. This method
    /// calculates the number of characters in the string and returns it
    /// as a `CoolInt` object.
    /// </summary>
    /// <returns>A `CoolInt` representing the number of characters in the string.</returns>
    public CoolInt Length() 
        => new(Value.Length);

    /// <summary>
    /// Concatenates the current string with another CoolString instance and returns a new CoolString
    /// containing the combined value.
    /// </summary>
    /// <param name="other">The other CoolString instance to concatenate with the current string.</param>
    /// <returns>A CoolString instance representing the concatenated result of the current string and the input string.</returns>
    public CoolString Concat(CoolString other) 
        => new(Value + other.Value);

    /// <summary>
    /// Extracts a substring from the current CoolString instance.
    /// The substring starts at the specified index and has the specified length.
    /// </summary>
    /// <param name="start">The zero-based starting index of the substring. Must be non-negative.</param>
    /// <param name="len">The length of the substring to extract. Must be non-negative.</param>
    /// <returns>A new CoolString instance containing the extracted substring.</returns>
    /// <exception cref="CoolRuntimeException">
    /// Thrown if the start index or length is out of bounds, or if the start index and length
    /// exceed the bounds of the string.
    /// </exception>
    public CoolString Substr(CoolInt start, CoolInt len)
    {
        if (start.Value < 0 || len.Value < 0 || start.Value + len.Value > Value.Length)
            throw new CoolRuntimeException("String.substr() out of range");
        
        return new CoolString(Value.Substring(start.Value, len.Value));
    }

    /// <summary>
    /// Converts the current CoolString object into its corresponding string representation.
    /// This method overrides the base implementation to return the string value stored
    /// within the CoolString instance.
    /// </summary>
    /// <returns>The string value of the current CoolString object.</returns>
    public override string AsString() 
        => Value;

    /// <summary>
    /// Returns the string representation of the CoolString object. This representation
    /// includes the string value enclosed in double quotes, with special characters (e.g.,
    /// backslashes, double quotes, newlines, tabs) properly escaped.
    /// </summary>
    /// <returns>
    /// A string containing the CoolString value in its escaped and quoted form.
    /// </returns>
    public override string ToString() 
        => $"\"{Escape(Value)}\"";

    /// <summary>
    /// Determines whether the specified object is equal to the current CoolString instance.
    /// This method performs a value-based comparison to check if the string values are identical.
    /// </summary>
    /// <param name="obj">The object to compare with the current CoolString instance.</param>
    /// <returns>
    /// Returns <c>true</c> if the specified object is a CoolString and has the same string value;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) 
        => obj is CoolString s && Value == s.Value;

    /// <summary>
    /// Computes and returns the hash code for the CoolString instance.
    /// The hash code is derived from the underlying string value, ensuring
    /// that equal string objects have the same hash code.
    /// </summary>
    /// <returns>
    /// An integer representing the hash code of the CoolString instance.
    /// </returns>
    public override int GetHashCode() 
        => Value.GetHashCode();

    /// <summary>
    /// Represents an empty string instance in the Cool programming language.
    /// This static readonly member provides a reusable, predefined representation
    /// of an empty <see cref="CoolString"/> object.
    /// </summary>
    public static readonly CoolString Empty = new("");

    /// <summary>
    /// Creates a new instance of <see cref="CoolString"/> from the provided value.
    /// If the input value is null or empty, returns the predefined <see cref="Empty"/> instance.
    /// </summary>
    /// <param name="value">The input string value to create the <see cref="CoolString"/> instance from.</param>
    /// <returns>A <see cref="CoolString"/> instance representing the given string value.</returns>
    public static CoolString From(string value) =>
        string.IsNullOrEmpty(value) ? Empty : new CoolString(value);

    /// <summary>
    /// Escapes special characters in a string for proper string representation in the Cool programming language.
    /// Converts characters such as backslashes, double quotes, newlines, and tabs into their escaped forms.
    /// </summary>
    /// <param name="s">The string to process for escaping special characters.</param>
    /// <returns>A new string with special characters replaced by their escaped equivalents.</returns>
    private static string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t");
}