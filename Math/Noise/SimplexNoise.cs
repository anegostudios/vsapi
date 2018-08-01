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


        public virtual double Noise(double x, double y)
        {
            double value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                value += octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * amplitudes[i];
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
