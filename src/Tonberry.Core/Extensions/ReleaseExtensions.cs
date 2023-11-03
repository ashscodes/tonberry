using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class ReleaseExtensions
{
    public static TonberryRelease AddCommits(this TonberryRelease release, TonberryCommitCollection commits)
    {
        release.Breaking = commits.Where(c => c.IsBreaking);
        release.Commits = commits.GroupBy(c => c.Type)?.ToDictionary(c => c.Key, c => c.AsEnumerable());
        release.Current = commits.Tag ?? new TonberryTag();
        release.Previous = commits.Previous;
        release.Time = commits?.FirstOrDefault()?.Author?.Time;
        commits.Sort();
        release.LatestCommit = commits.First;
        return release;
    }

    public static void GetNewTag(this TonberryReleaseCollection releases,
                                 TonberryChangelogOptions options,
                                 TonberryVersion configVersion)
    {
        releases.Sort();
        releases.Reverse();
        if (releases.First is not null)
        {
            releases.First.GetNewTag(options, configVersion);
            releases.Current = releases.First.Current;
        }
    }

    public static void GetNewTag(this TonberryRelease release,
                                 TonberryChangelogOptions options,
                                 TonberryVersion configVersion)
    {
        var newVersion = release.Previous.Version.GetNextIteration(options,
                                                                   release.Commits.ContainsKey("feat"),
                                                                   release.IsBreaking,
                                                                   configVersion);

        release.Current = new TonberryTag(release.LatestCommit.Id);
        release.Current.Version = newVersion;
    }

    public static bool TryWrite(this TonberryReleaseCollection releases,
                                TonberryConfiguration config,
                                TonberryChangelogOptions options,
                                out FileInfo tempFile)
    {
        tempFile = null;
        try
        {
            var writer = new ChangelogWriter(config, releases.ProjectName);
            switch (options)
            {
                case TonberryMigrateOptions:
                case TonberryNewOptions when releases.Count > 1:
                    foreach (var release in releases)
                    {
                        release.Write(config, writer);
                    }
                    break;
                case TonberryNewOptions when releases.Count == 1:
                case TonberryReleaseOptions:
                    releases.First.Write(config, writer);
                    if (!options.IsPreview && options is TonberryReleaseOptions)
                    {
                        writer.AddReleasesFromCurrentLog();
                    }
                    break;
                default:
                    break;
            }

            writer?.Dispose(out tempFile);
            return tempFile.Exists;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void Write(this TonberryRelease release, TonberryConfiguration config, ChangelogWriter writer)
    {
        writer.WriteReleaseHeader(release.Current, release.Time, release.Previous);
        if (release.IsConventional)
        {
            if (release.IsBreaking)
            {
                writer.WriteHeader(3, Resources.MarkdownBreakingChangeHeader);
                writer.WriteBreakingChanges(release.Breaking);
            }

            foreach (var commitConfig in config.CommitTypes)
            {
                if (release.Commits.ContainsKey(commitConfig.Name) && !commitConfig.IsHidden)
                {
                    writer.WriteCommitsByType(commitConfig, release.Commits[commitConfig.Name]);
                }
            }

            if (config.ListContributors)
            {
                var contributions = release.GetContributions(config).GroupBy(c => c.Author.Name).OrderBy(c => c.Key);
                if (contributions.Any())
                {
                    writer.WriteContributors(contributions);
                }
            }
        }
    }

    private static IEnumerable<TonberryCommit> GetContributions(this TonberryRelease release, TonberryConfiguration config)
    {
        foreach (var commitConfig in config.CommitTypes)
        {
            if (release.Commits.ContainsKey(commitConfig.Name) && !commitConfig.IsHidden)
            {
                foreach (var commit in release.Commits[commitConfig.Name])
                {
                    if (!config.ContainsMaintainer(commit.Author.Name))
                    {
                        yield return commit;
                    }
                }
            }
        }
    }
}