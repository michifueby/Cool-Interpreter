//-----------------------------------------------------------------------
// <copyright file="MethodSymbol.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>MethodSymbol</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Symbols;

/// <summary>
/// Represents a method symbol in the Cool language.
/// </summary>
/// <remarks>
/// This class encapsulates the name, return type, and formal parameters of a method.
/// It is used to define and store metadata about methods within a class.
/// </remarks>
public class MethodSymbol(string name, string returnType)
{
    /// <summary>
    /// Gets the name of the method.
    /// </summary>
    /// <remarks>
    /// This property represents the identifier of the method defined in the Cool language.
    /// It is used to uniquely distinguish the method within a class definition or scope.
    /// </remarks>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the return type of the method.
    /// </summary>
    /// <remarks>
    /// This property specifies the type of value that the method is expected to return.
    /// It defines the output type in the method's signature within the Cool language.
    /// </remarks>
    public string ReturnType { get; } = returnType;

    /// <summary>
    /// Gets the list of formal parameters of the method.
    /// </summary>
    /// <remarks>
    /// This property contains the formal parameters defined for the method,
    /// represented as a collection of <see cref="FormalSymbol"/> objects.
    /// Each formal parameter includes its name and type information.
    /// </remarks>
    public List<FormalSymbol> Formals { get; } = new();
}