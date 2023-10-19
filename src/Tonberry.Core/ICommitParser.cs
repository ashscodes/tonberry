namespace Tonberry.Core;

public interface ICommitParser
{
    string Breaking { get; set; }

    string Description { get; set; }

    string Scope { get; set; }

    string Synopsis { get; set; }

    string Type { get; set; }

    void Parse(string message, CommitParserDelegate parser);
}