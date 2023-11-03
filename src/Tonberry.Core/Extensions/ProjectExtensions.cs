using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class ProjectExtensions
{
    public static TonberryProjectCollection GetProjects<T>(this TonberryProjectCollection projects,
                                                           TonberryChangelogCommand<T> command) where T : TonberryChangelogOptions, new()
    {
        command.Config.Init();
        TonberryTagCollection tags = command.Config.GetTags();
        command.Config.Close();
        Ensure.IsFalse(tags.IsMonoRepo && !command.Config.IsMonoRepo, Resources.NoProjectsDefined);
        switch (tags.Count)
        {
            case int x when x == 0 && command.Config.IsMonoRepo:
                projects.GetUntaggedProjects(command.Config);
                break;
            case int x when x == 0 && !command.Config.IsMonoRepo:
                projects.GetUntaggedProject(command.Config);
                break;
            case int x when x > 0 && command.Config.IsMonoRepo:
                projects.GetTaggedProjects(command.Config, tags, (typeof(T) == typeof(TonberryReleaseOptions)));
                break;
            case int x when x > 0 && !command.Config.IsMonoRepo:
                projects.GetTaggedProject(command.Config, tags, (typeof(T) == typeof(TonberryReleaseOptions)));
                break;
            default:
                break;
        }

        return projects;
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

    private static void GetTaggedProject(this TonberryProjectCollection projects,
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

    private static void GetTaggedProjects(this TonberryProjectCollection projects,
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

    private static void GetUntaggedProject(this TonberryProjectCollection projects, BaseConfiguration config)
    {
        config.Init();
        var project = new TonberryProject(config.Name) { config.GetAllCommits() };
        projects.Add(project);
        config.Close();
    }

    private static void GetUntaggedProjects(this TonberryProjectCollection projects, TonberryConfiguration config)
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