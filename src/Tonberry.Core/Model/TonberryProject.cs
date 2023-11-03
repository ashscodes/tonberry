using System;
using System.Collections;
using System.Collections.Generic;

namespace Tonberry.Core.Model;

public class TonberryProject : ICollection<TonberryCommitCollection>
{
    private List<TonberryCommitCollection> _commitCollections = [];

    public int Count => _commitCollections.Count;

    public bool IsReadOnly => false;

    public string Name { get; }

    public TonberryProject(string name) => Name = name;

    public void Add(TonberryCommitCollection commitCollection)
    {
        if (commitCollection is null)
        {
            throw new ArgumentNullException(nameof(commitCollection));
        }

        _commitCollections.Add(commitCollection);
    }

    public void Clear() => _commitCollections.Clear();

    public bool Contains(TonberryCommitCollection commitCollection)
    {
        if (commitCollection is null)
        {
            throw new ArgumentNullException(nameof(commitCollection));
        }

        return _commitCollections.Contains(commitCollection);
    }

    public void CopyTo(TonberryCommitCollection[] array, int offset) => _commitCollections.CopyTo(0, array, offset, Count);

    public bool Remove(TonberryCommitCollection commitCollection)
        => Contains(commitCollection) && _commitCollections.Remove(commitCollection);

    public void Reverse() => _commitCollections.Reverse();

    public void Sort() => _commitCollections.Sort();

    public IEnumerator<TonberryCommitCollection> GetEnumerator() => _commitCollections.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class TonberryProjectCollection : ICollection<TonberryProject>
{
    private readonly List<TonberryProject> _projects = [];

    public int Count => _projects.Count;

    public bool IsReadOnly => false;

    public TonberryProject this[int index] => _projects[index];

    public void Add(TonberryProject project)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        if (project is not null)
        {
            _projects.Add(project);
        }
    }

    public void Clear() => _projects.Clear();

    public bool Contains(TonberryProject project)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        return _projects.Contains(project);
    }

    public void CopyTo(TonberryProject[] array, int offset) => _projects.CopyTo(0, array, offset, Count);

    public IEnumerator<TonberryProject> GetEnumerator() => _projects.GetEnumerator();

    public bool Remove(TonberryProject project) => Contains(project) && _projects.Remove(project);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}