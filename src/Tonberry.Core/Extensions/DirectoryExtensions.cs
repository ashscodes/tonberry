using System.IO;
using Tonberry.Core.Model;

namespace Tonberry.Core;

public static class DirectoryExtensions
{
    public static TonberryConfiguration ReadConfig(this DirectoryInfo directory)
    {
        var configPath = new FileInfo(Path.Combine(directory.FullName, Resources.TonberryRootConfig));
        Ensure.IsTrue(configPath.Exists, Resources.RootConfigNotFound);
        var contents = File.ReadAllText(configPath.FullName);
        Ensure.StringNotNullOrEmpty(contents, Resources.RootConfigNotFound);
        using var reader = new StringReader(contents);
        return Util.GetYamlDeserializer().Deserialize<TonberryConfiguration>(reader);
    }

    internal static FileInfo FindOrCreateFile(this DirectoryInfo directory, string fileName, bool recurse = true)
    {
        SearchOption searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var foundFile in directory.GetFiles(fileName, searchOption))
        {
            return foundFile;
        }

        return new FileInfo(Path.Combine(directory.FullName, fileName));
    }
}