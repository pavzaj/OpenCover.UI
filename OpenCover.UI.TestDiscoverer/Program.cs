﻿// This source code is released under the MIT License; Please read license.md file for more details.
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Web.Script.Serialization;

namespace OpenCover.UI.TestDiscoverer
{
    internal class Program
    {
        private static void Discover(string[] args, NamedPipeClientStream stream)
        {
            if (args != null && args.Length > 0)
            {
                var dlls = args.Skip(1);
                var nunitConsolePath = args.LastOrDefault();
                var tests = new Discoverer(dlls, nunitConsolePath).Discover();
                string serialized = string.Empty;

                if (tests != null)
                {
                    var jsSerializer = new JavaScriptSerializer();
                    serialized = jsSerializer.Serialize(tests);
                }

                Write(stream, serialized);
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 1)
                {
                    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", args[0], PipeDirection.InOut);
                    pipeClient.Connect();
                    Discover(args, pipeClient);

                    pipeClient.WaitForPipeDrain();

                    pipeClient.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Write(Stream stream, string json)
        {
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
        }
    }
}