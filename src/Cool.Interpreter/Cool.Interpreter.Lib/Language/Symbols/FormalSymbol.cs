namespace Cool.Interpreter.Lib.Language.Symbols;

public class FormalSymbol(string name, string type)
{
    public string Name { get; } = name;
    public string Type { get; } = type;
}