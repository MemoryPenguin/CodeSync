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
        
        [JsonProperty("Port")]
        public int Port { get; set; }
        [JsonProperty("AllowExternalRequests")]
        public bool AllowExternalRequests { get; set; }
        [JsonProperty("Extensions")]
        public string[] SyncedExtensions { get; set; }
    }
}
