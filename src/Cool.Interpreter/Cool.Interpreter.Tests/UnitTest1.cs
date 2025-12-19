//-----------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Legacy Unit Tests - See new test files for organized tests</summary>
//-----------------------------------------------------------------------

// ============================================================================
// NOTE: This file contains legacy tests. 
// For organized tests, see:
//   - ParsingTests.cs    : Syntax/parsing validation
//   - SemanticTests.cs   : Type checking and semantic analysis
//   - RuntimeTests.cs    : Runtime execution and error handling
//   - AlgorithmTests.cs  : Full program execution tests
//   - TestSummary.cs     : Batch testing with summary reports
// ============================================================================

using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

namespace Cool.Interpreter.Tests;

/// <summary>
/// Legacy test class - kept for backward compatibility.
/// New tests should be added to the appropriate specialized test class.
/// </summary>
[TestFixture]
public partial class LegacyTests
{
    private const string TestFilesRoot = "TestCases";
    private CoolInterpreter _interpreter = null!;

    [SetUp]
    public void Setup()
    {
        _interpreter = new CoolInterpreter();
    }

    /// <summary>
    /// Quick smoke test to verify the interpreter is working.
    /// </summary>
    [Test]
    public void SmokeTest_SimpleProgram_Succeeds()
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
            Assert.That(result.IsSuccess, Is.True, "Simple program should succeed");
            Assert.That(result.ReturnedValue, Is.EqualTo("7"));
        });
    }

    /// <summary>
    /// Quick smoke test for parsing.
    /// </summary>
    [Test]
    public void SmokeTest_Parsing_Succeeds()
    {
        // Arrange
        const string code = @"class Main { main(): Int { 0 }; };";

        // Act
        var result = _interpreter.TestParsing(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    /// <summary>
    /// Quick smoke test for semantic analysis.
    /// </summary>
    [Test]
    public void SmokeTest_Semantics_Succeeds()
    {
        // Arrange
        const string code = @"class Main { main(): Int { 0 }; };";

        // Act
        var result = _interpreter.TestSemantics(code);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }
}
