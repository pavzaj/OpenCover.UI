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
        private static void Discover(string[] args, Stream stream)
        {
            if (args != null && args.Length > 0)
            {
                var dlls = args.Skip(1);
                var tests = new Discoverer(dlls).Discover();
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
                    Discover(args, new MemoryStream());

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