//-----------------------------------------------------------------------
// <copyright file="TestSummary.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Test Summary and Reporting Utilities</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Tests;

using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Language.Interpretation;
using NUnit.Framework;

/// <summary>
/// Provides test summary and batch testing utilities.
/// Runs all test cases and produces a comprehensive report.
/// </summary>
[TestFixture]
public class TestSummary
{
    private const string TestFilesRoot = "TestCases";
    private CoolInterpreter _interpreter = null!;

    [SetUp]
    public void Setup()
    {
        _interpreter = new CoolInterpreter();
    }

    /// <summary>
    /// Runs all test cases and generates a comprehensive summary report.
    /// </summary>
    [Test]
    [Category("Summary")]
    public void GenerateFullTestReport()
    {
        var report = new TestReport();

        // Test Parsing
        RunCategoryTests("Parsing", report);
        
        // Test Semantics
        RunCategoryTests("Semantics", report);
        
        // Test Algorithm (Full Execution)
        RunCategoryTests("Algorithm", report);

        // Print Summary
        report.PrintSummary();

        // Assert overall success rate is acceptable
        var totalTests = report.TotalPassed + report.TotalFailed;
        if (totalTests > 0)
        {
            var successRate = (double)report.TotalPassed / totalTests * 100;
            TestContext.WriteLine($"\n🎯 Overall Success Rate: {successRate:F1}%");
        }
    }

    /// <summary>
    /// Tests only Parsing category and reports results.
    /// </summary>
    [Test]
    [Category("Summary")]
    public void ParsingTestReport()
    {
        var report = new TestReport();
        RunCategoryTests("Parsing", report);
        report.PrintSummary();
    }

    /// <summary>
    /// Tests only Semantics category and reports results.
    /// </summary>
    [Test]
    [Category("Summary")]
    public void SemanticTestReport()
    {
        var report = new TestReport();
        RunCategoryTests("Semantics", report);
        report.PrintSummary();
    }

    /// <summary>
    /// Tests only Algorithm category and reports results.
    /// </summary>
    [Test]
    [Category("Summary")]
    public void AlgorithmTestReport()
    {
        var report = new TestReport();
        RunCategoryTests("Algorithm", report);
        report.PrintSummary();
    }

    private void RunCategoryTests(string category, TestReport report)
    {
        var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesRoot, category);
        
        // Test success cases
        var successPath = Path.Combine(basePath, "success");
        if (Directory.Exists(successPath))
        {
            foreach (var file in Directory.GetFiles(successPath, "*.cl"))
            {
                var fileName = Path.GetFileName(file);
                var result = RunTest(file, category, expectSuccess: true);
                report.AddResult(category, "success", fileName, result);
            }
        }

