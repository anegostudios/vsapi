
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A definition for the types of chat that could occur.
    /// </summary>
    public enum EnumChatType
    {
        /// <summary>
        /// A command was successful.
        /// </summary>
        CommandSuccess,

        /// <summary>
        /// A command failed.
        /// </summary>
        CommandError,

        /// <summary>
        /// The message was sent to the player only.
        /// </summary>
        OwnMessage,

        /// <summary>
        /// The message was sent from another player.
        /// </summary>
        OthersMessage,

        /// <summary>
        /// The message was a notification (The world ends in 3 days, You cannot do this, ect)
        /// </summary>
        Notification,

        /// <summary>
        /// The message was sent to all the groups involved.
        /// </summary>
        AllGroups,

        /// <summary>
        /// The group has invited the player.
        /// </summary>
        GroupInvite,

        /// <summary>
        /// The player has joined or left the group.
        /// </summary>
        JoinLeave,

        /// <summary>
        /// There was a macro involved.
        /// </summary>
        Macro
    }
}
