using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Mono.Cecil;
using NUnit.Engine.Drivers;
using NUnit.Engine.Extensibility;
using OpenCover.UI.Model.Test;

namespace OpenCover.UI.TestDiscoverer.NUnit
{
    internal class NUnitConjugateDiscoverer : DiscovererBase
    {
        const string nunitTestLabelMarker = "*****";

        public NUnitConjugateDiscoverer(IEnumerable<string> dlls)
            : base(dlls)
        {
        }

        protected override List<TestClass> DiscoverTestsInAssembly(string dllPath, AssemblyDefinition assembly)
        {
            var result = new List<TestClass>();

            var testCases = GetNunitTestCasesFromDll(dllPath);

            foreach (var type in assembly.MainModule.Types)
            {
                var testCasesInClass = testCases.Where(p => p.StartsWith(type.FullName));

                if (testCasesInClass.Count() > 0)
                {
                    var testClassToAdd = new TestClass()
                    {
                        TestType = TestType.NUnit,
                        DLLPath = dllPath,
                        Name = type.Name,
                        Namespace = type.Namespace,
                    };

                    var testMethodsToAdd = new List<TestMethod>();

                    foreach (string testCase in testCasesInClass)
                        testMethodsToAdd.Add(new TestMethod() { Name = testCase.Substring(type.FullName.Length + 1), Traits = new[] { "No Traits" } });

                    testClassToAdd.TestMethods = testMethodsToAdd.ToArray();

                    result.Add(testClassToAdd);
                }
            }

            return result;
        }

        private static IList<string> GetUnitTestCasesWithNunitDriver(string dllPath, IFrameworkDriver nunitDriver)
        {
            nunitDriver.Load(dllPath, new Dictionary<string, object>());

            var testCasesXmlString = nunitDriver.Explore("");

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(testCasesXmlString);

            var testCasesXml = doc.FirstChild;

            var testCases = testCasesXml.SelectNodes("//test-case");

            var result = new List<string>();

            foreach (XmlNode tc in testCases)
                result.Add(tc.Attributes.GetNamedItem("fullname").Value);

            return result;
        }

        private ISet<string> GetNunitTestCasesFromDll(string dllPath)
        {
            var result = GetUnitTestCasesWithNunitDriver(dllPath, new NUnit3FrameworkDriver(AppDomain.CurrentDomain));

            result.Concat(GetUnitTestCasesWithNunitDriver(dllPath, new NUnit2FrameworkDriver(AppDomain.CurrentDomain)));

            return new HashSet<string>(result.Distinct());
        }
    }
}