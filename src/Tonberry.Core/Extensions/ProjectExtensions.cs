using System.IO;
using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class ProjectExtensions
{
    public static TonberryResultCollection Process(this TonberryProjectCollection projects,
                                                   TonberryConfiguration config,
                                                   TonberryChangelogOptions options)
    {
        var results = new TonberryResultCollection();
        bool success = false;
        foreach (var project in projects)
        {
            BaseConfiguration currentConfig = config.GetProjectConfig(project.Name);
            if (currentConfig is null)
            {
                var ex = new TonberryApplicationException(Resources.NoProjectDefined, project.Name);
                results.Add(new TonberryFileResult(project.Name, ex));
                continue;
            }

            TonberryFileResult result = null;
            var releases = project.GetReleases();
            releases.GetNewTag(options, config.Version);
            if (options is TonberryReleaseOptions releaseOptions && releaseOptions.VersionOnly)
            {
                result = new TonberryFileResult(config.Changelog, project.Name, null, releases.Current.Version);
                success = true;
            }
            else
            {
                success = releases.TryWrite(config, options, out FileInfo output);
                result = new TonberryFileResult(config.Changelog, project.Name, output, releases.Current.Version);
                if (!options.IsPreview && releases.First is not null)
                {
                    result.Write();
                    result.Success = success && config.TryAddRelease(releases.First);
                    if (result.Success)
                    {
                        config.ReleaseSha = releases.First.LatestCommit.Id;
                    }
                }
            }

            results.Add(result);
        }

        return results;
    }

    public static TonberryReleaseCollection GetReleases(this TonberryProject project)
    {
        project.Sort();
        project.Reverse();
        var releaseCollection = new TonberryReleaseCollection(project.Name);
        foreach (var commitCollection in project)
        {
            var release = new TonberryRelease(project.Name).AddCommits(commitCollection);
            releaseCollection.Add(release);
        }

        releaseCollection.Sort();
        releaseCollection.Reverse();
        return releaseCollection;
    }

    internal static void GetTaggedProject(this TonberryProjectCollection projects,
                                         BaseConfiguration config,
                                         TonberryTagCollection tags,
                                         bool isNewRelease)
    {
        config.Init();
        var project = config.GetProjectLatest(tags.GetLatestTag());
        if (!isNewRelease)
        {
            config.GetProjectHistory(project, tags);
        }

        config.Close();
        projects.Add(project);
    }

    internal static void GetTaggedProjects(this TonberryProjectCollection projects,
                                         TonberryConfiguration config,
                                         TonberryTagCollection tags,
                                         bool isNewRelease)
    {
        foreach (var project in config.Projects)
        {
            var projectTags = tags.GetProjectTags(project.Name);
            projects.GetTaggedProject(project, projectTags, isNewRelease);
        }

        if (!string.IsNullOrEmpty(config.Name))
        {
            var mainTags = tags.GetProjectTags(config.Name);
            projects.GetTaggedProject(config, mainTags, isNewRelease);
        }
    }

    internal static void GetUntaggedProject(this TonberryProjectCollection projects, BaseConfiguration config)
    {
        config.Init();
        var project = new TonberryProject(config.Name) { config.GetAllCommits() };
        projects.Add(project);
        config.Close();
    }

    internal static void GetUntaggedProjects(this TonberryProjectCollection projects, TonberryConfiguration config)
    {
        foreach (var project in config.Projects)
        {
            projects.GetUntaggedProject(project);
        }

        if (!string.IsNullOrEmpty(config.Name))
        {
            projects.GetUntaggedProject(config);
        }
    }
}