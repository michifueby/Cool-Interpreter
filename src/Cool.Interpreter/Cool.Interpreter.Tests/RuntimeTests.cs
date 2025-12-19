//-----------------------------------------------------------------------
// <copyright file="RuntimeTests.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Runtime Execution Tests</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Tests;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

/// <summary>
/// Test suite for runtime execution and error handling.
/// Tests abort(), division by zero, and other runtime exceptions.
/// </summary>
[TestFixture]
public class RuntimeTests
{
    private CoolInterpreter _interpreter = null!;

    [SetUp]
    public void Setup()
    {
        _interpreter = new CoolInterpreter();
    }

    #region Abort Tests

    /// <summary>
    /// Tests that calling abort() produces the correct runtime error.
    /// </summary>
    [Test]
    public void Runtime_Abort_ReportsAbortCalled()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Object { abort() };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "Program with abort() should not succeed");
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.AbortCalled), Is.True,
                $"Should report abort called error.\nDiagnostics: {FormatDiagnostics(result.Diagnostics)}");
        });
    }

    /// <summary>
    /// Tests abort() from a nested method call.
    /// </summary>
    [Test]
    public void Runtime_AbortInMethod_ReportsAbortCalled()
    {
        // Arrange
        const string code = @"
class Main {
    doAbort(): Object { abort() };
    main(): Object { doAbort() };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.AbortCalled), Is.True,
                "Should report abort called from nested method");
        });
    }

    #endregion

    #region Division By Zero Tests

    /// <summary>
    /// Tests that division by zero produces the correct runtime error.
    /// </summary>
    [Test]
    public void Runtime_DivisionByZero_ReportsError()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int { 10 / 0 };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "Division by zero should fail");
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.DivisionByZero), Is.True,
                $"Should report division by zero error.\nDiagnostics: {FormatDiagnostics(result.Diagnostics)}");
        });
    }

    /// <summary>
    /// Tests division by zero with a variable.
    /// </summary>
    [Test]
    public void Runtime_DivisionByZeroVariable_ReportsError()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let zero : Int <- 0 in 100 / zero
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.DivisionByZero), Is.True,
            "Should detect division by zero through variable");
    }

    #endregion

    #region String Substring Tests

    /// <summary>
    /// Tests that substr() with invalid range produces runtime error.
    /// </summary>
    [Test]
    public void Runtime_SubstrOutOfRange_ReportsError()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): String {
        ""Hello"".substr(0, 100)
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.SubstrOutOfRange), Is.True,
                $"Should report substr out of range.\nDiagnostics: {FormatDiagnostics(result.Diagnostics)}");
        });
    }

    /// <summary>
    /// Tests that substr() with negative index produces runtime error.
    /// </summary>
    [Test]
    public void Runtime_SubstrNegativeIndex_ReportsError()
    {
        // Arrange
        const string code = @"
class Main {
    main(): String {
        ""Hello"".substr(~1, 2)
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.That(result.IsSuccess, Is.False,
            "Substr with negative index should fail");
    }

    #endregion

    #region Successful Runtime Tests

    /// <summary>
    /// Tests simple arithmetic execution.
    /// </summary>
    [Test]
    public void Runtime_SimpleArithmetic_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int { 1 + 2 * 3 };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, 
                $"Simple arithmetic should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("7"));
        });
    }

    /// <summary>
    /// Tests valid division.
    /// </summary>
    [Test]
    public void Runtime_ValidDivision_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int { 100 / 10 };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ReturnedValue, Is.EqualTo("10"));
        });
    }

    /// <summary>
    /// Tests while loop execution with nested let bindings (single binding per let).
    /// </summary>
    [Test]
    public void Runtime_WhileLoop_Succeeds()
    {
        // Arrange - using nested single-binding lets instead of multi-binding
        const string code = @"
class Main {
    main(): Int {
        let i : Int <- 0 in
            let sum : Int <- 0 in {
                while i < 5 loop {
                    sum <- sum + i;
                    i <- i + 1;
                } pool;
                sum;
            }
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"While loop should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("10")); // 0+1+2+3+4 = 10
        });
    }

    /// <summary>
    /// Tests if-then-else execution.
    /// </summary>
    [Test]
    public void Runtime_IfThenElse_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        if 5 < 10 then 1 else 0 fi
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ReturnedValue, Is.EqualTo("1"));
        });
    }

    /// <summary>
    /// Tests case expression execution.
    /// </summary>
    [Test]
    public void Runtime_CaseExpression_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let x : Object <- 42 in
            case x of
                i : Int => i;
                s : String => 0;
                o : Object => ~1;
            esac
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Case expression should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("42"));
        });
    }

    /// <summary>
    /// Tests object creation and method dispatch.
    /// </summary>
    [Test]
    public void Runtime_NewObjectDispatch_Succeeds()
    {
        // Arrange
        const string code = @"
class Counter {
    value : Int <- 0;
    inc() : Int { value <- value + 1 };
    get() : Int { value };
};

class Main {
    main(): Int {
        let c : Counter <- new Counter in {
            c.inc();
            c.inc();
            c.get();
        }
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Object dispatch should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("2"));
        });
    }

    #endregion

    #region IO Tests

    /// <summary>
    /// Tests out_string() produces output.
    /// </summary>
    [Test]
    public void Runtime_OutString_ProducesOutput()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): SELF_TYPE { out_string(""Hello, World!\n"") };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"out_string should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.Output, Does.Contain("Hello, World!"));
        });
    }

    /// <summary>
    /// Tests out_int() produces output.
    /// </summary>
    [Test]
    public void Runtime_OutInt_ProducesOutput()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): SELF_TYPE { out_int(42) };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Output, Does.Contain("42"));
        });
    }

    #endregion

    #region Helper Methods

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        var list = diagnostics.ToList();
        if (!list.Any())
            return "(no diagnostics)";
        
        return string.Join("\n", list.Select(d => $"  [{d.Severity}:{d.Code}] {d.Message}"));
    }

    #endregion
}
