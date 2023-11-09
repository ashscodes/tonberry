namespace Tonberry.Core;

public interface ITonberryCommandOptions
{
    void Validate();
}

public interface ITonberryGitOptions : ITonberryCommandOptions
{
    bool IsPreview { get; set; }
}