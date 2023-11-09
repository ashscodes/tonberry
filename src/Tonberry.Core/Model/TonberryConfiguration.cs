using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Model;

public class TonberryConfiguration : BaseConfiguration
{
    [Required]
    [YamlMember(Alias = "commitTypes", ApplyNamingConventions = false)]
    public List<TonberryCommitType> CommitTypes { get; set; }

    [Required]
    [YamlMember(Alias = "commitUrlFormat", ApplyNamingConventions = false)]
    public string CommitUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/commit/{0}";

    [Required]
    [YamlMember(Alias = "compareUrlFormat", ApplyNamingConventions = false)]
    public string CompareUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/compare/{0}...{1}";

    [Required]
    [YamlMember(Alias = "includeEmojis", ApplyNamingConventions = false)]
    public bool IncludeEmojis { get; set; } = true;

    [Required]
    [YamlMember(Alias = "issueUrlFormat", ApplyNamingConventions = false)]
    public string IssueUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/issues/{0}";

    [Required]
    [YamlMember(Alias = "listContributors", ApplyNamingConventions = false)]
    public bool ListContributors { get; set; } = true;

    public List<string> Maintainers { get; set; }

    public List<TonberryProjectConfiguration> Projects { get; set; } = [new("Example")];

    [Required]
    [YamlMember(Alias = "projectUrl", ApplyNamingConventions = false)]
    public string ProjectUrlFormat { get; set; } = "https://<host>/<path>/<projectName>.git";

    [YamlMember(Alias = "releaseEmoji", ApplyNamingConventions = false)]
    public string ReleaseEmoji { get; set; } = ":rocket:";

    [YamlMember(Alias = "thankContributorText", ApplyNamingConventions = false)]
    public string ThankContributorText { get; set; }

    [YamlMember(Alias = "trackNonMonoRepoProjectCommits", ApplyNamingConventions = false)]
    public bool TrackNonMonoRepoProjectCommits { get; set; } = false;

    [Required]
    [YamlMember(Alias = "userUrlFormat", ApplyNamingConventions = false)]
    public string UserUrlFormat { get; set; } = "https://<host>/{0}";

    protected internal FileInfo Configuration { get; internal set; }

    protected internal bool IsMonoRepo => Projects is not null && Projects.Count > 0;

    public TonberryConfiguration() : base() { }

    public TonberryConfiguration(DirectoryInfo directory) : base(directory) { }

    public override string ToString() => Util.GetYamlSerializer().Serialize(this);

    public virtual void Save() => File.WriteAllText(Configuration.FullName, ToString());

    public virtual void Save(FileInfo file) => File.WriteAllText(file.FullName, ToString());
}

public class TonberryProjectConfiguration : BaseConfiguration
{
    [Required]
    [YamlMember(Alias = "relativePath", ApplyNamingConventions = false)]
    public string RelativePath { get; set; } = Resources.MonoRepoPathExample;

    internal TonberryProjectConfiguration() : base() { }

    internal TonberryProjectConfiguration(string name) : base(name) { }
}

public abstract class BaseConfiguration
{
    [YamlMember(Alias = "excludeFromChangelog", ApplyNamingConventions = false)]
    public List<string> Exclusions { get; set; }

    public string Language { get; set; }

    [Required]
    public string Name { get; set; }

    [YamlMember(Alias = "projectFile", ApplyNamingConventions = false)]
    public string ProjectFile { get; set; }

    [YamlMember(Alias = "releaseSha", ApplyNamingConventions = false, Description = Resources.ShaConfigMessage)]
    public string ReleaseSha { get; internal set; } = string.Empty;

    [YamlMember(Alias = "tagTemplate", ApplyNamingConventions = false)]
    public string TagTemplate { get; internal set; } = string.Empty;

    public TonberryVersion Version { get; set; } = new TonberryVersion(0, 0, 0);

    internal FileInfo Changelog { get; set; }

    internal DirectoryInfo Directory { get; set; }

    internal bool HasExclusions => Exclusions is not null && Exclusions.Count > 0;

    internal TonberryRepository Repository { get; set; }

    internal BaseConfiguration() { }

    internal BaseConfiguration(string name) => Name = name;

    internal BaseConfiguration(DirectoryInfo directory) => Directory = directory;
}