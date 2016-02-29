using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    struct Config
    {
        private static string[] defaultExtensions = new string[] { ".lua", ".rbxs" };
        
        /// <summary>
        /// The port the server should run on
        /// </summary>
        [JsonProperty("Port")]
        public int Port { get; set; }
        /// <summary>
        /// Which extensions to sync
        /// </summary>
        [JsonProperty("Extensions")]
        public string[] SyncedExtensions { get; set; }
        /// <summary>
        /// Files and paths to ignore
        /// </summary>
        [JsonProperty("Ignore")]
        public string[] IgnoredFiles { get; set; }
    }
}
