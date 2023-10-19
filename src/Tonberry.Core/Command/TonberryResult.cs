using System.Collections.Generic;

namespace Tonberry.Core.Command;

public class TonberryResult
{
    private Dictionary<string, object> _data;

    public string Message { get; set; }

    public bool Success { get; internal set; } = true;

    internal TonberryResult() { }

    internal TonberryResult(string message) => Message = message;

    internal TonberryResult(string message, bool success) : this(message) => Success = success;

    internal void Add(string key, object value)
    {
        _data ??= [];
        _data.Add(key, value);
    }

    internal void Copy(Dictionary<string, object> data)
    {
        if (_data is null)
        {
            _data = data;
        }
        else
        {
            foreach (var item in data)
            {
                _data.Add(item.Key, item.Value);
            }
        }
    }
}