using System;
using System.IO;
using Tonberry.Core.Model;

namespace Tonberry.Core.Sample;

internal class Program
{
    internal static int Main(string[] args)
    {
        var command = new TonberryMigrateCommand();
        var initCommand = new TonberryInitCommand()
        {
            Options = new TonberryInitOptions()
            {
                Name = "{MyProject}",
                Repository = new Uri("https://github.com/{myOrg}/{myRepo}")
            }
        };

        initCommand.CreateConfig(new DirectoryInfo(Directory.GetCurrentDirectory()));
        command.Config = initCommand.Config;
        command.Options = new TonberryMigrateOptions() { IsPreview = true };
        foreach (var result in command.Invoke())
        {
            result.Open();
        }

        return 0;
    }
}