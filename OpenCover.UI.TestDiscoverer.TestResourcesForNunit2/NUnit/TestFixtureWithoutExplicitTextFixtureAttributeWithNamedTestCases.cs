using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.TestResources.NUnit
{
    internal class TestFixtureWithoutExplicitTextFixtureAttributeWithNamedTestCases
    {
        [TestCase(true, TestName = "TestSomethingTrue")]
        [TestCase(true, TestName = "TestSomethingTrue2")]
        public void SomeNamedTestCase(bool input) { }
    }
}