using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync.Files
{
    class Watcher : IDisposable
    {
        private string watchRoot;
        private FileSystemWatcher watcher;
        private HashSet<FileChange> changes;

        /// <summary>
        /// All the extensions to be tracked. If empty, all extensions will be tracked.
        /// </summary>
        public HashSet<string> Extensions { get; private set; }

        public bool IsWatching
        {
            get
            {
                return watcher.EnableRaisingEvents;
            }
        }

        public Watcher(string root)
        {
            watchRoot = root;
            changes = new HashSet<FileChange>();
            Extensions = new HashSet<string>();
            watcher = new FileSystemWatcher(root);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;
            
            watcher.Changed += WatcherChange;
            watcher.Deleted += WatcherChange;
            watcher.Created += WatcherChange;
            watcher.Renamed += FileRename;
        }

        private bool ShouldSync(string path)
        {
            return Extensions.Count == 0 || Extensions.Contains(Path.GetExtension(path));
        }

        private void FileRename(object sender, RenamedEventArgs e)
        {
            if (ShouldSync(e.FullPath))
            {
                changes.Add(new FileChange(Utility.MakeRelativePath(watchRoot, e.FullPath)));
            }

            // emit delete event for old path so the plugin cleans it up
            if (ShouldSync(e.OldFullPath)) {
                changes.Add(new FileChange(Utility.MakeRelativePath(watchRoot, e.OldFullPath), ChangeType.Delete));
            }
        }

        private void WatcherChange(object sender, FileSystemEventArgs e)
        {
            if (ShouldSync(e.FullPath))
            {
                string relativePath = Utility.MakeRelativePath(watchRoot, e.FullPath);

                // Remove all prior changes like modifications
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    changes.RemoveWhere(change => change.Path == relativePath);
                }

                changes.Add(new FileChange(relativePath, e.ChangeType.ToChangeType()));
            }
        }

        public FileChange[] GetFileChanges()
        {
            FileChange[] result = new FileChange[changes.Count];
            changes.CopyTo(result);
            changes.Clear();
            return result;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        public bool HasNewChanges()
        {
            return changes.Count > 0;
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
