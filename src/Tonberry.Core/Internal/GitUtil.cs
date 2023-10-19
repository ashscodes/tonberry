using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LibGit2Sharp;

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

    public static void RemoveLocalTag(string remoteName = "origin")
    {
        try
        {
            InvokeGitCliCommand(Directory.GetCurrentDirectory(),
                                out string stdError,
                                out string stdOutput,
                                "fetch",
                                "--prune",
                                remoteName,
                                "+refs/tags/*:refs/tags/*");
        }
        catch
        {
            throw;
        }
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
        catch
        {
            throw;
        }
        finally
        {
            gitProcess.Dispose();
        }
    }

    private static void InvokeGitCliCommand(string directoryPath,
                                            out string stdError,
                                            out string stdOutput,
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
            stdError = gitProcess.StandardError.ReadToEnd();
            stdOutput = gitProcess.StandardOutput.ReadToEnd();
        }
        catch
        {
            throw;
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