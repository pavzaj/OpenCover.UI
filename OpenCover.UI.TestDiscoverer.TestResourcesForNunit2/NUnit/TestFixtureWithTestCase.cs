using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.TestResources.NUnit
{
    [TestFixture]
    public class TestFixtureWithTestCases
    {
        [TestCase(false)]
        [TestCase(true)]
        public void SomeTestCase(bool input)
        {
        }
    }
}