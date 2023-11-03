using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tonberry.Core.Model;

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

    public virtual void Open() => WriteInConsole();

    public virtual void Write() => WriteInConsole();

    public virtual void WriteInConsole() => Console.WriteLine(Output);
}

public class TonberryFileResult : ITonberryResult<FileInfo>
{
    public FileInfo CurrentLog { get; }

    public Exception Error { get; }

    public string Item { get; }

    public FileInfo Output { get; }

    public bool Success { get; internal set; }

    public TonberryVersion Version { get; }

    public TonberryFileResult(Exception exception, string item)
    {
        Error = exception;
        Item = item;
    }

    public TonberryFileResult(FileInfo currentLog, string item, FileInfo output, TonberryVersion version)
    {
        CurrentLog = currentLog;
        Item = item;
        Output = output;
        Version = version;
        if (Output is not null && Output.Exists)
        {
            Path.ChangeExtension(output.FullName, ".md");
        }
    }

    public virtual void Open()
    {
        if (Output is not null && Output.Exists)
        {
            Util.OpenFile(Output);
        }
    }

    public virtual void Write()
    {
        if (CurrentLog is not null && Output is not null && Output.Exists)
        {
            File.Move(Output.FullName, CurrentLog.FullName, true);
        }
    }

    public virtual void WriteInConsole()
    {
        if (Version is not null)
        {
            Console.WriteLine(Version);
        }
    }
}

public class TonberryResultCollection<T> : ICollection<ITonberryResult<T>>
{
    private readonly List<ITonberryResult<T>> _results = [];

    public int Count => _results.Count;

    public bool IsReadOnly => false;

    public void Add(ITonberryResult<T> result)
    {
        if (result is not null)
        {
            _results.Add(result);
        }
    }

    public void Clear() => _results.Clear();

    public bool Contains(ITonberryResult<T> result) => _results.Contains(result);

    public void CopyTo(ITonberryResult<T>[] array, int offset) => _results.CopyTo(0, array, offset, Count);

    public IEnumerator<ITonberryResult<T>> GetEnumerator() => _results.GetEnumerator();

    public bool Remove(ITonberryResult<T> result) => _results.Remove(result);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}