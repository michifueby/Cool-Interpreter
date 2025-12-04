using Antlr4.Runtime;
using NUnit.Framework;

namespace Cool.Interpreter.Tests;

public class Tests
{
    
    private static readonly Dictionary<string, string> TestCases = [];
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
    
    [Test]
    public void ManualTest()
    {
        var cat = "Algorithm";
        var testCases = LoadTestCaseByCategory(cat);
        foreach (var testCase in testCases)
        {
            var input = File.ReadAllText(testCase.Key);
            var expectedResult = testCase.Value == "success" ? 0 : -1;
            var actualResult = RunInterpreter(input);
            Console.WriteLine($"Test case: {testCase}, result: {actualResult}, expected: {expectedResult}");
            
            // Assert.That(expectedResult, Is.EqualTo(actualResult));
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
    
    private static int RunInterpreter(string input)
    {
        try
        {
            var stream = CharStreams.fromString(input);
            var lexer = new CoolLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CoolParser(tokens);
            var tree = parser.program();
            
            var visitor = new CoolBaseVisitor<int>(); // TODO: Replace with actual interpreter visitor
            return visitor.Visit(tree);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
}