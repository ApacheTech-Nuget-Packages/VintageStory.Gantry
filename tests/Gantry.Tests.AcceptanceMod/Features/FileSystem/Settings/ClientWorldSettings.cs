using Gantry.Services.FileSystem.Features;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Newtonsoft.Json;

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings
{
    [JsonObject]
    internal class ClientWorldSettings : FeatureSettings, IMessageProvider
    {
        /// <summary>
        ///     The message to display to the user.
        /// </summary>
        public string Message { get; set; } = "Client: Message from per-world settings file.";
    }
}