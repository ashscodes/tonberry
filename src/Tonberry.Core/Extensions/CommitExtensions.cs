using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class CommitExtensions
{
    public static TonberryResult Process(this TonberryCommit commit,
                                         TonberryConfiguration config,
                                         TonberryCommitOptions options)
    {
        config.Init();
        var message = commit.ToString();
        if (!config.HasStagedFiles())
        {
            try
            {
                Ensure.IsTrue(options.StageAll, Resources.NoStagedFiles);
            }
            catch (TonberryApplicationException ex)
            {
                config.Close();
                return new TonberryCommitResult(config.GetCommitPreview(message), ex);
            }

            config.Repository.StageAll();
        }

        if (!options.IsPreview)
        {
            config.Repository.Commit(message);
            if (options.Push)
            {
                config.Repository.TrackRemoteBranch();
                config.Repository.Push();
            }
        }

        config.Close();
        return new TonberryCommitResult("commit", config.GetCommitPreview(message), true);
    }

    public static IEnumerable<TonberryCommit> GetCommits(this ICommitLog commitLog, BaseConfiguration config)
    {
        if (commitLog is null)
        {
            yield return null;
        }

        Tree previousTree = null;
        foreach (Commit commit in commitLog)
        {
            if (config.HasExclusions && commit.IsExcluded(config, previousTree))
            {
                continue;
            }

            var tonberryCommit = commit.ToTonberryCommit();
            tonberryCommit.Parse();
            yield return tonberryCommit;
            previousTree = commit.Tree;
        }
    }

    private static IEnumerable<string> Enumerate(this Commit commit,
                                                 BaseConfiguration config,
                                                 Tree previous = null)
    {
        if (previous is not null)
        {
            var diffs = config.Repository.CompareTreeChanges(previous, commit.Tree);
            foreach (var diff in diffs)
            {
                yield return diff.Path;
            }
        }
        else
        {
            foreach (var path in commit.Tree.Enumerate())
            {
                yield return path;
            }
        }
    }

    private static IEnumerable<string> Enumerate(this Tree tree)
    {
        foreach (var item in tree)
        {
            if (item.TargetType == TreeEntryTargetType.Blob)
            {
                yield return item.Path;
            }

            if (item.TargetType == TreeEntryTargetType.Tree)
            {
                foreach (var path in ((Tree)item.Target).Enumerate())
                {
                    yield return path;
                }
            }
        }
    }

    private static bool IsExcluded(this Commit commit, BaseConfiguration config, Tree previous = null)
        => commit.Enumerate(config, previous).All(p => config.IsPathExcluded(p));

    private static TonberryCommit Parse(this TonberryCommit commit)
    {
        ICommitParser parser = TonberryOptions.CommitParser ?? new DefaultCommitParser();
        parser.Parse(commit.RawMessage);
        commit.Breaking = parser.Breaking;
        commit.Description = parser.Description;
        commit.Scope = parser.Scope;
        commit.Synopsis = parser.Synopsis;
        commit.Type = parser.Type;
        return commit;
    }

    private static TonberryAuthor ToTonberryAuthor(this Commit commit)
        => new TonberryAuthor()
        {
            Email = commit.Author.Email,
            Name = commit.Author.Name,
            Time = commit.Author.When
        };

    internal static TonberryCommit NewTonberryCommit(this TonberryCommitOptions options)
        => new TonberryCommit
        {
            Closes = options.Closes,
            Resolves = options.Resolves,
            Breaking = options.IsBreaking ? options.IsBreaking.ToString() : null,
            Description = options.Detail,
            Scope = options.Scope,
            Synopsis = options.Message,
            Type = options.Type
        };

    internal static TonberryCommit ToTonberryCommit(this Commit commit)
        => new TonberryCommit()
        {
            Author = commit.ToTonberryAuthor(),
            Id = commit.Sha,
            RawMessage = commit.Message
        };
}