namespace Tonberry.Core.Command.Options;

public class TonberryNewOptions : TonberryChangelogOptions
{
    public override bool IsPreview { get; set; } = false;

    public override bool IsTemp { get; set; } = false;

    public override void Validate() { }
}