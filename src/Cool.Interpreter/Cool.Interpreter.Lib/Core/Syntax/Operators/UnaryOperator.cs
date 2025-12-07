//-----------------------------------------------------------------------
// <copyright file="UnaryOperator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>UnaryOperator</summary>
//-----------------------------------------------------------------------s

namespace Cool.Interpreter.Lib.Core.Syntax.Operators;

/// <summary>
/// Specifies the type of unary operations that can be performed.
/// A unary operator operates on a single operand.
/// </summary>
public enum UnaryOperator
{
    /// <summary>
    /// Represents the unary negate operator, which inverts the sign of a numerical expression.
    /// This operator is used to negate an operand, effectively multiplying its value by -1.
    /// </summary>
    Negate,

    /// <summary>
    /// Represents the logical NOT operator, which inverts the truth value of a boolean expression.
    /// This operator evaluates to true if the operand is false, and evaluates to false if the operand is true.
    /// </summary>
    Not
}