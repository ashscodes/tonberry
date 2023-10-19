namespace Tonberry.Core.Command.Options;

public class TonberryMigrateOptions : TonberryChangelogOptions
{
    public override bool IsPreview { get; set; }

    public override bool IsTemp { get; set; }

    public override void Validate()
    {
        throw new System.NotImplementedException();
    }
}