//-----------------------------------------------------------------------
// <copyright file="CoolIo.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolIo</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

using Core.Exceptions;
using Evaluation;

/// <summary>
/// Represents the built-in CoolIO class, which provides input and output functionality
/// in the Cool language environment. It allows interaction with the runtime's input and
/// output streams, such as reading strings and integers, or writing strings and integers.
/// </summary>
public class CoolIo : CoolObject
{
    /// <summary>
    /// A private, read-only field representing the runtime environment of the Cool interpreter.
    /// This environment provides the necessary runtime context for executing input and output operations,
    /// such as access to input and output streams, symbol tables, and predefined class instances.
    /// </summary>
    private readonly CoolRuntimeEnvironment _env;

    /// <summary>
    /// Represents the built-in `CoolIo` class, which enables input and output functionality
    /// for programs written in the Cool language. This class allows interacting with runtime
    /// input and output mechanisms, such as printing strings or integers to the output stream
    /// or reading string and integer inputs.
    /// </summary>
    public CoolIo(CoolRuntimeEnvironment env) : base(PredefinedClasses.Io)
        => _env = env;

    /// <summary>
    /// Writes the string representation of the specified <see cref="CoolString"/> object
    /// to the output stream and returns the current <see cref="CoolIo"/> object instance
    /// to enable method chaining.
    /// </summary>
    /// <param name="s">The <see cref="CoolString"/> object whose value will be written to the output stream.</param>
    /// <returns>The current instance of the <see cref="CoolIo"/> object.</returns>
    public CoolIo OutString(CoolString s)
    {
        _env.Output.Write(s.Value);
        return this;
    }

    /// <summary>
    /// Outputs the value of the specified integer to the runtime's output stream and returns the current instance of the `CoolIo` class.
    /// </summary>
    /// <param name="i">The integer value to be written to the output stream.</param>
    /// <returns>The current instance of the `CoolIo` class, allowing method chaining.</returns>
    public CoolIo OutInt(CoolInt i)
    {
        _env.Output.Write(i.Value);
        return this;
    }

    /// <summary>
    /// Reads a string from the runtime's input stream and returns it as a `CoolString` object.
    /// If no input is provided, an empty `CoolString` is returned.
    /// </summary>
    /// <returns>A `CoolString` representing the string read from the input stream.</returns>
    public CoolString InString() 
        => new CoolString(_env.Input.ReadLine() ?? "");

    /// <summary>
    /// Reads an integer from the input stream in the Cool language runtime environment.
    /// If the input cannot be parsed as an integer, it defaults to returning a new instance of `CoolInt` with a value of 0.
    /// </summary>
    /// <returns>An instance of `CoolInt` representing the integer read from the input, or 0 if parsing fails.</returns>
    public CoolInt InInt()
    {
        var line = _env.Input.ReadLine();
        return int.TryParse(line, out var n) ? new CoolInt(n) : new CoolInt(0);
    }

    /// <summary>
    /// Terminates the execution of the program by throwing a `CoolRuntimeException`.
    /// This method is used to explicitly abort the runtime flow and signal an error state.
    /// </summary>
    /// <returns>This method does not return a value because it always throws an exception.</returns>
    /// <exception cref="CoolRuntimeException">Always thrown to indicate an abort operation.</exception>
    public override CoolObject Abort() 
        => throw new CoolRuntimeException("abort");

    /// <summary>
    /// Returns the name of the type associated with the object.
    /// This method provides a string representation indicating the runtime type identifier
    /// of the object, helpful for type introspection and debugging purposes.
    /// </summary>
    /// <returns>
    /// A `CoolString` containing the name of the type.
    /// </returns>
    public override CoolString TypeName() 
        => new CoolString("Io");

    /// <summary>
    /// Creates a new instance of the `Copy` method for the `CoolObject` class.
    /// </summary>
    /// <returns>
    /// Returns the current instance of the `CoolObject`.
    /// </returns>
    public override CoolObject Copy()
        => this;
}