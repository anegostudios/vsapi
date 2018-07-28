using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public delegate void ServerChatCommandDelegate(IServerPlayer player, int groupId, CmdArgs args);
    public delegate void ClientChatCommandDelegate(int groupId, CmdArgs args);

    public abstract class ChatCommand
    {
        public string Command;
        public string Syntax;
        public string Description;
        public string RequiredPrivilege;

        public abstract void CallHandler(IPlayer player, int groupId, CmdArgs args);

        public virtual string GetDescription()
        {
            return Description;
        }

        public virtual string GetSyntax()
        {
            return Syntax;
        }

        public virtual string GetHelpMessage()
        {
            return
                Command + ": " + Description + "\n" +
                "Syntax: " + Syntax
            ;
        }
    }

    public class ServerChatCommand : ChatCommand
    {
        public ServerChatCommandDelegate handler;

        public bool HasPrivilege(IServerPlayer player)
        {
            return player.HasPrivilege(RequiredPrivilege);
        }

        public override void CallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            handler((IServerPlayer)player, groupId, args);
        }
    }


    public class ClientChatCommand : ChatCommand
    {
        public ClientChatCommandDelegate handler;

        public override void CallHandler(IPlayer player, int groupId, CmdArgs args)
        {
            handler(groupId, args);
        }
    }
}
