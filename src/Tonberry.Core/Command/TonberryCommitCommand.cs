using Tonberry.Core.Command.Options;
using Tonberry.Core.Model;

namespace Tonberry.Core.Command;

public sealed class TonberryCommitCommand : ITonberryCommand<TonberryCommitOptions>
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