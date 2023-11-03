namespace Tonberry.Core;

public interface ITonberryResult<T>
{
    string Item { get; }

    T Output { get; }

    bool Success { get; }

    void Open();

    void Write();

    void WriteInConsole();
}
