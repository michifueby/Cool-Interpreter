namespace Cool.Interpreter.Lib.Language.Symbols;

public class MethodSymbol(string name, string returnType)
{
    public string Name { get; } = name;
    public string ReturnType { get; } = returnType;

    public List<FormalSymbol> Formals { get; } = new();
}