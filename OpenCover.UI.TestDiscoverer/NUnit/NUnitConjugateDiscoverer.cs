using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using OpenCover.UI.Model.Test;

namespace OpenCover.UI.TestDiscoverer.NUnit
{
    internal class NUnitConjugateDiscoverer : DiscovererBase
    {
        const string nunit3ResultSeparator = "Test Files";
        readonly string nunit3ConsoleExePath;

        public NUnitConjugateDiscoverer(IEnumerable<string> dlls, string nunit3ConsoleExePath)
            : base(dlls)
        {
            this.nunit3ConsoleExePath = nunit3ConsoleExePath;
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

        //private static IList<string> GetUnitTestCasesWithNunitDriver(string dllPath, IFrameworkDriver nunitDriver)
        //{
        //    nunitDriver.Load(dllPath, new Dictionary<string, object>());

        // var testCasesXmlString = nunitDriver.Explore("");

        // XmlDocument doc = new XmlDocument();

        // doc.LoadXml(testCasesXmlString);

        // var testCasesXml = doc.FirstChild;

        // var testCases = testCasesXml.SelectNodes("//test-case");

        // var result = new List<string>();

        // foreach (XmlNode tc in testCases) result.Add(tc.Attributes.GetNamedItem("fullname").Value);

        //    return result;
        //}

        //private ISet<string> GetNunitTestCasesFromDll(string dllPath)
        //{
        //    string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    var result = GetUnitTestCasesWithNunitDriver(dllPath, new NUnit3FrameworkDriver(AppDomain.CurrentDomain));

        // //result.Concat(GetUnitTestCasesWithNunitDriver(dllPath, new NUnit2FrameworkDriver(AppDomain.CurrentDomain)));

        //    return new HashSet<string>(result.Distinct());
        //}

        private ISet<string> GetNunitTestCasesFromDll(string dllPath)
        {
            StringCollection consoleOutputLines = new StringCollection();

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = nunit3ConsoleExePath,
                    Arguments = string.Format("{0} {1}", dllPath, "--explore --noresult --trace=Off --dispose-runners --inprocess"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.OutputDataReceived += (s, e) =>
            {
                lock (consoleOutputLines)
                {
                    if (e.Data != null)
                        consoleOutputLines.Add(e.Data);
                }
            };

            proc.Start();

            proc.BeginOutputReadLine();

            proc.WaitForExit();

            var result = new List<string>();

            for (int i = consoleOutputLines.Count - 1; i > 0; i--)
            {
                var value = consoleOutputLines[i];

                if (string.IsNullOrEmpty(value))
                    break;

                result.Add(value);
            }

            return new HashSet<string>(result.Distinct());
        }
    }
}