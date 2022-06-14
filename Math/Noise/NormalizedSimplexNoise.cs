using System;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// It's generally pretty hard to get a neatly normalized coherent noise function due to the way perlin/open simplex works (gauss curve) and how random numbers are generated. So instead of trying to find the perfect normalization factor and instead try to perform some approximate normalization this class allows a small overflow and brings it down very close to the [0, 1] range using tanh().
    /// 
    /// Returns values in a range of [0..1]
    /// </summary>
    public class NormalizedSimplexNoise
    {
        public double[] scaledAmplitudes2D;
        public double[] scaledAmplitudes3D;

        public double[] inputAmplitudes;
        public double[] frequencies;

        public SimplexNoiseOctave[] octaves;


        public NormalizedSimplexNoise(double[] inputAmplitudes, double[] frequencies, long seed)
        {
            this.frequencies = frequencies;
            this.inputAmplitudes = inputAmplitudes;

            octaves = new SimplexNoiseOctave[inputAmplitudes.Length];
            for (int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = new SimplexNoiseOctave(seed * 65599 + i);
            }

            CalculateAmplitudes(inputAmplitudes);
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
        public static NormalizedSimplexNoise FromDefaultOctaves(int quantityOctaves, double baseFrequency, double persistence, long seed)
        {            
            double[] frequencies = new double[quantityOctaves];
            double[] amplitudes = new double[quantityOctaves];

            for (int i = 0; i < quantityOctaves; i++)
            {
                frequencies[i] = Math.Pow(2, i) * baseFrequency;
                amplitudes[i] = Math.Pow(persistence, i);
            }

            return new NormalizedSimplexNoise(amplitudes, frequencies, seed);
        }



        internal virtual void CalculateAmplitudes(double []inputAmplitudes)
        {
            double normalizationValue3D = 0;
            double normalizationValue2D = 0;

            for (int i = 0; i < inputAmplitudes.Length; i++)
            {
                normalizationValue3D += inputAmplitudes[i] * Math.Pow(0.74 - 0.1, i + 1);
                normalizationValue2D += inputAmplitudes[i] * Math.Pow(0.83 - 0.1, i + 1);
            }


            scaledAmplitudes2D = new double[inputAmplitudes.Length];
            for (int i = 0; i < inputAmplitudes.Length; i++)
            {
                scaledAmplitudes2D[i] = inputAmplitudes[i] / normalizationValue2D;
            }


            scaledAmplitudes3D = new double[inputAmplitudes.Length];
            for (int i = 0; i < inputAmplitudes.Length; i++)
            {
                scaledAmplitudes3D[i] = inputAmplitudes[i] / normalizationValue3D;
            }
        }


        /// <summary>
        /// 2d noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual double Noise(double x, double y)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes2D.Length; i++)
            {
                value += 1.2 * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * scaledAmplitudes2D[i];
            }

            return Math.Tanh(value) / 2 + 0.5;
        }

        public double Noise(double x, double y, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes2D.Length; i++)
            {
                double val = octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * scaledAmplitudes2D[i];
                value += 1.2 * (val > 0 ? Math.Max(0, val - thresholds[i]) : Math.Min(0, val + thresholds[i]));
            }

            return Math.Tanh(value) / 2 + 0.5;
        }


        /// <summary>
        /// 3d noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public virtual double Noise(double x, double y, double z)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes3D.Length; i++)
            {
                value += 1.2 * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i], z * frequencies[i]) * scaledAmplitudes3D[i];
            }

            return Math.Tanh(value) / 2 + 0.5;
        }



        /// <summary>
        /// 3d Noise using custom amplitudes
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="amplitudes"></param>
        /// <returns></returns>
        public virtual double Noise(double x, double y, double z, double[] amplitudes)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes3D.Length; i++)
            {
                value += 1.2 * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i], z * frequencies[i]) * amplitudes[i];
            }

            return Math.Tanh(value) / 2 + 0.5;
        }

        public double Noise(double x, double y, double z, double[] amplitudes, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes3D.Length; i++)
            {
                double freq = frequencies[i];
                double val = octaves[i].Evaluate(x * freq, y * freq, z * freq) * amplitudes[i];
                value += 1.2 * (val > 0 ? Math.Max(0, val - thresholds[i]) : Math.Min(0, val + thresholds[i]));
            }

            return Math.Tanh(value) / 2 + 0.5;
        }
    }
}
