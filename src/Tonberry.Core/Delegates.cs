using Tonberry.Core.Command.Options;

namespace Tonberry.Core;

public delegate void CommitParserDelegate(string commitMessage,
                                          out string breaking,
                                          out string description,
                                          out string scope,
                                          out string synopsis,
                                          out string type);

public delegate void TonberryPrompt<T>(T options) where T : ITonberryCommandOptions;