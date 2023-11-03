using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tonberry.Core.Model;

namespace Tonberry.Core;

internal sealed class ChangelogWriter : StreamWriter
{
    private TonberryConfiguration _config;

    bool _isMonoRepoProject;

    string _projectName;

    private ChangelogReader _reader;

    private string _tempLog;

    public ChangelogWriter(TonberryConfiguration configuration, string projectName) : base(Path.GetTempFileName())
    {
        _config = configuration;
        _isMonoRepoProject = configuration.IsMonoRepoProject(projectName);
        _projectName = projectName;
        _tempLog = ((FileStream)BaseStream).Name;
        AutoFlush = true;
        Init(configuration.GetChangelog(projectName));
    }

    public void AddReleasesFromCurrentLog()
    {
        var buffer = new byte[4096];
        int length;
        while ((length = _reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            BaseStream.Write(buffer, 0, length);
        }
    }

    public void Dispose(out FileInfo tempLogPath)
    {
        _reader.Dispose();
        Dispose();
        tempLogPath = new FileInfo(_tempLog);
    }

    public void WriteBreakingChanges(IEnumerable<TonberryCommit> breaking)
    {
        foreach (var commit in breaking.OrderBy(bc => string.IsNullOrEmpty(bc.Scope))
                                       .ThenBy(bc => bc.Scope)
                                       .ThenBy(bc => bc.Breaking))
        {
            var link = GetCommitLink(commit.ShortId, _config.CommitUrlFormat, commit.Id);
            if (string.IsNullOrEmpty(commit.Scope))
            {
                WriteLine(Resources.MarkdownBreakingNoScope, commit.Breaking, link);
            }
            else
            {
                WriteLine(Resources.MarkdownBreaking, commit.Scope, commit.Breaking, link);
            }
        }

        WriteLine();
    }

    public void WriteChangelogHeader()
    {
        WriteLine(Resources.ChangelogHeader);
        WriteLine();
    }

    public void WriteCommit(TonberryCommit commit)
    {
        var link = GetCommitLink(commit.ShortId, _config.CommitUrlFormat, commit.Id);
        if (string.IsNullOrEmpty(commit.Scope))
        {
            WriteLine(Resources.MarkdownCommitNoScope, commit.Synopsis, link);
        }
        else
        {
            WriteLine(Resources.MarkdownCommit, commit.Scope, commit.Synopsis, link);
        }
    }

    public void WriteCommitsByType(TonberryCommitType configItem, IEnumerable<TonberryCommit> commits)
    {
        WriteHeader(3, configItem.LogDisplayName, _config.GetEmoji(configItem.Name));
        foreach (var commit in commits.OrderBy(c => string.IsNullOrEmpty(c.Scope))
                                      .ThenBy(c => c.Scope)
                                      .ThenBy(c => c.Synopsis))
        {
            WriteCommit(commit);
        }

        WriteLine();
    }

    public void WriteContributors(IOrderedEnumerable<IGrouping<string, TonberryCommit>> contributors)
    {
        string emoji = _config.IncludeEmojis ? Resources.MarkdownContributorEmoji : string.Empty;
        WriteHeader(3, Resources.MarkdownContributorTitle, emoji);
        WriteLine(Ensure.StringHasValue(_config.ThankContributorText, Resources.MarkdownContributorThanks));
        WriteLine();

        foreach (var contributor in contributors)
        {
            string contributorName = contributor.First().Author.Name;
            string link = GetMarkdownLink(contributorName, _config.UserUrlFormat, contributorName);
            IEnumerable<string> commitLinks = GetContributorLinks(contributor, _config.CommitUrlFormat);
            WriteLine(Resources.MarkdownContributorCommit, contributorName, string.Join(", ", commitLinks));
        }

        WriteLine();
    }

    public void WriteHeader(int level, string text, string emoji = null)
    {
        emoji = string.IsNullOrEmpty(emoji) ? string.Empty : emoji + " ";
        WriteLine(Resources.MarkdownHeader, new string('#', level), emoji, text);
        WriteLine();
    }

    public void WriteReleaseHeader(TonberryTag current, DateTimeOffset? time, TonberryTag last = null)
    {
        string emoji = _config.GetEmoji();
        if (!string.IsNullOrEmpty(emoji))
        {
            emoji += " ";
        }

        string tagTemplate = _config.GetTagTemplate(_projectName);
        string currentTag;
        string previousTag;
        if (_isMonoRepoProject)
        {
            currentTag = string.Format(tagTemplate, current.Version, _projectName);
            previousTag = string.Format(tagTemplate, current.Version, _projectName);
        }
        else
        {
            currentTag = string.Format(tagTemplate, current.Version);
            previousTag = string.Format(tagTemplate, current.Version);
        }

        string link = last?.Version is null ? currentTag : GetMarkdownLink(current.Version.ToString(),
                                                                           _config.CompareUrlFormat,
                                                                           currentTag,
                                                                           previousTag);

        WriteLine(Resources.MarkdownReleaseHeader,
                  new string('#', 2),
                  emoji,
                  link,
                  time.HasValue ? time.Value.ToString("yyyy-MM-dd") : null);

        WriteLine();
    }


    private void Init(FileInfo changeLog)
    {
        string line;
        _reader = new ChangelogReader(changeLog);
        if (!_reader.IsTonberryChangelog)
        {
            WriteHeader(1, _config.Name);
            WriteChangelogHeader();
        }
        else
        {
            bool emptyLine;
            bool tonberryHeader;
            do
            {
                line = _reader.ReadLine();
                emptyLine = line == string.Empty;
                tonberryHeader = line == Resources.ChangelogHeader;
            } while (!emptyLine && !tonberryHeader);

            WriteLine();
        }
    }

    private static IEnumerable<string> GetContributorLinks(IEnumerable<TonberryCommit> commits, string url)
    {
        foreach (var commit in commits)
        {
            yield return GetCommitLink(commit.ShortId, url, commit.Id);
        }
    }

    private static string GetCommitLink(string text,
                                        string url,
                                        params object[] args) => GetMarkdownLink($"#{text}", url, args);

    private static string GetMarkdownLink(string text, string url, params object[] args)
        => string.Format(Resources.MarkdownLink, text, string.Format(url, args));
}