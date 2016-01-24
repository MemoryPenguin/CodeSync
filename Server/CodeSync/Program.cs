using MemoryPenguin.CodeSync.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace MemoryPenguin.CodeSync
{
    class Program
    {
        private static Project project;

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CodeSync <configPath>");
                return 1;
            }

            Dictionary<string, dynamic> settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(args[0]));
            int port = (int)settings["Port"];
            bool allowExternal = (bool)settings["AllowExternalRequests"];
            string path = (string)settings["Path"];
            string syncTarget = (string)settings["SyncLocation"];

            project = new Project(path, syncTarget);

            ProjectServer server = new ProjectServer(project, port);
            server.Start();
            Console.Read();
            server.Stop();

            return 0;
        }
    }
}
