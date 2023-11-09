<p align="center">
  <img src="https://static.wikia.nocookie.net/finalfantasy/images/7/7f/FF8_Tonberry.png/revision/latest/scale-to-width-down/125?cb=20120506172231" />
</p>

# tonberry

**tonberry** is a simple tool to support conventional commits, semantic versioning, repository tagging and changelog creation from the git commit history for the current branch. The [core library](src/Tonberry.Core) should allow configuring a monorepo by defining the individual projects and their paths in the [`tonberry.config.yaml`](sample/tonberry.config.yml) file.

The following clients will be available soon:

- [tonberry CLI](src/Tonberry/) as a [dotnet-tool](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools).

- MSBuild task.

- [PSTonberry](https://github.com/ashscodes/PSTonberry) - A PowerShell module with additional options for customisation.

Most of the instruction below for using the core library will be replaced with docs and a quickstart for the CLI app when it is completed.

## Getting Started

Currently only the [Tonberry.Core](src/Tonberry.Core) library is available and it is without documentation, and it will not be supported directly. There is no issue with anyone downloading it and trying it out however.

A [sample](src/Tonberry.Core.Sample/Program.cs) of building a basic configuration, saving it and running a migration of an existing repository, where the repository is tagged, to **tonberry** is included.

## Configuration

The [**tonberry** config file](tonberry.config.yml) contains a number of options for customisation, but most of them are not required.

Options that are required are as follows:

| Option | Type | Default Value |
| --- | --- | --- |
| commitTypes | `List` of [TonberryCommitType](src/Tonberry.Core/Model/TonberryCommitType.cs) | default commit types |
| commitUrlFormat | `string` | `https://<host>/<path>/<projectName>/commit/{0}` |
| compareUrlFormat | `string` | `https://<host>/<path>/<projectName>/compare/{0}...{1}` |
| includeEmojis | `bool` | `true` |
| issueUrlFormat | `string` | `https://<host>/<path>/<projectName>/issues/{0}` |
| listContributors | `bool` | `true` |
| name | `string` | - |
| projectUrl | `string` | `https://<host>/<path>/<projectName>.git` |
| userUrlFormat | `string` | `https://<host>/{0}` |

## Commit & Tag Parsing

The commits and tags can have their default parsing overridden by implementing either `ICommitParser` or `ITagParser`. The default parsers, [DefaultCommitParser](src/Tonberry.Core/Model/DefaultCommitParser.cs) and [DefaultTagParser](src/Tonberry.Core/Model/DefaultTagParser.cs), give an example of how to implement the `Parse()` method.

The default tags will be:

- **Single project repository:** `v{0}` - where `{0}` is the semantic version.

- **Monorepo repository:** `v{0}/#/{1}` - where `{0}` is the semantic version, and `{1}` is the project name.

## Dependencies

:clap: Thanks to all the cool projects that have made this possible. You can check them our and their licenses below.

| Project | Dependencies |
| --- | --- |
| [Tonberry.Core](src/Tonberry.Core/) | [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) - [MITLicense](https://github.com/libgit2/libgit2sharp/blob/master/LICENSE.md) |
| [Tonberry.Prompt](src/Tonberry.Prompt/) | [Spectre.Console](https://github.com/spectreconsole/spectre.console) - [MITLicense](https://github.com/spectreconsole/spectre.console/blob/main/LICENSE.md) |
| [Tonberry](src/Tonberry/) | [Spectre.Console.Cli](https://github.com/spectreconsole/spectre.console) - [MITLicense](https://github.com/spectreconsole/spectre.console/blob/main/LICENSE.md) |

## Future Ideas

- Introduce branch strategies so you can automatically label a release as, for example, a Release Candidate, nightly build or alpha/beta in the config.

- Integrate the GitHub API to allow the choice of git commit history or labelled PR history.

## Contributions

Not taking any currently, because this is a, hopefully, working sample for my portfolio. This may change if the repo gets more attention and the client apps I plan to introduce gain some users.
