using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Tonberry.Core.Model;

namespace Tonberry.Core;

public class TonberryConfigurationConverter : IDictionaryConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is IDictionary dict)
        {
            var config = new TonberryConfiguration();

            SetConfigValue<List<TonberryCommitType>>(dict, nameof(config.CommitTypes), value =>
            {
                config.CommitTypes = ConvertItemsToList<TonberryCommitType>(dict, nameof(config.CommitTypes));
            });

            SetConfigValue<string>(dict, nameof(config.CommitUrlFormat), value => config.CommitUrlFormat = (string)value);
            SetConfigValue<string>(dict, nameof(config.CompareUrlFormat), value => config.CompareUrlFormat = (string)value);
            SetConfigValue<List<string>>(dict, nameof(config.Exclusions), value =>
            {
                config.Exclusions = ConvertItemsToList<string>(dict, nameof(config.Exclusions));
            });

            SetConfigValue<bool>(dict, nameof(config.IncludeEmojis), value => config.IncludeEmojis = (bool)value);
            SetConfigValue<string>(dict, nameof(config.IssueUrlFormat), value => config.IssueUrlFormat = (string)value);
            SetConfigValue<string>(dict, nameof(config.Language), value => config.Language = (string)value);
            SetConfigValue<bool>(dict, nameof(config.ListContributors), value => config.ListContributors = (bool)value);
            SetConfigValue<List<string>>(dict, nameof(config.Maintainers), value =>
            {
                config.Maintainers = ConvertItemsToList<string>(dict, nameof(config.Maintainers));
            });

            SetConfigValue<string>(dict, nameof(config.Name), value => config.Name = (string)value);
            SetConfigValue<List<TonberryProjectConfiguration>>(dict, nameof(config.CommitTypes), value =>
            {
                config.Projects = ConvertItemsToList<TonberryProjectConfiguration>(dict, nameof(config.CommitTypes));
            });

            SetConfigValue<string>(dict, nameof(config.ProjectFile), value => config.ProjectFile = (string)value);
            SetConfigValue<string>(dict, nameof(config.ProjectUrlFormat), value => config.ProjectUrlFormat = (string)value);
            SetConfigValue<string>(dict, nameof(config.ReleaseEmoji), value => config.ReleaseEmoji = (string)value);
            SetConfigValue<string>(dict, nameof(config.ReleaseSha), value => config.ReleaseSha = (string)value);
            SetConfigValue<string>(dict, nameof(config.TagTemplate), value => config.TagTemplate = (string)value);
            SetConfigValue<string>(dict, nameof(config.ThankContributorText), value => config.ThankContributorText = (string)value);
            SetConfigValue<bool>(dict, nameof(config.TrackNonMonoRepoProjectCommits), value => config.TrackNonMonoRepoProjectCommits = (bool)value);
            SetConfigValue<string>(dict, nameof(config.UserUrlFormat), value => config.UserUrlFormat = (string)value);
            SetConfigValue<TonberryVersion>(dict, nameof(config.Version), value =>
            {
                config.Version = ConvertItemToObject<TonberryVersion>(dict, nameof(TonberryVersion));
            });

            return config;
        }

        throw new NotSupportedException(
            string.Format(Resources.InvalidTypeCast, value.GetType(), typeof(TonberryProjectConfiguration)));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is TonberryConfiguration config)
        {
            if (destinationType == typeof(Hashtable))
            {
                return ConvertToIDictionary<Hashtable>(config);
            }
            else if (destinationType == typeof(Dictionary<string, object>))
            {
                return ConvertToIDictionary<Dictionary<string, object>>(config);
            }
        }

        throw new NotSupportedException(string.Format(Resources.InvalidTypeCast, value.GetType(), destinationType));
    }

    private static T ConvertToIDictionary<T>(TonberryConfiguration config) where T : IDictionary, new()
    {
        if (config == null)
        {
            return (T)default;
        }

        var dict = new T();

        SetDictionaryValue(dict, nameof(config.CommitUrlFormat), config.CommitUrlFormat);
        SetDictionaryValue(dict, nameof(config.CompareUrlFormat), config.CompareUrlFormat);
        SetDictionaryValue(dict, nameof(config.Exclusions), config.Exclusions.ToArray());
        SetDictionaryValue(dict, nameof(config.IncludeEmojis), config.IncludeEmojis);
        SetDictionaryValue(dict, nameof(config.IssueUrlFormat), config.IssueUrlFormat);
        SetDictionaryValue(dict, nameof(config.Language), config.Language);
        SetDictionaryValue(dict, nameof(config.ListContributors), config.ListContributors);
        SetDictionaryValue(dict, nameof(config.Maintainers), config.Maintainers.ToArray());
        SetDictionaryValue(dict, nameof(config.Name), config.Name);
        SetDictionaryValue(dict, nameof(config.ProjectFile), config.ProjectFile);
        SetDictionaryValue(dict, nameof(config.ProjectUrlFormat), config.ProjectUrlFormat);
        SetDictionaryValue(dict, nameof(config.ReleaseEmoji), config.ReleaseEmoji);
        SetDictionaryValue(dict, nameof(config.ReleaseSha), config.ReleaseSha);
        SetDictionaryValue(dict, nameof(config.TagTemplate), config.TagTemplate);
        SetDictionaryValue(dict, nameof(config.ThankContributorText), config.ThankContributorText);
        SetDictionaryValue(dict, nameof(config.TrackNonMonoRepoProjectCommits), config.TrackNonMonoRepoProjectCommits);
        SetDictionaryValue(dict, nameof(config.UserUrlFormat), config.UserUrlFormat);
        SetDictionaryValue(dict, nameof(config.Version), config.Version?.ToString());

        SetDictionaryValue(dict,
                           nameof(config.CommitTypes),
                           ConvertItemsToIDictionary<T, TonberryCommitType>(config.CommitTypes));

        SetDictionaryValue(dict,
                           nameof(config.Projects),
                           ConvertItemsToIDictionary<T, TonberryProjectConfiguration>(config.Projects));

        return dict;
    }
}

