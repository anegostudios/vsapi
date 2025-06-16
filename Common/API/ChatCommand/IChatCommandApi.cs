using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable


namespace Vintagestory.API.Common.CommandAbbr
{
    /// <summary>
    /// ChatCommand Abbreviations
    /// </summary>
    public static class IChatCommandExt
    {
        /// <summary>
        /// Alias of WithDescription()
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static IChatCommand WithDesc(this IChatCommand cmd, string description) => cmd.WithDescription(description);

        /// <summary>
        /// Alias for BeginSubCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IChatCommand BeginSub(this IChatCommand cmd, string name) => cmd.BeginSubCommand(name);

        /// <summary>
        /// Alias for BeginSubCommands
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IChatCommand BeginSubs(this IChatCommand cmd, params string[] name) => cmd.BeginSubCommands(name);

        /// <summary>
        /// Alias for EndSubCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static IChatCommand EndSub(this IChatCommand cmd) => cmd.EndSubCommand();
    }

    public static class CmdUtil
    {

        public delegate TextCommandResult EntityEachDelegate(Entity entity);
        public static TextCommandResult EntityEach(TextCommandCallingArgs args, EntityEachDelegate onEntity, int index = 0)
        {
            var entities = (Entity[])args.Parsers[index].GetValue();
            int successCnt = 0;

            if (entities.Length == 0)
            {
                return TextCommandResult.Error(Lang.Get("No matching player/entity found"), "nonefound");
            }

            TextCommandResult lastResult = null;
            foreach (var entity in entities)
            {
                lastResult = onEntity(entity);
                if (lastResult.Status == EnumCommandStatus.Success) successCnt++;
            }

            if (entities.Length == 1) return lastResult;

            return TextCommandResult.Success(Lang.Get("Executed commands on {0}/{1} entities", successCnt, entities.Length));
        }
    }
}   

namespace Vintagestory.API.Common
{
    public enum EnumCallerType
    {
        Player,
        Entity,
        Block,
        Console,
        Unknown
    }

    public class Caller
    {
        public EnumCallerType Type = EnumCallerType.Player;
        public string[] CallerPrivileges;
        public string CallerRole;
        public int FromChatGroupId;
        public Vec3d Pos;

        private IPlayer player;
        private Entity entity;

        public IPlayer Player {
            get {
                return player;
            }
            set
            {
                player = value;
                Entity = value.Entity;
                Type = EnumCallerType.Player;
            }
        }
        
        public Entity Entity
        {
            get
            {
                return entity;
            }
            set
            {
                entity = value;
                Pos = entity?.Pos.XYZ;
            }
        }

        public bool HasPrivilege(string privilege)
        {
            if (Player != null && Player.HasPrivilege(privilege)) return true;
            if (CallerPrivileges != null && (CallerPrivileges.Contains(privilege) || CallerPrivileges.Contains("*"))) return true;
            return false;
        }

        public IPlayerRole GetRole(ICoreServerAPI sapi)
        {
            if (Player is IServerPlayer splr) return splr.Role;
            return sapi.Permissions.GetRole(CallerRole);
        }

        public string GetName()
        {
            if (player != null) return "Player " + player.PlayerName;
            if (Type == EnumCallerType.Console) return "Console Admin";
            if (Type == EnumCallerType.Block) return "Block @" + Pos;
            if (Type == EnumCallerType.Entity) return "Entity "+entity.Code+" @" + Pos;
            return "Unknown caller";
        }
    }


    public class TextCommandCallingArgs
    {
        public string LanguageCode;
        public IChatCommand Command;
        public string SubCmdCode;
        public Caller Caller;
        public CmdArgs RawArgs;
        public List<ICommandArgumentParser> Parsers = new List<ICommandArgumentParser>();

        public int ArgCount
        {
            get
            {
                int sum = 0;
                foreach (var parser in Parsers)
                {
                    int cnt = parser.ArgCount;
                    if (cnt < 0) return -1;
                    sum += cnt;
                }
                return sum;
            }
        }

        public object this[int index]
        {
            get { return Parsers[index].GetValue(); }
        }
        public object LastArg => Parsers[Parsers.Count - 1].GetValue();
    }

