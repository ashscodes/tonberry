using System;
using Tonberry.Core.Model;

namespace Tonberry.Core.Sample;

internal class Program
{
    internal static int Main(string[] args)
    {
        var initTask = new TonberryInitTask("{MyProject}", new Uri("https://github.com/{myOrg}/{myRepo}"));
        var config = ((TonberryInitResult)initTask.Invoke()).GetResult();
        var migrateTask = new TonberryMigrateTask(config);
        migrateTask.Options = new TonberryMigrateOptions() { IsPreview = true };
        foreach (var result in migrateTask.Invoke())
        {
            result.Open();
        }

        return 0;
    }
}