using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// API Features to set up a network channel for custom server&lt;-&gt;client data exchange. Client side.
    /// </summary>
    public interface IClientNetworkAPI
    {
        /// <summary>   
        /// Supplies you with your very own and personal network channel with which you can send packets to the server. Use the same channelName on the client and server to have them link up.
        /// </summary>
        /// <param name="channelName">Unique channel identifier</param>
        /// <returns></returns>
        IClientNetworkChannel RegisterChannel(string channelName);

        /// <summary>
        /// Sends a blockentity interaction packet to the server. For quick an easy blockentity network communication without setting up a channel first.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendBlockEntityPacket(int x, int y, int z, int packetId, byte[] data = null);

        /// <summary>
        /// Sends a blockentity interaction packet to the server. For quick an easy blockentity network communication without setting up a channel first.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="internalPacket"></param>
        void SendBlockEntityPacket(int x, int y, int z, object internalPacket);

        /// <summary>
        /// Sends given packet data to the server. This let's you mess with the raw network communication and fiddle with internal engine packets if you know the protocol. For normal network communication you probably want to register your own network channel.
        /// </summary>
        /// <param name="data"></param>
        void SendArbitraryPacket(byte[] data);


        /// <summary>
        /// Sends given packet to server. For use with inventory supplied network packets only, since the packet format is not exposed to the api 
        /// </summary>
        /// <param name="packetClient"></param>
        void SendPacketClient(object packetClient);

        void SendHandInteraction(int mouseButton, BlockSelection blockSelection, EntitySelection entitySelection, EnumHandInteract beforeUseType, int state, EnumItemUseCancelReason cancelReason);
    }
}
