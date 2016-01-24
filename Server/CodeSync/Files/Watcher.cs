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
            watcher = new FileSystemWatcher(root);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;
            
            watcher.Changed += WatcherChange;
            watcher.Deleted += WatcherChange;
            watcher.Created += WatcherChange;
            watcher.Renamed += WatcherChange;
        }

        private void WatcherChange(object sender, FileSystemEventArgs e)
        {
            string relativePath = Utility.MakeRelativePath(watchRoot, e.FullPath);

            // Remove all prior changes like modifications
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                changes.RemoveWhere(change => change.Path == relativePath);
            }

            changes.Add(new FileChange(relativePath, e.ChangeType.ToChangeType()));
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
