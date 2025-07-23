using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)] //Disable parallel test execution to prevent issues with the WireMock server

namespace UnitTests;

public class AlphabeticalOrderer: ITestCaseOrderer
{
  public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
  {
    return testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
  }
}