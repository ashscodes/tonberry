namespace Tonberry.Core.Internal;

internal class TonberryCommitParser : ICommitParser
{
    public string Breaking { get; set; }

    public string Description { get; set; }

    public string Scope { get; set; }

    public string Synopsis { get; set; }

    public string Type { get; set; }

    public void Parse(string message, CommitParserDelegate parser)
    {
        parser ??= ParseTonberryCommit;
        parser(message,
               out string breaking,
               out string description,
               out string scope,
               out string synopsis,
               out string type);

        Breaking = breaking;
        Description = description;
        Scope = scope;
        Synopsis = synopsis;
        Type = type;
    }

    public static void ParseCoreMessage(string message, out string scope, out string synopsis, out string type)
    {
        scope = synopsis = string.Empty;
        type = "none";
        var tonberryCommitMatch = GitUtil.CommitMessageRegex().Match(message);
        if (!tonberryCommitMatch.Success)
        {
            synopsis = message;
        }
        else
        {
            if (tonberryCommitMatch.Groups["plain"].Success)
            {
                synopsis = tonberryCommitMatch.Groups["plain"].Value.Trim();
                return;
            }

            if (tonberryCommitMatch.Groups["scope"].Success)
            {
                scope = tonberryCommitMatch.Groups["scope"].Value.Trim();
            }

            if (tonberryCommitMatch.Groups["synopsis"].Success)
            {
                synopsis = tonberryCommitMatch.Groups["synopsis"].Value.Trim();
            }

            if (tonberryCommitMatch.Groups["type"].Success)
            {
                type = tonberryCommitMatch.Groups["type"].Value.Trim();
            }
        }
    }

    public static void ParseTonberryCommit(string message,
                                            out string breaking,
                                            out string description,
                                            out string scope,
                                            out string synopsis,
                                            out string type)
    {
        breaking = description = string.Empty;
        string[] commit = message.Split("\n\n");
        if (commit.Length > 1)
        {
            for (int i = 1; i < commit.Length; i++)
            {
                if (commit[i].Contains("BREAKING CHANGE: "))
                {
                    breaking = string.IsNullOrEmpty(breaking) ? commit[i] : breaking + "\n\n" + commit[i];
                }
                else
                {
                    description = string.IsNullOrEmpty(description) ? commit[i] : description + "\n\n" + commit[i];
                }
            }

            if (string.IsNullOrEmpty(breaking))
            {
                breaking = breaking.TrimEnd('\n').Replace('\n', ' ').Replace("  ", " ");
            }

            if (string.IsNullOrEmpty(description))
            {
                description = description.TrimEnd('\n').Replace('\n', ' ').Replace("  ", " ");
            }
        }

        ParseCoreMessage(commit[0], out scope, out synopsis, out type);
    }
}