using System.Collections.Generic;
using NUnit.Core;

namespace Examples
{
    public static class TestResultExtensions
    {
        public static IEnumerable<TestResult> FindFailedTests(this TestResult testResult)
        {
            if (testResult.FailureSite == FailureSite.Test)
                return new[] { testResult };

            var failedTests = new List<TestResult>();
            foreach (var childResult in testResult.Results)
                failedTests.AddRange(FindFailedTests((TestResult)childResult));
            return failedTests;
        }

        public static IEnumerable<TestResult> FindAllTests(this TestResult testResult)
        {
            if (testResult.Results == null || testResult.Results.Count == 0)
                return new[] { testResult };

            var tests = new List<TestResult>();
            foreach (var childResult in testResult.Results)
                tests.AddRange(FindAllTests((TestResult)childResult));
            return tests;
        }
    }
}