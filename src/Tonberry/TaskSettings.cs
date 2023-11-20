using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Tonberry.Core.Model;

namespace Tonberry;

public sealed class CommitSettings : BaseSettings<TonberryCommitOptions>, ITonberryCommitOptions
{
    [CommandOption("-c|--closes")]
    [Description(Resources.CommitClosesOpt)]
    public int[] Closes { get; set; } = null;

    [CommandOption("-d|--detail")]
    [Description(Resources.CommitDetailOpt)]
    public string Detail { get; set; } = string.Empty;

    [CommandOption("-b|--breaking")]
    [Description(Resources.CommitIsBreakingOpt)]
    public bool IsBreaking { get; set; } = false;

    [CommandOption("--preview")]
    [Description(Resources.CommitIsPreviewOpt)]
    public bool IsPreview { get; set; } = false;

    [CommandOption("-m|--message")]
    [Description(Resources.CommitMessageOpt)]
    public string Message { get; set; } = string.Empty;

    [CommandOption("--push")]
    [Description(Resources.CommitPushOpt)]
    public bool Push { get; set; } = false;

    [CommandOption("--resolves")]
    [Description(Resources.CommitResolvesOpt)]
    public string[] Resolves { get; set; } = null;

    [CommandOption("-s|--scope")]
    [Description(Resources.CommitScopeOpt)]
    public string Scope { get; set; } = string.Empty;

    [CommandOption("--stage-all")]
    [Description(Resources.CommitStageAllOpt)]
    public bool StageAll { get; set; } = false;

    [CommandOption("-t|--type")]
    [Description(Resources.CommitTypeOpt)]
    public string Type { get; set; } = string.Empty;

    public override TonberryCommitOptions GetOptions() => new((ITonberryCommitOptions)this);

    public override ValidationResult Validate()
    {
        if (IsBreaking && string.IsNullOrEmpty(Detail))
        {
            return ValidationResult.Error(Resources.NoDetailForBreakingChange);
        }

        return base.Validate();
    }
}

public sealed class InitSettings : BaseSettings<TonberryInitOptions>, ITonberryInitOptions
{
    [CommandArgument(1, "[name]")]
    [Description(Resources.InitNameOpt)]
    public string Name { get; set; } = string.Empty;

    [CommandOption("--open")]
    [Description(Resources.InitOpenOpt)]
    public bool Open { get; set; } = false;

    [CommandArgument(0, "[url]")]
    [Description(Resources.InitRepoOpt)]
    public Uri Repository { get; set; } = null;

    [CommandOption("-v|--version")]
    [Description(Resources.InitVersionOpt)]
    public TonberryVersion Version { get; set; } = null;

    public override TonberryInitOptions GetOptions() => new((ITonberryInitOptions)this);

    public override ValidationResult Validate() => base.Validate();
}

public sealed class MigrateSettings : BaseSettings<TonberryMigrateOptions>, ITonberryMigrateOptions
{
    [CommandOption("--preview")]
    [Description(Resources.ReleaseIsPreviewOpt)]
    public bool IsPreview { get; set; } = false;

    public override TonberryMigrateOptions GetOptions() => new((ITonberryMigrateOptions)this);

    public override ValidationResult Validate() => base.Validate();
}

public sealed class NewSettings : BaseSettings<TonberryNewOptions>, ITonberryNewOptions
{
    [CommandOption("-b|--build-label")]
    [Description(Resources.ReleaseBuildOpt)]
    public string Build { get; set; } = string.Empty;

    [CommandOption("--preview")]
    [Description(Resources.ReleaseIsPreviewOpt)]
    public bool IsPreview { get; set; } = false;

    [CommandOption("-rc|--release-candidate")]
    [Description(Resources.ReleaseIsRCOpt)]
    public bool IsReleaseCandidate { get; set; } = false;

    [CommandOption("-p|--prerelease-label")]
    [Description(Resources.ReleasePreReleaseOpt)]
    public string PreRelease { get; set; } = string.Empty;

    public override TonberryNewOptions GetOptions() => new((ITonberryNewOptions)this);

    public override ValidationResult Validate()
    {
        if (IsReleaseCandidate && !string.IsNullOrEmpty(PreRelease))
        {
            return ValidationResult.Error(Resources.InvalidPreReleaseLabel);
        }

        return base.Validate();
    }
}

public sealed class ReleaseSettings : BaseSettings<TonberryReleaseOptions>, ITonberryReleaseOptions
{
    [CommandOption("-b|--build-label")]
    [Description(Resources.ReleaseBuildOpt)]
    public string Build { get; set; } = string.Empty;

    [CommandOption("--bump-major")]
    [Description(Resources.ReleaseMajorOpt)]
    public bool BumpMajor { get; set; } = false;

    [CommandOption("--bump-minor")]
    [Description(Resources.ReleaseMinorOpt)]
    public bool BumpMinor { get; set; } = false;

    [CommandOption("--preview")]
    [Description(Resources.ReleaseIsPreviewOpt)]
    public bool IsPreview { get; set; } = false;

    [CommandOption("-rc|--release-candidate")]
    [Description(Resources.ReleaseIsRCOpt)]
    public bool IsReleaseCandidate { get; set; } = false;

    [CommandOption("-p|--prerelease-label")]
    [Description(Resources.ReleasePreReleaseOpt)]
    public string PreRelease { get; set; } = string.Empty;

    [CommandOption("-tomd|--to-markdown")]
    [Description(Resources.ReleaseTomdOpt)]
    public bool ToMarkdown { get; set; } = false;

    [CommandOption("-v|--version-only")]
    [Description(Resources.ReleaseVersionOnlyOpt)]
    public bool VersionOnly { get; set; } = false;

    public override TonberryReleaseOptions GetOptions() => new((ITonberryReleaseOptions)this);

    public override ValidationResult Validate()
    {
        if (BumpMajor && BumpMinor)
        {
            return ValidationResult.Error(Resources.InvalidBumpConfig);
        }

        if (IsReleaseCandidate && !string.IsNullOrEmpty(PreRelease))
        {
            return ValidationResult.Error(Resources.InvalidPreReleaseLabel);
        }

        if (ToMarkdown && VersionOnly)
        {
            return ValidationResult.Error(
                string.Format(Resources.InvalidConsoleOutputOptions, "--to-markdown", "--version-only"));
        }

        return base.Validate();
    }
}

public abstract class BaseSettings<T> : CommandSettings where T : TonberryTaskOptions, new()
{
    protected internal BaseSettings() { }

    public abstract T GetOptions();

    public override ValidationResult Validate()
    {
        try
        {
            T options = GetOptions();
            options.Validate();
            return base.Validate();
        }
        catch (Exception ex)
        {
            return ValidationResult.Error(ex.Message);
        }
    }
}