namespace Cool.Interpreter.Lib.Language.Symbols;

public sealed class SymbolTable
{
    private readonly Dictionary<string, ClassSymbol> _classes = new();

    public IReadOnlyDictionary<string, ClassSymbol> Classes => _classes;

    public void AddClass(ClassSymbol classSymbol)
        => _classes[classSymbol.Name] = classSymbol;

    public ClassSymbol? GetClass(string name)
        => _classes.TryGetValue(name, out var cls) ? cls : null;
}