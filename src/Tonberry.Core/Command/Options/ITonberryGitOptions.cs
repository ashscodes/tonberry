namespace Tonberry.Core.Command.Options;

public interface ITonberryGitOptions : ITonberryCommandOptions
{
    bool IsPreview { get; set; }
}