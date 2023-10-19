namespace Tonberry.Core.Command.Options;

public abstract class TonberryChangelogOptions : ITonberryGitOptions
{
    public abstract bool IsPreview { get; set; }

    public abstract bool IsTemp { get; set; }

    public abstract void Validate();
}