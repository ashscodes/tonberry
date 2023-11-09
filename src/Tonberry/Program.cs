using Spectre.Console;
using Spectre.Console.Cli;

namespace Tonberry;

internal class Program
{
    internal static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.SetApplicationName("tonberry");

            config.AddCommand<CommitCommand>("commit")
                  .WithDescription(Resources.CommitCmd)
                  .WithExample(Resources.CommitCmdExample1)
                  .WithExample(Resources.CommitCmdExample2)
                  .WithExample(Resources.CommitCmdExample3);

            config.AddCommand<InitCommand>("init")
                  .WithDescription(Resources.InitCmd)
                  .WithExample(Resources.InitCmdExample1)
                  .WithExample(Resources.InitCmdExample2)
                  .WithExample(Resources.InitCmdExample3);

            config.AddCommand<MigrateCommand>("migrate")
                  .WithDescription(Resources.MigrateCmd)
                  .WithExample(Resources.MigrateCmdExample1)
                  .WithExample(Resources.MigrateCmdExample2);

            config.AddCommand<NewCommand>("new")
                  .WithDescription(Resources.NewCmd)
                  .WithExample(Resources.NewCmdExample1)
                  .WithExample(Resources.NewCmdExample2)
                  .WithExample(Resources.NewCmdExample3);

            config.AddCommand<ReleaseCommand>("release")
                  .WithDescription(Resources.ReleaseCmd)
                  .WithExample(Resources.ReleaseCmdExample1)
                  .WithExample(Resources.ReleaseCmdExample2)
                  .WithExample(Resources.ReleaseCmdExample3)
                  .WithExample(Resources.ReleaseCmdExample4)
                  .WithExample(Resources.ReleaseCmdExample5);

            config.AddExample(Resources.CmdExample1);
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetExceptionHandler(exception =>
            {
                AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
                return 1;
            });
        });

        return app.Run(args);
    }
}