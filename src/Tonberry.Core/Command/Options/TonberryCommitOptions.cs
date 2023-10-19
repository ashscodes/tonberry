using System.Collections.Generic;
using Tonberry.Core.Internal;

namespace Tonberry.Core.Command.Options;

public sealed class TonberryCommitOptions : ITonberryGitOptions
{
    public IEnumerable<int> Closes { get; set; }

    public string Detail { get; set; }

    public bool IsBreaking { get; set; }

    public bool IsPreview { get; set; }

    public string Message { get; set; }

    public bool Push { get; set; }

    public IEnumerable<string> Resolves { get; set; }

    public string Scope { get; set; }

    public bool StageAll { get; set; }

    public string Type { get; set; }

    public TonberryCommitOptions() { }

    public TonberryCommitOptions(string message, string type)
    {
        Message = message;
        Type = type;
    }

    public void Validate()
    {
        Ensure.StringNotNullOrEmpty(Message, Resources.InvalidCommitMessage);
        Ensure.IsEnumValue<CommitType>(Type);
    }
}