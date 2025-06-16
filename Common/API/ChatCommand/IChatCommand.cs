using System;
using System.Collections.Generic;
using System.Text;

#nullable disable

namespace Vintagestory.API.Common;

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
    /// Aliases for this command or subcommand
    /// </summary>
    List<string> Aliases { get; }
        
    /// <summary>
    /// RootAliases for this command or subcommand
    /// </summary>
    List<string> RootAliases { get; }
        
    /// <summary>
    /// Get the prefix of this command
    /// </summary>
    string CommandPrefix { get; }

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
    string GetFullSyntaxConsole(Caller caller);
    string GetFullSyntaxHandbook(Caller caller, string indent = "", bool isRootAlias = false);

    /// <summary>
    /// Get a string showing how to call this command or subcommand
    /// </summary>
    string CallSyntax { get; }

    string CallSyntaxUnformatted { get; }

    /// <summary>
    /// Add text listing the parameters
    /// </summary>
    void AddParameterSyntax(StringBuilder sb, string indent);
    /// <summary>
    /// Add text explaining the form and interpretation of some of the more complex parameter types
    /// </summary>
    void AddSyntaxExplanation(StringBuilder sb, string indent);
    string GetFullName(string alias, bool isRootAlias = false);
    string GetCallSyntax(string alias , bool isRootAlias = false);
    string GetCallSyntaxUnformatted(string alias , bool isRootAlias = false);
}