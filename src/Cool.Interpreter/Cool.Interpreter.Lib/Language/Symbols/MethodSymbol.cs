//-----------------------------------------------------------------------
// <copyright file="MethodSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>MethodSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents a method symbol in the Cool language.
/// </summary>
/// <remarks>
/// This class encapsulates the name, return type, and formal parameters of a method.
/// It is used to define and store metadata about methods within a class.
/// </remarks>
public class MethodSymbol
{
    /// <summary>
    /// Gets the name of the construct, such as a class, method, or attribute, represented by the current symbol.
    /// </summary>
    /// <remarks>
    /// The name is a unique identifier that distinguishes this symbol within its local context.
    /// For methods or attributes, the name is typically defined within the scope of a class.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Gets the return type of the method represented by the current symbol.
    /// </summary>
    /// <remarks>
    /// The return type defines the data type of the value produced by the method.
    /// It can be a primitive type, a user-defined type, or a special keyword like "SELF_TYPE" which indicates the method returns the class type.
    /// </remarks>
    public string ReturnType { get; }

    /// <summary>
    /// Gets an immutable list of formal parameters associated with the method symbol.
    /// </summary>
    /// <remarks>
    /// Each formal parameter is represented as an instance of <see cref="FormalSymbol"/>, which includes the parameter's name and type.
    /// This property is used to define and validate the parameters required by the method.
    /// The order of the formal parameters in the list is preserved as defined in the method signature.
    /// </remarks>
    public ImmutableList<FormalSymbol> Formals { get; }

    /// <summary>
    /// Gets the location in the source code associated with the current symbol.
    /// </summary>
    /// <remarks>
    /// The location provides detailed information about the position, such as file path, line number, and column number,
    /// where the symbol is defined in the source code. It is used for diagnostics, error reporting, and debugging purposes.
    /// </remarks>
    public SourcePosition Location { get; }

    /// <summary>
    /// Represents metadata for a method in the Cool programming language.
    /// </summary>
    /// <remarks>
    /// A method symbol encapsulates details about a method, including its name,
    /// return type, formal parameters, and the location of its definition in the source code.
    /// This class is used in the Cool language interpreter for analyzing and handling method definitions.
    /// </remarks>
    private MethodSymbol(
        string name,
        string returnType,
        ImmutableList<FormalSymbol> formals,
        SourcePosition location)
    {
        Name = name;
        ReturnType = returnType;
        Formals    = formals;
        Location   = location;
    }

    /// <summary>
    /// Adds a new formal parameter to the method and returns a new instance of <see cref="MethodSymbol"/> with the updated parameters.
    /// </summary>
    /// <param name="name">The name of the formal parameter to be added.</param>
    /// <param name="type">The type of the formal parameter to be added.</param>
    /// <returns>A new <see cref="MethodSymbol"/> instance with the specified formal parameter added.</returns>
    public MethodSymbol AddFormal(string name, string type) =>
        new(Name, ReturnType, Formals.Add(new FormalSymbol(name, type)), Location);
}