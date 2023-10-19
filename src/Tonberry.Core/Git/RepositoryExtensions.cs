using System;
using System.Linq;
using LibGit2Sharp;

namespace Tonberry.Core.Git;

internal static class RepositoryExtensions
{
    internal const CommitSortStrategies SortStrategy = CommitSortStrategies.Time | CommitSortStrategies.Reverse;

    public static void CheckoutBranch(this Repository repository, string branchName, bool force = false)
    {
        Branch branch;
        if (repository.Branches.Any(b => b.FriendlyName == branchName))
        {
            branch = repository.Branches[branchName];
            Commands.Checkout(repository, branch);
        }
        else
        {
            if (force)
            {
                repository.CreateBranch(branchName);
            }
        }
    }

    public static ICommitLog GetCommitRangeBySha(this Repository repository, string from, string to)
    {
        var filter = new CommitFilter { SortBy = SortStrategy };
        var fromObject = repository.Lookup(from);
        if (fromObject is not null)
        {
            var toObject = repository.Lookup(to);
            if (toObject is not null)
            {
                filter.IncludeReachableFrom = fromObject.Sha;
                filter.ExcludeReachableFrom = toObject.Sha;
                return repository.Commits.QueryBy(filter);
            }

            throw new ArgumentOutOfRangeException(string.Format(Resources.ObjectNotFound, to));
        }

        throw new ArgumentOutOfRangeException(string.Format(Resources.ObjectNotFound, from));
    }

    public static ICommitLog GetCommitsFromSha(this Repository repository, string from)
    {
        var fromObject = repository.Lookup(from);
        if (fromObject is not null)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = fromObject.Sha,
                SortBy = SortStrategy
            };

            return repository.Commits.QueryBy(filter);
        }

        throw new ArgumentOutOfRangeException(string.Format(Resources.ObjectNotFound, from));
    }

    public static ICommitLog GetCommitsToSha(this Repository repository, string to)
    {
        var filter = new CommitFilter { SortBy = SortStrategy };
        var gitObject = repository.Lookup(to);
        if (gitObject is not null)
        {
            filter.ExcludeReachableFrom = gitObject.Sha;
            return repository.Commits.QueryBy(filter);
        }

        throw new ArgumentOutOfRangeException(string.Format(Resources.ObjectNotFound, to));
    }

    public static bool HasStagedFiles(this Repository repository, StatusOptions options = null)
    {
        options ??= new StatusOptions();
        return repository.RetrieveStatus(options).Any(s => s.State.ToString().Contains("Index"));
    }

    public static Commit NewCommit(this Repository repository, string message)
    {
        var author = repository.GetSignature();
        return repository.Commit(message, author, author, new CommitOptions());
    }

    public static void StageAll(this Repository repository) => Commands.Stage(repository, "*");

    public static void VerifyRemoteBranch(this Repository repository, string remoteName)
    {
        if (!repository.Head.IsTracking && repository.Head.TrackedBranch == null)
        {
            var remote = repository.Network.Remotes[remoteName];
            if (remote is null)
            {
                _ = repository.Branches.Update(repository.Head, branch =>
                {
                    branch.Remote = remote.Name;
                    branch.UpstreamBranch = repository.Head.CanonicalName;
                });
            }
        }
    }

    private static Signature GetSignature(this Repository repository)
        => new(repository.Config.Get<string>("user.name").Value,
               repository.Config.Get<string>("user.email").Value,
               DateTime.Now);
}