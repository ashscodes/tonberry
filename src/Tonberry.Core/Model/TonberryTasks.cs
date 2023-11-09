using System;
using System.IO;

namespace Tonberry.Core.Model;

public class TonberryCommitTask : TonberrySingleTask, ITonberryTask<TonberryCommitOptions>
{
    public TonberryCommitOptions Options { get; set; }

    public TonberryPrompt<TonberryCommitOptions> Prompt { get; set; }

    public TonberryCommitTask() { }

    public TonberryCommitTask(TonberryConfiguration configuration) : base(configuration) { }

    public override void SetDirectory(DirectoryInfo directory) => Options.Directory = directory;

    public override void SetOptions(ITonberryBaseOptions options)
    {
        if (options is TonberryCommitOptions commitOptions)
        {
            Options = commitOptions;
        }
    }

    public override void Validate()
    {
        try
        {
            Options.Validate();
        }
        catch (TonberryApplicationException ex)
        {
            if (Prompt is not null)
            {
                var options = Prompt(Options, ex);
                SetOptions(options);
                this.Validate();
                return;
            }

            throw ex;
        }
    }

    internal override TonberryResult Run() => Options.NewTonberryCommit().Process(Config, Options);
}

public class TonberryInitTask : TonberrySingleTask, ITonberryTask<TonberryInitOptions>
{
    public TonberryInitOptions Options { get; set; }

    public TonberryPrompt<TonberryInitOptions> Prompt { get; set; }

    public TonberryInitTask() : base() { }

    public TonberryInitTask(TonberryInitOptions options) : base() => Options = options;

    public TonberryInitTask(TonberryConfiguration configuration) : base(configuration) { }

    public TonberryInitTask(string name, Uri repositoryUrl) : this(new TonberryInitOptions(name, repositoryUrl)) { }

    public override void SetDirectory(DirectoryInfo directory) => Options.Directory = directory;

    public override void SetOptions(ITonberryBaseOptions options)
    {
        if (options is TonberryInitOptions initOptions)
        {
            Options = initOptions;
        }
    }

    public override void Validate() => Options.Validate();

    internal override TonberryResult Run()
    {
        if (Options is not null)
        {
            return new TonberryInitResult(new TonberryConfiguration(Options.Directory).Process(Options), true);
        }

        return new TonberryInitResult(new TonberryConfiguration(Options.Directory).Process(), true);
    }
}

public class TonberryMigrateTask : TonberryChangelogTask, ITonberryTask<TonberryMigrateOptions>
{
    public TonberryMigrateOptions Options { get; set; }

    public TonberryPrompt<TonberryMigrateOptions> Prompt { get; set; }

    public TonberryMigrateTask() : base() { }

    public TonberryMigrateTask(TonberryConfiguration configuration) : base(configuration) { }

    public override void SetDirectory(DirectoryInfo directory) => Options.Directory = directory;

    public override void SetOptions(ITonberryBaseOptions options)
    {
        if (options is TonberryMigrateOptions migrateOptions)
        {
            Options = migrateOptions;
        }
    }

    public override void Validate() => Options.Validate();

    internal override TonberryResultCollection Run() => Config.GetProjects(Options).Process(Config, Options);
}

public class TonberryNewTask : TonberryChangelogTask, ITonberryTask<TonberryNewOptions>
{
    public TonberryNewOptions Options { get; set; }

    public TonberryPrompt<TonberryNewOptions> Prompt { get; set; }

    public TonberryNewTask() : base() { }

    public TonberryNewTask(TonberryConfiguration configuration) : base(configuration) { }

    public override void SetDirectory(DirectoryInfo directory) => Options.Directory = directory;

    public override void SetOptions(ITonberryBaseOptions options)
    {
        if (options is TonberryNewOptions newOptions)
        {
            Options = newOptions;
        }
    }

    public override void Validate() => Options.Validate();

    internal override TonberryResultCollection Run() => Config.GetProjects(Options).Process(Config, Options);
}

public class TonberryReleaseTask : TonberryChangelogTask, ITonberryTask<TonberryReleaseOptions>
{
    public TonberryReleaseOptions Options { get; set; }

    public TonberryPrompt<TonberryReleaseOptions> Prompt { get; set; }

    public TonberryReleaseTask() : base() { }

    public TonberryReleaseTask(TonberryConfiguration configuration) : base(configuration) { }

    public override void SetDirectory(DirectoryInfo directory) => Options.Directory = directory;

    public override void SetOptions(ITonberryBaseOptions options)
    {
        if (options is TonberryReleaseOptions releaseOptions)
        {
            Options = releaseOptions;
        }
    }

    public override void Validate() => Options.Validate();

    internal override TonberryResultCollection Run() => Config.GetProjects(Options).Process(Config, Options);
}

public abstract class TonberryChangelogTask : TonberryMultiTask
{
    internal TonberryChangelogTask() : base() { }

    internal TonberryChangelogTask(TonberryConfiguration configuration) : base(configuration) { }
}

public abstract class TonberrySingleTask : TonberryTask
{
    internal TonberrySingleTask() : base() { }

    internal TonberrySingleTask(TonberryConfiguration configuration) : base(configuration) { }

    internal abstract TonberryResult Run();
}

public abstract class TonberryMultiTask : TonberryTask
{
    internal TonberryMultiTask() : base() { }

    internal TonberryMultiTask(TonberryConfiguration configuration) : base(configuration) { }

    internal abstract TonberryResultCollection Run();
}

public abstract class TonberryTask : ITonberryTask
{
    public TonberryConfiguration Config { get; set; }

    public bool Success { get; set; }

    internal TonberryTask() { }

    internal TonberryTask(TonberryConfiguration configuration) => Config = configuration;

    public abstract void SetDirectory(DirectoryInfo directory);

    public abstract void SetOptions(ITonberryBaseOptions options);

    public abstract void Validate();
}

public interface ITonberryTask
{
    TonberryConfiguration Config { get; set; }

    bool Success { get; set; }

    void Validate();
}

public interface ITonberryTask<T> where T : ITonberryOptionValidation, new()
{
    T Options { get; set; }

    TonberryPrompt<T> Prompt { get; set; }
}

public delegate T TonberryPrompt<T>(T options, TonberryApplicationException exception)
    where T : ITonberryOptionValidation;