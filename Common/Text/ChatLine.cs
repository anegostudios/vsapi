
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// an internal control containing the properties of a chat message.
    /// </summary>
    public class ChatLine
    {
        /// <summary>
        /// The UID of the player who sent the message.
        /// </summary>
        public string ByPlayerUID;

        /// <summary>
        /// The message that was sent.
        /// </summary>
        public string Message;

        /// <summary>
        /// The type of chat the message was sent as.
        /// </summary>
        public EnumChatType ChatType;
    }
}
