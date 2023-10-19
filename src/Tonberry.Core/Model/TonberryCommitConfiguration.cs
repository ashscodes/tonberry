using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tonberry.Core.Internal;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Model;

public sealed class TonberryCommitConfiguration
{
    [Required]
    public string Emoji { get; set; }

    [Required]
    [YamlMember(Alias = "doNotShowInChangeLog", ApplyNamingConventions = false)]
    public bool IsHidden { get; set; }

    [Required]
    [YamlMember(Alias = "logDisplayName", ApplyNamingConventions = false)]
    public string LogDisplayName { get; set; }

    [Required]
    public string Name { get; set; }

    public TonberryCommitConfiguration() { }

    public TonberryCommitConfiguration(string name, string logDisplayName, string emoji, bool isHidden) : this()
    {
        Emoji = emoji;
        IsHidden = isHidden;
        LogDisplayName = logDisplayName;
        Name = name;
    }

    internal string GetEmoji() => Ensure.StringHasValue(Emoji, string.Empty);

    internal static List<TonberryCommitConfiguration> GetDefault() =>
    [
        new("build", "Build", ":construction_worker:", false),
        new("chore", "Chores", ":notebook:", true),
        new("ci", "CI", ":ferris_wheel:", false),
        new("docs", "Documentation", ":books:", false),
        new("feat", "Features", ":sparkling_heart:", false),
        new("fix", "Bug Fixes", ":bug:", false),
        new("perf", "Performance Improvements", ":electric_plug:", false),
        new("refactor", "Code Refactoring", ":recycle:", false),
        new("revert", "Reverted Changes", ":dash:", false),
        new("style", "Styling", ":ribbon:", false),
        new("test", "Tests", ":vertical_traffic_light:", false),
    ];
}