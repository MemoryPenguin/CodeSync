namespace MemoryPenguin.CodeSync
{
    class Utility
    {
        /// <summary>
        /// Generates a relative path from a base. <b>Does not go 'up'.</b>
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="path">The path to create a relative path for.</param>
        /// <returns>A relative path to <c>path</c> from <c>basePath</c>.</returns>
        public static string MakeRelativePath(string basePath, string path)
        {
            return path.Substring(basePath.Length + 1);
        }
    }
}
