using Tonberry.Core.Model;

namespace Tonberry.Core;

public interface ITagParser
{
    bool IsMonoRepoTag { get; set; }

    string ProjectName { get; set; }

    TonberryVersion Version { get; set; }

    void Parse(string tag);
}