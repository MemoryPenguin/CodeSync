using MemoryPenguin.CodeSync.Files;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync
{
    /// <summary>
    /// Represents a project that can be synced.
    /// </summary>
    class Project : IDisposable
    {
        /// <summary>
        /// The absolute path to the root of the project.
        /// </summary>
        public string RootPath { get; private set; }
        /// <summary>
        /// A ROBLOX path, in the form Parent.Child.ChildChild, that determines where to sync the project to.
        /// </summary>
        public string RobloxStorageLocation { get; set; }

        private Watcher fileWatcher;

        public Project(string path, string robloxLocation, string[] extensions)
        {
            RootPath = path;
            RobloxStorageLocation = robloxLocation;

            fileWatcher = new Watcher(path);

            foreach (string ext in extensions)
            {
                fileWatcher.Extensions.Add(ext);
            }

            fileWatcher.Start();
        }

        /// <summary>
        /// Gets a list of all the files in the project.
        /// </summary>
        /// <returns>A list of relative <c>Uri</c> objects that reference a single file within the project's root directory.</returns>
        public List<string> GetFilesInProject()
        {
            List<string> results = new List<string>();
            Stack<string> stack = new Stack<string>();

            stack.Push(RootPath);

            while (stack.Count > 0)
            {
                string current = stack.Pop();

                foreach (string filePath in Directory.GetFiles(current))
                {
                    results.Add(Utility.MakeRelativePath(RootPath, filePath));
                }

                foreach (string dirPath in Directory.GetDirectories(current))
                {
                    stack.Push(dirPath);
                }
            }

            return results;
        }

        public FileChange[] GetChanges()
        {
            return fileWatcher.GetFileChanges();
        }

        public void Dispose()
        {
            fileWatcher.Stop();
            fileWatcher.Dispose();
        }
    }
}
