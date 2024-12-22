using System.Linq;
using System.Text.RegularExpressions;

namespace AutoEvent.API;

public static class StringExtensions
{
    /// <summary>
    /// Converts a <see cref="string"/> to camelCase convention.
    /// </summary>
    /// <param name="str">The string to be converted.</param>
    /// <param name="shouldReplaceSpecialChars">Indicates whether special chars has to be replaced or not.</param>
    /// <returns>Returns the new camelCase string.</returns>
    public static string ToCamelCase(this string str, bool shouldReplaceSpecialChars = false)
    {
        var x = str.Replace("_", "");
        if (x.Length == 0) return "null";
        x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
            m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
        x = char.ToLower(x[0]) + x.Substring(1);
        return shouldReplaceSpecialChars ? Regex.Replace(x, @"[^0-9a-zA-Z_]+", string.Empty) : x;
    }
    /// <summary>
    /// Converts a <see cref="string"/> to snake_case convention.
    /// </summary>
    /// <param name="str">The string to be converted.</param>
    /// <param name="shouldReplaceSpecialChars">Indicates whether special chars has to be replaced or not.</param>
    /// <returns>Returns the new snake_case string.</returns>
    public static string ToSnakeCase(this string str, bool shouldReplaceSpecialChars = true)
    {
        string snakeCaseString = string.Concat(str.Select((ch, i) => (i > 0) && char.IsUpper(ch) ? "_" + ch.ToString() : ch.ToString())).ToLower();

        return shouldReplaceSpecialChars ? Regex.Replace(snakeCaseString, @"[^0-9a-zA-Z_]+", string.Empty) : snakeCaseString;
    }
}