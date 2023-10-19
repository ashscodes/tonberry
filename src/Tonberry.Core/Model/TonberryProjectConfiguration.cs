using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Model;

public class TonberryProjectConfiguration : BaseConfiguration
{
    [YamlMember(Alias = "relativePath", ApplyNamingConventions = false)]
    public string RelativePath { get; set; } = Resources.MonoRepoPathExample;

    internal bool Exists { get; set; }

    public TonberryProjectConfiguration() { }

    internal TonberryProjectConfiguration(string name) => Name = name;

    public override void Validate(DirectoryInfo rootDirectory)
    {
        if (!RelativePath.Equals(Resources.MonoRepoPathExample))
        {
            var projectPath = new DirectoryInfo(Path.Combine(rootDirectory.FullName, RelativePath));
            Exists = projectPath.Exists;
            if (Exists)
            {
                Changelog = new FileInfo(Path.Combine(projectPath.FullName,
                                                      string.Format(Resources.ProjectChangelogFile, Name)));
            }

            return;
        }

        Exists = false;
    }

    protected internal override bool TryUpdateProjectFiles() => throw new NotImplementedException();
}