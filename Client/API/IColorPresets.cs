
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Facilitates the Accessibility tab wireframe colors setting.  Offers three preset color options, "Default", "Preset2" and "Preset3". The selection between these three options is chosen by the ClientSettings int value "guiColorsPreset".
    /// Within these presets, individual color values are stored by string key.
    /// </summary>
    public interface IColorPresets
    {
        /// <summary>
        /// Mods (e.g. VSSurvivalMod) can call this to insert into the presets their configured color keys and values, which will be specific to mod content
        /// </summary>
        /// <param name="asset"></param>
        void Initialize(Common.IAsset asset);

        /// <summary>
        /// To be called when the ClientSetting "guiColorsPreset" is changed
        /// </summary>
        void OnUpdateSetting();

        /// <summary>
        /// Called to fetch a color value from the currently selected preset colors
        /// </summary>
        int GetColor(string key);
    }
}