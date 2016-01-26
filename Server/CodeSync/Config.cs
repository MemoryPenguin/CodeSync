using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync
{
    struct Config
    {
        private static string[] defaultExtensions = new string[] { ".lua", ".rbxs" };
        
        public int Port { get; private set; }
        public bool AllowExternalRequests { get; private set; }
        public string Path { get; private set; }
        public string SyncLocation { get; private set; }
        public string[] SyncedExtensions { get; private set; }

        public static Config LoadFromFile(string path)
        {
            Dictionary<string, dynamic> settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(path));
            Config cfg = new Config();
            cfg.Port = settings.ContainsKey("Port") ? (int)settings["Port"] : 4114;
            cfg.AllowExternalRequests = settings.ContainsKey("AllowExternalRequests") ? (bool)settings["AllowExternalRequests"] : false;

            if (settings.ContainsKey("Path"))
            {
                cfg.Path = (string)settings["Path"];
            }
            else
            {
                throw new MalformedConfigException("Missing required config element Path");
            }

            if (settings.ContainsKey("SyncLocation"))
            {
                cfg.SyncLocation = (string)settings["SyncLocation"];
            }
            else
            {
                throw new MalformedConfigException("Missing required config element SyncLocation");
            }
                

            cfg.SyncedExtensions = settings.ContainsKey("Extensions") ? (string[])settings["Extensions"] : defaultExtensions;

            // ensure all exts start with '.'
            for (int i = 0; i < cfg.SyncedExtensions.Length; i++)
            {
                string ext = cfg.SyncedExtensions[i];
                if (ext.Substring(0, 1) != ".")
                {
                    ext = "." + ext;
                    cfg.SyncedExtensions[i] = ext;
                }
            }

            return cfg;
        }
    }

    class MalformedConfigException : Exception
    {
        public MalformedConfigException() { }
        public MalformedConfigException(string message) : base(message) { }
        public MalformedConfigException(string message, Exception inner) : base(message, inner) { }
    }
}
