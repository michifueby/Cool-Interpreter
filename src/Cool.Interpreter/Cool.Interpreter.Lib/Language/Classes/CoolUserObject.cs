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
    /// <summary>
    /// A private dictionary storing the attributes of the current
    /// Cool user-defined object. Each attribute is represented as a key-value pair
    /// where the key is the attribute's name, and the value is the corresponding
    /// CoolObject instance.
    /// </summary>
    private readonly Dictionary<string, CoolObject> _attributes;

    /// <summary>
    /// Represents a user-defined object in the Cool language runtime.
    /// Provides support for custom attributes and their initialization.
    /// </summary>
    public CoolUserObject(CoolClass @class, CoolRuntimeEnvironment env) : base(@class)
    {
        _attributes = new Dictionary<string, CoolObject>();

        // First, initialize ALL attributes to their default values
        // This ensures that self-referential initializers (like x <- x + 1) 
        // can access the attribute with its default value
        foreach (var attr in @class.GetAllAttributesInOrder())
        {
            _attributes[attr.Name] = DefaultValue(attr.TypeName);
        }

        // Object is created, then initialized explicitly via Initialize()
        // Factory calls Initialize() after construction
    }
    
    // Internal constructor not needed if we just populate.

    /// <summary>
    /// Updates the attribute with the specified name to the given value.
    /// </summary>
    /// <param name="name">The name of the attribute to set.</param>
    /// <param name="value">The new value for the attribute.</param>
    public void SetAttribute(string name, CoolObject value)
    {
        if (!_attributes.ContainsKey(name))
            throw new CoolRuntimeException($"Attribute '{name}' not found on object of type '{Class.Name}'");
        
        _attributes[name] = value ?? CoolVoid.Value;
    }

    /// <summary>
    /// Initializes the attributes of the current object using the provided runtime environment.
    /// </summary>
    /// <param name="env">The runtime environment used to evaluate and assign initial values to the object's attributes.</param>
    public void Initialize(CoolRuntimeEnvironment env)
    {
        var tempEnv = Environment.Empty.WithSelf(this);

        foreach (var attr in Class.GetAllAttributesInOrder())
        {
            if (attr.Initializer is ExpressionNode init)
            {
                var value = init.Accept(new CoolEvaluator(env, tempEnv));
                _attributes[attr.Name] = value;
            }
        }
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
    /// Checks whether an attribute with the specified name exists on this object.
    /// </summary>
    /// <param name="name">The name of the attribute to check.</param>
    /// <returns>True if the attribute exists; otherwise, false.</returns>
    public bool HasAttribute(string name)
        => _attributes.ContainsKey(name);

    /// <summary>
    /// Creates a shallow copy of the current object.
    /// </summary>
    /// <returns>A new instance of the object with copied attributes.</returns>
    public override CoolObject Copy()
    {
        // For mutable objects, Copy returns a shallow copy.
        // We need to create a new object and copy attributes.
        // But we need the env to create? Or just raw copy?
        // CoolUserObject constructor needs env?
        // We can make a private constructor.
        var copy = new CoolUserObject(Class);
        foreach(var kvp in _attributes) copy._attributes[kvp.Key] = kvp.Value;
        return copy;
    }

    private CoolUserObject(CoolClass @class) : base(@class) 
    {
         _attributes = new Dictionary<string, CoolObject>();
    }

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