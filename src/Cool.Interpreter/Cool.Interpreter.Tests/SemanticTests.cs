//-----------------------------------------------------------------------
// <copyright file="SemanticTests.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Semantic Analysis Tests</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Tests;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

/// <summary>
/// Test suite for semantic analysis validation.
/// Tests type checking, inheritance validation, and symbol resolution.
/// </summary>
[TestFixture]
public class SemanticTests
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
    /// Tests that files in Semantics/fail directory produce semantic errors.
    /// </summary>
    [TestCaseSource(nameof(GetSemanticFailFiles))]
    public void Semantic_InvalidFile_ReturnsErrors(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.TestSemantics(sourceCode, fileName);

        // Assert
        Assert.That(result.IsSuccess, Is.False,
            $"File '{fileName}' should have semantic errors but passed.\n" +
            $"Diagnostics: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests that files in Semantics/success directory pass semantic analysis.
    /// </summary>
    [TestCaseSource(nameof(GetSemanticSuccessFiles))]
    public void Semantic_ValidFile_Succeeds(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.TestSemantics(sourceCode, fileName);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"File '{fileName}' should pass semantic analysis.\n" +
            $"Errors: {FormatDiagnostics(result.Diagnostics)}");
    }

    #endregion

    #region Specific Semantic Error Tests

    /// <summary>
    /// Tests that duplicate class definitions are detected.
    /// </summary>
    [Test]
    public void Semantic_DuplicateClass_ReportsError()
    {
        // Arrange
        const string code = @"
class Foo { };
class Foo { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.DuplicateClass), Is.True,
                "Should report duplicate class error");
        });
    }

    /// <summary>
    /// Tests that redefining built-in classes is detected.
    /// </summary>
    [Test]
    public void Semantic_RedefineBuiltinClass_ReportsError()
    {
        // Arrange
        const string code = @"
class Int { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.RedefineBuiltin), Is.True,
                "Should report redefine builtin error");
        });
    }

    /// <summary>
    /// Tests that missing Main class is detected.
    /// </summary>
    [Test]
    public void Semantic_MissingMainClass_ReportsError()
    {
        // Arrange
        const string code = @"
class Foo { };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.MissingMain), Is.True,
                "Should report missing Main class error");
        });
    }

    /// <summary>
    /// Tests that inheriting from primitive types (Int, String, Bool) is detected.
    /// </summary>
    [TestCase("Int")]
    [TestCase("String")]
    [TestCase("Bool")]
    public void Semantic_InheritFromPrimitive_ReportsError(string primitiveType)
    {
        // Arrange
        string code = $@"
class Foo inherits {primitiveType} {{ }};
class Main {{ main(): Int {{ 0 }}; }};
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.InheritFromPrimitive), Is.True,
                $"Should report inherit from primitive ({primitiveType}) error");
        });
    }

    /// <summary>
    /// Tests that inheriting from undefined parent is detected.
    /// </summary>
    [Test]
    public void Semantic_UndefinedParent_ReportsError()
    {
        // Arrange
        const string code = @"
class Foo inherits NonExistent { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.UndefinedParent), Is.True,
                "Should report undefined parent error");
        });
    }

    /// <summary>
    /// Tests that inheritance cycles are detected.
    /// </summary>
    [Test]
    public void Semantic_InheritanceCycle_ReportsError()
    {
        // Arrange
        const string code = @"
class A inherits B { };
class B inherits C { };
class C inherits A { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.InheritanceCycle), Is.True,
                "Should report inheritance cycle error");
        });
    }

    /// <summary>
    /// Tests that undefined variables are detected.
    /// </summary>
    [Test]
    public void Semantic_UndefinedVariable_ReportsError()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int { undefinedVar };
};
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Diagnostics.Any(d => d.Code == CoolErrorCodes.UndefinedVariable), Is.True,
                "Should report undefined variable error");
        });
    }

    #endregion

    #region Valid Semantic Tests

    /// <summary>
    /// Tests that valid inheritance from Object works.
    /// </summary>
    [Test]
    public void Semantic_InheritFromObject_Succeeds()
    {
        // Arrange
        const string code = @"
class Foo inherits Object { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Should allow inheritance from Object.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests that valid inheritance from IO works.
    /// </summary>
    [Test]
    public void Semantic_InheritFromIO_Succeeds()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): SELF_TYPE { out_string(""Hello"") };
};
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Should allow inheritance from IO.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests that valid let bindings work.
    /// </summary>
    [Test]
    public void Semantic_ValidLetBinding_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let x : Int <- 5 in x + 1
    };
};
";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Should accept valid let binding.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<string> GetSemanticFailFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Semantics", "fail");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Semantics/fail directory not found at {path}");
            return Enumerable.Empty<string>();
        }
        return Directory.GetFiles(path, "*.cl");
    }

    private static IEnumerable<string> GetSemanticSuccessFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Semantics", "success");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Semantics/success directory not found at {path}");
            return Enumerable.Empty<string>();
        }
        return Directory.GetFiles(path, "*.cl");
    }

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (!errors.Any())
            return "(no errors)";
        
        return string.Join("\n", errors.Select(d => $"  [{d.Code}] {d.Message} at {d.Location}"));
    }

    #endregion
}
