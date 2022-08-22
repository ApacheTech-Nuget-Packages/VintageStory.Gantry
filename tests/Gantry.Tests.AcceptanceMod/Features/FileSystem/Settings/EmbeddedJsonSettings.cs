using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Newtonsoft.Json;

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings
{
    [JsonObject]
    internal class EmbeddedJsonSettings : IMessageProvider
    {
        /// <summary>
        ///     The message to display to the user.
        /// </summary>
        public string Message { get; set; }
    }
}