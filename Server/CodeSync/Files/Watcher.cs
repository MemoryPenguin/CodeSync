using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync.Files
{
    /// <summary>
    /// Responsible for watching a directory.
    /// </summary>
    class Watcher : IDisposable
    {
        private string watchRoot;
        private FileSystemWatcher watcher;
        private HashSet<FileChange> changes;

        /// <summary>
        /// All the extensions to be tracked. If empty, all extensions will be tracked.
        /// </summary>
        public ISet<string> Extensions { get; private set; }

        /// <summary>
        /// Whether the watcher is currently watching or not.
        /// </summary>
        public bool IsWatching { get { return watcher.EnableRaisingEvents; } }

        /// <summary>
        /// The root directory that the watcher is using.
        /// </summary>
        public string WatchRoot { get { return watchRoot; } }

        public Watcher(string root)
        {
            watchRoot = root;
            changes = new HashSet<FileChange>();
            Extensions = new HashSet<string>();
            watcher = new FileSystemWatcher(root);
            watcher.IncludeSubdirectories = true;
            
            watcher.Changed += WatcherChange;
            watcher.Deleted += WatcherChange;
            watcher.Created += WatcherChange;
            watcher.Renamed += FileRename; // special handler needed, see https://github.com/MemoryPenguin/CodeSync/issues/1
        }

        private bool ShouldSync(string path)
        {
            return Extensions.Count == 0 || Extensions.Contains(Path.GetExtension(path));
        }

        private void FileRename(object sender, RenamedEventArgs e)
        {
            if (ShouldSync(e.FullPath))
            {
                AddChange(new FileChange(e.FullPath, ChangeType.Modify));
            }

            // emit delete change for old path so the plugin cleans it up
            if (ShouldSync(e.OldFullPath))
            {
                AddChange(new FileChange(e.OldFullPath, ChangeType.Delete));
            }
        }

        private void WatcherChange(object sender, FileSystemEventArgs e)
        {
            if (ShouldSync(e.FullPath))
            {
                AddChange(new FileChange(e.FullPath, e.ChangeType.ToChangeType()));
            }
        }

        private void AddChange(FileChange newChange)
        {
            // Remove prior changes
            changes.RemoveWhere(change => change.Path == newChange.Path);
            changes.Add(newChange);
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
