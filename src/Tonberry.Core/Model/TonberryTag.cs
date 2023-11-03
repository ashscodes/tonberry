using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tonberry.Core.Model;

public class TonberryTag : IComparable, IComparable<TonberryTag>
{
    public bool IsMonoRepoTag { get; set; }

    public string ProjectName { get; set; }

    public string Sha { get; set; }

    public TonberryVersion Version { get; set; }

    internal bool IsNext { get; set; } = false;

    public TonberryTag() { }

    internal TonberryTag(string sha) => Sha = sha;

    public int CompareTo(object obj)
    {
        if (Version is null)
        {
            return 1;
        }

        if (obj is TonberryTag tag)
        {
            return CompareTo(tag);
        }

        throw new ArgumentException(string.Format(Resources.UnexpectedType,
                                                  typeof(TonberryTag).FullName,
                                                  obj.GetType().FullName));
    }

    public int CompareTo(TonberryTag other)
    {
        if (Version is null)
        {
            return 1;
        }

        return Version.CompareTo(other?.Version);
    }

    public override string ToString()
    {
        if (IsMonoRepoTag && !string.IsNullOrEmpty(ProjectName))
        {
            return string.Format(Resources.DefaultMonoRepoProjectTag, Version, ProjectName);
        }

        return string.Format(Resources.DefaultTag, Version);
    }
}

public class TonberryTagCollection : ICollection<TonberryTag>
{
    private readonly List<TonberryTag> _tags = [];

    public int Count => _tags.Count;

    public bool IsReadOnly => false;

    internal bool IsMonoRepo => _tags.DistinctBy(t => t.ProjectName).Count() > 1;

    public TonberryTag this[int index] => _tags[index];

    internal TonberryTagCollection() { }

    public void Add(TonberryTag tag)
    {
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (tag.Version is not null)
        {
            _tags.Add(tag);
        }
    }

    public void Add(IEnumerable<TonberryTag> tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        foreach (TonberryTag tag in tags)
        {
            Add(tag);
        }
    }

    public void Clear() => _tags.Clear();

    public bool Contains(TonberryTag tag)
    {
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        return _tags.Contains(tag);
    }

    public void CopyTo(TonberryTag[] array, int offset) => _tags.CopyTo(0, array, offset, Count);

    public IEnumerator<TonberryTag> GetEnumerator() => _tags.GetEnumerator();

    public TonberryTag GetLatestTag()
    {
        Sort();
        Reverse();
        return _tags.FirstOrDefault();
    }

    public TonberryTagCollection GetProjectTags(string projectName)
    {
        var tagCollection = new TonberryTagCollection()
        {
            { _tags.Where(t => t.ProjectName.Equals(projectName, Resources.StrCompare)) }
        };

        Sort();
        Reverse();
        return tagCollection;
    }

    public bool Remove(TonberryTag tag) => Contains(tag) && _tags.Remove(tag);

    public void Reverse() => _tags.Reverse();

    public void Sort() => _tags.Sort();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}