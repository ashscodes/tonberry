using System;
using System.IO;

namespace Tonberry.Core.Model;

public class TonberryCommitOptions : TonberryTaskOptions, ITonberryCommitOptions
{
    public int[] Closes { get; set; }

    public string Detail { get; set; }

    public bool IsBreaking { get; set; }

    public bool IsPreview { get; set; }

    public string Message { get; set; }

    public bool Push { get; set; }

    public string[] Resolves { get; set; }

    public string Scope { get; set; }

    public bool StageAll { get; set; }

    public string Type { get; set; }

    public TonberryCommitOptions() { }

    public TonberryCommitOptions(ITonberryCommitOptions options) : base()
    {
        Closes = options.Closes;
        Detail = options.Detail;
        IsBreaking = options.IsBreaking;
        IsPreview = options.IsPreview;
        Message = options.Message;
        Push = options.Push;
        Resolves = options.Resolves;
        Scope = options.Scope;
        StageAll = options.StageAll;
        Type = options.Type;
    }

    public override void Validate()
    {
        Ensure.IsEnumValue<CommitType>(Type);
        Ensure.StringNotNullOrEmpty(Message, Resources.InvalidCommitMessage);
    }
}

public class TonberryInitOptions : TonberryTaskOptions, ITonberryInitOptions
{
    public string Name { get; set; }

    public bool Open { get; set; }

    public Uri Repository { get; set; }

    public TonberryVersion Version { get; set; }

    public TonberryInitOptions() : base() { }

    public TonberryInitOptions(string name, Uri repository) : this()
    {
        Name = name;
        Repository = repository;
    }

    public TonberryInitOptions(ITonberryInitOptions options) : this(options.Name, options.Repository)
    {
        Open = options.Open;
        Version = options.Version;
    }

    public override void Validate() { }
}

public class TonberryReleaseOptions : TonberryVersionOptions, ITonberryReleaseOptions
{
    public bool BumpMajor { get; set; } = false;

    public bool BumpMinor { get; set; } = false;

    public bool ToMarkdown { get; set; } = false;

    public bool VersionOnly { get; set; } = false;

    public TonberryReleaseOptions() : base() { }

    public TonberryReleaseOptions(ITonberryReleaseOptions options) : base()
    {
        Build = options.Build;
        BumpMajor = options.BumpMajor;
        BumpMinor = options.BumpMinor;
        IsPreview = options.IsPreview;
        IsReleaseCandidate = options.IsReleaseCandidate;
        PreRelease = options.PreRelease;
        ToMarkdown = options.ToMarkdown;
        VersionOnly = options.VersionOnly;
    }
}

public class TonberryNewOptions : TonberryVersionOptions, ITonberryNewOptions
{
    public TonberryNewOptions() : base() { }

    public TonberryNewOptions(ITonberryNewOptions options) : base()
    {
        Build = options.Build;
        IsPreview = options.IsPreview;
        IsReleaseCandidate = options.IsReleaseCandidate;
        PreRelease = options.PreRelease;
    }
}

public abstract class TonberryVersionOptions : TonberryChangelogOptions
{
    public string Build { get; set; }

    public bool IsReleaseCandidate { get; set; } = false;

    public string PreRelease { get; set; }

    internal TonberryVersionOptions() : base() { }

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

        base.Validate();
    }
}

public class TonberryMigrateOptions : TonberryChangelogOptions, ITonberryMigrateOptions
{
    public TonberryMigrateOptions() : base() { }

    public TonberryMigrateOptions(ITonberryMigrateOptions options) : base()
    {
        IsPreview = options.IsPreview;
    }
}

public abstract class TonberryChangelogOptions : TonberryTaskOptions, ITonberryGitOptions
{
    public bool IsPreview { get; set; }

    internal TonberryChangelogOptions() { }

    public override void Validate() { }
}

public abstract class TonberryTaskOptions : ITonberryBaseOptions, ITonberryOptionValidation
{
    public DirectoryInfo Directory { get; set; }

    public abstract void Validate();
}

public interface ITonberryOptionValidation
{
    void Validate();
}

public interface ITonberryGitOptions
{
    bool IsPreview { get; set; }
}

public interface ITonberryBaseOptions { }

public interface ITonberryCommitOptions : ITonberryBaseOptions, ITonberryGitOptions
{
    int[] Closes { get; set; }

    string Detail { get; set; }

    bool IsBreaking { get; set; }

    string Message { get; set; }

    bool Push { get; set; }

    string[] Resolves { get; set; }

    string Scope { get; set; }

    bool StageAll { get; set; }

    string Type { get; set; }
}

public interface ITonberryInitOptions : ITonberryBaseOptions
{
    string Name { get; set; }

    bool Open { get; set; }

    Uri Repository { get; set; }

    TonberryVersion Version { get; set; }
}

public interface ITonberryMigrateOptions : ITonberryBaseOptions, ITonberryGitOptions { }

public interface ITonberryNewOptions : ITonberryBaseOptions, ITonberryGitOptions
{
    string Build { get; set; }

    bool IsReleaseCandidate { get; set; }

    string PreRelease { get; set; }
}

public interface ITonberryReleaseOptions : ITonberryBaseOptions, ITonberryGitOptions
{
    bool BumpMajor { get; set; }

    bool BumpMinor { get; set; }

    string Build { get; set; }

    bool IsReleaseCandidate { get; set; }

    string PreRelease { get; set; }

    bool ToMarkdown { get; set; }

    bool VersionOnly { get; set; }
}