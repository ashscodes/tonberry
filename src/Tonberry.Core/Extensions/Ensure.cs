using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tonberry.Core;

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

    public static void IsPositive(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void IsTrue(bool value, string message)
    {
        if (!value)
        {
            throw new TonberryApplicationException(message);
        }
    }

    public static void StringDoesNotEndWith(string value, char[] characters, string parameterName)
    {
        foreach (char character in characters)
        {
            if (value.EndsWith(character))
            {
                throw new FormatException(parameterName);
            }
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

    public static void ValueIsOfType<T>(object value, string message, out T item)
    {
        item = default;
        if (value is not T temp)
        {
            throw new ArgumentNullException(message);
        }

        item = temp;
    }

    public static void ValueNotNull(object value, string message)
    {
        if (value is null)
        {
            throw new TonberryApplicationException(message, value);
        }
    }
}