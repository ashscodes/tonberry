using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tonberry.Core.Model;

public class TonberryCommitResult : TonberryResult
{
    private string _output;

    public TonberryCommitResult() : base() { }

    public TonberryCommitResult(string item) : base(item) { }

    public TonberryCommitResult(string item, Exception exception) : base(item, exception) { }

    public TonberryCommitResult(string item, string output, bool success) : this(item)
    {
        _output = output;
        Success = success;
    }

    public override string GetResult() => _output;

    public override void Open() => WriteInConsole();

    public override void Write() => WriteInConsole();

    public override void WriteInConsole() => Console.WriteLine(_output);
}

public class TonberryFileResult : TonberryResult
{
    private FileInfo _currentLog;

    private FileInfo _output;

    public TonberryVersion Version { get; }

    public TonberryFileResult(string item, Exception exception) : base(item, exception) { }

    public TonberryFileResult(FileInfo currentLog, string item, FileInfo output, TonberryVersion version) : base(item)
    {
        _currentLog = currentLog;
        Item = item;
        _output = output;
        Version = version;
        if (_output is not null && _output.Exists)
        {
            Path.ChangeExtension(_output.FullName, ".md");
        }
    }

    public override FileInfo GetResult() => _output;

    public override void Open()
    {
        if (_output is not null && _output.Exists)
        {
            Util.OpenFile(_output);
        }
    }

    public override void Write()
    {
        if (_currentLog is not null && _output is not null && _output.Exists)
        {
            File.Move(_output.FullName, _currentLog.FullName, true);
        }
    }

    public override void WriteInConsole()
    {
        if (Version is not null)
        {
            Console.WriteLine(Version);
        }
    }

    public void WriteVersionAsMarkdown()
    {
        if (Version is not null)
        {
            Console.WriteLine(Resources.VersionMarkdownRow);
        }
    }
}

public class TonberryInitResult : TonberryResult
{
    private TonberryConfiguration _output;

    public TonberryInitResult() : base(Resources.TonberryRootConfig) { }

    public TonberryInitResult(Exception exception) : base(Resources.TonberryRootConfig, exception) { }

    public TonberryInitResult(TonberryConfiguration config, bool success) : this()
    {
        _output = config;
        Success = success;
    }

    public override TonberryConfiguration GetResult() => _output;

    public override void Open()
    {
        if (_output?.Configuration is not null)
        {
            Util.OpenFile(_output.Configuration);
        }
    }

    public override void Write() { }

    public override void WriteInConsole() { }
}

public class TonberryResultCollection : ICollection<TonberryResult>, ITonberryResult
{
    private readonly List<TonberryResult> _results = [];

    public int Count => _results.Count;

    public bool IsReadOnly => false;

    public bool Success => _results.All(result => result.Success);

    public void Add(TonberryResult result)
    {
        if (result is not null)
        {
            _results.Add(result);
        }
    }

    public void Clear() => _results.Clear();

    public bool Contains(TonberryResult result) => _results.Contains(result);

    public void CopyTo(TonberryResult[] array, int offset) => _results.CopyTo(0, array, offset, Count);

    public IEnumerator<TonberryResult> GetEnumerator() => _results.GetEnumerator();

    public object GetResult() => Success;

    public bool Remove(TonberryResult result) => _results.Remove(result);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class TonberryResult : ITonberryResult
{
    public Exception Error { get; }

    public string Item { get; init; }

    public bool Success { get; internal set; }

    internal TonberryResult() { }

    public TonberryResult(string item) => Item = item;

    internal TonberryResult(Exception exception)
    {
        Error = exception;
        Success = false;
    }

    internal TonberryResult(string item, Exception exception) : this(exception) => Item = item;

    public abstract object GetResult();

    public abstract void Open();

    public abstract void Write();

    public abstract void WriteInConsole();
}

public interface ITonberryResult
{
    bool Success { get; }

    object GetResult();
}