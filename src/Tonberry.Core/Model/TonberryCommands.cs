namespace Tonberry.Core.Model;

public class TonberryCommitCommand : ITonberryCommand<TonberryCommitOptions>
{
    public TonberryConfiguration Config { get; set; }

    public TonberryCommitOptions Options { get; set; }

    public TonberryPrompt<TonberryCommitOptions> Prompt { get; set; }

    public bool Success { get; set; }

    public TonberryCommitCommand() { }

    public TonberryCommitCommand(TonberryConfiguration configuration,
                                 TonberryCommitOptions options,
                                 TonberryPrompt<TonberryCommitOptions> prompt)
    {
        Config = configuration;
        Options = options;
        Prompt = prompt;
    }

    public void Validate()
    {
        try
        {
            Options.Validate();
        }
        catch (TonberryApplicationException)
        {
            Prompt(Options);
        }
    }
}

public class TonberryInitCommand : ITonberryCommand<TonberryInitOptions>
{
    public TonberryConfiguration Config { get; set; }

    public TonberryInitOptions Options { get; set; }

    public TonberryPrompt<TonberryInitOptions> Prompt { get; set; }

    public bool Success { get; set; }

    public void Validate() { }
}

public class TonberryMigrateCommand : TonberryChangelogCommand<TonberryMigrateOptions> { }

public class TonberryNewCommand : TonberryChangelogCommand<TonberryNewOptions> { }

public class TonberryReleaseCommand : TonberryChangelogCommand<TonberryReleaseOptions> { }

public abstract class TonberryChangelogCommand<T> : ITonberryCommand<T> where T : TonberryChangelogOptions, new()
{
    public TonberryConfiguration Config { get; set; }

    public bool Success { get; set; }

    public T Options { get; set; }

    public TonberryPrompt<T> Prompt { get; set; }

    internal TonberryChangelogCommand() { }

    public virtual void Validate() => Options.Validate();
}