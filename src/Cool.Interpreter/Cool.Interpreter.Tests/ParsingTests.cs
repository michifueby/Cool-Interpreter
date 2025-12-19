//-----------------------------------------------------------------------
// <copyright file="ParsingTests.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Parsing Tests</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Tests;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

/// <summary>
/// Test suite for parsing validation.
/// Tests syntax analysis using ANTLR-generated parser.
/// </summary>
[TestFixture]
public class ParsingTests
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
    /// Tests that files in Parsing/fail directory produce parse errors.
    /// </summary>
    [TestCaseSource(nameof(GetParsingFailFiles))]
    public void Parse_InvalidFile_ReturnsFailure(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.TestParsing(sourceCode, fileName);

        // Assert
        Assert.That(result.HasErrors, Is.True,
            $"File '{fileName}' should have parse errors but passed.\n" +
            $"Diagnostics: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests that files in Parsing/success directory parse successfully.
    /// </summary>
    [TestCaseSource(nameof(GetParsingSuccessFiles))]
    public void Parse_ValidFile_Succeeds(string filePath)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);

        // Act
        var result = _interpreter.TestParsing(sourceCode, fileName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True,
                $"File '{fileName}' should parse successfully.\n" +
                $"Errors: {FormatDiagnostics(result.Diagnostics)}");
            Assert.That(result.SyntaxTree, Is.Not.Null,
                "Syntax tree should not be null for successful parse");
        });
    }

    #endregion

    #region Specific Syntax Tests

    /// <summary>
    /// Tests parsing of a minimal valid program.
    /// </summary>
    [Test]
    public void Parse_MinimalProgram_Succeeds()
    {
        // Arrange
        const string code = @"class Main { main(): Int { 0 }; };";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.SyntaxTree, Is.Not.Null);
            Assert.That(result.SyntaxTree!.Classes, Has.Count.EqualTo(1));
        });
    }

    /// <summary>
    /// Tests parsing of class with inheritance.
    /// </summary>
    [Test]
    public void Parse_ClassWithInheritance_Succeeds()
    {
        // Arrange
        const string code = @"
class Foo inherits Object { };
class Main { main(): Int { 0 }; };
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.SyntaxTree!.Classes, Has.Count.EqualTo(2));
        });
    }

    /// <summary>
    /// Tests parsing of method with parameters.
    /// </summary>
    [Test]
    public void Parse_MethodWithParameters_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    add(x: Int, y: Int): Int { x + y };
    main(): Int { add(1, 2) };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Method with parameters should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of let expressions.
    /// </summary>
    [Test]
    public void Parse_LetExpression_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        let x : Int <- 5, y : Int <- 10 in x + y
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Let expression should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of case expressions.
    /// </summary>
    [Test]
    public void Parse_CaseExpression_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        case self of
            m : Main => 1;
            o : Object => 0;
        esac
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Case expression should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of while loops.
    /// </summary>
    [Test]
    public void Parse_WhileLoop_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Object {
        while true loop 0 pool
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"While loop should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of if-then-else.
    /// </summary>
    [Test]
    public void Parse_IfThenElse_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        if true then 1 else 0 fi
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"If-then-else should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of block expressions.
    /// </summary>
    [Test]
    public void Parse_BlockExpression_Succeeds()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        {
            1;
            2;
            3;
        }
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Block expression should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of dispatch expressions.
    /// </summary>
    [Test]
    public void Parse_DispatchExpressions_Succeeds()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): SELF_TYPE {
        out_string(""test"").out_int(42)
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Dispatch expressions should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    /// <summary>
    /// Tests parsing of static dispatch.
    /// </summary>
    [Test]
    public void Parse_StaticDispatch_Succeeds()
    {
        // Arrange
        const string code = @"
class Main inherits IO {
    main(): Object {
        self@IO.out_string(""test"")
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True,
            $"Static dispatch should parse.\nErrors: {FormatDiagnostics(result.Diagnostics)}");
    }

    #endregion

    #region Syntax Error Tests

    /// <summary>
    /// Tests that missing semicolon is detected.
    /// </summary>
    [Test]
    public void Parse_MissingSemicolon_Fails()
    {
        // Arrange - missing ; after class
        const string code = @"class Main { main(): Int { 0 }; }";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.HasErrors, Is.True,
            "Missing semicolon should cause parse error");
    }

    /// <summary>
    /// Tests that missing class keyword is detected.
    /// </summary>
    [Test]
    public void Parse_MissingClassKeyword_Fails()
    {
        // Arrange
        const string code = @"Main { main(): Int { 0 }; };";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.HasErrors, Is.True,
            "Missing class keyword should cause parse error");
    }

    /// <summary>
    /// Tests that unclosed block is detected.
    /// </summary>
    [Test]
    public void Parse_UnclosedBlock_Fails()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        { 1; 2;
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.HasErrors, Is.True,
            "Unclosed block should cause parse error");
    }

    /// <summary>
    /// Tests that if without fi is detected.
    /// </summary>
    [Test]
    public void Parse_IfWithoutFi_Fails()
    {
        // Arrange
        const string code = @"
class Main {
    main(): Int {
        if true then 1 else 0
    };
};
";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.HasErrors, Is.True,
            "If without fi should cause parse error");
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<string> GetParsingFailFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Parsing", "fail");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Parsing/fail directory not found at {path}");
            return Enumerable.Empty<string>();
        }
        return Directory.GetFiles(path, "*.cl");
    }

    private static IEnumerable<string> GetParsingSuccessFiles()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Parsing", "success");
        if (!Directory.Exists(path))
        {
            TestContext.WriteLine($"Warning: Parsing/success directory not found at {path}");
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
