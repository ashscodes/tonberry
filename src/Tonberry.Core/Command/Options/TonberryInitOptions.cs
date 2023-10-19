using System;
using Tonberry.Core.Internal;
using Tonberry.Core.Model;

namespace Tonberry.Core.Command.Options;

public class TonberryInitOptions : ITonberryCommandOptions
{
    public bool Force { get; set; }

    public string Name { get; set; }

    public Uri Repository { get; set; }

    public TonberryVersion Version { get; set; }

    public void Validate()
    {
        Ensure.StringNotNullOrEmpty(Name, Resources.StringIsNullOrEmpty, nameof(Name));
    }
}