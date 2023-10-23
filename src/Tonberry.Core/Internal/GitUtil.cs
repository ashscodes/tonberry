using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Tonberry.Core.Internal;

namespace Tonberry.Core;

internal static partial class GitUtil
{
    public static Repository GetRepository(string path)
        => Repository.IsValid(path)
            ? new Repository(path)
            : throw new TonberryApplicationException(Resources.NoRepositoryFound);

    public static FetchOptions NewFetchOptions(bool authorise, bool prune)
    {
        var options = new FetchOptions
        {
            Prune = prune
        };

        if (authorise)
        {
            options.CredentialsProvider = GetUserCredentials;
        }

        return options;
    }

    public static PushOptions NewPushOptions(bool authorise)
    {
        var options = new PushOptions();
        if (authorise)
        {
            options.CredentialsProvider = GetUserCredentials;
        }

        return options;
    }

    public static void RemoveLocalTags(string remoteName = "origin")
    {
        /*
            Output possibly to be used at a later date.
            Trying to avoid too much overlap with the git cli
            as we only want to provide tagging, changelog,
            versioning and conventional commit support.
        */

        _ = InvokeGitCliCommand(Directory.GetCurrentDirectory(),
                                "fetch",
                                "--prune",
                                remoteName,
                                "+refs/tags/*:refs/tags/*");
    }

    private static UsernamePasswordCredentials GetUserCredentials(string url,
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

    private static GitExeResult InvokeGitCliCommand(string directoryPath,
                                                    params string[] args)
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

    [GeneratedRegex(@"(?:(?<type>[A-Za-z]+(?:(|\s+)))(?:\((?<scope>.+?(?=\)))\)|):\s(?<message>.*)|(?<plain>[a-zA-Z].*))")]
    internal static partial Regex CommitMessageRegex();

    [GeneratedRegex("^((?<preRelease>[0-9A-Za-z][0-9A-Za-z\\-\\.]*))?(\\+(?<build>[0-9A-Za-z][0-9A-Za-z\\-\\.]*))?$")]
    internal static partial Regex SemanticLabelRegex();

    [GeneratedRegex(@"^[0-9A-Za-z][0-9A-Za-z\-\.]*$")]
    internal static partial Regex SemanticUnitRegex();
}