using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// API Features to set up a network channel for custom server&lt;-&gt;client data exchange. Server side.
    /// </summary>
    public interface IServerNetworkAPI : INetworkAPI
    {
        /// <summary>
        /// Supplies you with your very own and personal network channel that you can use to send packets across the network.  Use the same channelName on the client and server to have them link up.
        /// </summary>
        /// <param name="channelName">Unique channel identifier</param>
        /// <returns></returns>
        new IServerNetworkChannel RegisterChannel(string channelName);

        /// <summary>
        /// Returns a previously registered channeled, null otherwise
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        new IServerNetworkChannel GetChannel(string channelName);
        /// <summary>
        /// Supplies you with your very own and personal network channel that you can use to send packets across the network.  Use the same channelName on the client and server to have them link up.
        /// Do not send larger messages then 508 bytes since some clients may be behind NAT/firwalls that may drop your packets if they get fragmented
        /// </summary>
        /// <param name="channelName">Unique channel identifier</param>
        /// <returns></returns>
        new IServerNetworkChannel RegisterUdpChannel(string channelName);

        /// <summary>
        /// Returns a previously registered channeled, null otherwise
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        new IServerNetworkChannel GetUdpChannel(string channelName);

        /// <summary>
        /// Sends a blockentity packet to the given player. For quick an easy network communication without setting up a channel first.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendBlockEntityPacket(IServerPlayer player, BlockPos pos, int packetId, byte[] data = null);

        /// <summary>
        /// Sends a entity packet to the given player and entity. For quick an easy entity network communication without setting up a channel first.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="entityid"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendEntityPacket(IServerPlayer player, long entityid, int packetId, byte[] data = null);


        /// <summary>
        /// Sends a entity packet to all players in range. For quick an easy entity network communication without setting up a channel first.
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void BroadcastEntityPacket(long entityid, int packetId, byte[] data = null);


        /// <summary>
        /// Broadcasts a blockentity packet to all connected players. For quick an easy network communication without setting up a channel first.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void BroadcastBlockEntityPacket(BlockPos pos, int packetId, byte[] data = null);

        void BroadcastBlockEntityPacket(BlockPos pos, int packetId, byte[] data = null, params IServerPlayer[] skipPlayers);


        /// <summary>
        /// Sends a packet data to given players. This lets you mess with the raw network communication if you know the protocol. Use with caution! For normal network communication you probably want to register your own network channel.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="players"></param>
        void SendArbitraryPacket(byte[] data, params IServerPlayer[] players);

        /// <summary>
        /// (for internal use: packet should be a Packet_Server)
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="players"></param>
        void SendArbitraryPacket(object packet, params IServerPlayer[] players);

        /// <summary>
        /// Sends a packet data to everyone except given players. This lets you mess with the raw network communication if you know the protocol. Use with caution! For normal network communication you probably want to register your own network channel.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exceptPlayers"></param>
        void BroadcastArbitraryPacket(byte[] data, params IServerPlayer[] exceptPlayers);

        /// <summary>
        /// (for internal use: packet should be a Packet_Server)
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="exceptPlayers"></param>
        void BroadcastArbitraryPacket(object packet, params IServerPlayer[] exceptPlayers);


        /// <summary>
        /// Sends a blockentity packet to the given player. For quick an easy network communication without setting up a channel first. Uses ProtoBuf.net to serialize the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendBlockEntityPacket<T>(IServerPlayer player, BlockPos pos, int packetId, T data = default(T));

        /// <summary>
        /// Broadcasts a blockentity packet to all connected players. For quick an easy network communication without setting up a channel first. Uses ProtoBuf.net to serialize the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pos"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void BroadcastBlockEntityPacket<T>(BlockPos pos, int packetId, T data = default(T));
    }
}
