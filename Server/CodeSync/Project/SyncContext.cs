using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryPenguin.CodeSync.Project
{
    /// <summary>
    /// Represents a sync context, which synchronizes a folder tree to Studio.
    /// </summary>
    class SyncContext : IDisposable
    {
        private FileSystemWatcher watcher;
        private ICollection<string> extensions;

        public SyncContext(string path)
        {
            watcher = new FileSystemWatcher(path);
            extensions = new HashSet<string>();
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
