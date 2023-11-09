
using System;
using System.Linq;
using Spectre.Console;
using Tonberry.Core;
using Tonberry.Core.Model;

namespace Tonberry.Prompt;

public static class UserPrompts
{
    public static TonberryCommitOptions CommitPrompt(TonberryCommitOptions options,
                                                     TonberryApplicationException exception)
    {
        var newOptions = new TonberryCommitOptions(options);
        if (string.IsNullOrEmpty(newOptions.Type))
        {
            newOptions.Type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(GetEnumValues<CommitType>())
                    .MoreChoicesText(Resources.MoreChoices)
                    .PageSize(4)
                    .Title(Resources.CommitTypePrompt));

        }


        newOptions.Scope = AnsiConsole.Prompt(new TextPrompt<string>(Resources.CommitScopePrompt).AllowEmpty());
        if (string.IsNullOrEmpty(newOptions.Message))
        {
            newOptions.Message = AnsiConsole.Prompt(
                new TextPrompt<string>(Resources.CommitMessagePrompt).Validate(message =>
                    {
                        return message.Length switch
                        {
                            < 5 => ValidationResult.Error(Resources.CommitMessageTooShort),
                            > 80 => ValidationResult.Error(Resources.CommitMessageTooLong),
                            _ => ValidationResult.Success()
                        };
                    }));
        }

        newOptions.IsBreaking = AnsiConsole.Confirm(Resources.CommitIsBreakingPrompt, false);
        if (newOptions.IsBreaking)
        {
            newOptions.Detail = AnsiConsole.Prompt(
                new TextPrompt<string>(Resources.CommitBreakingPrompt).Validate(detail =>
                    {
                        return detail.Length switch
                        {
                            < 5 => ValidationResult.Error(Resources.CommitBreakingMessageTooShort),
                            _ => ValidationResult.Success()
                        };
                    }));
        }
        else
        {
            newOptions.Detail = AnsiConsole.Prompt(new TextPrompt<string>(Resources.CommitDetailPrompt).AllowEmpty());
        }

        newOptions.Closes = AnsiConsole.Prompt(new TextPrompt<int[]>(Resources.CommitClosesPrompt).AllowEmpty());
        newOptions.Resolves = AnsiConsole.Prompt(new TextPrompt<string[]>(Resources.CommitResolvesPrompt).AllowEmpty());
        return newOptions;
    }

    private static string[] GetEnumValues<TEnum>() where TEnum : struct, Enum
        => Enum.GetValues<TEnum>().Select(v => v.ToString()).ToArray();
}