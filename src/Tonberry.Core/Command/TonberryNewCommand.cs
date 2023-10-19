using Tonberry.Core.Command.Options;

namespace Tonberry.Core.Command;

public class TonberryNewCommand : TonberryChangelogCommand<TonberryNewOptions>
{
    public override TonberryNewOptions Options { get; set; }

    public override TonberryPrompt<TonberryNewOptions> Prompt { get; set; }
}