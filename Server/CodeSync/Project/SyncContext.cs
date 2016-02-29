using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryPenguin.CodeSync.Project
{
    /// <summary>
    /// Represents a synchronization context, which manipulates a directory tree.
    /// </summary>
    class SyncContext : IDisposable
    {
        private FileSystemWatcher watcher;

        /// <summary>
        /// The root path that the context is moving from the file system to ROBLOX Studio.
        /// </summary>
        public string RootPath { get; private set; }

        /// <summary>
        /// The file extensions the context is synchronizing.
        /// </summary>
        public ICollection<string> Extensions { get; private set; }

        public SyncContext(string path)
        {
            watcher = new FileSystemWatcher(path);
            Extensions = new HashSet<string>();
        }

        private string MakeRelativePath(string subPath)
        {

        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
