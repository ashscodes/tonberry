using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tonberry.Core.Model;

public class TonberryCommit : IComparable, IComparable<TonberryCommit>
{
    internal IEnumerable<int> Closes;

    internal IEnumerable<string> Resolves;

    public TonberryAuthor Author { get; set; }

    public string Breaking { get; set; }

    public string Description { get; set; }

    public string Id { get; set; }

    public string Scope { get; set; }

    public string ShortId => Id is not null ? Id[0..8] : string.Empty;

    public string Synopsis { get; set; }

    public string Type { get; set; }

    internal bool IsBreaking => !string.IsNullOrEmpty(Breaking);

    internal bool IsConventional => !string.IsNullOrEmpty(Type) && !Type.Equals("none");

    internal string RawMessage { get; set; }

    internal TonberryCommit() { }

    public int CompareTo(object obj)
    {
        if (Author?.Time is null)
        {
            return 1;
        }

        if (obj is TonberryCommitCollection commits)
        {
            return CompareTo(commits);
        }

        throw new ArgumentException(string.Format(Resources.UnexpectedType,
                                                  typeof(TonberryCommit).FullName,
                                                  obj.GetType().FullName));
    }

    public int CompareTo(TonberryCommit other)
    {
        if (Author?.Time is null)
        {
            return 1;
        }

        if (other?.Author?.Time is null)
        {
            return -1;
        }

        return Author.Time.CompareTo(other.Author.Time);
    }

    public override string ToString()
    {
        var commit = new StringBuilder();
        commit.Append(Type);
        if (!string.IsNullOrEmpty(Scope))
        {
            commit.Append('(')
                  .Append(Scope)
                  .Append(')');
        }

        commit.Append(": ");
        commit.Append(Synopsis);
        if (!string.IsNullOrEmpty(Description))
        {
            commit.AppendLine().AppendLine();
            if (IsBreaking)
            {
                commit.Append(Resources.Breaking);
            }

            commit.Append(Description);
        }

        if (Closes.Any())
        {
            var closes = Closes.Select(i => string.Format(Resources.CommitCloses, i));
            commit.AppendLine()
                  .AppendLine()
                  .Append(string.Join(", ", closes));
        }

        if (Resolves.Any())
        {
            var resolves = Resolves.Select(i => string.Format(Resources.CommitResolves, i));
            commit.AppendLine()
                  .AppendLine()
                  .Append(string.Join(", ", resolves));
        }

        return commit.ToString();
    }
}

public class TonberryCommitCollection : ICollection<TonberryCommit>, IComparable, IComparable<TonberryCommitCollection>
{
    private readonly List<TonberryCommit> _commits = [];

    public int Count => _commits.Count;

    public bool IsReadOnly => false;

    public TonberryTag Previous { get; set; }

    public TonberryTag Tag { get; set; }

    public TonberryCommit this[int index] => _commits[index];

    internal TonberryCommit First => _commits.FirstOrDefault();

    internal TonberryCommit Last => _commits.LastOrDefault();

    internal TonberryCommitCollection() { }

    internal TonberryCommitCollection(IEnumerable<TonberryCommit> commits,
                                      TonberryTag tag,
                                      TonberryTag previous = null)
    {
        Previous = previous;
        Tag = tag;
        Add(commits);
        if (Count > 0)
        {
            Sort();
            Reverse();
        }
    }

    public void Add(TonberryCommit commit)
    {
        if (commit is null)
        {
            throw new ArgumentNullException(nameof(commit));
        }

        _commits.Add(commit);
    }

    public void Add(IEnumerable<TonberryCommit> commits)
    {
        foreach (TonberryCommit commit in commits)
        {
            Add(commit);
        }
    }

    public void Clear() => _commits.Clear();

    public int CompareTo(object obj)
    {
        if (Tag?.Version is null)
        {
            return 1;
        }

        if (obj is TonberryCommitCollection commits)
        {
            return CompareTo(commits);
        }

        throw new ArgumentException(string.Format(Resources.UnexpectedType,
                                                  typeof(TonberryCommitCollection).FullName,
                                                  obj.GetType().FullName));
    }

    public int CompareTo(TonberryCommitCollection other)
    {
        if (Tag is null)
        {
            return 1;
        }

        return Tag.CompareTo(other?.Tag);
    }

    public bool Contains(TonberryCommit commit)
    {
        if (commit is null)
        {
            throw new ArgumentNullException(nameof(commit));
        }

        return _commits.Contains(commit);
    }

    public void CopyTo(TonberryCommit[] array, int offset) => _commits.CopyTo(0, array, offset, Count);

    public IEnumerator<TonberryCommit> GetEnumerator() => _commits.GetEnumerator();

    public bool Remove(TonberryCommit commit) => Contains(commit) && _commits.Remove(commit);

    public void Reverse() => _commits.Reverse();

    public void Sort() => _commits.Sort();

    public override string ToString() => Tag.Version.ToString();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}