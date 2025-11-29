//-----------------------------------------------------------------------
// <copyright file="CoolInt.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolInt</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Classes.BuiltIn;

/// <summary>
/// Represents an integer type in the Cool programming language.
/// The `CoolInt` class wraps an integer value and extends the `CoolObject` base class,
/// thereby allowing it to integrate seamlessly within the Cool runtime environment.
/// This class provides functionality to convert the wrapped integer value to a string,
/// compare two `CoolInt` objects for equality, and access its integer value.
/// </summary>
public class CoolInt : CoolObject
{
    public CoolInt(CoolClass @class) : base(@class)
    {
    }
}