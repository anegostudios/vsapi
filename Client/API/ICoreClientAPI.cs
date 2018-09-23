using System;
using System.Collections.Generic;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The core api implemented by the client. The main interface for accessing the client. Contains all sub components and some miscellaneous methods.
    /// </summary>
    public interface ICoreClientAPI : ICoreAPI
    {
        /// <summary>
        /// The settings instance.
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// The local Logger instance.
        /// </summary>
        ILogger Logger { get; }

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
        /// Shows a client side only chat message in the current chat channel
        /// </summary>
        /// <param name="message"></param>
        void ShowChatNotification(string message);

        /// <summary>
        /// Shows a message to the client as if the client typed it, but without sending it to the server. This also causes client commands to get executed.
        /// </summary>
        /// <param name="message"></param>
        void SendMessageToClient(string message);

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="message"></param>
        /// <param name="groupId"></param>
        /// <param name="data"></param>
        void SendMessageToServer(string message, int groupId, string data = null);

        /// <summary>
        /// Sends a message to the server in the players currently active channel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        void SendMessageToServer(string message, string data = null);

        
    }
}
