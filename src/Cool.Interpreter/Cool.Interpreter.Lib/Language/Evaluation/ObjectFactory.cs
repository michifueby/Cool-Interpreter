//-----------------------------------------------------------------------
// <copyright file="ObjectFactory.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ObjectFactory</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Evaluation;

using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;

/// <summary>
/// Provides functionality for creating instances of CoolObject based on the class definition
/// and runtime environment. This factory supports the creation of both predefined (e.g., Int, String)
/// and user-defined objects.
/// </summary>
public static class ObjectFactory
{
    /// <summary>
    /// Creates a new instance of a CoolObject based on the given class and runtime environment.
    /// </summary>
    /// <param name="class">The class definition of the object to create.</param>
    /// <param name="env">The runtime environment used to create the object.</param>
    /// <returns>A new instance of the specified CoolObject or a default object based on the class name.</returns>
    public static CoolObject Create(CoolClass @class, CoolRuntimeEnvironment env) =>
        @class.Name switch
        {
            "Int"     => CoolInt.Zero,
            "String"  => CoolString.Empty,
            "Bool"    => CoolBool.False,
            "IO"      => env.Io,
            "Object"  => env.ObjectRoot,
            _          => CreateUserObject(@class, env)
        };

    /// <summary>
    /// Creates a new user-defined object of type CoolUserObject based on the specified class and runtime environment.
    /// </summary>
    /// <param name="class">The class definition describing the structure and behavior of the user-defined object to create.</param>
    /// <param name="env">The runtime environment used to initialize and manage the object.</param>
    /// <returns>A fully initialized instance of CoolUserObject, adhering to the specified class definition.</returns>
    private static CoolUserObject CreateUserObject(CoolClass @class, CoolRuntimeEnvironment env)
    {
        if (env.StackDepth >= CoolRuntimeEnvironment.MaxStackDepth)
        {
            throw new Cool.Interpreter.Lib.Core.Exeptions.CoolRuntimeException(
                "Stack overflow: maximum recursion depth exceeded.", 
                Cool.Interpreter.Lib.Core.Syntax.SourcePosition.None);
        }

        // Check for circular initialization - prevent creating an object of a class that's currently being initialized
        if (env.ClassesBeingInitialized.Contains(@class.Name))
        {
            throw new Cool.Interpreter.Lib.Core.Exeptions.CoolRuntimeException(
                $"Circular initialization dependency detected: class '{@class.Name}' cannot create an instance of itself during attribute initialization.", 
                Cool.Interpreter.Lib.Core.Syntax.SourcePosition.None);
        }

        var nextEnv = env.WithStackDepth(env.StackDepth + 1)
                         .WithClassBeingInitialized(@class.Name);
        var obj = new CoolUserObject(@class, nextEnv);
        obj.Initialize(nextEnv);
        return obj;
    }
}