using System.Diagnostics;
using System.IO;

namespace Tonberry.Core.Internal;

internal class GitExeResult
{
    public string StandardError { get; set; }

    public string StandardOutput { get; set; }

    public GitExeResult(Process process)
    {
        StandardError = process.StandardError.ReadToEnd();
        StandardOutput = process.StandardOutput.ReadToEnd();
    }

    public GitExeResult(StreamReader standardError, StreamReader standardOutput)
    {
        StandardError = standardError.ReadToEnd();
        StandardOutput = standardOutput.ReadToEnd();
    }
}