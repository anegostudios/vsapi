using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// OpenSimplex noise of supplied amplitude and frequency. 
    /// </summary>
    public class NewSimplexFractalNoise
    {
        long[] octaveSeeds;
        float[] amplitudes;
        double[] frequencies;
        long seed;

        public NewSimplexFractalNoise(float[] amplitudes, double[] frequencies, long seed)
        {
            this.amplitudes = amplitudes;
            this.frequencies = frequencies;
            this.seed = seed;

            octaveSeeds = new long[amplitudes.Length];

            for (int i = 0; i < octaveSeeds.Length; i++)
            {
                octaveSeeds[i] = seed * 65599 + i;
            }
        }

        public NewSimplexFractalNoise(double[] amplitudes, double[] frequencies, long seed) :
            this(Array.ConvertAll(amplitudes, value => (float)value), frequencies, seed)
        { }


        /// <summary>
        /// Generates the amplitudes and frequencies using following formulas 
        /// freq[i] = baseFrequency * 2^i
        /// amp[i] = persistence^i
        /// </summary>
        /// <param name="quantityOctaves"></param>
        /// <param name="baseFrequency"></param>
        /// <param name="persistence"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static NewSimplexFractalNoise FromDefaultOctaves(int quantityOctaves, double baseFrequency, double persistence, long seed)
        {
            double[] frequencies = new double[quantityOctaves];
            double[] amplitudes = new double[quantityOctaves];

            for (int i = 0; i < quantityOctaves; i++)
            {
                frequencies[i] = Math.Pow(2, i) * baseFrequency;
                amplitudes[i] = Math.Pow(persistence, i);
            }

            return new NewSimplexFractalNoise(amplitudes, frequencies, seed);
        }


        public virtual float Noise(double x, double y)
        {
            float value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double f = frequencies[i];
                value += NewSimplexNoiseLayer.Evaluate(octaveSeeds[i], x * f, y * f) * amplitudes[i];
            }

            return value;
        }


        public void VectorValuedNoise(double x, double y, out float distX, out float distY)
        {
            distX = distY = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double f = frequencies[i];
                NewSimplexNoiseLayer.VectorEvaluate(octaveSeeds[i], x * f, y * f, out float distXHere, out float distYHere);
                distX += distXHere * amplitudes[i];
                distY += distYHere * amplitudes[i];
            }
        }


        public float Noise(double x, double y, float[] thresholds)
        {
            float value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double f = frequencies[i];
                float val = NewSimplexNoiseLayer.Evaluate(octaveSeeds[i], x * f, y * f) * amplitudes[i];
                value += val > 0.0f ? Math.Max(0.0f, val - thresholds[i]) : Math.Min(0.0f, val + thresholds[i]);
            }

            return value;
        }

        public float Noise(double x, double y, double[] thresholds)
        {
            float value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double f = frequencies[i];
                float val = NewSimplexNoiseLayer.Evaluate(octaveSeeds[i], x * f, y * f) * amplitudes[i];
                value += val > 0.0f ? Math.Max(0.0f, val - (float)thresholds[i]) : Math.Min(0.0f, val + (float)thresholds[i]);
            }

            return value;
        }


        public double NoiseWithThreshold(double x, double y, double threshold)
        {
            double value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double offset = threshold * amplitudes[i];
                double val = NewSimplexNoiseLayer.Evaluate(octaveSeeds[i], x * frequencies[i], y * frequencies[i]) * amplitudes[i];
                value += val > 0 ? Math.Max(0, val - offset) : Math.Min(0, val + offset);
            }

            return value;
        }


        public virtual double Noise(double x, double y, double z)
        {
            float value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                double frequency = frequencies[i];
                value += NewSimplexNoiseLayer.Evaluate_ImprovedXZ(octaveSeeds[i], x * frequency, y * frequency, z * frequency) * amplitudes[i];
            }

            return value;
        }

        public virtual float AbsNoise(double x, double y, double z)
        {
            float value = 0;

            for (int i = 0; i < amplitudes.Length; i++)
            {
                value += Math.Abs(NewSimplexNoiseLayer.Evaluate_ImprovedXZ(octaveSeeds[i], x * frequencies[i], y * frequencies[i], z * frequencies[i]) * amplitudes[i]);
            }

            return value;
        }



        public NewSimplexFractalNoise Clone()
        {
            return new NewSimplexFractalNoise((double[])amplitudes.Clone(), (double[])frequencies.Clone(), seed);
        }
    }
}
