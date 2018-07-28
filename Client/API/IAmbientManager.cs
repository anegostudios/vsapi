using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API
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
        /// The blended fog density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFogDensity { get; }
        /// <summary>
        /// The blended fog min, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedFogMin { get; }
        /// <summary>
        /// The blended cloud brightness, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedCloudBrightness { get; }
        /// <summary>
        /// The blended large cloud density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedLargeCloudDensity { get; }
        /// <summary>
        /// The blended small cloud density, calculated every frame from the list of modifiers
        /// </summary>
        float BlendedSmallCloudDensity { get; }
    }
}
