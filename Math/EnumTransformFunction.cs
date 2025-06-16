
#nullable disable
namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Used for EvolvingNatFloat value transforms
    /// </summary>
    [DocumentAsJson]
    public enum EnumTransformFunction
    {
        /// <summary>
        /// y = firstval
        /// </summary>
        IDENTICAL,
        /// <summary>
        /// y = firstval + factor * seq
        /// </summary>
        LINEAR,
        /// <summary>
        /// y = factor > 0 ? Math.Min(0, firstval + factor * seq) : Math.Max(0, firstval + factor * seq)
        /// </summary>
        LINEARNULLIFY,
        /// <summary>
        /// firstval - firstval / Math.Abs(firstval) * factor * seq
        /// </summary>
        LINEARREDUCE,
        /// <summary>
        /// firstval + firstval / Math.Abs(firstval) * factor * seq
        /// </summary>
        LINEARINCREASE,
        /// <summary>
        /// firstval + Math.Sign(factor) * (factor * seq) * (factor * seq)
        /// </summary>
        QUADRATIC,
        /// <summary>
        /// firstval + 1f / (1f + factor * seq)
        /// </summary>
        INVERSELINEAR,
        /// <summary>
        /// firstval + (float)Math.Sqrt(factor * seq)
        /// </summary>
        ROOT,
        /// <summary>
        /// firstval + GameMath.FastSin(factor * seq)
        /// </summary>
        SINUS,
        /// <summary>
        /// firstval * GameMath.Min(5 * Math.Abs(GameMath.FastSin(factor * seq)), 1)
        /// </summary>
        CLAMPEDPOSITIVESINUS,
        /// <summary>
        /// firstval + GameMath.FastCos(factor * seq)
        /// </summary>
        COSINUS,
        /// <summary>
        /// firstval + GameMath.SmoothStep(factor * seq)
        /// </summary>
        SMOOTHSTEP
    }
}
