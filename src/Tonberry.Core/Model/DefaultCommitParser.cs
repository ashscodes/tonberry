using System.Text.RegularExpressions;

namespace Tonberry.Core.Model;

internal partial class DefaultCommitParser : ICommitParser
{
    public string Breaking { get; set; }

    public string Description { get; set; }

    public string Scope { get; set; }

    public string Synopsis { get; set; }

    public string Type { get; set; }

    public void Parse(string message)
    {
        string[] commit = message.Split("\n\n");
        if (commit.Length > 1)
        {
            for (int i = 1; i < commit.Length; i++)
            {
                if (commit[i].Contains("BREAKING CHANGE: "))
                {
                    Breaking = string.IsNullOrEmpty(Breaking) ? commit[i] : Breaking + "\n\n" + commit[i];
                }
                else
                {
                    Description = string.IsNullOrEmpty(Description) ? commit[i] : Description + "\n\n" + commit[i];
                }
            }

            if (!string.IsNullOrEmpty(Breaking))
            {
                Breaking = Breaking.TrimEnd('\n').Replace('\n', ' ').Replace("  ", " ");
            }

            if (!string.IsNullOrEmpty(Description))
            {
                Description = Description.TrimEnd('\n').Replace('\n', ' ').Replace("  ", " ");
            }
        }

        ParseSynopsis(commit[0]);
    }

    private void ParseSynopsis(string commitMessage)
    {
        Type = "none";
        var tonberryCommitMatch = CommitMessageRegex().Match(commitMessage);
        if (!tonberryCommitMatch.Success)
        {
            Synopsis = commitMessage;
        }
        else
        {
            if (tonberryCommitMatch.Groups["plain"].Success)
            {
                Synopsis = tonberryCommitMatch.Groups["plain"].Value.Trim();
                return;
            }

            if (tonberryCommitMatch.Groups["scope"].Success)
            {
                Scope = tonberryCommitMatch.Groups["scope"].Value.Trim();
            }

            if (tonberryCommitMatch.Groups["synopsis"].Success)
            {
                Synopsis = tonberryCommitMatch.Groups["synopsis"].Value.Trim();
            }

            if (tonberryCommitMatch.Groups["type"].Success)
            {
                Type = tonberryCommitMatch.Groups["type"].Value.Trim();
            }
        }
    }

    [GeneratedRegex(@"(?:(?<type>[A-Za-z]+(?:(|\s+)))(?:\((?<scope>.+?(?=\)))\)|):\s(?<synopsis>.*)|(?<plain>[a-zA-Z].*))")]
    internal static partial Regex CommitMessageRegex();
}

public interface ICommitParser
{
    string Breaking { get; set; }

    string Description { get; set; }

    string Scope { get; set; }

    string Synopsis { get; set; }

    string Type { get; set; }

    void Parse(string message);
}