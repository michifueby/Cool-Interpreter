using System;
using System.Collections.Generic;
using System.IO;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

namespace Cool.Interpreter.Tests;

[TestFixture]
public class ParserTests
{
    private CoolInterpreter _interpreter;
    private const string TestFilesRoot = "TestCases";

    [SetUp]
    public void Setup()
    {
        _interpreter = new CoolInterpreter();
    }

    [TestCaseSource(nameof(GetSuccessParsingFiles))]
    public void Parse_ValidFile_ReturnsSuccess(string filePath, string fileName)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);

        // Act
        var result = _interpreter.TestParsing(sourceCode, fileName);

        if (result.HasErrors)
            Console.WriteLine(string.Join(Environment.NewLine, result.Diagnostics));

        // Assert
        Assert.That(result.HasErrors, Is.False, $"File '{fileName}' should parse successfully but had errors.");
    }

    [TestCaseSource(nameof(GetFailedParsingFiles))]
    public void Parse_InvalidFile_ReturnsFailure(string filePath, string fileName)
    {
        // Arrange
        string sourceCode = File.ReadAllText(filePath);

        // Act
        var result = _interpreter.TestParsing(sourceCode, fileName);

        // Assert
        Assert.That(result.HasErrors, Is.True, $"File '{fileName}' should fail parsing but succeeded.");
    }

    private static IEnumerable<TestCaseData> GetSuccessParsingFiles()
    {
        return GetFiles("success");
    }

    private static IEnumerable<TestCaseData> GetFailedParsingFiles()
    {
        return GetFiles("fail");
    }

    private static IEnumerable<TestCaseData> GetFiles(string subfolder)
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, "Parsing", subfolder);
        if (!Directory.Exists(path))
            yield break;

        foreach (var file in Directory.GetFiles(path, "*.cl"))
        {
            var fileName = Path.GetFileName(file);
            yield return new TestCaseData(file, fileName).SetName($"{subfolder}_{fileName}");
        }
    }
}
