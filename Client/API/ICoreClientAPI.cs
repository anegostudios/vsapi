using System;
using System.Collections.Generic;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using static Vintagestory.API.Common.VtmlUtil;

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
        Dictionary<string, API.Common.Action<LinkTextComponent>> LinkProtocols { get; }

        /// <summary>
        /// Add your own rich text elements here. Your will need to convert a VTML tag into a RichTextComponentBase element. 
        /// </summary>
        Dictionary<string, Tag2RichTextDelegate> TagConverters { get; }

        /// <summary>
        /// The settings instance.
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// The local Logger instance.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Platform independent ui methods and features. 
        /// </summary>
        IXPlatformInterface Forms { get; }

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
        /// Loads the rgb (plant or water) tint value at given position and multiplies it byte-wise with supplied color
        /// </summary>
        /// <param name="tintIndex"></param>
        /// <param name="color"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int ApplyColorTintOnRgba(int tintIndex, int color, int posX, int posY, int posZ, bool flipRb = true);

        /// <summary>
        /// Loads the rgb (plant or water) tint value for given rain and temp value and multiplies it byte-wise with supplied color
        /// </summary>
        /// <param name="tintIndex"></param>
        /// <param name="color"></param>
        /// <param name="rain"></param>
        /// <param name="temp"></param>
        /// <param name="flipRb"></param>
        /// <returns></returns>
        int ApplyColorTintOnRgba(int tintIndex, int color, int rain, int temp, bool flipRb = true);

        /// <summary>
        /// True if the game is currently paused (only available in singleplayer)
        /// </summary>
        bool IsGamePaused { get; }

        /// <summary>
        /// If true, the player is in gui-less mode (through the F4 key)
        /// </summary>
        bool HideGuis { get; }

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
        /// API for Meshing in the Mainthread
        /// </summary>
        ITesselatorAPI Tesselator { get; }

        /// <summary>
        /// API for Meshing in a background thread. This getter returns you a new, thread safe instance of the tesselator system, so if you have to tesselate a lot, just retrieve it once
        /// </summary>
        ITesselatorAPI TesselatorThreadSafe { get; }

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
        /// API for Rendering stuff onto the screen using OpenGL
        /// </summary>
        IShaderAPI Shader { get; }

        /// <summary>
        /// API for doing sending/receiving network packets
        /// </summary>
        IClientNetworkAPI Network { get; }

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
        bool RegisterCommand(ClientChatCommand chatcommand);

        /// <summary>
        /// Registers a chat command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="descriptionMsg"></param>
        /// <param name="syntaxMsg"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
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
        void RegisterLinkProtocol(string protocolname, Common.Action<LinkTextComponent> onLinkClicked);

        /// <summary>
        /// Shows a client side only chat message in the current chat channel. Uses the same code paths a server => client message takes. Does not execute client commands.
        /// </summary>
        /// <param name="message"></param>
        void ShowChatMessage(string message);
        
        /// <summary>
        /// Shows a client side only chat message in the current chat channel. Uses the same code paths a server => client message takes. Does not execute client commands.
        /// </summary>
        /// <param name="message"></param>
        [Obsolete("Use ShowChatMessage() instead")]
        void ShowChatNotification(string message);

        /// <summary>
        /// Shows a vibrating red text in the players screen
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
        MusicTrack StartTrack(AssetLocation soundLocation, float priority, EnumSoundType soundType, API.Common.Action<ILoadedSound> onLoaded = null);

        

        /// <summary>
        /// Returns the currently playing music track, if any is playing
        /// </summary>
        IMusicTrack CurrentMusicTrack { get; }
    }
}