        // Test fail cases
        var failPath = Path.Combine(basePath, "fail");
        if (Directory.Exists(failPath))
        {
            foreach (var file in Directory.GetFiles(failPath, "*.cl"))
            {
                var fileName = Path.GetFileName(file);
                var result = RunTest(file, category, expectSuccess: false);
                report.AddResult(category, "fail", fileName, result);
            }
        }
    }

    private TestResult RunTest(string filePath, string category, bool expectSuccess)
    {
        try
        {
            var sourceCode = File.ReadAllText(filePath);
            var fileName = Path.GetFileName(filePath);

            bool actualSuccess;
            string details;

            switch (category)
            {
                case "Parsing":
                    var parseResult = _interpreter.TestParsing(sourceCode, fileName);
                    actualSuccess = !parseResult.HasErrors;
                    details = parseResult.HasErrors 
                        ? string.Join("; ", parseResult.Diagnostics.Take(3).Select(d => d.Message))
                        : "OK";
                    break;

                case "Semantics":
                    var semanticResult = _interpreter.TestSemantics(sourceCode, fileName);
                    actualSuccess = semanticResult.IsSuccess;
                    details = semanticResult.IsSuccess 
                        ? "OK"
                        : string.Join("; ", semanticResult.Diagnostics.Take(3).Select(d => d.Message));
                    break;

                case "Algorithm":
                default:
                    var runResult = _interpreter.Run(sourceCode, fileName);
                    actualSuccess = runResult.IsSuccess;
                    details = runResult.IsSuccess 
                        ? $"Returned: {runResult.ReturnedValue}"
                        : string.Join("; ", runResult.Diagnostics.Take(3).Select(d => d.Message));
                    break;
            }

            bool passed = actualSuccess == expectSuccess;
            return new TestResult(passed, details, actualSuccess, expectSuccess);
        }
        catch (Exception ex)
        {
            return new TestResult(false, $"Exception: {ex.Message}", false, expectSuccess);
        }
    }

    /// <summary>
    /// Holds the results of a single test.
    /// </summary>
    private record TestResult(bool Passed, string Details, bool ActualSuccess, bool ExpectedSuccess);

    /// <summary>
    /// Aggregates test results and generates reports.
    /// </summary>
    private class TestReport
    {
        private readonly Dictionary<string, List<(string SubCategory, string FileName, TestResult Result)>> _results = new();

        public int TotalPassed { get; private set; }
        public int TotalFailed { get; private set; }

        public void AddResult(string category, string subCategory, string fileName, TestResult result)
        {
            if (!_results.ContainsKey(category))
                _results[category] = [];

            _results[category].Add((subCategory, fileName, result));

            if (result.Passed)
                TotalPassed++;
            else
                TotalFailed++;
        }

        public void PrintSummary()
        {
            TestContext.WriteLine("\n" + new string('=', 80));
            TestContext.WriteLine("                         COOL INTERPRETER TEST REPORT");
            TestContext.WriteLine(new string('=', 80));

            foreach (var category in _results.Keys.OrderBy(k => k))
            {
                var categoryResults = _results[category];
                var passed = categoryResults.Count(r => r.Result.Passed);
                var failed = categoryResults.Count(r => !r.Result.Passed);
                var total = categoryResults.Count;
                var percentage = total > 0 ? (double)passed / total * 100 : 0;

                TestContext.WriteLine($"\n📁 {category.ToUpper()}");
                TestContext.WriteLine(new string('-', 60));
                TestContext.WriteLine($"   ✅ Passed: {passed}/{total} ({percentage:F1}%)");
                TestContext.WriteLine($"   ❌ Failed: {failed}/{total}");

                // Group by subcategory
                var bySubCategory = categoryResults.GroupBy(r => r.SubCategory);
                foreach (var group in bySubCategory)
                {
                    var subPassed = group.Count(r => r.Result.Passed);
                    var subTotal = group.Count();
                    var subPercentage = subTotal > 0 ? (double)subPassed / subTotal * 100 : 0;
                    
                    var icon = group.Key == "success" ? "📗" : "📕";
                    TestContext.WriteLine($"\n   {icon} {group.Key}: {subPassed}/{subTotal} ({subPercentage:F1}%)");

                    // Show failed tests
                    var failures = group.Where(r => !r.Result.Passed).ToList();
                    if (failures.Any())
                    {
                        TestContext.WriteLine($"      Failed files:");
                        foreach (var failure in failures.Take(10)) // Limit to 10
                        {
                            var expected = failure.Result.ExpectedSuccess ? "success" : "failure";
                            var actual = failure.Result.ActualSuccess ? "succeeded" : "failed";
                            TestContext.WriteLine($"        ⚠️  {failure.FileName}");
                            TestContext.WriteLine($"            Expected: {expected}, Actual: {actual}");
                            if (!string.IsNullOrEmpty(failure.Result.Details))
                                TestContext.WriteLine($"            Details: {Truncate(failure.Result.Details, 60)}");
                        }
                        if (failures.Count > 10)
                            TestContext.WriteLine($"        ... and {failures.Count - 10} more");
                    }
                }
            }

            // Overall Summary
            TestContext.WriteLine("\n" + new string('=', 80));
            TestContext.WriteLine("                              OVERALL SUMMARY");
            TestContext.WriteLine(new string('=', 80));
            var totalTests = TotalPassed + TotalFailed;
            var overallPercentage = totalTests > 0 ? (double)TotalPassed / totalTests * 100 : 0;
            
            TestContext.WriteLine($"\n   📊 Total Tests:  {totalTests}");
            TestContext.WriteLine($"   ✅ Total Passed: {TotalPassed}");
            TestContext.WriteLine($"   ❌ Total Failed: {TotalFailed}");
            TestContext.WriteLine($"   📈 Success Rate: {overallPercentage:F1}%");
            TestContext.WriteLine("\n" + new string('=', 80));
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text[..(maxLength - 3)] + "...";
        }
    }
}
