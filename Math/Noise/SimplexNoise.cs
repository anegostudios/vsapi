using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Perlin noise of supplied amplitude and frequency. 
    /// </summary>
    public class SimplexNoise
    {
        public SimplexNoiseOctave[] octaves;

        public double[] amplitudes;
        public double[] frequencies;
        long seed;

        public SimplexNoise(double[] amplitudes, double[] frequencies, long seed)
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
        

        /// <summary>
        /// Generates the octaves and frequencies using following formulas 
        /// freq[i] = baseFrequency * 2^i
        /// amp[i] = persistence^i
        /// </summary>
        /// <param name="quantityOctaves"></param>
        /// <param name="baseFrequency"></param>
        /// <param name="persistence"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static SimplexNoise FromDefaultOctaves(int quantityOctaves, double baseFrequency, double persistence, long seed)
        {
            double[] frequencies = new double[quantityOctaves];
            double[] amplitudes = new double[quantityOctaves];

            for (int i = 0; i < quantityOctaves; i++)
            {
                frequencies[i] = Math.Pow(2, i) * baseFrequency;
                amplitudes[i] = Math.Pow(persistence, i);
            }

            return new SimplexNoise(amplitudes, frequencies, seed);
        }


        public virtual double Noise(double x, double y)
        {
            double value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                value += octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * amplitudes[i];
            }

            return value;
        }


        public double Noise(double x, double y, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double val = octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * amplitudes[i];
                value += (val > 0 ? Math.Max(0, val - thresholds[i]) : Math.Min(0, val + thresholds[i]));
            }

            return value;
        }




        public virtual double Noise(double x, double y, double z)
        {
            double value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                value += octaves[i].Evaluate(x * frequencies[i], y * frequencies[i], z * frequencies[i]) * amplitudes[i];
            }

            return value;
        }




        public SimplexNoise Clone()
        {
            return new SimplexNoise((double[])amplitudes.Clone(), (double[])frequencies.Clone(), seed);
        }
    }
}
