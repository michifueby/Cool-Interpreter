using Cool.Interpreter.Lib.Core.Exeptions;

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

public sealed class CoolBuiltinObject : CoolObject
{
    public CoolBuiltinObject(CoolClass @class) : base(@class)
    {
    }

    public override CoolObject Abort() => 
        throw new CoolRuntimeException("abort() called");

    public override CoolString TypeName() => 
        new CoolString(Class.Name);

    public override CoolObject Copy() => this; // shallow copy

    public override string ToString() => 
        $"<builtin {Class.Name}>";
}