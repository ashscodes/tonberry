using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tonberry.Core.Internal;

[DebuggerStepThrough]
internal static class Ensure
{
    public static void EnumerableNotNullOrEmpty<T>(IEnumerable<T> enumerable, string message)
    {
        ValueNotNull(enumerable, message);
        if (!enumerable.Any())
        {
            throw new TonberryApplicationException(message);
        }
    }

    public static void IsEnumValue<TEnum>(string value) where TEnum : struct, Enum
    {
        if (!Enum.TryParse(value, true, out TEnum _))
        {
            string enumValues = string.Join(", ", Enum.GetValues<TEnum>());
            throw new TonberryApplicationException(Resources.InvalidEnumType, value, enumValues);
        }
    }

    public static void IsFalse(bool value, string message)
    {
        if (value)
        {
            throw new TonberryApplicationException(message);
        }
    }

    public static void IsTrue(bool value, string message)
    {
        if (!value)
        {
            throw new TonberryApplicationException(message);
        }
    }

    public static string StringHasValue(string value, string defaultString)
        => string.IsNullOrEmpty(value) ? defaultString : value;

    public static void StringNotNullOrEmpty(string value, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TonberryApplicationException(message, args);
        }
    }

    public static void ValueNotNull(object value, string message)
    {
        if (value is null)
        {
            throw new TonberryApplicationException(message);
        }
    }
}