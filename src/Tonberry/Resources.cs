namespace Tonberry;

internal static class Resources
{
    public static string[] CmdExample1 = new[] { "tonberry", "--help" };

    public const string CommitCmd = "Creates a new conventional commit. If '-m|--message' and '-t|--type' are not specified, a prompt will start.";

    public const string CommitClosesOpt = "Allows a comma separated list of integers to be provided that will have 'closes #' prefixed to them in the commit message. If you want to resolve issues in another repository, use the '-r|--resolves' option.";

    public const string CommitDetailOpt = "Any additional extended information about the commit that clarifies the message. Is required if '-b|--breaking' is specified.";

    public static string[] CommitCmdExample1 = new[] { "commit" };

    public static string[] CommitCmdExample2 = new[] { "commit", "--type", "feat", "--message", "Add new cool feature." };

    public static string[] CommitCmdExample3 = new[] { "commit", "--type", "fix", "--message", "Fix the bad bug.", "--push" };

    public const string CommitIsBreakingOpt = "Specifies that the commits is a breaking change.";

    public const string CommitIsPreviewOpt = "Specifies that only a preview of the commit message should be returned.";

    public const string CommitMessageOpt = "The core commit message. Limited to 80 characters.";

    public const string CommitPushOpt = "Whether the commit should be pushed to the current remote branch.";

    public const string CommitResolvesOpt = "Allows a comma separated list of issues in other repositories which will be appended to the commit message with the prefix 'resolves '. You can specify this lists as so 'myorg/myrepo#100, myorg/myrepo#102'. If you just want to close issues in the current repository, use the '-c|--closes' option.";

    public const string CommitScopeOpt = "Optional scope for the conventional commit.";

    public const string CommitStageAllOpt = "Option to stage all files currently unstaged.";

    public const string CommitTypeOpt = "The type of the conventional commit. You may currently specify one of the following: feat, fix, perf, test, docs, refactor, style, revert, build, ci, chore";

    public const string InitCmd = "Creates a new tonberry configuration file in the current directory.";

    public static string[] InitCmdExample1 = new[] { "init" };

    public static string[] InitCmdExample2 = new[] { "init", "https://github.com/myOrg/myProject" };

    public static string[] InitCmdExample3 = new[] { "init", "https://github.com/myOrg/myProject", "My Awesome Project" };

    public const string InitNameOpt = "Name of your project/repository. If this is not specified, the name of the current folder will be used.";

    public const string InitOpenOpt = "If specified the command will try to open the config file in your default application for yaml.";

    public const string InitRepoOpt = "Main url for the repository.";

    public const string InitVersionOpt = "The semantic version of your project.";

    public const string MigrateCmd = "Creates a new CHANGELOG from the current repository's tags, if they are semantically versioned.";

    public static string[] MigrateCmdExample1 = new[] { "migrate" };

    public static string[] MigrateCmdExample2 = new[] { "migrate", "--preview" };

    public const string NewCmd = "Creates a new CHANGELOG whether you are just getting started, or you have been managing your commits, CHANGELOG and tags without any tools.";

    public static string[] NewCmdExample1 = new[] { "new" };

    public static string[] NewCmdExample2 = new[] { "new", "--preview" };

    public static string[] NewCmdExample3 = new[] { "new", "--prerelease-label", "beta", "--build", "nightly123" };

    public const string ReleaseCmd = "Creates a new release for all specified projects in the current repository, adds the changes to the CHANGELOG and creates a new commit/tag for the new version.";

    public static string[] ReleaseCmdExample1 = new[] { "release" };

    public static string[] ReleaseCmdExample2 = new[] { "release", "--version-only" };

    public static string[] ReleaseCmdExample3 = new[] { "release", "--release-candidate" };

    public static string[] ReleaseCmdExample4 = new[] { "release", "--bump-major" };

    public static string[] ReleaseCmdExample5 = new[] { "release", "--bump-minor" };

    public const string ReleaseBuildOpt = "Add a build label to the generated semantic version.";

    public const string ReleaseIsPreviewOpt = "Provides a preview only of the CHANGELOG which will open in the default program for markdown files on your system.";

    public const string ReleaseIsRCOpt = "Specifies that the prerelease label for the generated semantic version will be 'rc.x', which is automatically incremented if the previous version was a release candidate too.";

    public const string ReleaseMajorOpt = "Forces the next generated semantic version to bump the major version of all projects i.e. 1.2.0 to 2.0.0.";

    public const string ReleaseMinorOpt = "Forces the next generated semantic version to bump the minor version of all projects i.e. 0.4.0 to 0.5.0.";

    public const string ReleasePreReleaseOpt = "Specifies a custom prerelease label for the generated semantic version will be 'label.x', which is automatically incremented if the previous version had the same label.";

    public const string ReleaseTomdOpt = "Prints all the project names and their new versions as a markdown table. If you do not want a release to be created, you must specify '-p|--preview' too.";

    public const string ReleaseVersionOnlyOpt = "Provides the generated semantic version for a potential release only.";

    #region ValidationMessages

    public const string InvalidBumpConfig = "'--bump-major' and '--bump-minor' cannot be specified together.";

    public const string InvalidConsoleOutputOptions = "'{0}' and '{1}' cannot be used together as they both print console output.";

    public const string InvalidPreReleaseLabel = "'-rc|--release-candidate' and '-p|--prerelease-label' cannot be specified together.";

    public const string InvalidVersionString = "The value passed could not be parsed to a semantic version, please check the value and try again.";

    public const string NoDetailForBreakingChange = "You must provide a message with the '-d|--detail' option for a breaking change.";

    #endregion ValidationMessages

    #region Misc

    public const string VersionMarkdownHeader = "| Project Name | Version |\n|---|---|";

    #endregion Misc
}