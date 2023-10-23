using System.Diagnostics;
using System.IO;

namespace Tonberry.Core.Command;

public class TonberryChangelogResult : ITonberryResult<FileInfo>
{
    public string Item { get; }

    public FileInfo Output { get; }

    public bool Success { get; }

    public TonberryChangelogResult(string item, FileInfo output, bool success)
    {
        Item = item;
        Output = output;
        Success = success;
    }

    public virtual void Preview()
    {
        if (Output is not null && Output.Exists)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(Output.FullName)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }

    public virtual void Write(FileInfo changelog)
    {
        if (changelog is not null && Output is not null && Output.Exists)
        {
            File.Move(Output.FullName, changelog.FullName, true);
        }
    }
}