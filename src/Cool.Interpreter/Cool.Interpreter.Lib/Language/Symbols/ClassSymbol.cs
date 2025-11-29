namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents a class symbol in the context of a symbol table. A class symbol defines
/// the structure and relationships of a class, including its name, an optional parent class,
/// methods, and attributes.
/// </summary>
/// <param name="name">The name of the class.</param>
/// <param name="parentName">
/// The name of the parent class from which this class inherits, if any.
/// If null, the class does not inherit from any other class.
/// </param>
public class ClassSymbol(string name, string? parentName = null)
{
    /// <summary>
    /// Gets the name of the class, method, attribute, or formal parameter
    /// represented by the current symbol instance.
    /// This typically serves as the identifier in the context of a symbol table.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the name of the parent class from which the current class inherits, if any.
    /// This property is null if the current class does not have a parent class.
    /// </summary>
    public string? ParentName { get; } = parentName;

    /// <summary>
    /// Gets the collection of methods defined within the class symbol. Each method
    /// is represented by a <see cref="MethodSymbol"/> instance, which provides
    /// information about the method's name, return type, and formal parameters.
    /// This collection serves as a mapping between method names and their corresponding
    /// symbols, enabling lookups and ensuring unique identification of each method
    /// within the class.
    /// </summary>
    public Dictionary<string, MethodSymbol> Methods { get; } = new();

    /// <summary>
    /// Gets a collection of attributes associated with the class symbol.
    /// Attributes represent the properties or fields of a class and are defined by their name and type.
    /// </summary>
    public Dictionary<string, AttributeSymbol> Attributes { get; } = new();
}