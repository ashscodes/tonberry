using System;
using System.ComponentModel;
using System.Globalization;
using Tonberry.Core.Model;

namespace Tonberry;

public sealed class TonberryVersionTypeConverter : TypeConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string stringValue)
        {
            throw new ArgumentException(Resources.InvalidVersionString);
        }

        if (TonberryVersion.TryParse(stringValue, out TonberryVersion tonberryVersion))
        {
            return tonberryVersion;
        }

        throw new ArgumentException(Resources.InvalidVersionString);
    }
}