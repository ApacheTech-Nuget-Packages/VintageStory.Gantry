using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Provides extension methods to aid working with strings, including null safety, extracting initials, and XML serialisation.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringExtensions
{
    /// <summary>
    ///     Guards against null strings by returning an empty string if the string is null.
    /// </summary>
    /// <param name="param">The string to check for null.</param>
    /// <returns>The original string if not null; otherwise, an empty string.</returns>
    /// <remarks>
    ///     This method is useful for ensuring that string operations do not throw <see cref="NullReferenceException"/>.
    /// </remarks>
    public static string EmptyIfNull(this string param)
    {
        return param ?? string.Empty;
    }

    /// <summary>
    ///     Extracts the initials from a PascalCase string and converts them to lowercase.
    ///     For example, "HelloWorld" becomes "hw".
    /// </summary>
    /// <param name="value">The PascalCase string from which to extract initials.</param>
    /// <returns>A lowercase string containing the initials of the PascalCase string, or an empty string if input is null or empty.</returns>
    /// <example>
    /// <code>
    /// var result = "HelloWorld".ToLowerCaseInitials();
    /// Console.WriteLine(result); // hw
    /// </code>
    /// </example>
    /// <remarks>
    ///     This method is useful for generating abbreviations or short codes from PascalCase identifiers.
    /// </remarks>
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
    ///     Serialises an object to an XML string with indentation using <see cref="XmlSerializer"/>.
    /// </summary>
    /// <param name="this">The object to serialise to XML.</param>
    /// <returns>The XML string representation of the object, with indentation.</returns>
    /// <remarks>
    ///     This method is useful for debugging or exporting objects in a human-readable XML format.
    ///     The object must be serialisable by <see cref="XmlSerializer"/>.
    /// </remarks>
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