using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.TestResources.NUnit2
{
    public class TestFixtureWithoutExplicitTextFixtureAttributeWithTestCaseSource
    {
        public static object[] TestCaseSourceForTests = new object[]
                {
                   new object [] { "testDataFromTestCaseSource1" },
                   new object[]{ "testDataFromTestCaseSource2" },
                   new object[] { "testDataFromTestCaseSource3" },
                };

        [TestCaseSource("TestCaseSourceForTests")]
        public void SomeTestCasesFromTestCaseSource(string testDataFromTestCaseSource)
        {
        }
    }
}