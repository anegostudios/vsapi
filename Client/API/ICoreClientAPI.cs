using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The core api implemented by the client. The main interface for accessing the client. Contains all sub components and some miscellaneous methods.
    /// </summary>
    public interface ICoreClientAPI : ICoreAPI
    {
        /// <summary>
        /// Add your own link protocol here if you want to implement a custom protocol. E.g. image://url-to-picture
        /// </summary>
        Dictionary<string, Action<LinkTextComponent>> LinkProtocols { get; }

        /// <summary>
        /// Add your own rich text elements here. Your will need to convert a VTML tag into a RichTextComponentBase element. 
        /// </summary>
        Dictionary<string, Tag2RichTextDelegate> TagConverters { get; }

        /// <summary>
        /// The clients game settings as stored in the clientsettings.json
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// Platform independent ui methods and features. 
        /// </summary>
        IXPlatformInterface Forms { get; }

        /// <summary>
        /// Api to the client side macros system
        /// </summary>
        IMacroManager MacroManager { get; }

        /// <summary>
        /// Amount of milliseconds ellapsed since client startup
        /// </summary>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Amount of milliseconds ellapsed while in a running game that is not paused
        /// </summary>
        long InWorldEllapsedMilliseconds { get; }

        /// <summary>
        /// True if the client is currently in the process of exiting
        /// </summary>
        bool IsShuttingDown { get; }

        /// <summary>
        /// True if the game is currently paused (only available in singleplayer)
        /// </summary>
        bool IsGamePaused { get; }

        /// <summary>
        /// True if this is a singleplayer session
        /// </summary>
        bool IsSinglePlayer { get; }

        bool OpenedToLan { get; }

        /// <summary>
        /// If true, the player is in gui-less mode (through the F4 key)
        /// </summary>
        bool HideGuis { get; }

        /// <summary>
        /// True if all SendPlayerNowReady() was sent, signalling the player is now ready (called by the character selector upon submit)
        /// </summary>
        bool PlayerReadyFired { get; }



        /// <summary>
        /// API Component to control the clients ambient values
        /// </summary>
        IAmbientManager Ambient { get; }

        /// <summary>
        /// API Component for registering to various Events
        /// </summary>
        new IClientEventAPI Event { get; }

        /// <summary>
        /// API for Rendering stuff onto the screen using OpenGL
        /// </summary>
        IRenderAPI Render { get; }

        /// <summary>
        /// API for GUI Related methods
        /// </summary>
        IGuiAPI Gui { get; }

        /// <summary>
        /// API for Mouse / Keyboard input related things
        /// </summary>
        IInputAPI Input { get; }

        /// <summary>
        /// Holds the default meshes of all blocks
        /// </summary>
        ITesselatorManager TesselatorManager { get; }

        /// <summary>
        /// API for Meshing in the Mainthread. Thread safe.
        /// </summary>
        ITesselatorAPI Tesselator { get; }

        
        /// <summary>
        /// API for the Block Texture Atlas
        /// </summary>
        IBlockTextureAtlasAPI BlockTextureAtlas { get; }

        /// <summary>
        /// API for the Item Texture Atlas
        /// </summary>
        IItemTextureAtlasAPI ItemTextureAtlas { get; }

        /// <summary>
        /// API for the Entity Texture Atlas
        /// </summary>
        ITextureAtlasAPI EntityTextureAtlas { get; }

        /// <summary>
        /// Fetch color configs, used for accessibility e.g. for knapping wireframe gridlines
        /// </summary>
        IColorPresets ColorPreset { get; }

        /// <summary>
        /// API for Rendering stuff onto the screen using OpenGL
        /// </summary>
        IShaderAPI Shader { get; }

        /// <summary>
        /// API for doing sending/receiving network packets
        /// </summary>
        new IClientNetworkAPI Network { get; }

        /// <summary>
        /// API for accessing anything in the game world
        /// </summary>
        new IClientWorldAccessor World { get; }

        /// <summary>
        /// Active GUI objects.
        /// </summary>
        IEnumerable<object> OpenedGuis { get; }


        /// <summary>
        /// Registers a chat command
        /// </summary>
        /// <param name="chatcommand"></param>
        /// <returns></returns>
        [Obsolete("Use ChatCommand subapi instead")]
        bool RegisterCommand(ClientChatCommand chatcommand);

        /// <summary>
        /// Registers a chat command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="descriptionMsg"></param>
        /// <param name="syntaxMsg"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [Obsolete("Use ChatCommand subapi instead")]
        bool RegisterCommand(string command, string descriptionMsg, string syntaxMsg, ClientChatCommandDelegate handler);


        /// <summary>
        /// Registers an entity renderer for given entity
        /// </summary>
        /// <param name="className"></param>
        /// <param name="rendererType"></param>
        void RegisterEntityRendererClass(string className, Type rendererType);

        /// <summary>
        /// Register a link protocol handler
        /// </summary>
        /// <param name="protocolname"></param>
        /// <param name="onLinkClicked"></param>
        void RegisterLinkProtocol(string protocolname, Action<LinkTextComponent> onLinkClicked);

        /// <summary>
        /// Shows a client side only chat message in the current chat channel. Uses the same code paths a server => client message takes. Does not execute client commands.
        /// </summary>
        /// <param name="message"></param>
        void ShowChatMessage(string message);

        /// <summary>
        /// Triggers a discovery event. HudDiscoveryMessage registers to this event and fades in/out a "discovery message" on the players screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorCode"></param>
        /// <param name="text"></param>
        void TriggerIngameDiscovery(object sender, string errorCode, string text);

        /// <summary>
        /// Triggers an in-game-error event. HudIngameError registers to this event and shows a vibrating red text on the players screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorCode"></param>
        /// <param name="text"></param>
        void TriggerIngameError(object sender, string errorCode, string text);

        
        /// <summary>
        /// Same as <see cref="ShowChatMessage(string)"/> but will also execute client commands if they are prefixed with a dot.
        /// </summary>
        /// <param name="message"></param>
        void TriggerChatMessage(string message);

        /// <summary>
        /// Sends a chat message to the server
        /// </summary>
        /// <param name="message"></param>
        /// <param name="groupId"></param>
        /// <param name="data"></param>
        void SendChatMessage(string message, int groupId, string data = null);

        /// <summary>
        /// Sends a chat message to the server in the players currently active channel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        void SendChatMessage(string message, string data = null);


        /// <summary>
        /// Tells the music engine to load and immediately start given track once loaded, if the priority is higher than the currently playing track. May also be stopped while playing if another track with a higher priority is started.
        /// If you supply an onLoaded method the track is not started immediately and you can manually start it at any given time by calling sound.Start()
        /// </summary>
        /// <param name="soundLocation"></param>
        /// <param name="priority"></param>
        /// <param name="soundType"></param>
        /// <param name="onLoaded"></param>
        /// <returns></returns>
        MusicTrack StartTrack(AssetLocation soundLocation, float priority, EnumSoundType soundType, Action<ILoadedSound> onLoaded = null);

        void StartTrack(MusicTrack track, float priority, EnumSoundType soundType, bool playnow=true);


        /// <summary>
        /// Returns the currently playing music track, if any is playing
        /// </summary>
        IMusicTrack CurrentMusicTrack { get; }

        void PauseGame(bool paused);
    }
}
