using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Tonberry.Core.Model;

namespace Tonberry.Core;

public sealed class TonberryVersionConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string)
               || sourceType == typeof(Version)
               || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
    {
        return destinationType == typeof(Version)
               || destinationType == typeof(string)
               || destinationType == typeof(InstanceDescriptor)
               || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string strVersion)
        {
            try
            {
                return TonberryVersion.Parse(strVersion);
            }
            catch (Exception e)
            {
                throw new FormatException(
                    string.Format(Resources.InvalidPrimitive, strVersion, nameof(TonberryVersion)), e);
            }
        }

        if (value is Version version)
        {
            return new TonberryVersion(version.ToString());
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                     CultureInfo culture,
                                     object value,
                                     Type destinationType)
    {
        Ensure.ValueNotNull(destinationType, Resources.ValueIsNull);
        if (value is TonberryVersion version)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ctor = typeof(TonberryVersion).GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                                                                              null,
                                                                              new Type[] { typeof(string) },
                                                                              null);

                try
                {
                    Ensure.ValueNotNull(ctor, Resources.ValueIsNull);
                    return new InstanceDescriptor(ctor, new object[] { version.ToString() });
                }
                catch (Exception)
                {
                    return base.ConvertTo(context, culture, value, destinationType);
                }
            }

            if (destinationType == typeof(string))
            {
                return version.ToString();
            }

            if (destinationType == typeof(Version))
            {
                return new Version(version.Major, version.Minor, version.Patch);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool IsValid(ITypeDescriptorContext context, object value)
    {
        if (value is string strVersion)
        {
            return TonberryVersion.TryParse(strVersion, out _);
        }

        if (value is Version version)
        {
            return (new TonberryVersion(version)) is not null;
        }

        return value is TonberryVersion;
    }
}