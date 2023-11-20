using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Model;

[TypeConverter(typeof(TonberryCommitTypeConverter))]
public sealed class TonberryCommitType
{
    [Required]
    public string Emoji { get; set; }

    [Required]
    [YamlMember(Alias = "doNotShowInChangelog", ApplyNamingConventions = false)]
    public bool IsHidden { get; set; }

    [Required]
    [YamlMember(Alias = "logDisplayName", ApplyNamingConventions = false)]
    public string LogDisplayName { get; set; }

    [Required]
    public string Name { get; set; }

    public TonberryCommitType() { }

    public TonberryCommitType(string name, string logDisplayName, string emoji, bool isHidden) : this()
    {
        Emoji = emoji;
        IsHidden = isHidden;
        LogDisplayName = logDisplayName;
        Name = name;
    }

    internal string GetEmoji() => Ensure.StringHasValue(Emoji, string.Empty);

    internal static List<TonberryCommitType> GetDefault() =>
    [
        new("feat", "Features", ":sparkling_heart:", false),
        new("fix", "Bug Fixes", ":bug:", false),
        new("perf", "Performance Improvements", ":electric_plug:", false),
        new("test", "Tests", ":vertical_traffic_light:", false),
        new("docs", "Documentation", ":books:", false),
        new("refactor", "Code Refactoring", ":recycle:", false),
        new("style", "Styling", ":ribbon:", false),
        new("revert", "Reverted Changes", ":dash:", false),
        new("build", "Build", ":construction_worker:", false),
        new("ci", "CI", ":ferris_wheel:", false),
        new("chore", "Chores", ":notebook:", true),
    ];
}