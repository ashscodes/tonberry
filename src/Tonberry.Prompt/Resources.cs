namespace Tonberry.Prompt;

internal static class Resources
{
    public const string CommitBreakingPrompt = "[blue][[Required]][/] Please provide detail about the breaking change:";

    public const string CommitBreakingMessageTooShort = "[red]![/] The extended message should be at least five characters and explain the nature of the breaking change.";

    public const string CommitClosesPrompt = "[grey][[Optional]][/] Are any issues closed by this commit? (Requires comma separated list of integers):";

    public const string CommitDetailPrompt = "[grey][[Optional]][/] Please provide any additional detail about the commit:";

    public const string CommitIsBreakingPrompt = "[blue][[Required]][/] Is your commit a breaking change?";

    public const string CommitMessagePrompt = "[blue][[Required]][/] What is your commit message?";

    public const string CommitMessageTooLong = "[red]![/] The commit message should be no more than 80 characters.";

    public const string CommitMessageTooShort = "[red]![/] The commit message should be at least 5 characters.";

    public const string CommitResolvesPrompt = "[grey][[Optional]][/] Are any issues in another repository resolved by this commit? (Requires comma separated list of 'org/repo#123'):";

    public const string CommitScopePrompt = "[grey][[Optional]][/] What is the scope of your commit?";

    public const string CommitTypePrompt = "[blue][[Required]][/] What is the type of your commit?";

    public const string MoreChoices = "[grey](Move up and down to reveal more options)[/]";
}