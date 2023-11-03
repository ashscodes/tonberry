using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tonberry.Core.Model;

public class TonberryRelease : TonberryReleaseBase, IComparable, IComparable<TonberryRelease>
{
    public IEnumerable<TonberryCommit> Breaking { get; internal set; }

    public Dictionary<string, IEnumerable<TonberryCommit>> Commits { get; internal set; }

    public int Count => Commits.Values.Sum(commits => commits.Count());

    public TonberryTag Previous { get; internal set; }

    public DateTimeOffset? Time { get; internal set; }

    internal bool IsBreaking => Breaking is not null && Breaking.Any();

    internal bool IsConventional => Commits.Values.Any(commits => commits.Any(commit => commit.IsConventional));

    internal bool IsGeneralRelease => Previous.Version.IsGeneralRelease;

    internal TonberryCommit LatestCommit { get; set; }

    public TonberryRelease(string projectName) : base(projectName) { }

    public int CompareTo(object obj)
    {
        if (Previous is null)
        {
            return 1;
        }

        if (obj is TonberryRelease release)
        {
            return CompareTo(release);
        }

        throw new ArgumentException(string.Format(Resources.UnexpectedType,
                                                  typeof(TonberryRelease).FullName,
                                                  obj.GetType().FullName));
    }

    public int CompareTo(TonberryRelease other) => Previous is null ? 1 : Previous.CompareTo(other?.Previous);
}

public class TonberryReleaseCollection : TonberryReleaseBase, ICollection<TonberryRelease>
{
    private readonly List<TonberryRelease> _releases = [];

    public int Count => _releases.Count;

    public bool IsReadOnly => false;

    internal TonberryRelease First => _releases.FirstOrDefault();

    public TonberryReleaseCollection(string projectName) : base(projectName) { }

    public void Add(TonberryRelease release)
    {
        if (release is null)
        {
            throw new ArgumentNullException(nameof(release));
        }

        _releases.Add(release);
    }

    public void Clear() => _releases.Clear();

    public bool Contains(TonberryRelease release) => release is null ? false : _releases.Contains(release);

    public void CopyTo(TonberryRelease[] array, int offset) => _releases.CopyTo(0, array, offset, Count);

    public IEnumerator<TonberryRelease> GetEnumerator() => _releases.GetEnumerator();

    public bool Remove(TonberryRelease release) => Contains(release) && _releases.Remove(release);

    public void Reverse() => _releases.Reverse();

    public void Sort() => _releases.Sort();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class TonberryReleaseBase
{
    public TonberryTag Current { get; set; }

    public string ProjectName { get; }

    internal TonberryReleaseBase(string projectName) : base() => ProjectName = projectName;
}