//-----------------------------------------------------------------------
// <copyright file="AlgorithmTests.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Algorithm/Full Execution Tests</summary>
//-----------------------------------------------------------------------

using System.IO;

namespace Cool.Interpreter.Tests;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

/// <summary>
/// Test suite for full program execution (Algorithm tests).
/// Tests complete Cool programs through all phases: parsing, semantic analysis, and execution.
/// </summary>
[TestFixture]
public class AlgorithmTests
{
    private const string TestFilesRoot = "TestCases";
    private CoolInterpreter _interpreter = null!;

    [SetUp]
    public void Setup()
    {
        _interpreter = new CoolInterpreter();
    }

    #region Automatic Test Case Discovery

    /// <summary>
    /// Tests that files in Algorithm/fail directory fail during execution.
    /// </summary>
    [TestCaseSource(nameof(GetAlgorithmFailFiles))]
    public void Algorithm_InvalidFile_Fails(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.Run(sourceCode, fileName);

        // Assert
        Assert.That(result.IsSuccess, Is.False,
            $"File '{fileName}' should fail execution but succeeded.\n" +
            $"Output: {result.Output}\n" +
            $"Returned: {result.ReturnedValue}");
    }

    /// <summary>
    /// Tests that files in Algorithm/success directory execute successfully.
    /// </summary>
    [TestCaseSource(nameof(GetAlgorithmSuccessFiles))]
    public void Algorithm_ValidFile_Succeeds(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.Run(sourceCode, fileName);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"File '{fileName}' should execute successfully.\n" +
            $"Errors: {FormatDiagnostics(result.Diagnostics)}");
    }

    #endregion

    #region Specific Algorithm Tests

    /// <summary>
    /// Tests factorial calculation.
    /// </summary>
    [Test]
    public void Algorithm_Factorial_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    factorial(n: Int): Int {
        if n = 0 then 1 else n * factorial(n - 1) fi
    };
    main(): Int { factorial(5) };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Factorial should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("120")); // 5! = 120
        });
    }

    /// <summary>
    /// Tests Fibonacci calculation.
    /// </summary>
    [Test]
    public void Algorithm_Fibonacci_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    fib(n: Int): Int {
        if n <= 1 then n else fib(n - 1) + fib(n - 2) fi
    };
    main(): Int { fib(10) };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Fibonacci should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("55")); // fib(10) = 55
        });
    }

    /// <summary>
    /// Tests inheritance and method override.
    /// </summary>
    [Test]
    public void Algorithm_InheritanceOverride_Succeeds()
    {
        // Arrange
        const string code = @"
class Animal {
    speak(): Int { 0 };
};

class Dog inherits Animal {
    speak(): Int { 1 };
};

class Cat inherits Animal {
    speak(): Int { 2 };
};

class Main {
    main(): Int {
        let d : Animal <- new Dog,
            c : Animal <- new Cat
        in d.speak() + c.speak()
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Inheritance override should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("3")); // Dog(1) + Cat(2) = 3
        });
    }

    /// <summary>
    /// Tests string operations.
    /// </summary>
    [Test]
    public void Algorithm_StringOperations_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let s : String <- ""Hello"" in s.length()
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"String operations should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("5"));
        });
    }

    /// <summary>
    /// Tests comparison operators.
    /// </summary>
    [TestCase("1 < 2", "true")]
    [TestCase("2 < 1", "false")]
    [TestCase("1 <= 1", "true")]
    [TestCase("2 <= 1", "false")]
    [TestCase("1 = 1", "true")]
    [TestCase("1 = 2", "false")]
    public void Algorithm_Comparisons_Succeeds(string expr, string expected)
    {
        // Arrange
        string code = $@"
class Main {{
    main(): Bool {{ {expr} }};
}};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Comparison '{expr}' should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo(expected));
        });
    }

    /// <summary>
    /// Tests boolean operations.
    /// </summary>
    [Test]
    public void Algorithm_BooleanNot_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Bool { not false };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Boolean NOT should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("true"));
        });
    }

    /// <summary>
    /// Tests isvoid operator.
    /// </summary>
    [Test]
    public void Algorithm_IsVoid_Succeeds()
    {
        // Arrange
        const string code = @"
class Foo { };
class Main {
    x : Foo;
    main(): Bool { isvoid x };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"isvoid should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("true"));
        });
    }

    /// <summary>
    /// Tests new operator with class initialization.
    /// </summary>
    [Test]
    public void Algorithm_NewWithInit_Succeeds()
    {
        // Arrange
        const string code = @"
class Counter {
    count : Int <- 10;
    get(): Int { count };
};

class Main {
    main(): Int { (new Counter).get() };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"New with init should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("10"));
        });
    }

    /// <summary>
    /// Tests self reference.
    /// </summary>
    [Test]
    public void Algorithm_SelfReference_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    value(): Int { 42 };
    main(): Int { self.value() };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Self reference should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("42"));
        });
    }

    /// <summary>
    /// Tests nested let bindings.
    /// </summary>
    [Test]
    public void Algorithm_NestedLet_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let x : Int <- 1 in
            let y : Int <- 2 in
                let z : Int <- 3 in
                    x + y + z
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"Nested let should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.ReturnedValue, Is.EqualTo("6"));
        });
    }

    /// <summary>
    /// Tests copy() method.
    /// </summary>
    [Test]
    public void Algorithm_Copy_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Bool {
        let x : Main <- self.copy() in
            not (x = self)
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Copy should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests type_name() method.
    /// </summary>
    [Test]
    public void Algorithm_TypeName_Succeeds()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): SELF_TYPE {
        out_string(type_name())
    };
};
";

        // Act
        var result = _interpreter.Run(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"type_name should succeed.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.Output, Does.Contain("Main"));
        });
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<string> GetAlgorithmFailFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Algorithm", "fail");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Algorithm/fail directory not found at {path}");
            return Enumerable.Empty<string>();
        }
        return Directory.GetFiles(path, "*.cl");
    }

    private static IEnumerable<string> GetAlgorithmSuccessFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Algorithm", "success");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Algorithm/success directory not found at {path}");
            return Enumerable.Empty<string>();
        }
        return Directory.GetFiles(path, "*.cl");
    }

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (!errors.Any())
            return "(no errors)";
        
        return string.Join("\n", errors.Select(d => $"  [{d.Code}] {d.Message}"));
    }

    #endregion
}
