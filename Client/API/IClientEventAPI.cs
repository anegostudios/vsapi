using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void MouseEventDelegate(MouseEvent e);
    public delegate void KeyEventDelegate(KeyEvent e);
    public delegate void PlayerEventDelegate(IClientPlayer byPlayer);

    

    public delegate bool IsPlayerReadyDelegate(ref EnumHandling handling);

    public delegate void FileDropDelegate(FileDropEvent e);

    public delegate void IngameErrorDelegate(object sender, string errorCode, string text);
    public delegate void IngameDiscoveryDelegate(object sender, string discoveryCode, string text);

    public delegate void OnGamePauseResume(bool isPaused);
    public delegate void ChatLineDelegate(int groupId, string message, EnumChatType chattype, string data);
    public delegate void ClientChatLineDelegate(int groupId, ref string message, ref EnumHandling handled);

    /// <summary>
    /// OldBlock param may be null!
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="oldBlock"></param>
    public delegate void BlockChangedDelegate(BlockPos pos, Block oldBlock);


    /// <summary>
    /// A custom itemstack render handler. This method is called after Collectible.OnBeforeRender(). For render target gui, the gui shader and its uniforms are already fully prepared, you may only call RenderMesh() and ignore the modelMat, position and size values - stack sizes however, are not covered by this.
    /// </summary>
    /// <param name="inSlot">The slot in which the itemstack resides in</param>
    /// <param name="renderInfo">The render info for this stack, you can choose to ignore these values</param>
    /// <param name="modelMat">The model transformation matrix with position and size already preapplied, you can choose to ignore this value</param>
    /// <param name="posX">The center x-position where the stack has to be rendered</param>
    /// <param name="posY">The center y-position where the stack has to be rendered</param>
    /// <param name="posZ">The depth position. Higher values might be required for very large models, which can cause them to poke through dialogs in front of them, however</param>
    /// <param name="size">The size of the stack that has to be rendered</param>
    /// <param name="color">The requested color, usually always white</param>
    /// <param name="rotate">Whether or not to rotate it (some parts of the game have this on or off)</param>
    /// <param name="showStackSize">Whether or not to show the stack size (some parts of the game have this on or off)</param>
    public delegate void ItemRenderDelegate(ItemSlot inSlot, ItemRenderInfo renderInfo, Matrixf modelMat, double posX, double posY, double posZ, float size, int color, bool rotate = false, bool showStackSize = true);



    public interface IAsyncParticleManager
    {
        int Spawn(IParticlePropertiesProvider particleProperties);
        IBlockAccessor BlockAccess { get; }

        int ParticlesAlive(EnumParticleModel model);
    }

    /// <summary>
    /// Return false to stop spawning particles
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="manager"></param>
    /// <returns></returns>
    public delegate bool ContinousParticleSpawnTaskDelegate(float dt, IAsyncParticleManager manager);


    public class FileDropEvent
    {
        public string Filename;
        public bool Handled;
    }

    public class DummyRenderer : IRenderer
    {
        public double RenderOrder { get; set; }
        public int RenderRange { get; set; }
        public System.Action<float> action;

        public void Dispose()
        {

        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            action(deltaTime);
        }
    }

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
        event ClientChatLineDelegate OnSendChatMessage;

        /// <summary>
        /// Called when a player joins. The Entity of the player might be null if out of range!
        /// </summary>
        event PlayerEventDelegate PlayerJoin;

        /// <summary>
        /// Called whenever a player disconnects (timeout, leave, disconnect, kick, etc.). The Entity of the player might be null if out of range!
        /// </summary>
        event PlayerEventDelegate PlayerLeave;

        /// <summary>
        /// Called when the player dies
        /// </summary>
        event PlayerEventDelegate PlayerDeath;

        /// <summary>
        /// Fired when a player is ready to join but awaits any potential mod-user interaction, such as a character selection screen
        /// </summary>
        event IsPlayerReadyDelegate IsPlayerReady;

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
        /// When the player wants to leave the world to go back to the main menu
        /// </summary>
        event Action LeaveWorld;

        /// <summary>
        /// When the player left the world to go back to the main menu
        /// </summary>
        event Action LeftWorld;

        /// <summary>
        /// When a player block has been modified. OldBlock param may be null!
        /// </summary>
        event BlockChangedDelegate BlockChanged;

        /// <summary>
        /// When player tries to modify a block
        /// </summary>
        event TestBlockAccessDelegate TestBlockAccess;

        /// <summary>
        /// Fired before a player changes their active slot (such as selected hotbar slot).
        /// Allows for the event to be cancelled depending on the return value.
        /// Note: Not called when the server forcefully changes active slot.
        /// </summary>
        event Common.Func<ActiveSlotChangeEventArgs, EnumHandling> BeforeActiveSlotChanged;

        /// <summary>
        /// Fired after a player changes their active slot (such as selected hotbar slot).
        /// </summary>
        event Action<ActiveSlotChangeEventArgs> AfterActiveSlotChanged;

        /// <summary>
        /// Fired when something fires an ingame error
        /// </summary>
        event IngameErrorDelegate InGameError;

        /// <summary>
        /// Fired when something triggers a discovery event, such as the lore system
        /// </summary>
        event IngameDiscoveryDelegate InGameDiscovery;

        /// <summary>
        /// Fired when the GuiColorsPreset client setting is changed, since meshes may need to be redrawn
        /// </summary>
        event Action ColorsPresetChanged;

        /// <summary>
        /// Registers a rendering handler to be called during every render frame
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderStage"></param>
        /// <param name="profilingName">If set, the frame profile will record the frame cost for this renderer</param>
        void RegisterRenderer(IRenderer renderer, EnumRenderStage renderStage, string profilingName = null);

        /// <summary>
        /// Registers a rendering handler to be called during every render frame
        /// <br/>Additionally reserves a range of render orders which no other renderer should then use
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderStage"></param>
        /// <param name="profilingName"></param>
        /// <param name="reservedFirstOrder"></param>
        /// <param name="reservedLastOrder"></param>
        /// <param name="firstType"></param>
        void RegisterRenderer(IRenderer renderer, EnumRenderStage renderStage, string profilingName, double reservedFirstOrder, double reservedLastOrder, Type firstType);

        /// <summary>
        /// Removes a previously registered rendering handler.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderStage"></param>
        void UnregisterRenderer(IRenderer renderer, EnumRenderStage renderStage);

        /// <summary>
        /// Registers a custom itemstack renderer for given collectible object. If none is registered, the default renderer is used. For render target gui, the gui shader and its uniforms are already fully prepared, you may only call RenderMesh() and ignore the modelMat, position and size values - stack sizes however, are not covered by this.
        /// </summary>
        /// <param name="forObj"></param>
        /// <param name="rendererDelegate"></param>
        /// <param name="target"></param>
        void RegisterItemstackRenderer(CollectibleObject forObj, ItemRenderDelegate rendererDelegate, EnumItemRenderTarget target);

        /// <summary>
        /// Removes a previously registered itemstack renderer
        /// </summary>
        /// <param name="forObj"></param>
        /// <param name="target"></param>
        void UnregisterItemstackRenderer(CollectibleObject forObj, EnumItemRenderTarget target);



        /// <summary>
        /// Set up an asynchronous particle spawner. The async particle simulation does most of the work in a seperate thread and thus runs a lot faster, with the down side of not being exaclty in sync with player interactions. This method of spawning particles is best suited for ambient particles, such as rain fall.
        /// </summary>
        /// <param name="handler"></param>
        void RegisterAsyncParticleSpawner(ContinousParticleSpawnTaskDelegate handler);


        /// <summary>
        /// Fired when server assets were received and all texture atlases have been created, also all sounds loaded
        /// </summary>
        event Action BlockTexturesLoaded;

        /// <summary>
        /// Fired when the player tries to reload the shaders (happens when graphics settings are changed)
        /// </summary>
        event ActionBoolReturn ReloadShader;
        
        /// <summary>
        /// Called when textures got reloaded
        /// </summary>
        event Action ReloadTextures;

        /// <summary>
        /// Called when the client received the level finalize packet from the server
        /// </summary>
        event Action LevelFinalize;

        /// <summary>
        /// Called when shapes got reloaded
        /// </summary>
        event Action ReloadShapes;

        /// <summary>
        /// Called when the hotkeys are changed
        /// </summary>
        event Action HotkeysChanged;



        /// <summary>
        /// Provides low level access to the mouse down event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate MouseDown;
        /// <summary>
        /// Provides low level access to the mouse up event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate MouseUp;
        /// <summary>
        /// Provides low level access to the mouse move event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event MouseEventDelegate MouseMove;
        /// <summary>
        /// Provides low level access to the key down event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event KeyEventDelegate KeyDown;
        /// <summary>
        /// Provides low level access to the key up event. If e.Handled is set to true, the event will not be handled by the game
        /// </summary>
        event KeyEventDelegate KeyUp;

        /// <summary>
        /// Fired when the user drags&amp;drops a file into the game window
        /// </summary>
        event FileDropDelegate FileDrop;
    }
}
