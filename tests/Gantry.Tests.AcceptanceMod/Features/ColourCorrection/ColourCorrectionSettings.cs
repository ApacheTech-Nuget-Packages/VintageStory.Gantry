using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Tests.AcceptanceMod.Features.ColourCorrection.Enums;
using Newtonsoft.Json;

namespace Gantry.Tests.AcceptanceMod.Features.ColourCorrection
{
    /// <summary>
    ///     User definable settings for the Colour Correction feature. Overrides the game's default colour and saturation balance.
    /// </summary>
    [JsonObject]
    public sealed class ColourCorrectionSettings : FeatureSettings
    {
        /// <summary>
        ///     Represents a value indicating whether the colour correction shader is enabled, or not.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Represents a preset of colour balance and saturation settings, used to simulate various colour vision deficiencies.
        /// </summary>
        /// <value>The currently active preset of <see cref="ColourCorrectionSettings"/>.</value>
        public ColourVisionType Preset { get; set; }

        /// <summary>
        ///     Represents how much Red to apply to the scene.
        /// </summary>
        /// <value>A <see cref="float"/> value that represents the balance of the Red colour channel within the game scene.</value>
        public float Red { get; set; } = 1f;

        /// <summary>
        ///     Represents how much Green to apply to the scene.
        /// </summary>
        /// <value>A <see cref="float"/> value that represents the balance of the Green colour channel within the game scene.</value>
        public float Green { get; set; } = 1f;

        /// <summary>
        ///     Represents how much Blue to apply to the scene.
        /// </summary>
        /// <value>A <see cref="float"/> value that represents the balance of the Blue colour channel within the game scene.</value>
        public float Blue { get; set; } = 1f;

        /// <summary>
        ///     Represents the colour saturation of the scene. Lower values fade colours out, higher values make colours more vibrant.
        /// </summary>
        /// <value>A <see cref="float"/> value that represents the saturation of the colours within the game scene.</value>
        public float Saturation { get; set; } = 1f;
    }
}