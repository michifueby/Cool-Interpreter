namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents an attribute symbol in the context of a programming language's symbol table.
/// An attribute is defined by its name and type, and is typically tied to a class or object.
/// </summary>
public class AttributeSymbol(string name, string type)
{
    public string Name { get; } = name;
    public string Type { get; } = type;
}