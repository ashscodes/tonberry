using System;

namespace Tonberry.Core.Model;

public class TonberryAuthor : IComparable, IComparable<TonberryAuthor>, IEquatable<TonberryAuthor>
{
    public string Email { get; set; }

    public string Name { get; set; }

    public DateTimeOffset Time { get; set; }

    public TonberryAuthor() { }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not TonberryAuthor author)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return CompareTo(author);
    }

    public int CompareTo(TonberryAuthor other) => Time.CompareTo(other.Time);

    public bool Equals(TonberryAuthor other) => Time.Equals(other.Time);

    public override bool Equals(object obj) => Equals(obj as TonberryAuthor);

    public override int GetHashCode() => HashCode.Combine(Email, Name, Time);
}