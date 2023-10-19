using System.ComponentModel.DataAnnotations;
using System.IO;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Model;

public abstract class BaseConfiguration
{
    [YamlMember(Alias = "language", ApplyNamingConventions = false)]
    public string Language { get; set; } = "csharp";

    [Required]
    public string Name { get; set; }

    [YamlMember(Alias = "projectFile", ApplyNamingConventions = false)]
    public string ProjectFile { get; set; } = "example.csproj";

    [YamlMember(Alias = "releaseSha", ApplyNamingConventions = false, Description = "Auto-Generated: Please do not edit value.")]
    public string ReleaseSha { get; internal set; } = string.Empty;

    public TonberryVersion Version { get; set; } = TonberryVersion.None;

    protected internal FileInfo Changelog { get; internal set; }

    protected internal DirectoryInfo Directory { get; internal set; }

    public BaseConfiguration() { }

    public abstract void Validate(DirectoryInfo directory);

    protected internal abstract bool TryUpdateProjectFiles();
}