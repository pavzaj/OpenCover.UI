using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.TestResources.NUnit2
{
    internal class TestFixtureWithoutExplicitTextFixtureAttributeWithNamedTestCases
    {
        [TestCase(true, TestName = "TestSomethingTrue")]
        [TestCase(true, TestName = "TestSomethingTrue2")]
        public void SomeNamedTestCase(bool input) { }
    }
}