// This source code is released under the MIT License; Please read license.md file for more details.
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace OpenCover.UI.TestDiscoverer
{
    internal class Program
    {
        private static string Discover(string[] dlls, Stream stream, string nunit3ConsoleExePath)
        {
            string discoveredTestsInJson = string.Empty;

            if (dlls != null && dlls.Length > 0)
            {
                var tests = new Discoverer(dlls, nunit3ConsoleExePath).Discover();

                if (tests != null)
                    discoveredTestsInJson = new JavaScriptSerializer().Serialize(tests);
            }

            return discoveredTestsInJson;
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 1)
                {
                    var pipeGuid = args[0];

                    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeGuid, PipeDirection.InOut);

                    pipeClient.Connect();

                    var dlls = args.Skip(2).ToArray();

                    var nunit3ConsoleExePath = Encoding.UTF8.GetString(Convert.FromBase64String(args[1]));

                    Write(pipeClient, Discover(dlls, pipeClient, nunit3ConsoleExePath));

                    pipeClient.WaitForPipeDrain();

                    pipeClient.Close();
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();

                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);

                foreach (var arg in args)
                    sb.AppendLine(arg);

                string fileName = DateTime.Now.ToString("YYYY-MM-DD hh:mm:ss");

                Guid parsedGuid;

                if (args.Length > 0 && Guid.TryParse(args[0], out parsedGuid))
                    fileName = parsedGuid.ToString();

                try
                {
                    File.WriteAllText(fileName, sb.ToString());
                }
                catch (Exception) { }

                Console.WriteLine(sb.ToString());
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