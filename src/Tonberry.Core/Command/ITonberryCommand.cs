using Tonberry.Core.Command.Options;
using Tonberry.Core.Model;

namespace Tonberry.Core.Command;

public interface ITonberryCommand
{
    TonberryConfiguration Config { get; set; }

    bool Success { get; set; }

    void Validate();
}

public interface ITonberryCommand<T> : ITonberryCommand where T : ITonberryCommandOptions, new()
{
    T Options { get; set; }

    TonberryPrompt<T> Prompt { get; set; }
}