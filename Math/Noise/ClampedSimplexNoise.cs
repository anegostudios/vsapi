
#nullable disable
namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Perlin noise of supplied amplitude and frequency. The resulting value is clamped to 0...1
    /// </summary>
    public class ClampedSimplexNoise
    {
        public SimplexNoiseOctave[] octaves;

        public double[] amplitudes;
        public double[] frequencies;
        long seed;

        public ClampedSimplexNoise(double[] amplitudes, double[] frequencies, long seed)
        {
            this.amplitudes = amplitudes;
            this.frequencies = frequencies;
            this.seed = seed;

            octaves = new SimplexNoiseOctave[amplitudes.Length];

            for (int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = new SimplexNoiseOctave(seed * 65599 + i);
            }

        }


        public virtual double Noise(double x, double y, double offset = 0)
        {
            double value = 1;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                value += octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * amplitudes[i];
            }

            return GameMath.Clamp(value / 2 + offset, 0, 1);
        }


        public ClampedSimplexNoise Clone()
        {
            return new ClampedSimplexNoise((double[])amplitudes.Clone(), (double[])frequencies.Clone(), seed);
        }
    }
}
