namespace Vintagestory.API.Server
{
    /// <summary>
    /// API Features to set up a network channel for custom server&lt;-&gt;client data exchange. Server side.
    /// </summary>
    public interface IServerNetworkAPI
    {
        /// <summary>   
        /// Supplies you with your very own and personal network channel that you can use to send packets across the network.  Use the same channelName on the client and server to have them link up.
        /// </summary>
        /// <param name="channelName">Unique channel identifier</param>
        /// <returns></returns>
        IServerNetworkChannel RegisterChannel(string channelName);

        /// <summary>
        /// Sends a blockentity packet to the given player. For quick an easy network communication without setting up a channel first.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendBlockEntityPacket(IServerPlayer player, int x, int y, int z, int packetId, byte[] data = null);

        /// <summary>
        /// Sends a entity packet to the given player and entity. For quick an easy entity network communication without setting up a channel first.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void SendEntityPacket(IServerPlayer player, long entityid, int packetId, byte[] data = null);


        /// <summary>
        /// Sends a entity packet to all players in range. For quick an easy entity network communication without setting up a channel first.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void BroadcastEntityPacket(long entityid, int packetId, byte[] data = null);


        /// <summary>
        /// Broadcasts a blockentity packet to all connected players. For quick an easy network communication without setting up a channel first.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="packetId"></param>
        /// <param name="data"></param>
        void BroadcastBlockEntityPacket(int x, int y, int z, int packetId, byte[] data = null);


        /// <summary>
        /// Sends a packet data to given players. This let's you mess with the raw network communication if you know the protocol. Use with caution! For normal network communication you probably want to register your own network channel.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="players"></param>
        void SendArbitraryPacket(byte[] data, params IServerPlayer[] players);

        /// <summary>
        /// Sends a packet data to everyone except given players.This let's you mess with the raw network communication if you know the protocol. Use with caution! For normal network communication you probably want to register your own network channel.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exceptPlayers"></param>
        void BroadcastArbitraryPacket(byte[] data, params IServerPlayer[] exceptPlayers);
    }
}
