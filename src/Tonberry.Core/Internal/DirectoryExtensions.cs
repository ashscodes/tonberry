using System.IO;

namespace Tonberry.Core;

internal static class DirectoryExtensions
{
    public static FileInfo FindOrCreateFile(this DirectoryInfo directory, string fileName, bool recurse = true)
    {
        SearchOption searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var foundFile in directory.GetFiles(fileName, searchOption))
        {
            return foundFile;
        }

        return new FileInfo(Path.Combine(directory.FullName, fileName));
    }
}