public class TonberryProjectConfigurationConverter : IDictionaryConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is IDictionary dict)
        {
            var config = new TonberryProjectConfiguration();

            SetConfigValue<List<string>>(dict, nameof(config.Exclusions), value =>
            {
                config.Exclusions = ConvertItemsToList<string>(dict, nameof(config.Exclusions));
            });

            SetConfigValue<string>(dict, nameof(config.Language), value => config.Language = (string)value);
            SetConfigValue<string>(dict, nameof(config.Name), value => config.Name = (string)value);
            SetConfigValue<string>(dict, nameof(config.ProjectFile), value => config.ProjectFile = (string)value);
            SetConfigValue<string>(dict, nameof(config.RelativePath), value => config.RelativePath = (string)value);
            SetConfigValue<string>(dict, nameof(config.ReleaseSha), value => config.ReleaseSha = (string)value);
            SetConfigValue<string>(dict, nameof(config.TagTemplate), value => config.TagTemplate = (string)value);
            SetConfigValue<TonberryVersion>(dict, nameof(config.Version), value =>
            {
                config.Version = ConvertItemToObject<TonberryVersion>(dict, nameof(TonberryVersion));
            });

            return config;
        }

        throw new NotSupportedException(
            string.Format(Resources.InvalidTypeCast, value.GetType(), typeof(TonberryProjectConfiguration)));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is TonberryProjectConfiguration config)
        {
            if (destinationType == typeof(Hashtable))
            {
                return ConvertToIDictionary<Hashtable>(config);
            }
            else if (destinationType == typeof(Dictionary<string, object>))
            {
                return ConvertToIDictionary<Dictionary<string, object>>(config);
            }
        }

        throw new NotSupportedException(string.Format(Resources.InvalidTypeCast, value.GetType(), destinationType));
    }

    private static T ConvertToIDictionary<T>(TonberryProjectConfiguration config) where T : IDictionary, new()
    {
        if (config == null)
        {
            return (T)default;
        }

        var dict = new T();

        SetDictionaryValue(dict, nameof(config.Exclusions), config.Exclusions.ToArray());
        SetDictionaryValue(dict, nameof(config.Language), config.Language);
        SetDictionaryValue(dict, nameof(config.Name), config.Name);
        SetDictionaryValue(dict, nameof(config.ProjectFile), config.ProjectFile);
        SetDictionaryValue(dict, nameof(config.RelativePath), config.RelativePath);
        SetDictionaryValue(dict, nameof(config.ReleaseSha), config.ReleaseSha);
        SetDictionaryValue(dict, nameof(config.TagTemplate), config.TagTemplate);
        SetDictionaryValue(dict, nameof(config.Version), config.Version?.ToString());

        return dict;
    }
}

