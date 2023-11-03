using System.IO;
using System.Linq;
using Tonberry.Core.Model;

namespace Tonberry.Core;

public static class CommandExtensions
{
    public static void CreateConfig<T>(this ITonberryCommand<T> command, DirectoryInfo directory)
        where T : ITonberryCommandOptions, new()
    {
        if (command.Options is not null && command.Options is TonberryInitOptions initOptions)
        {
            command.Config = new TonberryConfiguration(directory).Create(initOptions);
            return;
        }

        command.Config = new TonberryConfiguration(directory).Create();
    }

    public static TonberryResultCollection<FileInfo> Invoke<T>(this TonberryChangelogCommand<T> command)
        where T : TonberryChangelogOptions, new()
    {
        command.Validate();
        var projects = new TonberryProjectCollection().GetProjects(command);
        TonberryResultCollection<FileInfo> results = new();
        bool success = false;
        foreach (var project in projects)
        {
            BaseConfiguration config;
            TonberryFileResult result = null;
            try
            {
                config = command.GetProjectConfig(project.Name);
            }
            catch (TonberryApplicationException ex)
            {
                results.Add(new TonberryFileResult(ex, project.Name));
                continue;
            }

            var releases = project.GetReleases();
            releases.GetNewTag(command.Options, config.Version);
            if (command.Options is TonberryReleaseOptions releaseOptions && releaseOptions.VersionOnly)
            {
                result = new TonberryFileResult(config.Changelog, project.Name, null, releases.Current.Version);
                success = true;
            }
            else
            {
                success = releases.TryWrite(command.Config, command.Options, out FileInfo output);
                result = new TonberryFileResult(config.Changelog, project.Name, output, releases.Current.Version);
                if (!command.Options.IsPreview && releases.First is not null)
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

    public static TonberryCommitResult Invoke(this TonberryCommitCommand command)
    {
        command.Validate();
        var commit = command.Options.NewTonberryCommit();
        var message = commit.ToString();
        command.Config.Init();
        if (!command.Config.HasStagedFiles())
        {
            Ensure.IsTrue(command.Options.StageAll, Resources.NoStagedFiles);
            command.Config.Repository.StageAll();
        }

        if (!command.Options.IsPreview)
        {
            command.Config.Repository.Commit(message);
            if (command.Options.Push)
            {
                command.Config.Repository.TrackRemoteBranch();
                command.Config.Repository.Push();
            }
        }

        command.Config.Close();
        return new TonberryCommitResult("commit", command.Config.GetCommitPreview(message), true);
    }

    public static void ReadConfig<T>(this ITonberryCommand<T> command, DirectoryInfo directory)
        where T : ITonberryCommandOptions, new()
    {
        var config = new FileInfo(Path.Combine(directory.FullName, Resources.TonberryRootConfig));
        Ensure.IsTrue(config.Exists, Resources.RootConfigNotFound);
        var contents = File.ReadAllText(config.FullName);
        Ensure.StringNotNullOrEmpty(contents, Resources.RootConfigNotFound);
        using var reader = new StringReader(contents);
        command.Config = Util.GetYamlDeserializer().Deserialize<TonberryConfiguration>(reader);
    }

    internal static BaseConfiguration GetProjectConfig<T>(this ITonberryCommand<T> command, string name)
        where T : ITonberryCommandOptions, new()
    {
        if (command.Config.ContainsProject(name))
        {
            if (name.Equals(command.Config.Name, Resources.StrCompare))
            {
                return command.Config;
            }

            return command.Config.Projects.FirstOrDefault(project => name.Equals(project.Name, Resources.StrCompare));
        }

        throw new TonberryApplicationException(Resources.NoProjectDefined, name);
    }
}