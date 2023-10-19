namespace Tonberry.Core.Command.Options;

public class TonberryReleaseOptions : TonberryChangelogOptions
{
    public bool BumpMajorVersion { get; set; } = false;

    public bool BumpMinorVersion { get; set; } = false;

    public bool IsReleaseCandidate { get; set; } = false;

    public string ReleaseLabel { get; set; }

    public bool VersionOnly { get; set; } = false;

    public override bool IsPreview { get; set; } = false;

    public override bool IsTemp { get; set; } = false;

    public TonberryReleaseOptions() { }

    public override void Validate() { }
}