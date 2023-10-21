using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Newtonsoft.Json;

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings
{
    [JsonObject]
    internal class ClientGlobalSettings : FeatureSettings, IMessageProvider
    {
        /// <summary>
        ///     The message to display to the user.
        /// </summary>
        public string Message { get; set; } = "Client: Message from global settings file.";
    }
}