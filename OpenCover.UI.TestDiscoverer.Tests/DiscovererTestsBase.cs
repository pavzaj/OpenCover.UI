﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace OpenCover.UI.TestDiscoverer.Tests
{
    [TestFixture]
    public abstract class DiscovererTestsBase
    {
        protected void AssertDiscoveredMethod(Type testFixtureInAssemblyToDiscoverTestsIn, params string[] expectedTestMethodsName)
        {
            // Arrange
            var discoverer = new Discoverer(new List<string> { testFixtureInAssemblyToDiscoverTestsIn.Assembly.Location }, @"..\..\..\packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe");

            // Act
            var discoveredTests = discoverer.Discover();

            // Assert
            discoveredTests.Should().NotBeNullOrEmpty();

            var discoveredTest = discoveredTests.SingleOrDefault(x => x.Name == testFixtureInAssemblyToDiscoverTestsIn.Name);
            discoveredTest.Should().NotBeNull();

            var discoveredMethodsNames = discoveredTest.TestMethods.Select(p => p.Name);

            foreach (var expectedTestMethodName in expectedTestMethodsName)
                Assert.Contains(expectedTestMethodName, discoveredMethodsNames.ToList());
        }
    }
}