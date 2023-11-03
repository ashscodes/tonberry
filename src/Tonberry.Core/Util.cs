using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibGit2Sharp;
using Tonberry.Core.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tonberry.Core;

internal static class Util
{
    public static GitExeResult InvokeGitCliCommand(string directoryPath, params string[] args)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "git",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            StandardErrorEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false,
            WorkingDirectory = directoryPath
        };

        for (int i = 0; i < args.Length; i++)
        {
            processInfo.ArgumentList.Add(args[i]);
        }

        var gitProcess = new Process
        {
            StartInfo = processInfo
        };

        try
        {
            gitProcess.Start();
            gitProcess.WaitForExit();
            return new GitExeResult(gitProcess);
        }
        finally
        {
            gitProcess.Dispose();
        }
    }

    public static PushOptions NewPushOptions()
    {
        var options = new PushOptions();
        options.CredentialsProvider = GetUserCredentials;
        return options;
    }

    internal static UsernamePasswordCredentials GetUserCredentials(string url,
                                                                   string userNameFromUrl,
                                                                   SupportedCredentialTypes credentialTypes)
    {
        var uri = new Uri(url);
        var processInfo = new ProcessStartInfo
        {
            Arguments = "credential fill",
            FileName = "git",
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var gitProcess = new Process
        {
            StartInfo = processInfo
        };

        gitProcess.Start();
        gitProcess.StandardInput.NewLine = "\n";
        gitProcess.StandardInput.WriteLine($"protocol={uri.Scheme}");
        gitProcess.StandardInput.WriteLine($"host={uri.Host}");
        gitProcess.StandardInput.WriteLine($"path={uri.AbsolutePath}");
        gitProcess.StandardInput.WriteLine();

        try
        {
            string username = string.Empty;
            string password = string.Empty;
            string currentLine;
            while ((currentLine = gitProcess.StandardOutput.ReadLine()) != null)
            {
                string[] credentials = currentLine.Split('=');
                if (credentials[0] == "password")
                {
                    password = credentials[1];
                }

                if (credentials[0] == "username")
                {
                    username = credentials[1];
                }
            }

            return new UsernamePasswordCredentials
            {
                Password = password,
                Username = username
            };
        }
        finally
        {
            gitProcess.Dispose();
        }
    }

    internal static IDeserializer GetYamlDeserializer() => new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithRequiredPropertyValidation()
        .WithTypeConverter(new TonberryVersionConverter())
        .IgnoreUnmatchedProperties()
        .Build();

    internal static ISerializer GetYamlSerializer() => new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new TonberryVersionConverter())
        .Build();

    internal static void OpenFile(FileInfo file, string app = null)
    {
        using (Process process = new Process())
        {
            if (string.IsNullOrEmpty(app))
            {
                process.StartInfo.FileName = file.FullName;
            }
            else
            {
                process.StartInfo.Arguments = file.FullName;
                process.StartInfo.FileName = app;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}

internal class GitExeResult
{
    public string StandardError { get; set; }

    public string StandardOutput { get; set; }

    public GitExeResult(Process process)
    {
        StandardError = process.StandardError.ReadToEnd();
        StandardOutput = process.StandardOutput.ReadToEnd();
    }

    public GitExeResult(StreamReader standardError, StreamReader standardOutput)
    {
        StandardError = standardError.ReadToEnd();
        StandardOutput = standardOutput.ReadToEnd();
    }
}

internal class TonberryRepository : IDisposable
{
    internal string BranchName => Repository?.Head?.FriendlyName;

    internal string RemoteName => Repository?.Head?.RemoteName;

    internal Commit Tip => Repository?.Head?.Tip;

    internal Repository Repository { get; set; }

    internal TonberryRepository(Repository repository) => Repository = repository;

    public void Dispose()
    {
        Repository.Dispose();
        Repository = null;
    }

    internal Tag ApplyTag(string tagName, Commit commit) => Repository?.ApplyTag(tagName, commit.Sha);

    internal Commit Commit(string message)
    {
        var signature = new Signature(Repository.Config.Get<string>("user.name").Value,
                                      Repository.Config.Get<string>("user.email").Value,
                                      DateTimeOffset.Now);

        return Repository?.Commit(message, signature, signature, new CommitOptions());
    }

    internal TreeChanges CompareTreeChanges(Tree oldTree, Tree newTree)
        => Repository?.Diff?.Compare<TreeChanges>(oldTree, newTree);

    internal IEnumerable<Tag> GetTags() => Repository?.Tags;

    internal GitObject Lookup(string value) => Repository?.Lookup(value);

    internal ICommitLog Query(CommitFilter filter) => Repository?.Commits?.QueryBy(filter);

    internal RepositoryStatus RetrieveStatus(StatusOptions options) => Repository?.RetrieveStatus(options);

    internal void Push() => Repository?.Network?.Push(Repository?.Head, Util.NewPushOptions());

    internal void Push(Tag tag) => Repository?.Network?.Push(Repository?.Network?.Remotes[RemoteName],
                                                             tag.CanonicalName,
                                                             tag.CanonicalName,
                                                             Util.NewPushOptions());

    internal void StageAll() => Commands.Stage(Repository, "*");

    internal void TrackRemoteBranch()
    {
        if (Repository is not null)
        {
            if (!Repository.Head.IsTracking && Repository.Head.TrackedBranch == null)
            {
                var remote = Repository.Network.Remotes[RemoteName];
                if (remote is not null)
                {
                    _ = Repository.Branches.Update(Repository.Head, branch =>
                    {
                        branch.Remote = remote.Name;
                        branch.UpstreamBranch = Repository.Head.CanonicalName;
                    });
                }
            }
        }
    }
}