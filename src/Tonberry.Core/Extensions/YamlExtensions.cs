using Tonberry.Core.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Tonberry.Core;

public static class YamlExtensions
{
    public static DeserializerBuilder WithRequiredPropertyValidation(this DeserializerBuilder builder)
        => builder.WithNodeDeserializer(n =>
        {
            return new PropertyValidationDeserializer(n);
        }, s => s.InsteadOf<ObjectNodeDeserializer>());
}