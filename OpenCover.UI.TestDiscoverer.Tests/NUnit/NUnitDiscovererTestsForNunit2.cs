using System;
using NUnit.Framework;
using OpenCover.UI.TestDiscoverer.TestResources.NUnit2;

namespace OpenCover.UI.TestDiscoverer.Tests.NUnit
{
    public class NUnitDiscovererTestsForNunit2 : DiscovererTestsBase
    {
        [TestCase(typeof(RegularTestFixture), "RegularTestMethod")]
        [TestCase(typeof(TestFixtureWithoutExplicitTestFixtureAttribute), "TestMethodInTestFixtureWithoutExplicitTestFixtureAttribute")]
        [TestCase(typeof(TestFixtureWithTestCases), "SomeTestCase(True)", "SomeTestCase(False)")]
        [TestCase(typeof(TestFixtureWithNamedTestCases), "TestSomethingTrue", "TestSomethingTrue2")]
        [TestCase(typeof(TestFixtureWithoutExplicitTextFixtureAttributeWithTestCaseSource), "SomeTestCasesFromTestCaseSource(\"testDataFromTestCaseSource1\")", "SomeTestCasesFromTestCaseSource(\"testDataFromTestCaseSource2\")", "SomeTestCasesFromTestCaseSource(\"testDataFromTestCaseSource3\")")]
        public void Discover_Finds_All_Test_Cases_In_Assembly(Type testFixtureInAssemblyToDiscoverTestsIn, params string[] expectedNameOfFirstTestMethod)
        {
            AssertDiscoveredMethod(testFixtureInAssemblyToDiscoverTestsIn, expectedNameOfFirstTestMethod);
        }
    }
}