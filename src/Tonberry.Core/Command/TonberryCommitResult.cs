namespace Tonberry.Core.Command;

public class TonberryCommitResult : ITonberryResult<string>
{
    public string Item { get; }

    public string Output { get; }

    public bool Success { get; }

    public TonberryCommitResult(string item, string output, bool success)
    {
        Item = item;
        Output = output;
        Success = success;
    }
}