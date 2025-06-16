using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    public delegate void ServerChatCommandDelegate(IServerPlayer player, int groupId, CmdArgs args);
    public delegate void ClientChatCommandDelegate(int groupId, CmdArgs args);

    /// <summary>
    /// A base class for all chat commands.
    /// </summary>
    public abstract class ChatCommand
    {
        /// <summary>
        /// the command calling name.
        /// </summary>
        public string Command;

        /// <summary>
        /// The syntax of the command.
        /// </summary>
        public string Syntax;

        /// <summary>
        /// The description of the command.
        /// </summary>
        public string Description;

        /// <summary>
        /// The required privilage for the command to be ran.
        /// </summary>
        public string RequiredPrivilege;

        /// <summary>
        /// The call handler for the command.
        /// </summary>
        /// <param name="player">The player calling the command.</param>
        /// <param name="groupId">The groupID of the player.</param>
        /// <param name="args">The arguments of the command.</param>
        public abstract void CallHandler(IPlayer player, int groupId, CmdArgs args);

        /// <summary>
        /// gets the description of the command.
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription()
        {
            return Description;
        }

        /// <summary>
        /// Gets the syntax of the command.
        /// </summary>
        /// <returns></returns>
        public virtual string GetSyntax()
        {
            return Syntax;
        }

        /// <summary>
        /// Gets the help message of the command.
        /// </summary>
        /// <returns></returns>
        public virtual string GetHelpMessage()
        {
            return
                Command + ": " + Description + "\n" +
                "Syntax: " + Syntax
            ;
        }
    }

    /// <summary>
    /// A handler for creating server commands.
    /// </summary>
    public class ServerChatCommand : ChatCommand
    {
        public ServerChatCommandDelegate handler;

        /// <summary>
        /// Whether or not the player has the privilage to run the command.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool HasPrivilege(IServerPlayer player)
        {
            return player.HasPrivilege(RequiredPrivilege);
        }

        public override void CallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            handler((IServerPlayer)player, groupId, args);
        }
    }

    /// <summary>
    /// A handle for creating client commands.
    /// </summary>
    public class ClientChatCommand : ChatCommand
    {
        public ClientChatCommandDelegate handler;

        public override void CallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            handler(groupId, args);
        }
    }
}
