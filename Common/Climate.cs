using System;

#nullable disable

namespace Vintagestory.API.Common;

public class Climate
{
    /// <summary>
    /// This value is update once the config is loaded and the Sealevel is known
    /// </summary>
    public static int Sealevel = 110;

    /// <summary>
    /// Temperature conversion factor used to get real temperatures from the climate's temperature int
    /// Used to convert from real temperature range float [-50 , 40] to int [0 , 255]
    /// </summary>
    public static float TemperatureScaleConversion = 255f / 60f;

    /// <summary>
    /// Convert from real temperature range float [-50 , 40] to int [0 , 255]
    /// </summary>
    /// <param name="temperature"></param>
    /// <returns></returns>
    public static int DescaleTemperature(float temperature)
    {
        return Math.Clamp((int)((temperature + 20) * TemperatureScaleConversion), 0, 255);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="rainfall"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int GetRainFall(int rainfall, int y)
    {
        return Math.Clamp(rainfall + (y - Sealevel) / 2 + 5 * Math.Clamp(8 + Sealevel - y, 0, 8), 0, 255);
    }

    /// <summary>
    /// Convert from int [0 , 255] range to real temperature range float [-20 , 40]
    /// The distToSealevel/1.5f is also hardcoded in shaderincluds/colormap.vsh
    /// </summary>
    /// <param name="unscaledTemp"></param>
    /// <param name="distToSealevel"></param>
    /// <returns></returns>
    public static int GetScaledAdjustedTemperature(int unscaledTemp, int distToSealevel)
    {
        return Math.Clamp((int)((unscaledTemp - distToSealevel / 1.5f) / TemperatureScaleConversion) - 20, -20, 40);
    }

    /// <summary>
    /// Convert from int [0 , 255] range to real temperature range float [-20 , 40]
    /// The distToSealevel/1.5f is also hardcoded in shaderincluds/colormap.vsh
    /// </summary>
    /// <param name="unscaledTemp"></param>
    /// <param name="distToSealevel"></param>
    /// <returns></returns>
    public static float GetScaledAdjustedTemperatureFloat(int unscaledTemp, int distToSealevel)
    {
        return Math.Clamp((unscaledTemp - distToSealevel / 1.5f) / TemperatureScaleConversion - 20, -20, 40);
    }

    /// <summary>
    /// Convert from int [0 , 255] range to real temperature range float [-50 , 40]
    /// The distToSealevel/1.5f is also hardcoded in shaderincluds/colormap.vsh
    ///
    /// This exists since the client had a different value for min
    /// </summary>
    /// <param name="unscaledTemp"></param>
    /// <param name="distToSealevel"></param>
    /// <returns></returns>
    public static float GetScaledAdjustedTemperatureFloatClient(int unscaledTemp, int distToSealevel)
    {
        return Math.Clamp((unscaledTemp - distToSealevel / 1.5f) / TemperatureScaleConversion - 20, -50, 40);
    }

    /// <summary>
    /// Convert from int [0 , 255] range to real temperature range int [-20 , 40]
    /// The distToSealevel/1.5f is also hardcoded in shaderincluds/colormap.vsh
    /// </summary>
    /// <param name="unscaledTemp"></param>
    /// <param name="distToSealevel"></param>
    /// <returns></returns>
    public static int GetAdjustedTemperature(int unscaledTemp, int distToSealevel)
    {
        return (int)Math.Clamp(unscaledTemp - distToSealevel / 1.5f, 0, 255);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="rain"></param>
    /// <param name="scaledTemp"></param>
    /// <param name="posYRel"></param>
    /// <returns></returns>
    public static int GetFertility(int rain, float scaledTemp, float posYRel)
    {
        float f = Math.Min(255, rain / 2f + Math.Max(0, rain * DescaleTemperature(scaledTemp) / 512f));

        // Reduce fertility by up to 100 in mountains, but only if fertility is above 100 already
        float weight = 1 - Math.Max(0, (80 - f) / 80);

        return (int)Math.Max(0, f - Math.Max(0, 50 * (posYRel - 0.5f)) * weight);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="rain"></param>
    /// <param name="unscaledTemp"></param>
    /// <param name="posYRel"></param>
    /// <returns></returns>
    public static int GetFertilityFromUnscaledTemp(int rain, int unscaledTemp, float posYRel)
    {
        float f = Math.Min(255, rain / 2f + Math.Max(0, rain * unscaledTemp / 512f));

        // Reduce fertility by up to 100 in mountains, but only if fertility is above 100 already
        float weight = 1 - Math.Max(0, (80 - f) / 80);

        return (int)Math.Max(0, f - Math.Max(0, 50 * (posYRel - 0.5f)) * weight);
    }
}
