using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Tonberry.Core.Model;

public partial class TonberryVersion : IComparable, IComparable<TonberryVersion>, IEquatable<TonberryVersion>
{
    public string Build { get; internal set; }

    public bool HasBuildLabel => !string.IsNullOrEmpty(Build);

    public bool IsGeneralRelease => Major > 0 && !IsPreRelease && !HasBuildLabel;

    public bool IsPreRelease => !string.IsNullOrEmpty(PreRelease);

    public bool IsReleaseCandidate => IsPreRelease && PreRelease.StartsWith("rc");

    public int Major => Version?.Major is not null ? Version.Major : 0;

    public int Minor => Version?.Minor is not null ? Version.Minor : 0;

    public int Patch => Version?.Build is not null ? Version.Build : 0;

    public string PreRelease { get; internal set; }

    public int? PreReleaseIteration => GetPreReleaseIteration();

    internal bool HasVersion => Major > 0 || Minor > 0 || Patch > 0;

    internal Version Version { get; set; }

    public TonberryVersion(int major) : this(major, 0, 0) { }

    public TonberryVersion(int major, int minor) : this(major, minor, 0) { }

    public TonberryVersion(int major, int minor, int patch) : this()
    {
        Ensure.IsPositive(major, nameof(major));
        Ensure.IsPositive(minor, nameof(minor));
        Ensure.IsPositive(patch, nameof(patch));
        Version = new Version(major, minor, patch);
    }

    public TonberryVersion(string version) => Parse(version);

    public TonberryVersion(Version version)
    {
        Ensure.ValueNotNull(version, nameof(version));
        Version = new Version(version.Major, version.Minor, version.Build < 0 ? 0 : version.Build);
    }

