using Tonberry.Core.Command.Options;

namespace Tonberry.Core.Command;

public class TonberryReleaseCommand : TonberryChangelogCommand<TonberryReleaseOptions>
{
    public override TonberryReleaseOptions Options { get; set; }

    public override TonberryPrompt<TonberryReleaseOptions> Prompt { get; set; }

    public TonberryReleaseCommand() : base() { }
}