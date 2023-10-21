using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Newtonsoft.Json;

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings
{
    [JsonObject]
    internal class ServerGlobalSettings : FeatureSettings, IMessageProvider
    {
        /// <summary>
        ///     The message to display to the user.
        /// </summary>
        public string Message { get; set; } = "Server: Message from global settings file.";
    }
}