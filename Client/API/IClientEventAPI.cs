using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public delegate void MouseEventDelegate(MouseEvent e);
    public delegate void KeyEventDelegate(KeyEvent e);
    public delegate void PlayerEventDelegate(IClientPlayer byPlayer);
    

    public delegate void OnGamePauseResume(bool isPaused);
    public delegate void ChatLineDelegate(int groupId, string message, EnumChatType chattype, string data);
    public delegate void ClientChatLineDelegate(int groupId, ref string message);

    /// <summary>
    /// Contains some client specific events you can hook into
    /// </summary>
    public interface IClientEventAPI : IEventAPI
    {
        /// <summary>
        /// Called when a chat message was received
        /// </summary>
        event ChatLineDelegate ChatMessage;

        /// <summary>
        /// Called before a chat message is sent to the server
        /// </summary>
        //event ClientChatLineDelegate OnSendChatMessage;

        /// <summary>
        /// Called when a player joins. The Entity of the player might be null if out of range!
        /// </summary>
        event PlayerEventDelegate PlayerJoin;

        /// <summary>
        /// Called whenever a player disconnects (timeout, leave, disconnect, kick, etc.). The Entity of the player might be null if out of range!
        /// </summary>
        event PlayerEventDelegate PlayerLeave;


        /// <summary>
        /// Called when a players entity got in range
        /// </summary>
        event PlayerEventDelegate PlayerEntitySpawn;

        /// <summary>
        /// Called whenever a players got out of range
        /// </summary>
        event PlayerEventDelegate PlayerEntityDespawn;


        /// <summary>
        /// When the game was paused/resumed (only in single player)
        /// </summary>
        event OnGamePauseResume PauseResume;

        /// <summary>
        /// When the player leaves the world to go back to the main menu
        /// </summary>
        event API.Common.Action LeaveWorld;

        /// <summary>
        /// When a player block has been modified
        /// </summary>
        event Common.Action<BlockPos> BlockChanged;

        /// <summary>
        /// When the player changed his active hotbar slot
        /// </summary>
        event Common.Action ActiveSlotChanged;

        /// <summary>
        /// Registers a rendering handler to be called during every render frame
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderStage"></param>
        /// <param name="profilingName">If set, the frame profile will record the frame cost for this renderer</param>
        void RegisterRenderer(IRenderer renderer, EnumRenderStage renderStage, string profilingName = null);

        /// <summary>
        /// Removes a previously registered rendering handler.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderStage"></param>
        void UnregisterRenderer(IRenderer renderer, EnumRenderStage renderStage);

        /// <summary>
        /// Called when server assetes were received and all texture atlases have been created
        /// </summary>
        /// <param name="handler"></param>
        event Common.Action BlockTexturesLoaded;
        
        /// <summary>
        /// Fired when the player tries to reload the shaders (happens when graphics settings are changed)
        event ActionBoolReturn ReloadShader;
        
        /// <summary>
        /// Called when textures got reloaded
        /// </summary>
        event Common.Action ReloadTextures;

        /// <summary>
        /// Called when the client received the level finalize packet from the server
        /// </summary>
        event Common.Action LevelFinalize;

        /// <summary>
        /// Called when shapes got reloaded
        /// </summary>
        event Common.Action ReloadShapes;




        /// <summary>
        /// Provides low level access to the mouse down event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate OnMouseDown;
        /// <summary>
        /// Provides low level access to the mouse up event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate OnMouseUp;
        /// <summary>
        /// Provides low level access to the mouse move event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate OnMouseMove;
        /// <summary>
        /// Provides low level access to the key down event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event KeyEventDelegate OnKeyDown;
        /// <summary>
        /// Provides low level access to the key up event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event KeyEventDelegate OnKeyUp;
    }
}
