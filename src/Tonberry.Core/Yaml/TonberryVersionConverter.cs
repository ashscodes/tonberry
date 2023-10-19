using System;
using Tonberry.Core.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Tonberry.Core.Yaml;

public class TonberryVersionConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(TonberryVersion);

    public object ReadYaml(IParser parser, Type type)
    {
        string versionStr = string.Empty;
        if (parser.Current != null && parser.Current is Scalar scalar)
        {
            versionStr = scalar.Value;
        }

        parser.MoveNext();
        return TonberryVersion.TryParse(versionStr, out TonberryVersion version) ? version : null;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
        => emitter.Emit(new Scalar(
            null,
            null,
            value is not null && value is TonberryVersion version ? version.ToString() : string.Empty,
            ScalarStyle.Any,
            true,
            false));
}