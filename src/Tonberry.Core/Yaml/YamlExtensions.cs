using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Tonberry.Core.Yaml;

public static class YamlExtensions
{
    public static DeserializerBuilder WithRequiredPropertyValidation(this DeserializerBuilder builder)
        => builder.WithNodeDeserializer(n =>
        {
            return new PropertyValidationDeserializer(n);
        }, s => s.InsteadOf<ObjectNodeDeserializer>());
}