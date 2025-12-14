
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Language.Extensions;

namespace Cool.Interpreter.Lib.Language.Classes;

using System.Collections.Immutable;
using System.Text;
using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Evaluation;

/// <summary>
/// Represents an immutable user-defined object in the Cool runtime environment.
/// This class encapsulates attributes and their values, providing functionality
/// to manage and query object state while adhering to the runtime's immutability constraints.
/// </summary>
public sealed class CoolUserObject : CoolObject
{
    private ImmutableDictionary<string, CoolObject> _attributes;

    public CoolUserObject(CoolClass @class, CoolRuntimeEnvironment env) : base(@class)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, CoolObject>(StringComparer.Ordinal);

        foreach (var attr in @class.GetAllAttributesInOrder())
        {
            var def = attr.Initializer is null ? DefaultValue(attr.TypeName) : CoolVoid.Value; // placeholder
            builder[attr.Name] = def;
        }
        _attributes = builder.ToImmutable();

        // Run attribute initialisers NOW – they may contain code
        Initialize(env);
    }

    private CoolUserObject(CoolClass @class, ImmutableDictionary<string, CoolObject> attributes) : base(@class)
        => _attributes = attributes;

    public CoolUserObject WithAttribute(string name, CoolObject value)
        => new(Class, _attributes.SetItem(name, value ?? CoolVoid.Value));

    public void Initialize(CoolRuntimeEnvironment env)
    {
        var builder = _attributes.ToBuilder();
        var tempEnv = Environment.Empty.WithSelf(this);

        foreach (var attr in Class.GetAllAttributesInOrder())
        {
            if (attr.Initializer is ExpressionNode init)
            {
                var value = init.Accept(new CoolEvaluator(env));
                builder[attr.Name] = value;
            }
        }
        _attributes = builder.ToImmutable();
    }

    public CoolObject GetAttribute(string name)
        => _attributes.TryGetValue(name, out var v) ? v : throw new CoolRuntimeException($"No attribute {name}");
    
    public override CoolObject Copy() => this; // shallow copy — correct per spec

    public override string ToString()
    {
        var attrs = string.Join(", ", _attributes.Select(kv => $"{kv.Key}={kv.Value.AsString()}"));
        return $"<{Class.Name} {{{attrs}}}>";
    }

    private static CoolObject DefaultValue(string typeName) => typeName switch
    {
        "Int"    => CoolInt.Zero,
        "String" => CoolString.Empty,
        "Bool"   => CoolBool.False,
        _        => CoolVoid.Value
    };
}