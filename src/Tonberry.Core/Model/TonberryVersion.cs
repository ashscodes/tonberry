using System;
using System.Text;

namespace Tonberry.Core.Model;

public class TonberryVersion : IComparable, IComparable<TonberryVersion>, IEquatable<TonberryVersion>
{
    private int? _releaseCandidateVersion = null;

    internal static TonberryVersion General = new(1, 0, 0);

    internal static TonberryVersion None = new(0, 0, 0);

    public string BuildLabel { get; internal set; }

    public bool HasBuildLabel => !string.IsNullOrEmpty(BuildLabel);

    public bool HasPreReleaseLabel => !string.IsNullOrEmpty(PreReleaseLabel);

    public bool IsReleaseCandidate => !string.IsNullOrEmpty(PreReleaseLabel) && PreReleaseLabel.StartsWith("rc");

    public int Major => Version.Major;

    public int Minor => Version.Minor;

    public int Patch => Version.Build;

    public string PreReleaseLabel { get; internal set; }

    internal int RCVersion
    {
        get
        {
            if (_releaseCandidateVersion is null && IsReleaseCandidate)
            {
                foreach (char separator in new[] { '.', '-' })
                {
                    if (PreReleaseLabel.Contains(separator))
                    {
                        var preReleaseSplit = PreReleaseLabel.Split(separator);
                        if (int.TryParse(preReleaseSplit[1], out int releaseCandidateVersion))
                        {
                            _releaseCandidateVersion = releaseCandidateVersion;
                            return _releaseCandidateVersion.Value;
                        }
                    }
                }
            }

            return _releaseCandidateVersion.HasValue ? _releaseCandidateVersion.Value : 0;
        }
        set => _releaseCandidateVersion = value;
    }

    internal Version Version { get; set; }

    public TonberryVersion(int major) : this(major, 0, 0) { }

    public TonberryVersion(int major, int minor) : this(major, minor, 0) { }

    public TonberryVersion(int major, int minor, int patch)
    {
        if (major < 0)
        {
            throw new ArgumentNullException(nameof(major));
        }

        if (minor < 0)
        {
            throw new ArgumentNullException(nameof(minor));
        }

        if (patch < 0)
        {
            throw new ArgumentNullException(nameof(patch));
        }

        Version = new Version(major, minor, patch);
    }

