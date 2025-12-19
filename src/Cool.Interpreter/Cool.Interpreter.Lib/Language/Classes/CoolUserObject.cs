//-----------------------------------------------------------------------
// <copyright file="CoolUserObject.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolUserObject</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Language.Extensions;
using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Evaluation;

/// <summary>
/// Represents an immutable user-defined object in the Cool runtime environment.
/// This class encapsulates attributes and their values, providing functionality
/// to manage and query object state while adhering to the runtime's immutability constraints.
/// </summary>
public class CoolUserObject : CoolObject
{
    /// <summary>
    /// A private, immutable dictionary storing the attributes of the current
    /// Cool user-defined object. Each attribute is represented as a key-value pair
    /// where the key is the attribute's name, and the value is the corresponding
    /// CoolObject instance.
    /// </summary>
    private ImmutableDictionary<string, CoolObject> _attributes;

    /// <summary>
    /// Represents a user-defined object in the Cool language runtime.
    /// Provides support for custom attributes and their initialization.
    /// </summary>
    public CoolUserObject(CoolClass @class, CoolRuntimeEnvironment env) : base(@class)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, CoolObject>(StringComparer.Ordinal);

        foreach (var attr in @class.GetAllAttributesInOrder())
        {
            var def = attr.Initializer is null ? DefaultValue(attr.TypeName) : CoolVoid.Value; 
            builder[attr.Name] = def;
        }
        _attributes = builder.ToImmutable();

        Initialize(env);
    }

    /// <summary>
    /// Initializes a new instance of the CoolUserObject class with the specified class and attributes.
    /// </summary>
    /// <param name="class"></param>
    /// <param name="attributes"></param>
    private CoolUserObject(CoolClass @class, ImmutableDictionary<string, CoolObject> attributes) : base(@class)
        => _attributes = attributes;

    /// <summary>
    /// Creates a new instance of the current object with an updated attribute, adding or replacing the specified attribute with the provided value.
    /// </summary>
    /// <param name="name">The name of the attribute to be added or updated.</param>
    /// <param name="value">The value to set for the specified attribute. If null, it assigns the default void value.</param>
    /// <returns>A new instance of the object with the updated attribute set.</returns>
    public CoolUserObject WithAttribute(string name, CoolObject value)
        => new(Class, _attributes.SetItem(name, value ?? CoolVoid.Value));

    /// <summary>
    /// Initializes the attributes of the current object using the provided runtime environment.
    /// </summary>
    /// <param name="env">The runtime environment used to evaluate and assign initial values to the object's attributes.</param>
    public void Initialize(CoolRuntimeEnvironment env)
    {
        var builder = _attributes.ToBuilder();
        var tempEnv = Environment.Empty.WithSelf(this);

        foreach (var attr in Class.GetAllAttributesInOrder())
        {
            if (attr.Initializer is ExpressionNode init)
            {
                var value = init.Accept(new CoolEvaluator(env, tempEnv));
                builder[attr.Name] = value;
            }
        }
        _attributes = builder.ToImmutable();
    }

    /// <summary>
    /// Retrieves the value of an attribute specified by its name.
    /// </summary>
    /// <param name="name">The name of the attribute to retrieve.</param>
    /// <returns>The value of the attribute as a <see cref="CoolObject"/>.</returns>
    /// <exception cref="CoolRuntimeException">Thrown if the specified attribute does not exist.</exception>
    public CoolObject GetAttribute(string name)
        => _attributes.TryGetValue(name, out var v) ? v : throw new CoolRuntimeException($"No attribute {name}");

    /// <summary>
    /// Creates a shallow copy of the current object.
    /// </summary>
    /// <returns>The current instance of the object, as Cool objects are immutable by design.</returns>
    public override CoolObject Copy() => this; // shallow copy — correct per spec

    /// <summary>
    /// Converts the current instance of the CoolUserObject to its string representation.
    /// </summary>
    /// <returns>A string representation of the object, including the class name and its attributes in the format "<ClassName {attr1=value1, attr2=value2}>".</returns>
    public override string ToString()
    {
        // Avoid printing attributes to prevent infinite recursion if object refers to itself
        return $"<{Class.Name}>";
    }

    /// <summary>
    /// Retrieves the default value of a specified Cool language type.
    /// </summary>
    /// <param name="typeName">The name of the type for which the default value is requested, e.g., "Int", "String", or "Bool".</param>
    /// <returns>An instance of the default value corresponding to the specified type. Returns <c>CoolInt.Zero</c> for "Int", <c>CoolString.Empty</c> for "String", <c>CoolBool.False</c> for "Bool", and <c>CoolVoid.Value</c> for unsupported types.</returns>
    private static CoolObject DefaultValue(string typeName) => typeName switch
    {
        "Int" => CoolInt.Zero,
        "String" => CoolString.Empty,
        "Bool"   => CoolBool.False,
        _        => CoolVoid.Value
    };
}