namespace Tonberry.Core.Command;

public interface ITonberryResult<T>
{
    string Item { get; }

    T Output { get; }

    bool Success { get; }
}