    public TonberryVersion(int major,
                           int minor,
                           int patch,
                           string label) : this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(label))
        {
            PreReleaseLabel = GitUtil.SemanticLabelRegex()
                                     .IsMatch(label) ? label : throw new FormatException(nameof(label));
        }
    }

    public TonberryVersion(int major,
                           int minor,
                           int patch,
                           string preRelease,
                           string build) : this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(build))
        {
            BuildLabel = GitUtil.SemanticUnitRegex()
                                .IsMatch(build) ? build : throw new FormatException(nameof(build));
        }

        if (!string.IsNullOrEmpty(preRelease))
        {
            PreReleaseLabel = GitUtil.SemanticUnitRegex()
                                     .IsMatch(preRelease) ? preRelease : throw new FormatException(nameof(preRelease));
        }
    }

    public TonberryVersion(Version version)
    {
        if (version == null)
        {
            throw new ArgumentNullException(nameof(version));
        }

        Version = new Version(version.Major, version.Minor, version.Build == -1 ? 0 : version.Build);
    }

    private TonberryVersion(Version version, string build, string preRelease) : this(version)
    {
        BuildLabel = build;
        PreReleaseLabel = preRelease;
    }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not TonberryVersion version)
        {
            throw new ArgumentException(null, nameof(obj));
        }

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
            return Major > other.Major ? 1 : -1;
        }

        if (Minor != other.Minor)
        {
            return Minor > other.Minor ? 1 : -1;
        }

        if (Patch != other.Patch)
        {
            return Patch > other.Patch ? 1 : -1;
        }

        if (IsReleaseCandidate && other.IsReleaseCandidate && RCVersion != other.RCVersion)
        {
            return RCVersion > other.RCVersion ? 1 : -1;
        }

        return CompareLabels(other.PreReleaseLabel);
    }

    public bool Equals(TonberryVersion other)
        => other is not null
           && (Major == other.Major)
           && (Minor == other.Minor)
           && (Patch == other.Patch)
           && string.Equals(PreReleaseLabel, other.PreReleaseLabel, StringComparison.Ordinal);

    public void RemoveLabels()
    {
        BuildLabel = string.Empty;
        PreReleaseLabel = string.Empty;
    }

    public override bool Equals(object obj) => Equals(obj as TonberryVersion);

    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreReleaseLabel, BuildLabel);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Version.ToString(3));
        if (HasPreReleaseLabel)
        {
            sb.Append("-" + PreReleaseLabel);
        }

        if (HasBuildLabel)
        {
            sb.Append("+" + BuildLabel);
        }

        return sb.ToString();
    }

    public static int CompareTo(TonberryVersion reference, TonberryVersion compare)
    {
        if (reference != null)
        {
            return reference.CompareTo(compare);
        }

        if (compare != null)
        {
            return -1;
        }

        return 0;
    }

    public static TonberryVersion Parse(string versionStr)
    {
        if (string.IsNullOrEmpty(versionStr))
        {
            throw new ArgumentNullException(nameof(versionStr));
        }

        return TryParse(versionStr, out TonberryVersion version, out Exception ex) ? version : throw ex;
    }

    public static bool TryParse(string versionStr, out TonberryVersion version) => TryParse(versionStr, out version, out _);

    public static bool operator ==(TonberryVersion reference, TonberryVersion compare)
        => reference is null ? compare is null : reference.Equals(compare);

    public static bool operator !=(TonberryVersion reference, TonberryVersion compare) => !(reference == compare);

    public static bool operator <(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) < 0;

    public static bool operator <=(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) <= 0;

    public static bool operator >(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) > 0;

    public static bool operator >=(TonberryVersion reference, TonberryVersion compare) => CompareTo(reference, compare) >= 0;

    public static explicit operator TonberryVersion(Version version) => new(version);

    public static implicit operator Version(TonberryVersion version) => version.Version;

    private int CompareLabels(string comparison)
    {
        if (string.IsNullOrEmpty(PreReleaseLabel))
        {
            return string.IsNullOrEmpty(comparison) ? 0 : 1;
        }

        if (string.IsNullOrEmpty(comparison))
        {
            return -1;
        }

        var preReleaseSplit = PreReleaseLabel.Split('.');
        var comparisonSplit = comparison.Split('.');
        var iterations = preReleaseSplit.Length < comparisonSplit.Length ? preReleaseSplit.Length : comparisonSplit.Length;
        for (int i = 0; i < iterations; i++)
        {
            var preReleasePart = preReleaseSplit[i];
            var comparisonPart = comparisonSplit[i];
            bool isPreReleasePartNumber = int.TryParse(preReleasePart, out int preReleasePartNumber);
            bool isComparisonPartNumber = int.TryParse(comparisonPart, out int comparisonPartNumber);
            if (isPreReleasePartNumber && isComparisonPartNumber)
            {
                if (preReleasePartNumber != comparisonPartNumber)
                {
                    return preReleasePartNumber < comparisonPartNumber ? -1 : 1;
                }
            }
            else if (preReleasePart.Equals("rc") && !comparisonPartNumber.Equals("rc"))
            {
                return 1;
            }
            else if (!preReleasePart.Equals("rc") && comparisonPartNumber.Equals("rc"))
            {
                return -1;
            }
            else
            {
                if (isPreReleasePartNumber)
                {
                    return -1;
                }

                if (isComparisonPartNumber)
                {
                    return 1;
                }

                int stringComparisonResult = string.CompareOrdinal(preReleasePart, comparisonPart);
                if (stringComparisonResult != 0)
                {
                    return stringComparisonResult;
                }
            }
        }

        return preReleaseSplit.Length.CompareTo(comparisonSplit.Length);
    }

    private static bool TryParse(string versionStr, out TonberryVersion version, out Exception ex)
    {
        ex = null;
        version = null;
        if (versionStr is null)
        {
            return false;
        }

        if (versionStr.EndsWith('.') || versionStr.EndsWith('+') || versionStr.EndsWith('-'))
        {
            ex = new FormatException(nameof(versionStr));
            return false;
        }

        string buildLabel = string.Empty;
        string preReleaseLabel = string.Empty;
        string standardVersion;
        int hyphenIndex = versionStr.IndexOf('-');
        int plusIndex = versionStr.IndexOf('+');
        if (hyphenIndex > plusIndex)
        {
            if (plusIndex == -1)
            {
                preReleaseLabel = versionStr[(hyphenIndex + 1)..];
                standardVersion = versionStr[0..hyphenIndex];
            }
            else
            {
                buildLabel = versionStr[(plusIndex + 1)..];
                hyphenIndex = -1;
                standardVersion = versionStr[0..plusIndex];
            }
        }
        else
        {
            if (plusIndex == -1)
            {
                standardVersion = versionStr;
            }
            else if (hyphenIndex == -1)
            {
                buildLabel = versionStr[(plusIndex + 1)..];
                standardVersion = versionStr[0..plusIndex];
            }
            else
            {
                buildLabel = versionStr[(plusIndex + 1)..];
                preReleaseLabel = versionStr[(hyphenIndex + 1)..(plusIndex - hyphenIndex - 1)];
                standardVersion = versionStr[0..hyphenIndex];
            }
        }

        if ((hyphenIndex != -1 && string.IsNullOrEmpty(preReleaseLabel))
            || (plusIndex != -1 && string.IsNullOrEmpty(buildLabel))
            || string.IsNullOrEmpty(standardVersion))
        {
            ex = new FormatException(nameof(versionStr));
            return false;
        }

        if (!Version.TryParse(standardVersion, out Version tempVersion))
        {
            ex = new FormatException(nameof(standardVersion));
            return false;
        }

        version = new TonberryVersion(tempVersion, buildLabel, preReleaseLabel);
        return true;
    }
}