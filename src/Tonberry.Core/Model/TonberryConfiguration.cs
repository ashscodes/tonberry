using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Tonberry.Core.Command.Options;
using Tonberry.Core.Internal;
using Tonberry.Core.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tonberry.Core.Model;

public class TonberryConfiguration : BaseConfiguration
{
    [Required]
    [YamlMember(Alias = "commitTypes", ApplyNamingConventions = false)]
    public List<TonberryCommitConfiguration> CommitTypes { get; set; }

    [Required]
    [YamlMember(Alias = "commitUrlFormat", ApplyNamingConventions = false)]
    public string CommitUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/commit/{0}";

    [Required]
    [YamlMember(Alias = "compareUrlFormat", ApplyNamingConventions = false)]
    public string CompareUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/compare/{0}...{1}";

    [YamlMember(Alias = "excludeFromChangelog", ApplyNamingConventions = false)]
    public List<string> Exclusions { get; set; }

    [Required]
    [YamlMember(Alias = "includeEmojis", ApplyNamingConventions = false)]
    public bool IncludeEmojis { get; set; } = true;

    [Required]
    [YamlMember(Alias = "issueUrlFormat", ApplyNamingConventions = false)]
    public string IssueUrlFormat { get; set; } = "https://<host>/<path>/<projectName>/issues/{0}";

    public List<string> Maintainers { get; set; }

    [Required]
    [YamlMember(Alias = "primaryLanguage", ApplyNamingConventions = false)]
    public string PrimaryLanguage { get; set; }

    public List<TonberryProjectConfiguration> Projects { get; set; } = [new("Example")];

    [Required]
    [YamlMember(Alias = "projectUrl", ApplyNamingConventions = false)]
    public string ProjectUrlFormat { get; set; } = "https://<host>/<path>/<projectName>.git";

    [YamlMember(Alias = "releaseEmoji", ApplyNamingConventions = false)]
    public string ReleaseEmoji { get; set; } = ":rocket:";

    [Required]
    [YamlMember(Alias = "thankContributors", ApplyNamingConventions = false)]
    public bool ThankContributors { get; set; } = true;

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

    protected TonberryConfiguration(DirectoryInfo directory) : this() => Validate(directory);

    protected TonberryConfiguration(DirectoryInfo directory,
                                    TonberryInitOptions options) : this(directory) => Initialise(options);

    public override string ToString() => GetYamlSerialiser().Serialize(this);

    public override void Validate(DirectoryInfo directory)
    {
        Directory = directory;
        Changelog = Directory?.FindOrCreateFile(Resources.ChangelogFile, false);
        Configuration = new FileInfo(Path.Combine(Directory.FullName, Resources.TonberryRootConfig));
        if (IsMonoRepo)
        {
            foreach (var project in Projects)
            {
                project.Validate(Directory);
            }
        }
    }

    public virtual void Save() => File.WriteAllText(Configuration.FullName, ToString());

    public static TonberryConfiguration Read(FileSystemInfo file)
    {
        var config = file is FileInfo fileInfo ?
            fileInfo : new(Path.Combine(file.FullName, Resources.TonberryRootConfig));

        Ensure.IsTrue(config.Exists, Resources.RootConfigNotFound);
        var contents = File.ReadAllText(config.FullName);
        Ensure.StringNotNullOrEmpty(contents, Resources.RootConfigNotFound);
        using var reader = new StringReader(contents);
        return GetYamlDeserializer().Deserialize<TonberryConfiguration>(reader);
    }

    private void Initialise(TonberryInitOptions options)
    {
        CommitTypes ??= TonberryCommitConfiguration.GetDefault();
        Maintainers = ["Your.Email"];
        Name = options.Name;
        Version = options.Version;
        if (options.Repository is not null)
        {
            CommitUrlFormat = options.Repository.AbsoluteUri + "/commit/{0}";
            CompareUrlFormat = options.Repository.AbsoluteUri + "/compare/{0}...{1}";
            IssueUrlFormat = options.Repository.AbsoluteUri + "/issues/{0}";
            ProjectUrlFormat = options.Repository.AbsoluteUri + ".git";
            UserUrlFormat = options.Repository.Scheme + "://" + options.Repository.Host + "/{0}";
        }
    }

    protected internal override bool TryUpdateProjectFiles() => throw new NotImplementedException();

    protected internal static IDeserializer GetYamlDeserializer() => new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithRequiredPropertyValidation()
        .WithTypeConverter(new TonberryVersionConverter())
        .IgnoreUnmatchedProperties()
        .Build();

    protected internal static ISerializer GetYamlSerialiser() => new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new TonberryVersionConverter())
        .Build();

    internal bool ContainsProject(string name)
        => Name.Equals(name, Resources.StrCompare) || Projects.Any(p => p.Name.Equals(name, Resources.StrCompare));

    internal FileInfo GetChangelog(string projectName)
    {
        if (projectName.Equals(Name, Resources.StrCompare))
        {
            return Changelog;
        }

        return Projects?.FirstOrDefault(ct => ct.Name.Equals(projectName,
                                                             Resources.StrCompare))?.Changelog;
    }

    internal TonberryCommitConfiguration GetCommitType(string name)
        => CommitTypes.FirstOrDefault(c => c.Name.Equals(name, Resources.StrCompare));

    internal string GetEmoji(string name = null)
    {
        if (IncludeEmojis)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var commitType = CommitTypes.FirstOrDefault(ct => ct.Name.Equals(name, Resources.StrCompare));
                return commitType is null ? string.Empty : commitType.GetEmoji();
            }

            return Ensure.StringHasValue(ReleaseEmoji, string.Empty);
        }

        return string.Empty;
    }

    internal bool IsMaintainer(string name)
        => Maintainers is not null && Maintainers.Any(m => m.Equals(name, Resources.StrCompare));
}