// ReSharper disable UnusedMember.Global

using System.ComponentModel;

namespace Gantry.Tests.AcceptanceMod.Features.ColourCorrection.Enums
{
    /// <summary>
    ///     Represents a specific form of colour vision deficiency.
    /// </summary>
    public enum ColourVisionType
    {
        /// <summary>
        ///     Standard human vision.
        /// </summary>
        [Description("Standard human vision.")]
        Trichromacy,

        /// <summary>
        ///     No perception of Red colours.
        /// </summary>
        [Description("No perception of Red colours.")]
        Protanopia,

        /// <summary>
        ///     Reduced perception of Red colours.
        /// </summary>
        [Description("Reduced perception of Red colours.")]
        Protanomaly,

        /// <summary>
        ///     No perception of Green colours.
        /// </summary>
        [Description("No perception of Green colours.")]
        Deuteranopia,

        /// <summary>
        ///     Reduced perception of Green colours.
        /// </summary>
        [Description("Reduced perception of Green colours.")]
        Deuteranomaly,

        /// <summary>
        ///     No perception of Blue colours.
        /// </summary>
        [Description("No perception of Blue colours.")]
        Tritanopia,

        /// <summary>
        ///     Reduced perception of Blue colours.
        /// </summary>
        [Description("Reduced perception of Blue colours.")]
        Tritanomaly,

        /// <summary>
        ///     No perception of any colours.
        /// </summary>
        [Description("No perception of any colours.")]
        Achromatopsia,

        /// <summary>
        ///     Reduced perception of all colours.
        /// </summary>
        [Description("Reduced perception of all colours.")]
        Achromatomaly
    }
}