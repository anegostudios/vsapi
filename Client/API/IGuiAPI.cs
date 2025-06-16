using Cairo;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public interface IGuiAPI
    {
        /// <summary>
        /// Just a default gpu-uploaded quad for 2d texture rendering, for your convenience
        /// </summary>
        MeshRef QuadMeshRef { get; }

        /// <summary>
        /// List of all registered guis
        /// </summary>
        List<GuiDialog> LoadedGuis { get; }

        /// <summary>
        /// List of all currently opened guis
        /// </summary>
        List<GuiDialog> OpenedGuis { get; }


        /// <summary>
        /// A utility class that does text texture generation for you
        /// </summary>
        TextTextureUtil TextTexture { get; }

        /// <summary>
        /// A utlity class that helps draw text
        /// </summary>
        TextDrawUtil Text { get; }

        /// <summary>
        /// A utility class that contains a bunch of hardcoded icons
        /// </summary>
        IconUtil Icons { get; }


        /// <summary>
        /// Returns a ElementBounds that is always the size of the game window
        /// </summary>
        ElementBounds WindowBounds { get; }


        /// <summary>
        /// If there is a currenly opened dialog or hud element, the method will return the bounds occuppying that area, otherwise null
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        List<ElementBounds> GetDialogBoundsInArea(EnumDialogArea area);

        /// <summary>
        /// Creates a new gui composition
        /// </summary>
        /// <param name="dialogName"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        GuiComposer CreateCompo(string dialogName, ElementBounds bounds);


        /// <summary>
        /// Register given dialog(s) to the gui/input event listening chain. You only need to call this if your dialog has to listen to events 
        /// even while closed. The method GuiDialog.TryOpen() also does the register if not registered already.
        /// </summary>
        /// <param name="dialogs"></param>
        void RegisterDialog(params GuiDialog[] dialogs);

        /// <summary>
        /// Removes given texture from graphics card memory
        /// </summary>
        /// <param name="textureid"></param>
        /// <returns></returns>
        void DeleteTexture(int textureid);


        /// <summary>
        /// Loads an external .svg file into a texture. Will return null if the file is not found
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="textureWidth"></param>
        /// <param name="textureHeight"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        LoadedTexture LoadSvg(AssetLocation loc, int textureWidth, int textureHeight, int width = 0, int height = 0, int? color = 0);


        void DrawSvg(IAsset svgAsset, ImageSurface intoSurface, int posx, int posy, int width = 0, int height = 0, int? color = 0);
        void DrawSvg(IAsset svgAsset, ImageSurface intoSurface, Matrix transform, int posx, int posy, int width = 0, int height = 0, int? color = 0);

        /// <summary>
        /// Loads an external .svg file into a texture. Will return null if the file is not found
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="textureWidth"></param>
        /// <param name="textureHeight"></param>
        /// <param name="padding"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        LoadedTexture LoadSvgWithPadding(AssetLocation loc, int textureWidth, int textureHeight, int padding = 0, int? color = 0);

        /// <summary>
        /// Load the contents of a cairo surface into a opengl texture. Returns the texture id
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="linearMag"></param>
        /// <returns></returns>
        int LoadCairoTexture(Cairo.ImageSurface surface, bool linearMag);

        /// <summary>
        /// Load the contents of a cairo surface into a opengl texture. Re-uses the supplied texture exists and the size is correct. Otherwise deletes the texture and regenerates it.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="linearMag"></param>
        /// <param name="intoTexture"></param>
        /// <returns></returns>
        void LoadOrUpdateCairoTexture(Cairo.ImageSurface surface, bool linearMag, ref LoadedTexture intoTexture);


        /// <summary>
        /// Retrieve the saved dialog position from settings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Vec2i GetDialogPosition(string key);

        /// <summary>
        /// Remember the dialog position for given dialog key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pos"></param>
        void SetDialogPosition(string key, Vec2i pos);

        /// <summary>
        /// Plays a sound, non location specific
        /// </summary>
        /// <param name="soundname">The name of the sound</param>
        /// <param name="randomizePitch">If true, the pitch is slightly randomized each time</param>
        /// <param name="volume"></param>
        void PlaySound(string soundname, bool randomizePitch = false, float volume = 1f);

        /// <summary>
        /// Plays a sound, non location specific.
        /// </summary>
        /// <param name="soundname">The name of the sound</param>
        /// <param name="randomizePitch">If true, the pitch is slightly randomized each time</param>
        /// <param name="volume"></param>
        void PlaySound(AssetLocation soundname, bool randomizePitch = false, float volume = 1f);

        /// <summary>
        /// Requests the given GUI to be given focus.
        /// </summary>
        /// <param name="guiDialog">The dialogue wanting attention.</param>
        void RequestFocus(GuiDialog guiDialog);

        /// <summary>
        /// Triggers the opening of a dialogue.  
        /// </summary>
        /// <param name="guiDialog">The dialogue to be opened.</param>
        void TriggerDialogOpened(GuiDialog guiDialog);

        /// <summary>
        /// Triggers the closing of a dialogue.
        /// </summary>
        /// <param name="guiDialog">The dialogue to be closed.</param>
        void TriggerDialogClosed(GuiDialog guiDialog);

        /// <summary>
        /// Opens up a confirm dialog asking the player if he wants to open an external link
        /// </summary>
        /// <param name="href"></param>
        void OpenLink(string href);
    }
}
