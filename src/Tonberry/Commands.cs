using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Spectre.Console;
using Spectre.Console.Cli;
using Tonberry.Core;
using Tonberry.Core.Model;
using Tonberry.Prompt;

namespace Tonberry;

public sealed class CommitCommand : SingleTaskAction<CommitSettings, TonberryCommitTask, TonberryCommitOptions, TonberryCommitResult>
{
    internal CommitCommand() : base() => _task.Prompt = UserPrompts.CommitPrompt;

    public override int Execute([NotNull] CommandContext context, [NotNull] CommitSettings settings)
    {
        base.Execute(context, settings);
        if (settings.IsPreview)
        {
            _result.WriteInConsole();
        }

        return 0;
    }
}

public sealed class InitCommand : SingleTaskAction<InitSettings, TonberryInitTask, TonberryInitOptions, TonberryInitResult>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] InitSettings settings)
    {
        base.Execute(context, settings);
        if (settings.Open)
        {
            _result.Open();
        }

        return 0;
    }
}

public sealed class MigrateCommand : MultiTaskAction<MigrateSettings, TonberryMigrateTask, TonberryMigrateOptions>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] MigrateSettings settings)
    {
        base.Execute(context, settings);
        if (settings.IsPreview)
        {
            foreach (TonberryFileResult result in _results)
            {
                result.Open();
            }
        }

        return 0;
    }
}

public sealed class NewCommand : MultiTaskAction<NewSettings, TonberryNewTask, TonberryNewOptions>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] NewSettings settings)
    {
        base.Execute(context, settings);
        if (settings.IsPreview)
        {
            foreach (TonberryFileResult result in _results)
            {
                result.Open();
            }
        }

        return 0;
    }
}

public sealed class ReleaseCommand : MultiTaskAction<ReleaseSettings, TonberryReleaseTask, TonberryReleaseOptions>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ReleaseSettings settings)
    {
        base.Execute(context, settings);
        if (settings is { VersionOnly: false, ToMarkdown: true })
        {
            Console.WriteLine(Resources.VersionMarkdownHeader);
        }

        foreach (TonberryFileResult result in _results)
        {
            if (settings is { VersionOnly: true, ToMarkdown: false })
            {
                result.WriteInConsole();
            }

            if (settings is { VersionOnly: false, ToMarkdown: true })
            {
                result.WriteVersionAsMarkdown();
            }

            if (settings.IsPreview)
            {
                result.Open();
            }
        }

        return 0;
    }
}

public abstract class MultiTaskAction<S, C, T> : BaseCommand<S, C, T>
    where S : BaseSettings<T>, new()
    where C : TonberryMultiTask, new()
    where T : TonberryTaskOptions, new()
{
    protected internal TonberryResultCollection _results;

    internal MultiTaskAction() : base() { }

    public override int Execute([NotNull] CommandContext context, [NotNull] S settings)
    {
        _results = _task.Invoke();
        return 0;
    }
}

public abstract class SingleTaskAction<S, C, T, R> : BaseCommand<S, C, T>
    where S : BaseSettings<T>, new()
    where C : TonberrySingleTask, new()
    where T : TonberryTaskOptions, new()
    where R : TonberryResult, new()
{
    protected internal R _result;

    internal SingleTaskAction() : base() { }

    public override int Execute([NotNull] CommandContext context, [NotNull] S settings)
    {
        _result = (R)_task.Invoke();
        return 0;
    }
}

public abstract class BaseCommand<S, C, T> : Command<S>
    where S : BaseSettings<T>, new()
    where C : TonberryTask, new()
    where T : TonberryTaskOptions, new()
{
    protected internal C _task;

    internal BaseCommand() => _task = new C();

    public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] S settings)
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        if (_task is not TonberryInitTask)
        {
            try
            {
                _task.Config = currentDirectory.ReadConfig();
            }
            catch (Exception ex)
            {
                return ValidationResult.Error(ex.Message);
            }
        }

        _task.SetOptions(settings.GetOptions());
        _task.SetDirectory(currentDirectory);
        return base.Validate(context, settings);
    }
}