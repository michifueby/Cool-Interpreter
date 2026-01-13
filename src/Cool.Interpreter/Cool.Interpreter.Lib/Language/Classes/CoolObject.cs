//-----------------------------------------------------------------------
// <copyright file="CoolObject.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolObject</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes;

using Core.Exceptions;
using BuiltIn;

/// <summary>
/// Base class for all Cool runtime values. In Cool, *everything is an object* —
/// even integers, booleans, strings, and null.
/// </summary>
public abstract class CoolObject
{
    /// <summary>
    /// Represents the runtime class of a Cool object.
    /// This property holds the CoolClass instance associated with the object,
    /// which defines its structure, methods, and behavior.
    /// </summary>
    public CoolClass Class { get; }

    /// <summary>
    /// Represents the base abstract class for all objects in the Cool language.
    /// Provides a foundation for objects with mandatory association to their class,
    /// and the capability to handle runtime behavior such as aborting execution.
    /// </summary>
    protected CoolObject(CoolClass @class)
        => Class = @class ?? throw new ArgumentNullException(nameof(@class));

    /// <summary>
    /// Terminates the current execution and raises an exception to indicate a runtime error.
    /// This method is intended to explicitly stop the program when the operation cannot continue.
    /// </summary>
    /// <returns>Does not return a value as it always throws an exception.</returns>
    /// <exception cref="CoolRuntimeException">Thrown to signal an abnormal termination of the program.</exception>
    public virtual CoolObject Abort() 
        => throw new CoolRuntimeException("abort() called");

    /// <summary>
    /// Returns the name of the class to which the current object belongs.
    /// This method provides the runtime type information for the object
    /// in the form of a string representing its class name.
    /// </summary>
    /// <returns>
    /// A <see cref="CoolString"/> containing the name of the object's class.
    /// </returns>
    public virtual CoolString TypeName() 
        => new CoolString(Class.Name);

    /// <summary>
    /// Creates a shallow copy of the current object.
    /// </summary>
    /// <returns>
    /// Returns the current instance of the object.
    /// </returns>
    public virtual CoolObject Copy() 
        => this; // shallow copy by default

    /// <summary>
    /// Converts the current instance to its string representation.
    /// This method provides a textual description of the object instance,
    /// which is intended to be implementation-specific for derived classes.
    /// </summary>
    /// <returns>A string representation of the object.</returns>
    public virtual string AsString() 
        => ToString();

    /// <summary>
    /// Returns a string representation of the object, providing a meaningful description
    /// based on the specific implementation in each derived class.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() 
        => $"<{Class.Name} object>";
}