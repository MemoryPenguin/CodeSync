﻿using System;

namespace MemoryPenguin.CodeSync
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CodeSync <configPath>");
                return 1;
            }
            Config config;

            try
            {
                config = Config.LoadFromFile(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error parsing config: {e.Message}");
                return 1;
            }

            Project project = new Project(config.Path, config.SyncLocation, config.SyncedExtensions);
            ProjectServer server = new ProjectServer(project, config.Port);
            server.Start();
            Console.Read();
            server.Stop();

            return 0;
        }
    }
}
