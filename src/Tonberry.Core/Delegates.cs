namespace Tonberry.Core;

public delegate void TonberryPrompt<T>(T options) where T : ITonberryCommandOptions;