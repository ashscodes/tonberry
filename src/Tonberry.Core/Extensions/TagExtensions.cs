using Tonberry.Core.Model;

namespace Tonberry.Core;

internal static class TagExtensions
{
    public static void Parse(this TonberryTag tag, string friendlyName)
    {
        ITagParser parser = TonberryOptions.TagParser ?? new DefaultTagParser();
        parser.Parse(friendlyName);
        tag.IsMonoRepoTag = parser.IsMonoRepoTag;
        tag.ProjectName = parser.ProjectName;
        tag.Version = parser.Version;
    }
}