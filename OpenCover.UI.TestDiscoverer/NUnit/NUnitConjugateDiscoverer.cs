﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using OpenCover.UI.Model.Test;

namespace OpenCover.UI.TestDiscoverer.NUnit
{
    internal class NUnitConjugateDiscoverer : DiscovererBase
    {
        const string nunitTestLabelMarker = "*****";
        string nunitConsolePath;

        public NUnitConjugateDiscoverer(IEnumerable<string> dlls, string nunitConsolePath)
            : base(dlls)
        {
            this.nunitConsolePath = nunitConsolePath;
        }

        protected override List<TestClass> DiscoverTestsInAssembly(string dllPath, AssemblyDefinition assembly)
        {
            var result = new List<TestClass>();

            System.Console.WriteLine("Detecting Nunit tests");

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
<<<<<<< HEAD
                        testMethodsToAdd.Add(new TestMethod() { Name = testCase.Substring(type.FullName.Length + 1), Traits = new[] { "No Traits" } });
=======
                        testMethodsToAdd.Add(new TestMethod() { Name = testCase.Substring(type.FullName.Length + 1) });
>>>>>>> c6cf78833f29c47959437034b60cab9ce6fc0f3d

                    testClassToAdd.TestMethods = testMethodsToAdd.ToArray();

                    result.Add(testClassToAdd);
                }
            }

            return result;
        }

        private IList<string> GetNunitTestCasesFromDll(string dllPath)
        {
            StringCollection values = new StringCollection();

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