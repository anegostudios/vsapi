using System;
using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// A message to be sent across the network. Is serialized/deserialized using protobuf
    /// </summary>
    public interface INetworkMessage { };


    /// <summary>
    /// Handler for processing a message
    /// </summary>
    /// <param name="fromPlayer"></param>
    /// <param name="packet"></param>
    public delegate void NetworkClientMessageHandler<T>(IServerPlayer fromPlayer, T packet);


    /// <summary>
    /// Represent a custom network channel for sending messages between client and server
    /// </summary>
    public interface IServerNetworkChannel : INetworkChannel
    {
        /// <summary>
        /// Registers a handler for when you send a packet with given messageId. Must be registered in the same order as on the server.
        /// </summary>
        /// <param name="type"></param>
        new IServerNetworkChannel RegisterMessageType(Type type);

        /// <summary>
        /// Registers a handler for when you send a packet with given messageId. Must be registered in the same order as on the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        new IServerNetworkChannel RegisterMessageType<T>();

        /// <summary>
        /// Registers a handler for when you send a packet with given messageId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageHandler"></param>
        IServerNetworkChannel SetMessageHandler<T>(NetworkClientMessageHandler<T> messageHandler);

        /// <summary>
        /// Sends a packet to given player
        /// </summary>
        /// <param name="message"></param>
        /// <param name="players"></param>
        void SendPacket<T>(T message, params IServerPlayer[] players);

        /// <summary>
        /// Sends a packet to given player, where the byte[] data has already been serialized
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="players"></param>
        void SendPacket<T>(T message, byte[] data, params IServerPlayer[] players);

        /// <summary>
        /// When called on Sends a packet to all connected player, except given players
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptPlayers"></param>
        void BroadcastPacket<T>(T message, params IServerPlayer[] exceptPlayers);

    }
}
