//-----------------------------------------------------------------------
// <copyright file="Environment.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Environment</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using System.Collections.Immutable;
using Core.Exceptions;
using Classes;
using Classes.BuiltIn;

/// <summary>
/// Immutable execution environment (frame) for Cool evaluation.
/// Contains the current 'self' object and local variables (let, parameters, etc.).
/// Thread-safe, pure functional, and designed for high-performance evaluation.
/// </summary>
public class Environment
{
    /// <summary>
    /// Represents an immutable execution environment (frame) for the Cool
    /// programming language evaluation. This environment stores the current
    /// 'self' object and a collection of local variables, enabling thread-safe
    /// and high-performance evaluation in a pure functional manner.
    /// </summary>
    public Environment(CoolObject self, ImmutableDictionary<string, CoolObject> locals)
    {
        Self = self;
        Locals = locals;
    }
    
    /// <summary>
    /// The current 'self' object. Never null.
    /// </summary>
    public CoolObject Self { get; }

    /// <summary>
    /// Local variables: let bindings, method parameters, temporaries.
    /// Never null — defaults to empty dictionary.
    /// </summary>
    public ImmutableDictionary<string, CoolObject> Locals { get; }

    /// <summary>
    /// Represents an empty instance of the environment with no local variables
    /// and a self-reference set to a void object. This is a predefined static member
    /// intended to be used as a default or initial state.
    /// </summary>
    public static readonly Environment Empty = new(
        CoolVoid.Value,
        ImmutableDictionary<string, CoolObject>.Empty);

    /// <summary>
    /// Creates a new instance of the <see cref="Environment"/> class with a specified 'self' object.
    /// This method ensures immutability by returning a new environment only when the provided 'self' object
    /// differs from the current one, otherwise it returns the existing environment.
    /// </summary>
    /// <param name="newSelf">The new 'self' object to be associated with this environment.
    /// If null is provided, a default <see cref="CoolVoid"/> instance is used.</param>
    /// <returns>A new <see cref="Environment"/> instance with the specified 'self' object,
    /// or the current instance if no changes are required.</returns>
    public Environment WithSelf(CoolObject newSelf) =>
        ReferenceEquals(Self, newSelf ?? CoolVoid.Value)
            ? this
            : new Environment(newSelf ?? CoolVoid.Value, Locals);

    /// <summary>
    /// Returns a new environment that includes the specified local variable binding.
    /// If the given variable already exists and its value is the same as the provided one,
    /// this method returns the current environment instance without creating a new one.
    /// </summary>
    /// <param name="name">The name of the local variable to be added or updated in the environment.</param>
    /// <param name="value">The value to be assigned to the local variable. If null, it defaults to a CoolVoid value.</param>
    /// <returns>A new environment instance with the updated local variable binding, or the current instance if no changes are made.</returns>
    public Environment WithLocal(string name, CoolObject value) =>
        Locals.TryGetValue(name, out var existing) && ReferenceEquals(existing, value ?? CoolVoid.Value)
            ? this
            : new Environment(Self, Locals.SetItem(name, value ?? CoolVoid.Value));

    /// <summary>
    /// Retrieves the value of the specified identifier from the current environment.
    /// It first checks if the identifier is "self", then searches locals,
    /// and finally attempts to retrieve an attribute from the 'self' object.
    /// </summary>
    /// <param name="name">The name of the identifier to look up.</param>
    /// <returns>The object associated with the specified identifier.</returns>
    /// <exception cref="CoolRuntimeException">
    /// Thrown when the identifier is not found in the environment or the 'self' object does not contain the specified attribute.
    /// </exception>
    public CoolObject Lookup(string name)
    {
        return name switch
        {
            "self" => Self,
            _ => Locals.TryGetValue(name, out var local)
                ? local
                : Self is CoolUserObject userObj
                    ? userObj.GetAttribute(name)
                    : throw new CoolRuntimeException($"Undefined identifier: {name}")
        };
    }

    /// <summary>
    /// Returns a string representation of the environment object, including the name
    /// of the current 'self' object's class and the total number of local variables
    /// stored in the environment. This provides a concise and human-readable summary
    /// of the environment's state.
    /// </summary>
    /// <returns>A string describing the environment, including the class name of 'self' and the count of local variables.</returns>
    public override string ToString()
    {
        var selfName = Self.Class.Name;
        var localCount = Locals.Count;
        return $"<Env self={selfName} locals={localCount}>";
    }
}