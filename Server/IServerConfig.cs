using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Server
{
    public enum EnumWhitelistMode
    {
        /// <summary>
        /// Singleplayer OpenToLan: All players can join, Dedicated server: Only whitelisted players can join
        /// </summary>
        Default,
        /// <summary>
        /// All players can join
        /// </summary>
        Off,
        /// <summary>
        /// Only whitelisted players can join
        /// </summary>
        On
    }

    /// <summary>
    /// The servers configuration
    /// </summary>
    public interface IServerConfig
    {
        /// <summary>
        /// The current network port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Name of the server, currently not used
        /// </summary>
        string ServerName { get; set; }

        /// <summary>
        /// Displays when the user logs in.
        /// </summary>
        string WelcomeMessage { get; set; }
        /// <summary>
        /// Max amount of concurrent players, any beyond will be denied to join
        /// If the cli argument --maxclients is set then the value provided by the cli arg will be returned instead of the one from the config
        /// </summary>
        int MaxClients { get; set; }
        /// <summary>
        /// Password the player has to supply to join
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// How many chunks in each direction should be loaded at most for each player (1 chunk = 32 blocks)
        /// </summary>
        int MaxChunkRadius { get; set; }
        /// <summary>
        /// Of often the server should tick in milliesconds
        /// </summary>
        float TickTime { get; set; }
        /// <summary>
        /// Horizontal distance in chunks from each player to tick blocks randomly
        /// </summary>
        int BlockTickChunkRange { get; set; }

        /// <summary>
        /// The maximum number of blocks to tick per server tick
        /// </summary>
        int MaxMainThreadBlockTicks { get; set; }

        /// <summary>
        /// The number of blocks to sample for ticks each pass within a single chunk
        /// </summary>
        int RandomBlockTicksPerChunk { get; set; }

        /// <summary>
        /// The interval of time in ms between each execution of the random tick system
        /// </summary>
        int BlockTickInterval { get; set; }

        /// <summary>
        /// List of player roles
        /// </summary>
        List<IPlayerRole> Roles { get; }

        /// <summary>
        /// Default player role
        /// </summary>
        string DefaultRoleCode { get; set; }

        /// <summary>
        /// AntiAbuse protection level. Use not recommended, it is very buggy at the moment
        /// </summary>
        EnumProtectionLevel AntiAbuse { get; set; }

        /// <summary>
        /// If true, only whitelisted players can join
        /// </summary>
        EnumWhitelistMode WhitelistMode { get; set; }

        /// <summary>
        /// Default spawn position for players
        /// </summary>
        [Obsolete("No longer used. Retrieve value from the savegame instead")]
        PlayerSpawnPos DefaultSpawn { get; set; }

        /// <summary>
        /// Whether or not to allow Player versus Player
        /// </summary>
        bool AllowPvP { get; set; }

        /// <summary>
        /// Whether or not fire should spread
        /// </summary>
        bool AllowFireSpread { get; set; }

        /// <summary>
        /// Whether or not falling blocks should fall (e.g. sand and gravel) and floating single rock blocks should break
        /// </summary>
        bool AllowFallingBlocks { get; set; }

        /// <summary>
        /// Used to disable certain features in HostedMode (wgen commands, ...)
        /// </summary>
        bool HostedMode { get; set; }

        /// <summary>
        /// Used to enable/disable the /moddb commands in HostedMode only
        /// </summary>
        bool HostedModeAllowMods { get; set; }

        float SpawnCapPlayerScaling { get; set; }

        /// <summary>
        /// Whether or not to log breaking and placing of blocks to the server-build.log
        /// </summary>
        public bool LogBlockBreakPlace { get; set; }

        /// <summary>
        /// After how many line we should split the log file into a new one (server-build.log -> server-build-2.log)
        /// </summary>
        public uint LogFileSplitAfterLine { get; set; }
    }
}
