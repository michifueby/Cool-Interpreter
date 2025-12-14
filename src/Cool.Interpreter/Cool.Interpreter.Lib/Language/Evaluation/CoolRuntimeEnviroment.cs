


namespace Cool.Interpreter.Lib.Language.Evaluation;

using System;
using System.IO;
using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Symbols;

public class CoolRuntimeEnvironment
{
    public SymbolTable SymbolTable { get; }
    public TextWriter Output { get; }
    public TextReader Input { get; }

    // These are created once and never changed
    public CoolObject ObjectRoot { get; }
    public CoolIo Io { get; }

    public CoolRuntimeEnvironment(
        SymbolTable symbolTable,
        TextWriter output,
        TextReader input,
        CoolObject objectRoot,
        CoolIo io)
    {
        SymbolTable = symbolTable;
        Output      = output;
        Input       = input;
        ObjectRoot  = objectRoot;
        Io          = io;
    }

    /// <summary>
    /// Main constructor — builds the full runtime environment.
    /// </summary>
    public CoolRuntimeEnvironment(
        SymbolTable symbolTable,
        TextWriter? output = null,
        TextReader? input = null)
    {
        SymbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        Output      = output ?? Console.Out;
        Input       = input  ?? Console.In;

        // Build Object root class from symbol table
        var objectSymbol = symbolTable.GetClass("Object")
            ?? throw new InvalidOperationException("Class 'Object' not found in symbol table");

        var objectRuntimeClass = RuntimeClassFactory.FromSymbol(objectSymbol, this);

        // Create the single root object (shared by all Object instances)
        ObjectRoot = ObjectFactory.Create(objectRuntimeClass, this);

        // IO is the only object that needs the environment reference
        Io = new CoolIo(this);
    }

    // Fluent builders — preserve expensive ObjectRoot and Io when possible
    public CoolRuntimeEnvironment WithOutput(TextWriter writer) =>
        new(SymbolTable, writer, Input, ObjectRoot, Io);

    public CoolRuntimeEnvironment WithInput(TextReader reader) =>
        new(SymbolTable, Output, reader, ObjectRoot, Io);

    public CoolRuntimeEnvironment WithSymbolTable(SymbolTable newTable)
    {
        // We cannot reuse ObjectRoot/Io — they depend on the old symbol table
        return new CoolRuntimeEnvironment(newTable, Output, Input);
    }
}