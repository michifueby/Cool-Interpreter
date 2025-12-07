//-----------------------------------------------------------------------
// <copyright file="BinaryOperator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>BinaryOperator</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Operators;

/// <summary>
/// Defines a set of binary operators that can be used in expressions.
/// The enumerators represent various operations that can be applied to two operands,
/// such as arithmetic operations or comparisons.
/// </summary>
public enum BinaryOperator
{
    /// <summary>
    /// Represents the addition binary operator in expressions.
    /// This operator is used to add the values of two operands.
    /// </summary>
    Plus,

    /// <summary>
    /// Represents the subtraction binary operator in expressions.
    /// This operator is used to subtract the value of the right operand from the value of the left operand.
    /// </summary>
    Minus,

    /// <summary>
    /// Represents the multiplication binary operator in expressions.
    /// This operator is used to calculate the product of two operand values.
    /// </summary>
    Multiply,

    /// <summary>
    /// Represents the division binary operator in expressions.
    /// This operator is used to divide the value of the left operand by the value of the right operand.
    /// </summary>
    Divide,

    /// <summary>
    /// Represents the "less than" binary operator in expressions.
    /// This operator is used to compare two operands and evaluates to true if the first operand is less than the second.
    /// </summary>
    LessThan,

    /// <summary>
    /// Represents the "less than or equal to" comparison operator in expressions.
    /// This operator is used to evaluate whether the value of the left operand
    /// is less than or equal to the value of the right operand.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Represents the equality comparison operator in expressions.
    /// This operator is used to determine if the values of two operands are equal.
    /// </summary>
    Equal
}