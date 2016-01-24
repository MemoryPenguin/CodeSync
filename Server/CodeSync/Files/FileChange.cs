using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MemoryPenguin.CodeSync.Files
{
    enum ChangeType
    {
        Modify = 0,
        Delete = 1
    }

    struct FileChange : IEquatable<FileChange>
    {
        public string Path { get; private set; }
        public ChangeType Type { get; private set; }

        public FileChange(string path, ChangeType type)
        {
            Path = path;
            Type = type;
        }

        public FileChange(string path) : this(path, ChangeType.Modify) { }

        public override bool Equals(object obj)
        {
            return obj is FileChange && Equals((FileChange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Path.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                return hash;
            }
        }

        public bool Equals(FileChange other)
        {
            return Path == other.Path && Type == other.Type;
        }
    }

    static class ChangeTypeExtender
    {
        public static ChangeType ToChangeType(this WatcherChangeTypes x)
        {
            switch (x)
            {
                case WatcherChangeTypes.Deleted:
                    return ChangeType.Delete;
                default:
                    return ChangeType.Modify;
            }
        }
    }
}
