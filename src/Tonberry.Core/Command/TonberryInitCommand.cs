using Tonberry.Core.Command.Options;
using Tonberry.Core.Model;

namespace Tonberry.Core.Command;

public class TonberryInitCommand : ITonberryCommand<TonberryInitOptions>
{
    public TonberryConfiguration Config { get; set; }

    public TonberryInitOptions Options { get; set; }

    public TonberryPrompt<TonberryInitOptions> Prompt { get; set; }

    public bool Success { get; set; }

    public void Validate() => Options.Validate();
}