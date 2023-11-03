using System;
using System.Collections.Generic;

namespace Tonberry.Core.Model;

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
        Ensure.IsEnumValue<CommitType>(Type);
        Ensure.StringNotNullOrEmpty(Message, Resources.InvalidCommitMessage);
    }
}

public class TonberryInitOptions : ITonberryCommandOptions
{
    public bool Force { get; set; }

    public string Name { get; set; }

    public Uri Repository { get; set; }

    public TonberryVersion Version { get; set; }

    public void Validate() => Ensure.StringNotNullOrEmpty(Name, Resources.StringIsNullOrEmpty, nameof(Name));
}

public class TonberryMigrateOptions : TonberryChangelogOptions
{
    public override bool IsPreview { get; set; } = false;

    public override void Validate() { }
}

public class TonberryNewOptions : TonberryChangelogOptions
{
    public string Build { get; set; }

    public bool IsReleaseCandidate { get; set; } = false;

    public string PreRelease { get; set; }

    public override bool IsPreview { get; set; } = false;

    public override void Validate()
    {
        if (!string.IsNullOrEmpty(Build) && Build.StartsWith('+'))
        {
            Build = Build.TrimStart('+');
        }

        if (!string.IsNullOrEmpty(PreRelease) && PreRelease.StartsWith('-'))
        {
            PreRelease = PreRelease.TrimStart('-');
        }

        if (IsReleaseCandidate)
        {
            PreRelease = "rc";
        }
    }
}

public class TonberryReleaseOptions : TonberryChangelogOptions
{
    public bool BumpMajor { get; set; } = false;

    public bool BumpMinor { get; set; } = false;

    public string Build { get; set; }

    public bool IsReleaseCandidate { get; set; } = false;

    public string PreRelease { get; set; }

    public bool VersionOnly { get; set; } = false;

    public override bool IsPreview { get; set; } = false;

    public TonberryReleaseOptions() { }

    public override void Validate()
    {
        if (!string.IsNullOrEmpty(Build) && Build.StartsWith('+'))
        {
            Build = Build.TrimStart('+');
        }

        if (!string.IsNullOrEmpty(PreRelease) && PreRelease.StartsWith('-'))
        {
            PreRelease = PreRelease.TrimStart('-');
        }

        if (IsReleaseCandidate)
        {
            PreRelease = "rc";
        }
    }
}

public abstract class TonberryChangelogOptions : ITonberryGitOptions
{
    public abstract bool IsPreview { get; set; }

    public abstract void Validate();
}