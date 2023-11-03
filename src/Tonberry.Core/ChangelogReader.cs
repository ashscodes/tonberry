using System.IO;

namespace Tonberry.Core;

internal sealed class ChangelogReader : StreamReader
{
    internal bool IsTonberryChangelog { get; set; } = false;

    public ChangelogReader(FileInfo changelog) : base(changelog.FullName)
    {
        if (!changelog.Exists || changelog.Length == 0)
        {
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            ReadLine();
        }

        IsTonberryChangelog = string.Equals(ReadLine(),
                                            Resources.ChangelogHeader,
                                            Resources.StrCompare);

        DiscardBufferedData();
        BaseStream.Seek(0, SeekOrigin.Begin);
    }
}