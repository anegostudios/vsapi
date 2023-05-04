using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;


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
        string GetSyntaxExplanation();

        /// <summary>
        /// Used by the async system
        /// </summary>
        /// <param name="data"></param>
        void SetValue(object data);
    }


    public delegate TextCommandResult OnCommandDelegate(TextCommandCallingArgs args);
    public delegate TextCommandResult CommandPreconditionDelegate(TextCommandCallingArgs args);

    public interface IChatCommand
    {
        /// <summary>
        /// Name of this command plus parent command names
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Name of this command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the description of this command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the detailed description of this command
        /// </summary>
        string AdditionalInformation { get; }

        /// <summary>
        /// Get the examples of this command
        /// </summary>
        string[] Examples { get; }

        /// <summary>
        /// True if either name or privilege has not been set
        /// </summary>
        bool Incomplete { get; }

        /// <summary>
        /// Retrieve subcommand
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand this[string name] { get; }
        /// <summary>
        /// If return value is error, command cannot be executed
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        IChatCommand WithPreCondition(CommandPreconditionDelegate p);
        /// <summary>
        /// Sets the command name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand WithName(string name);
        /// <summary>
        /// Registers alternative names for this command
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand WithAlias(params string[] name);
        /// <summary>
        /// Registers an alternative name for this command, always at the root level, i.e. /name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand WithRootAlias(string name);
        /// <summary>
        /// Set command description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IChatCommand WithDescription(string description);
        /// <summary>
        /// Set additional detailed command description, for command-specific help
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        IChatCommand WithAdditionalInformation(string detail);
        /// <summary>
        /// Define one ore more examples on how this command can be executed
        /// </summary>
        /// <param name="examaples"></param>
        /// <returns></returns>
        IChatCommand WithExamples(params string[] examaples);
        /// <summary>
        /// Define command arguments, you'd usually want to use one of the parsers supplied from from capi.ChatCommands.Parsers
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IChatCommand WithArgs(params ICommandArgumentParser[] args);
        /// <summary>
        /// Define the required privilege to run this command / subcommand
        /// </summary>
        /// <param name="privilege"></param>
        /// <returns></returns>
        IChatCommand RequiresPrivilege(string privilege);

        /// <summary>
        /// This command can only be run if the caller is a player
        /// </summary>
        /// <returns></returns>
        IChatCommand RequiresPlayer();

        
        /// <summary>
        /// Define/Modify a subcommnad. Returns a new subcommand instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand BeginSubCommand(string name);

        /// <summary>
        /// Define/Modify multiple subcommands. Returns a new subcommand instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IChatCommand BeginSubCommands(params string[] name);

        /// <summary>
        /// Leave current subcommand. Returns parent command instance.
        /// </summary>
        /// <returns></returns>
        IChatCommand EndSubCommand();
        /// <summary>
        /// Define method to be called when the command is executed
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IChatCommand HandleWith(OnCommandDelegate handler);
        /// <summary>
        /// Manually execute this command
        /// </summary>
        /// <param name="callargs"></param>
        /// <param name="onCommandComplete"></param>
        void Execute(TextCommandCallingArgs callargs, Action<TextCommandResult> onCommandComplete = null);
        /// <summary>
        /// Confirm whether the specified caller has the required privilege for this command
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        bool IsAvailableTo(Caller caller);
        /// <summary>
        /// Optional validation step that ensures that all the command and all its subcommands has a name, handler, privilege and description defined
        /// </summary>
        void Validate();
        IChatCommand IgnoreAdditionalArgs();

        IEnumerable<IChatCommand> Subcommands { get; }
        Dictionary<string, IChatCommand> AllSubcommands { get; }

        /// <summary>
        /// Get a string showing how to call this command or subcommand
        /// </summary>
        string MethodSyntax(string name);

        /// <summary>
        /// Add text listing the parameters
        /// </summary>
        void AddParameterSyntax(StringBuilder sb);
        /// <summary>
        /// Add text explaining the form and interpretation of some of the more complex parameter types
        /// </summary>
        void AddSyntaxExplanation(StringBuilder sb);
    }

    public interface IChatCommandApi
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
    }
}
