using System;

namespace Vintagestory.API.Client
{
    public interface INetworkChannel
    {
        /// <summary>
        /// The channel name this channel was registered with
        /// </summary>
        string ChannelName { get; }
    }


    /// <summary>
    /// Handler for processing a message
    /// </summary>
    /// <param name="networkMessage"></param>
    public delegate void NetworkServerMessageHandler<T>(T networkMessage);


    /// <summary>
    /// Represent a custom network channel for sending messages between client and server
    /// </summary>
    public interface IClientNetworkChannel : INetworkChannel
    {
        /// <summary>
        /// Registers a handler for when you send a packet with given messageId
        /// </summary>
        /// <param name="type"></param>
        IClientNetworkChannel RegisterMessageType(Type type);

        /// <summary>
        /// Registers a handler for when you send a packet with given messageId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IClientNetworkChannel RegisterMessageType<T>();

        /// <summary>
        /// Registers a handler for when you send a packet with given messageId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        IClientNetworkChannel SetMessageHandler<T>(NetworkServerMessageHandler<T> handler);

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="message"></param>
        void SendPacket<T>(T message);
    }


}