    internal TonberryVersion() { }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }

        Ensure.ValueIsOfType(obj, nameof(obj), out TonberryVersion version);
        return CompareTo(version);
    }

    public int CompareTo(TonberryVersion other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Major != other.Major)
        {
            return Major.CompareTo(other.Major);
        }

        if (Minor != other.Minor)
        {
            return Minor.CompareTo(other.Minor);
        }

        if (Patch != other.Patch)
        {
            return Patch.CompareTo(other.Patch);
        }

        return ComparePreRelease(other.PreRelease, other.Build);
    }

    public bool Equals(TonberryVersion other)
         => other is not null
           && (Major == other.Major)
           && (Minor == other.Minor)
           && (Patch == other.Patch)
           && string.Equals(PreRelease, other.PreRelease, Resources.StrCompare)
           && string.Equals(Build, other.Build, Resources.StrCompare);

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        Ensure.ValueIsOfType(obj, nameof(obj), out TonberryVersion version);
        return Equals(version);
    }

    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreRelease, Build);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Version.ToString(3));
        if (IsPreRelease)
        {
            sb.Append("-" + PreRelease);
        }

        if (HasBuildLabel)
        {
            sb.Append("+" + Build);
        }

        return sb.ToString();
    }

    public static int CompareTo(TonberryVersion reference, TonberryVersion comparison)
        => reference is not null ? reference.CompareTo(comparison) : comparison is not null ? -1 : 0;

    public static TonberryVersion Parse(string value)
    {
        Ensure.StringNotNullOrEmpty(value, nameof(value));
        return TryParse(value, out TonberryVersion version) ? version : null;
    }

    public static bool TryParse(string value, out TonberryVersion version)
    {
        version = null;
        Ensure.StringNotNullOrEmpty(value, nameof(value));
        Ensure.StringDoesNotEndWith(value, ['.', '+', '-'], nameof(value));
        Match match = SemanticVersion().Match(value);
        if (match.Success)
        {
            var major = int.Parse(match.Groups["major"].Value);
            var minor = int.Parse(match.Groups["minor"].Value);
            var patch = int.Parse(match.Groups["patch"].Value);
            version = new TonberryVersion(major, minor, patch);
            if (match.Groups["prerelease"].Success)
            {
                version.PreRelease = match.Groups["prerelease"].Value;
            }

            if (match.Groups["buildmetadata"].Success)
            {
                version.Build = match.Groups["buildmetadata"].Value;
            }

            return true;
        }

        return false;
    }

    public static bool operator ==(TonberryVersion reference, TonberryVersion compare)
        => reference is null ? compare is null : reference.Equals(compare);

    public static bool operator !=(TonberryVersion reference, TonberryVersion compare) => !(reference == compare);

    public static bool operator <(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) < 0;

    public static bool operator <=(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) <= 0;

    public static bool operator >(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) > 0;

    public static bool operator >=(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) >= 0;

    public static explicit operator TonberryVersion(Version version) => new(version);

    public static implicit operator Version(TonberryVersion version) => version.Version;

    private int CompareBuild(string build)
    {
        var compareBuildNull = string.IsNullOrEmpty(build);
        var thisBuildNull = string.IsNullOrEmpty(Build);
        if (thisBuildNull && compareBuildNull)
        {
            return 0;
        }
        else if (thisBuildNull && !compareBuildNull)
        {
            return 1;
        }
        else if (!thisBuildNull && compareBuildNull)
        {
            return -1;
        }
        else
        {
            var thisSplit = Build.Split('.');
            var compareSplit = build.Split('.');
            var minimum = thisSplit.Length < compareSplit.Length ? thisSplit.Length : compareSplit.Length;
            for (int i = 0; i < minimum; i++)
            {
                var comparison = compareSplit[i];
                var reference = thisSplit[i];
                var compareIsNumeric = int.TryParse(comparison, out int compareNumber);
                var refIsNumeric = int.TryParse(reference, out int refNumber);
                if (refIsNumeric && compareIsNumeric)
                {
                    if (refNumber != compareNumber)
                    {
                        return refNumber.CompareTo(compareNumber);
                    }
                }
                else if (refIsNumeric)
                {
                    return -1;
                }
                else if (compareIsNumeric)
                {
                    return 1;
                }
                else
                {
                    int stringCompare = string.CompareOrdinal(reference, comparison);
                    if (stringCompare != 0)
                    {
                        return stringCompare;
                    }
                }
            }

            return thisSplit.Length.CompareTo(compareSplit.Length);
        }
    }

    private int ComparePreRelease(string preRelease, string build)
    {
        var compareBuildNull = string.IsNullOrEmpty(build);
        var comparePreReleaseNull = string.IsNullOrEmpty(preRelease);
        var thisBuildNull = string.IsNullOrEmpty(Build);
        var thisPreReleaseNull = string.IsNullOrEmpty(PreRelease);
        if (thisPreReleaseNull && comparePreReleaseNull)
        {
            if (!compareBuildNull || !thisBuildNull)
            {
                return CompareBuild(build);
            }

            return 0;
        }
        else if (thisPreReleaseNull && !comparePreReleaseNull)
        {
            return 1;
        }
        else if (!thisPreReleaseNull && comparePreReleaseNull)
        {
            return -1;
        }
        else
        {
            var thisSplit = PreRelease.Split('.');
            var compareSplit = preRelease.Split('.');
            var minimum = thisSplit.Length < compareSplit.Length ? thisSplit.Length : compareSplit.Length;
            for (int i = 0; i < minimum; i++)
            {
                var comparison = compareSplit[i];
                var reference = thisSplit[i];
                var compareIsNumeric = int.TryParse(comparison, out int compareNumber);
                var refIsNumeric = int.TryParse(reference, out int refNumber);
                if (refIsNumeric && compareIsNumeric)
                {
                    if (refNumber != compareNumber)
                    {
                        return refNumber.CompareTo(compareNumber);
                    }
                }
                else if (reference.Equals("rc", Resources.StrCompare) && !comparison.Equals("rc", Resources.StrCompare))
                {
                    return 1;
                }
                else if (!reference.Equals("rc", Resources.StrCompare) && comparison.Equals("rc", Resources.StrCompare))
                {
                    return -1;
                }
                else
                {
                    if (refIsNumeric)
                    {
                        return -1;
                    }

                    if (compareIsNumeric)
                    {
                        return 1;
                    }

                    int stringCompare = string.CompareOrdinal(reference, comparison);
                    if (stringCompare != 0)
                    {
                        return stringCompare;
                    }
                }
            }

            if (!compareBuildNull || !thisBuildNull)
            {
                return CompareBuild(build);
            }

            return thisSplit.Length.CompareTo(compareSplit.Length);
        }
    }

    private TonberryVersion GetNext(bool bumpMajor,
                                    bool bumpMinor,
                                    bool hasNewFeatures,
                                    bool isBreaking,
                                    string buildLabel,
                                    string releaseLabel)
    {
        TonberryVersion next = null;
        if (!IsPreRelease)
        {
            if ((isBreaking && IsGeneralRelease) || bumpMajor)
            {
                next = new(Major + 1, 0, 0);
            }
            else if ((isBreaking && !IsGeneralRelease)
                     || (hasNewFeatures && IsGeneralRelease)
                     || bumpMinor)
            {
                next = new(Major, Minor + 1, 0);
            }
            else
            {
                next = new(Major, Minor, Patch + 1);
            }
        }
        else
        {
            next = new(Major, Minor, Patch);
        }

        next.Build = Ensure.StringHasValue(buildLabel, string.Empty);
        next.PreRelease = GetNextLabel(PreRelease, releaseLabel);
        return next;
    }

    private int? GetPreReleaseIteration() => IsPreRelease ? TryGetIteration(PreRelease, out _) : null;

    private static string GetNextLabel(string label, string nextLabel)
    {
        if (!string.IsNullOrEmpty(nextLabel))
        {
            int currentIteration = TryGetIteration(label, out int index);
            if (!string.IsNullOrEmpty(label))
            {
                string currentLabel = label.Substring(0, index + 1);
                if (currentLabel.Equals(nextLabel))
                {
                    return currentLabel + (currentIteration + 1);
                }
            }

            return nextLabel + "." + currentIteration;
        }

        return string.Empty;
    }

    private static int TryGetIteration(string label, out int index)
    {
        index = -1;
        var dotIndex = label.LastIndexOf('.');
        var hyphenIndex = label.LastIndexOf('-');
        var segment = string.Empty;
        if (dotIndex > hyphenIndex && dotIndex > 0)
        {
            index = dotIndex;
            segment = label.Substring(dotIndex + 1);
        }

        if (hyphenIndex > dotIndex && hyphenIndex > 0)
        {
            index = hyphenIndex;
            segment = label.Substring(hyphenIndex + 1);
        }

        return int.TryParse(segment, out int iteration) ? iteration : 0;
    }

    internal TonberryVersion GetNextIteration(TonberryChangelogOptions options,
                                              bool hasNewFeatures,
                                              bool isBreaking,
                                              TonberryVersion configVersion)
    {
        TonberryVersion next;
        if (options is TonberryReleaseOptions ro)
        {
            next = GetNext(ro.BumpMajor, ro.BumpMinor, hasNewFeatures, isBreaking, ro.Build, ro.PreRelease);
        }
        else if (options is TonberryNewOptions no && configVersion.HasVersion)
        {
            next = configVersion;
            next.Build = Ensure.StringHasValue(next.Build, GetNextLabel(string.Empty, no.Build));
            next.PreRelease = Ensure.StringHasValue(next.Build, GetNextLabel(string.Empty, no.PreRelease));
        }
        else
        {
            next = GetNext(false, false, hasNewFeatures, isBreaking, string.Empty, string.Empty);
        }

        return next;
    }

    // Try the regex here: https://regex101.com/r/XFJZ9E/1
    [GeneratedRegex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")]
    internal static partial Regex SemanticVersion();
}