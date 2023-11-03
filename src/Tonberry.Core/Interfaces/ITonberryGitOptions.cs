namespace Tonberry.Core;

public interface ITonberryGitOptions : ITonberryCommandOptions
{
    bool IsPreview { get; set; }
}