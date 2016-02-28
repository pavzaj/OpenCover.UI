using System.Collections.Generic;
using OpenCover.UI.Model.Test;

namespace OpenCover.UI.TestDiscoverer
{
    internal interface IDiscoverer
    {
        List<TestClass> Discover();
    }
}