using JetBrains.Annotations;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Extension methods to aid working with strings.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringExtensions
{
    /// <summary>
    ///     Guards against null strings by returning an empty string if the string is null.
    /// </summary>
    /// <param name="param">The string to check.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static string EmptyIfNull(this string param)
    {
        return param ?? string.Empty;
    }

    /// <summary>
    ///     Extracts the initials from a PascalCase string and converts them to lowercase.
    /// </summary>
    /// <param name="value">The PascalCase string from which to extract initials.</param>
    /// <returns>A lowercase string containing the initials of the PascalCase string.</returns>
    /// <example>
    /// <code>
    /// var result = "HelloWorld".ToLowerCaseInitials();
    /// Console.WriteLine(result); // hw
    /// </code>
    /// </example>
    public static string ToLowerCaseInitials(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var initials = new StringBuilder();
        initials.Append(value[0]);
        for (var i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i])) initials.Append(value[i]);
        }
        return initials.ToString().ToLower();
    }

    /// <summary>
    ///     Serialises an object to an XML string with indentation.
    /// </summary>
    /// <param name="this">The object to serialise.</param>
    /// <returns>The XML string representation of the object.</returns>
    public static string ToXml(this object @this)
    {
        var xmlSerializer = new XmlSerializer(@this.GetType());
        var stringBuilder = new StringBuilder();

        using (var writer = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
        {
            xmlSerializer.Serialize(writer, @this);
        }

        return stringBuilder.ToString();
    }
}