using System.IO;
using System.Linq;
using LibGit2Sharp;
using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class ConfigurationExtensions
{
    private static CommitFilter GetFilter(GitObject from, GitObject to)
    {
        var filter = new CommitFilter()
        {
            SortBy = CommitSortStrategies.Time
        };

        if (from is not null)
        {
            filter.IncludeReachableFrom = from.Sha;
        }

        if (to is not null)
        {
            filter.ExcludeReachableFrom = to.Sha;
        }

        return filter;
    }

    internal static void Close(this BaseConfiguration config) => config?.Repository?.Dispose();

    internal static bool ContainsMaintainer(this TonberryConfiguration config, string name)
        => config.Maintainers is not null && config.Maintainers.Any(m => m.Equals(name, Resources.StrCompare));

    internal static bool ContainsProject(this TonberryConfiguration config, string name)
        => config.Name.Equals(name, Resources.StrCompare)
           || (config.Projects is not null && config.Projects.Any(p => p.Name.Equals(name, Resources.StrCompare)));

    internal static TonberryConfiguration Process(this TonberryConfiguration config)
    {
        if (config.Directory is not null)
        {
            config.Changelog = config.Directory?.FindOrCreateFile(Resources.ChangelogFile, false);
            config.Configuration = new FileInfo(Path.Combine(config.Directory.FullName, Resources.TonberryRootConfig));
            config.Name = config.Directory.Name;
        }

        if (config.IsMonoRepo)
        {
            config.Exclusions ??= [];
            foreach (var project in config.Projects)
            {
                config.Exclusions.Add(project.RelativePath);
                project.Directory = new DirectoryInfo(Path.Combine(config.Directory.FullName, project.RelativePath));
                if (project.Directory.Exists)
                {
                    project.Changelog = new FileInfo(Path.Combine(project.Directory.FullName,
                                                                  string.Format(Resources.ProjectChangelogFile,
                                                                                project.Name)));
                }
            }
        }

        return config;
    }

    internal static TonberryConfiguration Process(this TonberryConfiguration config, TonberryInitOptions options)
    {
        config.Process();
        config.CommitTypes ??= TonberryCommitType.GetDefault();
        config.Maintainers = ["Your.Name"];
        config.Name = Ensure.StringHasValue(options.Name, config.Directory.Name);
        config.Version = options.Version;
        if (options.Repository is not null)
        {
            config.CommitUrlFormat = options.Repository.AbsoluteUri + "/commit/{0}";
            config.CompareUrlFormat = options.Repository.AbsoluteUri + "/compare/{0}...{1}";
            config.IssueUrlFormat = options.Repository.AbsoluteUri + "/issues/{0}";
            config.ProjectUrlFormat = options.Repository.AbsoluteUri + ".git";
            config.UserUrlFormat = options.Repository.Scheme + "://" + options.Repository.Host + "/{0}";
        }

        return config;
    }

    internal static string GetCommitPreview(this BaseConfiguration config, string message)
        => string.Format(Resources.CommitPreview,
                         config.Repository.RemoteName,
                         config.Repository.BranchName,
                         config.Repository.Tip.Sha[0..8],
                         message);

    internal static TonberryCommitCollection GetAllCommits(this BaseConfiguration config)
    {
        if (config.Repository is not null)
        {
            GitObject head = config.Repository.Lookup("HEAD");
            var filter = GetFilter(head, null);
            var commitLog = config.Repository.Query(filter);
            var commits = commitLog.GetCommits(config);
            return new TonberryCommitCollection(commits, null, null);
        }

        return [];
    }

    internal static FileInfo GetChangelog(this TonberryConfiguration config, string projectName)
    {
        if (projectName.Equals(config.Name, Resources.StrCompare))
        {
            return config.Changelog;
        }

        return config.Projects?.FirstOrDefault(ct => ct.Name.Equals(projectName,
                                                             Resources.StrCompare))?.Changelog;
    }

    internal static TonberryCommitCollection GetCommits(this BaseConfiguration config,
                                                        TonberryTag current = null,
                                                        TonberryTag previous = null)
    {
        if (config.Repository is not null)
        {
            GitObject currentTag = null;
            GitObject previousTag = null;
            if (current is not null)
            {
                currentTag = config.Repository.Lookup(current.Sha);
            }

            if (previous is not null)
            {
                previousTag = config.Repository.Lookup(previous.Sha);
            }

            var filter = GetFilter(currentTag, previousTag);
            var commitLog = config.Repository.Query(filter);
            var commits = commitLog.GetCommits(config);
            return new TonberryCommitCollection(commits, current, previous);
        }

        return [];
    }

    internal static TonberryCommitType GetCommitType(this TonberryConfiguration config, string name)
        => config.CommitTypes.FirstOrDefault(c => c.Name.Equals(name, Resources.StrCompare));

    internal static string GetEmoji(this TonberryConfiguration config, string name = null)
    {
        if (config.IncludeEmojis)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var commitType = config.CommitTypes.FirstOrDefault(ct => ct.Name.Equals(name, Resources.StrCompare));
                return commitType is null ? string.Empty : commitType.GetEmoji();
            }

            return Ensure.StringHasValue(config.ReleaseEmoji, string.Empty);
        }

        return string.Empty;
    }

    internal static BaseConfiguration GetProjectConfig(this TonberryConfiguration config, string projectName)
    {
        if (config.ContainsProject(projectName))
        {
            if (projectName.Equals(config.Name, Resources.StrCompare))
            {
                return config;
            }

            return config.Projects.FirstOrDefault(project => projectName.Equals(project.Name, Resources.StrCompare));
        }

        return null;
    }

    internal static TonberryProject GetProjectHistory(this BaseConfiguration config,
                                                      TonberryProject project,
                                                      TonberryTagCollection tags)
    {
        var latestTag = tags.GetLatestTag();
        for (int i = 1; i < tags.Count; i++)
        {
            project.Add(config.GetCommits(latestTag, tags[i]));
            latestTag = tags[i];
        }

        project.Add(config.GetCommits(latestTag));
        return project;
    }

    internal static TonberryProject GetProjectLatest(this BaseConfiguration config, TonberryTag to)
        => new TonberryProject(config.Name) { config.GetCommits(null, to) };

    internal static TonberryProjectCollection GetProjects<T>(this TonberryConfiguration config, T options)
        where T : TonberryChangelogOptions, new()
    {
        var projects = new TonberryProjectCollection();
        config.Init();
        TonberryTagCollection tags = config.GetTags();
        config.Close();
        Ensure.IsFalse(tags.IsMonoRepo && !config.IsMonoRepo, Resources.NoProjectsDefined);
        switch (tags.Count)
        {
            case int x when x == 0 && config.IsMonoRepo:
                projects.GetUntaggedProjects(config);
                break;
            case int x when x == 0 && !config.IsMonoRepo:
                projects.GetUntaggedProject(config);
                break;
            case int x when x > 0 && config.IsMonoRepo:
                projects.GetTaggedProjects(config, tags, (options is TonberryReleaseOptions));
                break;
            case int x when x > 0 && !config.IsMonoRepo:
                projects.GetTaggedProject(config, tags, (options is TonberryReleaseOptions));
                break;
            default:
                break;
        }

        return projects;
    }

    internal static TonberryTagCollection GetTags(this TonberryConfiguration config)
    {
        if (config.Repository is not null)
        {
            var tagCollection = new TonberryTagCollection();
            foreach (var tag in config.Repository.GetTags())
            {
                var tonberryTag = new TonberryTag(tag.Target.Sha);
                tonberryTag.Parse(tag.FriendlyName);
                tonberryTag.ProjectName = Ensure.StringHasValue(tonberryTag.ProjectName, config.Name);
                if (tonberryTag.Version is not null)
                {
                    tagCollection.Add(tonberryTag);
                }
            }

            config.Close();
            var latest = tagCollection.GetLatestTag();
            if (latest is not null)
            {
                latest.IsNext = true;
            }

            return tagCollection;
        }

        return [];
    }

    internal static string GetTagTemplate(this BaseConfiguration config)
    {
        if (config is TonberryProjectConfiguration)
        {
            return Ensure.StringHasValue(config.TagTemplate, Resources.DefaultMonoRepoProjectTag);
        }

        return Ensure.StringHasValue(config.TagTemplate, Resources.DefaultTag);
    }

    internal static string GetTagTemplate(this TonberryConfiguration config, string projectName)
    {
        var projectConfig = config.GetProjectConfig(projectName);
        return config.GetTagTemplate();
    }

    internal static bool HasStagedFiles(this BaseConfiguration config, StatusOptions options = null)
    {
        if (config.Repository is not null)
        {
            options ??= new StatusOptions();
            return config.Repository.RetrieveStatus(options).Any(s => s.State.ToString().Contains("Index"));
        }

        return false;
    }

    internal static bool IsMonoRepoProject(this TonberryConfiguration config, string projectName)
        => config.Projects is not null && config.Projects.Any(p => projectName.Equals(p.Name, Resources.StrCompare));

    internal static bool IsPathExcluded(this BaseConfiguration config, string path)
    {
        if (config.Exclusions is null || config.Exclusions.Count == 0)
        {
            return false;
        }

        return config.Exclusions.All(e => path.StartsWith(e));
    }

    internal static BaseConfiguration Init(this BaseConfiguration config)
    {
        if (config.Directory.Exists)
        {
            if (Repository.IsValid(config.Directory.FullName))
            {
                config.Repository = new TonberryRepository(new Repository(config.Directory.FullName));
                return config;
            }
        }

        throw new TonberryApplicationException(Resources.NoRepositoryFound, config.Directory.FullName);
    }

    internal static bool TryAddRelease(this BaseConfiguration config, TonberryRelease release)
    {
        if (config.Repository is not null)
        {
            string releaseTag;
            string tagTemplate = config.GetTagTemplate();
            if (config is TonberryProjectConfiguration)
            {
                releaseTag = string.Format(tagTemplate, release.Current.Version, release.ProjectName);
            }
            else
            {
                releaseTag = string.Format(tagTemplate, release.Current.Version);
            }

            Commit commit = config.Repository.Commit(string.Format(Resources.ReleaseMessage, releaseTag));
            Tag tag = config.Repository.ApplyTag(tagTemplate, commit);
            config.Repository.StageAll();
            config.Repository.Push();
            config.Repository.Push(tag);
            return true;
        }

        return false;
    }
}