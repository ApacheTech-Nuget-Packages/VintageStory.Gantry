using ProtoBuf;

namespace Gantry.Tests.AcceptanceMod.Features.Network.Packets
{
    /// <summary>
    ///     A packet used to test the network service.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class TestMessagePacket
    {
        /// <summary>
        ///     The message to send to the server.
        /// </summary>
        public string Message { get; set; }
    }
}