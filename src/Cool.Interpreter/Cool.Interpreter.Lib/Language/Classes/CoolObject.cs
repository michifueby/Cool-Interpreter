//-----------------------------------------------------------------------
// <copyright file="CoolObject.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolObject</summary>
//-----------------------------------------------------------------------

using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;

namespace Cool.Interpreter.Lib.Language.Classes;

/// <summary>
/// Base class for all Cool runtime values. In Cool, *everything is an object* —
/// even integers, booleans, strings, and null.
/// </summary>
public abstract class CoolObject
{
    public CoolClass Class { get; }

    protected CoolObject(CoolClass @class)
        => Class = @class ?? throw new ArgumentNullException(nameof(@class));

    // ─── Object methods (Section 8.1) ───
    public virtual CoolObject Abort() => throw new CoolRuntimeException("abort() called");
    public virtual CoolString TypeName() => new CoolString(Class.Name);
    public virtual CoolObject Copy() => this; // shallow copy by default

    public virtual string AsString() => ToString();
    public override string ToString() => $"<{Class.Name} object>";
}