public class TonberryCommitTypeConverter : IDictionaryConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is IDictionary dict)
        {
            var config = new TonberryCommitType();

            SetConfigValue<string>(dict, nameof(config.Emoji), value => config.Emoji = (string)value);
            SetConfigValue<bool>(dict, nameof(config.IsHidden), value => config.IsHidden = (bool)value);
            SetConfigValue<string>(dict, nameof(config.LogDisplayName), value => config.LogDisplayName = (string)value);
            SetConfigValue<string>(dict, nameof(config.Name), value => config.Name = (string)value);

            return config;
        }

        throw new NotSupportedException(
            string.Format(Resources.InvalidTypeCast, value.GetType(), typeof(TonberryProjectConfiguration)));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is TonberryCommitType commitType)
        {
            if (destinationType == typeof(Hashtable))
            {
                return ConvertToIDictionary<Hashtable>(commitType);
            }
            else if (destinationType == typeof(Dictionary<string, object>))
            {
                return ConvertToIDictionary<Dictionary<string, object>>(commitType);
            }
        }

        throw new NotSupportedException(string.Format(Resources.InvalidTypeCast, value.GetType(), destinationType));
    }

    private static T ConvertToIDictionary<T>(TonberryCommitType commitType) where T : IDictionary, new()
    {
        if (commitType == null)
        {
            return (T)default;
        }

        var dict = new T();

        SetDictionaryValue(dict, nameof(commitType.Emoji), commitType.Emoji);
        SetDictionaryValue(dict, nameof(commitType.IsHidden), commitType.IsHidden);
        SetDictionaryValue(dict, nameof(commitType.LogDisplayName), commitType.LogDisplayName);
        SetDictionaryValue(dict, nameof(commitType.Name), commitType.Name);

        return dict;
    }
}

public class IDictionaryConverter : TypeConverter
{
    internal IDictionaryConverter() { }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(Hashtable) || sourceType == typeof(Dictionary<string, object>);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(Hashtable) || destinationType == typeof(Dictionary<string, object>);
    }

    internal static T ConvertFromObject<T>(object value)
    {
        if (value is null)
        {
            return default;
        }

        try
        {
            var converter = ConverterCache.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
        }
        catch (Exception)
        {
            return default;
        }
    }

    internal static T[] ConvertFromObjects<T>(object[] values)
    {
        if (values == null)
        {
            return null;
        }

        return values.Where(v => v is not null).Select(v => ConvertFromObject<T>(v)).ToArray();
    }

    internal static T ConvertItemToObject<T>(IDictionary dictionary, string propertyName) where T : class
    {
        if (!dictionary.Contains(propertyName))
        {
            return null;
        }

        return ConvertFromObject<T>(dictionary[propertyName]);
    }

    internal static T[] ConvertItemsToIDictionary<T, O>(List<O> items) where T : IDictionary, new()
        => items?.Cast<object>()?.Select(i => ConvertToObject<T>(i)).ToArray();

    internal static List<T> ConvertItemsToList<T>(IDictionary dictionary, string propertyName) where T : class
    {
        if (!dictionary.Contains(propertyName))
        {
            return null;
        }

        var obj = dictionary[propertyName];
        if (obj is object[] items)
        {
            return new List<T>(ConvertFromObjects<T>(items));
        }
        else if (obj is IList list)
        {
            return new List<T>(ConvertFromObjects<T>(list.Cast<object>().ToArray()));
        }

        return new List<T>() { ConvertFromObject<T>(obj) };
    }

    internal static T ConvertToObject<T>(object value)
    {
        if (value is null)
        {
            return default;
        }

        try
        {
            var converter = ConverterCache.GetConverter(typeof(T));
            return (T)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(T));
        }
        catch (Exception)
        {
            return default;
        }
    }

    internal static void SetConfigValue<T>(IDictionary dictionary, string propertyName, Action<object> setter)
    {
        if (dictionary.Contains(propertyName))
        {
            var value = dictionary[propertyName];
            if (value is T typedValue)
            {
                setter(typedValue);
            }
        }
    }

    internal static void SetDictionaryValue(IDictionary dictionary, string propertyName, object value)
    {
        if (value is string strValue && !string.IsNullOrEmpty(strValue))
        {
            dictionary[propertyName] = strValue;
            return;
        }

        if (value is not null)
        {
            dictionary[propertyName] = value;
        }
    }
}

internal static class ConverterCache
{
    private static readonly Dictionary<Type, TypeConverter> _converters = [];

    internal static TypeConverter GetConverter(Type type)
    {
        if (!_converters.TryGetValue(type, out var converter))
        {
            converter = TypeDescriptor.GetConverter(type);
            _converters[type] = converter;
        }

        return converter;
    }
}