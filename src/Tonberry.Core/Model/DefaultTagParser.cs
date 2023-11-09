namespace Tonberry.Core.Model;

public class DefaultTagParser : ITagParser
{
    public bool IsMonoRepoTag { get; set; }

    public string ProjectName { get; set; }

    public TonberryVersion Version { get; set; }

    public void Parse(string tag)
    {
        if (tag.StartsWith('v'))
        {
            tag = tag.TrimStart('v');
        }

        if (tag.Contains("/#/"))
        {
            var tagSections = tag.Split("/#/");
            tag = tagSections[0];
            if (tagSections.Length == 2)
            {
                IsMonoRepoTag = true;
                ProjectName = tagSections[1];
            }
        }

        Version = TonberryVersion.TryParse(tag, out TonberryVersion version) ? version : null;
    }
}

public interface ITagParser
{
    bool IsMonoRepoTag { get; set; }

    string ProjectName { get; set; }

    TonberryVersion Version { get; set; }

    void Parse(string tag);
}