//-----------------------------------------------------------------------
// <copyright file="CoolRuntimeEnvironment.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolRuntimeEnvironment</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using System;
using System.Collections.Immutable;
using System.IO;
using Classes;
using Classes.BuiltIn;
using Symbols;

/// <summary>
/// Represents the runtime environment for the Cool programming language,
/// facilitating the execution of Cool programs by providing access to
/// the symbol table, predefined objects, input/output streams, and other runtime components.
/// </summary>
public class CoolRuntimeEnvironment
{
    /// <summary>
    /// Gets the symbol table used for managing and storing information about symbols
    /// (such as classes, methods, and variables) within the runtime environment.
    /// This property enables efficient symbol resolution and scope management during program evaluation.
    /// </summary>
    public SymbolTable SymbolTable { get; }

    /// <summary>
    /// Gets the output stream used by the runtime environment to write data,
    /// often to standard output or a specified output target, enabling
    /// operations such as displaying results or diagnostic messages during program execution.
    /// </summary>
    public TextWriter Output { get; }

    /// <summary>
    /// Gets the input stream used by the runtime environment to read data,
    /// typically from standard input or a specified input source, enabling
    /// operations such as reading user-provided values during program execution.
    /// </summary>
    public TextReader Input { get; }

    /// <summary>
    /// Gets the root object of the runtime environment, which is the instance of the "Object" class.
    /// </summary>
    public CoolObject ObjectRoot { get; }

    /// <summary>
    /// Gets the IO functionality within the runtime environment,
    /// enabling interaction with input and output streams for Cool programs.
    /// This property holds the instance of the predefined "IO" object, which
    /// facilitates operations such as reading user input and writing output.
    /// </summary>
    public CoolIo Io { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoolRuntimeEnvironment"/> class.
    /// </summary>
    /// <param name="symbolTable"></param>
    /// <param name="output"></param>
    /// <param name="input"></param>
    /// <param name="objectRoot"></param>
    /// <param name="io"></param>
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
        StackDepth  = 0;
        ClassesBeingInitialized = ImmutableHashSet<string>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoolRuntimeEnvironment"/> class.
    /// </summary>
    /// <param name="symbolTable"></param>
    /// <param name="output"></param>
    /// <param name="input"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CoolRuntimeEnvironment(
        SymbolTable symbolTable,
        TextWriter? output = null,
        TextReader? input = null)
    {
        SymbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        Output      = output ?? Console.Out;
        Input       = input  ?? Console.In;

        // ObjectRoot is now correctly built via the factory using predefined class
        var objectRuntimeClass = RuntimeClassFactory.FromSymbol(symbolTable.GetClass("Object"), this);
        ObjectRoot = ObjectFactory.Create(objectRuntimeClass, this);

        // IO is the only object that needs the environment reference
        Io = new CoolIo(this);
        StackDepth = 0;
        ClassesBeingInitialized = ImmutableHashSet<string>.Empty;
    }

    /// <summary>
    /// Current recursion depth (for stack overflow detection).
    /// </summary>
    public int StackDepth { get; }

    /// <summary>
    /// Classes currently being initialized to detect circular initialization dependencies.
    /// </summary>
    public ImmutableHashSet<string> ClassesBeingInitialized { get; }

    public const int MaxStackDepth = 4;

    /// <summary>
    /// Private constructor used by WithOutput and WithInput to create a new environment
    /// while checking that IO is correctly re-bound to the new environment.
    /// </summary>
    private CoolRuntimeEnvironment(
        SymbolTable symbolTable,
        TextWriter output,
        TextReader input,
        CoolObject objectRoot,
        int stackDepth,
        ImmutableHashSet<string> classesBeingInitialized)
    {
        SymbolTable = symbolTable;
        Output      = output;
        Input       = input;
        ObjectRoot  = objectRoot;
        StackDepth  = stackDepth;
        ClassesBeingInitialized = classesBeingInitialized;
        // Re-create IO so it points to THIS new environment instance
        Io          = new CoolIo(this);
    }

    /// <summary>
    /// Creates a new runtime environment with the specified output writer.
    /// </summary>
    /// <param name="writer">The text writer to be used for output operations within the runtime environment.</param>
    /// <returns>A new instance of <c>CoolRuntimeEnvironment</c> configured with the specified output writer.</returns>
    public CoolRuntimeEnvironment WithOutput(TextWriter writer) =>
        new(SymbolTable, writer, Input, ObjectRoot, StackDepth, ClassesBeingInitialized);

    /// <summary>
    /// Creates a new instance of the runtime environment with the specified input source.
    /// </summary>
    /// <param name="reader">A <see cref="TextReader"/> used to supply input to the runtime environment.</param>
    /// <returns>A new instance of <see cref="CoolRuntimeEnvironment"/> configured with the provided input source.</returns>
    public CoolRuntimeEnvironment WithInput(TextReader reader) =>
        new(SymbolTable, Output, reader, ObjectRoot, StackDepth, ClassesBeingInitialized);

    public CoolRuntimeEnvironment WithStackDepth(int depth) =>
        new(SymbolTable, Output, Input, ObjectRoot, depth, ClassesBeingInitialized);

    public CoolRuntimeEnvironment WithClassBeingInitialized(string className) =>
        new(SymbolTable, Output, Input, ObjectRoot, StackDepth, ClassesBeingInitialized.Add(className));
}