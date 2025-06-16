
#nullable disable
namespace Vintagestory.API.Server
{
    public class Privilege
    {
       
        public static string[] AllCodes()
        {
            return new string[]
            {
                buildblocks,
                useblock,
                buildblockseverywhere,
                useblockseverywhere,
                attackplayers,
                attackcreatures,
                freemove,
                gamemode,
                pickingrange,
                chat,
                kick,
                ban,
                whitelist,
                setwelcome,
                announce,
                readlists,
                give,
                claimland,
                setspawn,
                controlserver,
                tp,
                time,
                grantrevoke,
                root,
                commandplayer,
                controlplayergroups,
                manageplayergroups,
                selfkill
            };
        }

        
        /// <summary>
        /// Place or break blocks
        /// </summary>
        public static string buildblocks = "build";

        /// <summary>
        /// Interact with block (e.g. door, chest)
        /// </summary>
        public static string useblock = "useblock";

        /// <summary>
        /// Place or break blocks everywhere, completely ignoring area permissons. You still need to be in creative mode however.
        /// </summary>
        public static string buildblockseverywhere = "buildblockseverywhere";

        /// <summary>
        /// Use blocks everywhere, completely ignoring area permissons. You still need to be in creative mode however.
        /// </summary>
        public static string useblockseverywhere = "useblockseverywhere";

        /// <summary>
        /// Cause damage to a player
        /// </summary>
        public static string attackplayers = "attackplayers";

        /// <summary>
        /// Cause damage to any non-player creature
        /// </summary>
        public static string attackcreatures = "attackcreatures";

        /// <summary>
        /// Ability to fly or change movepseed
        /// </summary>
        public static string freemove = "freemove";

        /// <summary>
        /// Ability to set own game mode
        /// </summary>
        public static string gamemode = "gamemode";

        /// <summary>
        /// Ability to set own picking range
        /// </summary>
        public static string pickingrange = "pickingrange";

        /// <summary>
        /// Ability to chat
        /// </summary>
        public static string chat = "chat";

        /// <summary>
        /// Ability to use the /kill command
        /// </summary>
        public static string selfkill = "selfkill";

        /// <summary>
        /// Ability to kick players
        /// </summary>
        public static string kick = "kick";

        /// <summary>
        /// Ability to ban/unban a player
        /// </summary>
        public static string ban = "ban";

        /// <summary>
        /// Ability to whitelist/unwhitelist a player
        /// </summary>
        public static string whitelist = "whitelist";

        /// <summary>
        /// Ability to set welcome message
        /// </summary>
        public static string setwelcome = "setwelcome";

        /// <summary>
        /// Ability to make a server wide announcement
        /// </summary>
        public static string announce = "announce";

        /// <summary>
        /// Ability to see client, group, banned user and area lists
        /// </summary>
        public static string readlists = "readlists";

        /// <summary>
        /// Ability to create block from given block id
        /// </summary>
        public static string give = "give";

        /// <summary>
        /// Ability to claim lands
        /// </summary>
        public static string claimland = "areamodify";

        /// <summary>
        /// Ability to set default spawn
        /// </summary>
        public static string setspawn = "setspawn";

        /// <summary>
        /// Ability to restart/shutdown server, reload mods, etc.
        /// </summary>
        public static string controlserver = "controlserver";

        /// <summary>
        /// Ability to teleport
        /// </summary>
        public static string tp = "tp";

        /// <summary>
        /// Ability to read, modify game world time
        /// </summary>
        public static string time = "time";

        /// <summary>
        /// Ability to set player role and ability to grant/revoke individual privileges. A player can only grant the same or a lower level group and the same or less privileges.
        /// </summary>
        public static string grantrevoke = "grantrevoke";

        /// <summary>
        /// Ability to do everything and have all permissions
        /// </summary>
        public static string root = "root";

        /// <summary>
        /// Ability to issue a command for another player (e.g. teleport another player or set another players game mode)
        /// </summary>
        public static string commandplayer = "commandplayer";

        /// <summary>
        /// Ability to join/leave/invite/op own player groups and their chat channels
        /// </summary>
        public static string controlplayergroups = "controlplayergroups";

        /// <summary>
        /// Ability to create/disband own player groups and their chat channels
        /// </summary>
        public static string manageplayergroups = "manageplayergroups";


    }
}
