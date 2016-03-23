using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.TestResources.NUnit2
{
    [TestFixture]
    public class TestFixtureWithNamedTestCases
    {
        [TestCase(true, TestName = "TestSomethingTrue")]
        [TestCase(true, TestName = "TestSomethingTrue2")]
        public void SomeNamedTestCase(bool input) { }
    }
}