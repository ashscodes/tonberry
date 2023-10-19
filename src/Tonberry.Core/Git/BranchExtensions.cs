using LibGit2Sharp;

namespace Tonberry.Core.Git;

internal static class BranchExtensions
{
    public static string PreviewCommit(this Branch branch, string message) => string.Format(Resources.CommitPreview,
                                                                                            branch.RemoteName,
                                                                                            branch.FriendlyName,
                                                                                            branch.Tip.Sha[0..8],
                                                                                            message);
}