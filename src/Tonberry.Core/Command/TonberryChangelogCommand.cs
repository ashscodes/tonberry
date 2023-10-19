using Tonberry.Core.Command.Options;
using Tonberry.Core.Model;

namespace Tonberry.Core.Command;

public abstract class TonberryChangelogCommand<T> : ITonberryCommand<T> where T : TonberryChangelogOptions, new()
{
    public TonberryConfiguration Config { get; set; }

    public bool Success { get; set; }

    public abstract T Options { get; set; }

    public abstract TonberryPrompt<T> Prompt { get; set; }

    internal TonberryChangelogCommand() { }

    public virtual void Validate() => Options.Validate();
}