    public enum EnumCommandStatus
    {
        NoSuchCommand,
        Success,
        /// <summary>
        /// Command cannot execute at this point, likely doing an async call. Prints no output. 
        /// </summary>
        Deferred,
        /// <summary>
        /// The command encountered an issue
        /// </summary>
        Error,
        /// <summary>
        /// Command status is unknown because this is a legacy command using the old method of registering commands
        /// </summary>
        UnknownLegacy
    }

    public class TextCommandResult
    {
        public string ErrorCode;
        /// <summary>
        /// Will be displayed with a Lang.Get()
        /// </summary>
        public string StatusMessage;
        public EnumCommandStatus Status;
        public object Data;
        public object[] MessageParams;

        public static TextCommandResult Success(string message = "", object data = null) => new TextCommandResult() { Status = EnumCommandStatus.Success, Data = data, StatusMessage = message };
        public static TextCommandResult Error(string message, string errorCode = "") => new TextCommandResult() { Status = EnumCommandStatus.Error, StatusMessage = message, ErrorCode = errorCode };

        public static TextCommandResult Deferred => new TextCommandResult() { Status = EnumCommandStatus.Deferred };

        public static OnCommandDelegate DeferredHandler => (args) => Deferred;
    }

    public enum EnumParseResult
    {
        Good,
        Bad,
        Deferred,
        DependsOnSubsequent
    }

    public enum EnumParseResultStatus
    {
        Loading,
        Ready,
        Error
    }
    public class AsyncParseResults
    {
        public EnumParseResultStatus Status;
        public object Data;
    }



    public interface ICommandArgumentParser {
        /// <summary>
        /// Return -1 to ignore arg count checking
        /// </summary>
        int ArgCount { get; }
        string LastErrorMessage { get; }
        string ArgumentName { get; }
        bool IsMandatoryArg { get; }
        bool IsMissing { get; set; }

        void PreProcess(TextCommandCallingArgs args);

        /// <summary>
        /// Parse the args.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="onReady">Only needs to be called when returning Deferred as parseresult</param>
        /// <returns></returns>
        EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null);
        string[] GetValidRange(CmdArgs args);
        object GetValue();
        string GetSyntax();
        string GetSyntaxExplanation(string indent);

        /// <summary>
        /// Used by the async system
        /// </summary>
        /// <param name="data"></param>
        void SetValue(object data);
    }


    public delegate TextCommandResult OnCommandDelegate(TextCommandCallingArgs args);
    public delegate TextCommandResult CommandPreconditionDelegate(TextCommandCallingArgs args);

    public interface IChatCommandApi : IEnumerable<KeyValuePair<string, IChatCommand>>, IEnumerable
    {
        IChatCommand this[string name] { get; }
        IChatCommand Create();
        IChatCommand Create(string name);

        IChatCommand Get(string name);
        IChatCommand GetOrCreate(string name);

        CommandArgumentParsers Parsers { get; }

        /// <summary>
        /// Executes a parsed command
        /// </summary>
        /// <param name="name">Name of the command without arguments, without prefix</param>
        /// <param name="args"></param>
        /// <param name="onCommandComplete">Called when the command finished executing</param>
        /// <returns></returns>
        void Execute(string name, TextCommandCallingArgs args, Action<TextCommandResult> onCommandComplete = null);

        /// <summary>
        /// Executes a raw command 
        /// </summary>
        /// <param name="message">Full command line, e.g. /entity spawn chicken-hen 1</param>
        /// <param name="args"></param>
        /// <param name="onCommandComplete">Called when the command finished executing</param>
        /// <returns></returns>
        void ExecuteUnparsed(string message, TextCommandCallingArgs args, Action<TextCommandResult> onCommandComplete = null);
        
        /// <summary>
        /// Get all commands ordered by name ASC
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Dictionary<string, IChatCommand> GetOrdered(Dictionary<string, IChatCommand> command)
        {
            return command.OrderBy(s => s.Key).ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Get all commands from <see cref="IChatCommandApi"/> ordered by name ASC
        /// </summary>
        /// <param name="chatCommandApi"></param>
        /// <returns></returns>
        static Dictionary<string, IChatCommand> GetOrdered(IChatCommandApi chatCommandApi)
        {
            return chatCommandApi.OrderBy(s => s.Key).ToDictionary(i => i.Key, i => i.Value);
        }
    }
}
