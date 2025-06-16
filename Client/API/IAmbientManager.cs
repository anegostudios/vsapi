using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// This is the system that manages the worlds ambient settings, such as fog color/density and clouds.
    /// The ambient manager basically blends ambientmodifiers on top of each other to generate the final ambient values.
    /// Blending is in order how the modifiers are held in CurrentModifiers in the likes of
    /// float weight = modifier.FogMin.Weight;
    /// BlendedFogMin = w * modifier.FogMin.Value + (1 - w) * BlendedFogMin;
    /// </summary>
    public interface IAmbientManager
    {
        /// <summary>
        /// The base value or background ambient to overlay everything onto
        /// </summary>
        AmbientModifier Base { get; }

        /// <summary>
        /// The list of modifiers that result in the blended values
        /// </summary>
        OrderedDictionary<string, AmbientModifier> CurrentModifiers { get; }


        /// <summary>
        /// The blended fog color, calculated every frame from the list of modifiers
        /// </summary>
        Vec4f BlendedFogColor { get; }

        /// <summary>
        /// The blended ambient color, calculated every frame from the list of modifiers
        /// </summary>
        Vec3f BlendedAmbientColor { get; }

        /// <summary>
        /// The blended fog density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFogDensity { get; }

        float BlendedFogBrightness { get; }

        /// <summary>
        /// The blended flat fog density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFlatFogDensity { get; set; }

        /// <summary>
        /// The blended flat fog y-offset, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFlatFogYOffset { get; set; }

        /// <summary>
        /// BlendedFlatFogYPos + SeaLevel - MainCamera.TargetPosition.Y
        /// </summary>
        float BlendedFlatFogYPosForShader { get; set; }

        /// <summary>
        /// The blended fog min, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFogMin { get; }
        /// <summary>
        /// The blended cloud brightness, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedCloudBrightness { get; }
        /// <summary>
        /// The blended cloud density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedCloudDensity { get; }
        float BlendedSceneBrightness { get; }

        /// <summary>
        /// The update loop for this manager. Runs every frame.
        /// </summary>
        /// <param name="dt">the Delta or change in Time</param>
        void UpdateAmbient(float dt);
    }
}
