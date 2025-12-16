using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

namespace Cool.Interpreter.Tests;

public partial class Tests
{
    
    private static readonly Dictionary<string, string> TestCases = [];
    
    [SetUp]
    public void Setup()
    {
    }


    private static int EvaluateResult(string result)
    {
        var match = EvaluateParseResultRegex().Match(result);
        return result.Contains("failed with") || result.Contains("error") ? 0 : int.Parse(match.Value);
    }
    
    /// <summary>
    /// Expected result is greater than 0 indicating successful parse and execution
    /// </summary>
    [Test]
    public void TestSingleParseSuccess()
    {
        var input = File.ReadAllText("TestCases/Parsing/success/abort.cl");
        var result = RunInterpreter(input, Path.GetFileNameWithoutExtension(input));
        Console.WriteLine("Result: " + result);

        Assert.Multiple(() =>
        {
            Assert.That(EvaluateResult(result.ToString()), Is.GreaterThan(0));
            Assert.That(result.IsSuccess, Is.True);
        });
    }
    
    /// <summary>
    /// Expected result is 0 due to parse failure or a number of errors
    /// </summary>
    [Test]
    public void TestSingleParseFail()
    {
        var input = File.ReadAllText("TestCases/Parsing/fail/baddispatch2.cl");
        var result = RunInterpreter(input, Path.GetFileNameWithoutExtension(input));
        Console.WriteLine("Result: " + result);
        Assert.Multiple(() =>
        {
            Assert.That(EvaluateResult(result.ToString()), Is.EqualTo(0));
            Assert.That(result.IsSuccess, Is.False);
        });
    }
    
    /// <summary>
    /// Automatic testing of all test cases in a given category ["Algorithm", "Parsing", "Semantics"]
    /// </summary>
    [Test]
    public void AutomaticTesting()
    {
        const string cat = "Parsing";
        var testCases = LoadTestCaseByCategory(cat);
        foreach (var testCase in testCases)
        {
            // during debugging: switch between only successful or failing cases
            // if (testCase.Value.Contains("success")) continue;
            
            Console.Write("Running test case: " + testCase.Key);
            
            var input = File.ReadAllText(testCase.Key);
            var expectedResult = testCase.Value == "success";
            var actualResult = RunInterpreter(input);
            
            Assert.Multiple(() =>
            {
                switch (expectedResult)
                {
                    case false:
                        Assert.That(EvaluateResult(actualResult.ToString()), Is.EqualTo(0), 
                            $"Test case '{testCase.Key}' expected to fail.\nResult: {actualResult}");
                        Assert.That(actualResult.IsSuccess, Is.False);
                        Console.WriteLine(" - Passed");
                        break;
                    case true:
                        Assert.That(EvaluateResult(actualResult.ToString()), Is.GreaterThan(0), 
                            $"Test case '{testCase.Key}' expected to succeed. \nResult: {actualResult}");
                        // Assert.That(actualResult.IsSuccess, Is.True);
                        Console.WriteLine(" - Passed");
                        break;
                }
            });
        }
    }
    
    private static Dictionary<string, string> LoadTestCaseByCategory(string category)
    {
        TestCases.Clear();
        var testCases = Directory.GetFiles("TestCases", "*.cl", SearchOption.AllDirectories);
        foreach (var testCase in testCases)
        {
            // get parent-parent directory name as category
            var parentParentDir = Directory.GetParent(Directory.GetParent(testCase)?.FullName ?? "")?.Name ?? "unknown";
            // get parent directory name as test status
            var testStatus = Directory.GetParent(testCase)?.Name ?? "unknown";
            if (parentParentDir == category)
            {
                TestCases.Add(testCase, testStatus);
            }
        }
        return TestCases;
    }
    
    private static InterpretationResult RunInterpreter(string input, string? sourceName = null)
    {
        try
        {
            var interpreter = new CoolInterpreter();
            return interpreter.Run(input, sourceName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Get the number of output characters for successful parsing or 0 if failed
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?<=Output length:\s)\d+|(?<=failed with )\d+")]
    private static partial Regex EvaluateParseResultRegex();
}