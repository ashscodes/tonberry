using Tonberry.Core.Command.Options;

namespace Tonberry.Core.Command;

public class TonberryMigrateCommand : TonberryChangelogCommand<TonberryMigrateOptions>
{
    public override TonberryMigrateOptions Options { get; set; }

    public override TonberryPrompt<TonberryMigrateOptions> Prompt { get; set; }
}