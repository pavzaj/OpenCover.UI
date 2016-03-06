using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Engine.Drivers;
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

        private IList<string> GetNunitTestCasesFromDll(string dllPath)
        {
            StringCollection values = new StringCollection();
            var driv = new NUnit3FrameworkDriver(AppDomain.CurrentDomain);

            driv.Load(dllPath, new Dictionary<string, object>());

            var testCasesXml = driv.Explore("");

            var nunitConsolePath = new FileInfo(Assembly.GetAssembly(this.GetType()).Location).Directory + "\\nunit-console.exe";

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = nunitConsolePath,
                    Arguments = string.Format("{0} {1}", dllPath, "/labels /nologo /noshadow /timeout=1"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.OutputDataReceived += (s, e) =>
            {
                lock (values)
                {
                    if (e.Data != null && e.Data.StartsWith(nunitTestLabelMarker))
                        values.Add(e.Data.Split(' ')[1]);
                }
            };

            proc.Start();

            proc.BeginOutputReadLine();

            proc.WaitForExit();

            var result = new List<string>();

            foreach (var line in values)
                result.Add(line);

            return result;
        }
    }
}