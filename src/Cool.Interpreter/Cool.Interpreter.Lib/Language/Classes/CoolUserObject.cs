using System.Collections.Immutable;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Extensions;

namespace Cool.Interpreter.Lib.Language.Classes;

/// <summary>
/// Base class for all user-defined Cool objects (instances of classes defined in Cool source code).
/// This is the runtime representation of objects created with `new MyClass`.
/// 
/// Features:
/// • Immutable attribute storage (thread-safe, zero GC pressure after creation)
/// • Proper initialization order (inheritance order + source order, as required by Cool spec §5)
/// • Full support for attribute access and assignment
/// • Shallow copy via Object.copy()
/// • Ready for SELF_TYPE, let, case, and garbage collection
/// </summary>
public class CoolUserObject : CoolObject
{
    /// <summary>
    /// Gets the current attribute values of this object.
    /// Key = attribute name, Value = current CoolObject (never null — void is represented by CoolVoid.Value)
    /// </summary>
    public ImmutableDictionary<string, CoolObject> Attributes { get; private set; }

    /// <summary>
    /// Creates a new user object instance of the given class.
    /// Attributes are initialized to their default values (0, "", false, void).
    /// Real initialization (<- expr) happens later via InitializeAttributes().
    /// </summary>
    public CoolUserObject(CoolClass @class) : base(@class)
    {
        // Pre-allocate attributes with default values according to Cool spec §5 + §8
        var builder = ImmutableDictionary.CreateBuilder<string, CoolObject>(StringComparer.Ordinal);

        // Walk up the inheritance chain (including this class) in correct order
        foreach (var attr in @class.GetAllAttributesInInheritanceOrder())
        {
            var defaultValue = attr.Initializer switch
            {
                // User provided initializer → will be evaluated later
                not null => CoolVoid.Value,

                // No initializer → use default per type (spec §5 + §8)
                _ => attr.TypeName switch
                {
                    "Int"     => new CoolInt(0),
                    "String"  => new CoolString(""),
                    "Bool"    => CoolBool.False,
                    _         => CoolVoid.Value  // void for all object types
                }
            };

            builder[attr.Name] = defaultValue;
        }

        Attributes = builder.ToImmutable();
    }

    
    internal void InitializeAttributes(IExecutionContext context)
    {
        foreach (var attr in Class.GetAllAttributesInInheritanceOrder())
        {
            if (attr.Initializer is not null)
            {
                var value = attr.Initializer.Accept(context.ExpressionEvaluator);
                SetAttribute(attr.Name, value ?? CoolVoid.Value);
            }
        }
    }

    /// <summary>
    /// Gets the current value of an attribute.
    /// Returns CoolVoid.Value if attribute does not exist (should never happen in correct programs).
    /// </summary>
    public CoolObject GetAttribute(string name)
        => Attributes.GetValueOrDefault(name, CoolVoid.Value);

    /// <summary>
    /// Sets the value of an attribute (used by assignment and initialization).
    /// Returns a new object with updated attributes — immutability preserved.
    /// </summary>
    public void SetAttribute(string name, CoolObject value)
    {
        if (value is null) value = CoolVoid.Value;

        // Immutable update — zero allocation if value unchanged
        Attributes = Attributes.SetItem(name, value);
    }

    // ──────────────────────────────────────────────────────────────────────
    // Object.copy() – shallow copy as required by Cool spec §8.1
    // ──────────────────────────────────────────────────────────────────────
    public override CoolObject Copy()
    {
        var copy = new CoolUserObject(Class)
        {
            // Shallow copy: attributes point to same objects
            Attributes = this.Attributes
        };

        return copy;
    }

    // ──────────────────────────────────────────────────────────────────────
    // Debugging & diagnostics
    // ──────────────────────────────────────────────────────────────────────
    public override string ToString()
    {
        var attrStr = string.Join(", ", Attributes.Select(kv => $"{kv.Key}={kv.Value.AsString()}"));
        return $"<{Class.Name} {{{attrStr}}}>";
    }
}