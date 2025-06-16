using System;
using System.Runtime.CompilerServices;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// It's generally pretty hard to get a neatly normalized coherent noise function due to the way perlin/open simplex works (gauss curve) and how random numbers are generated. So instead of trying to find the perfect normalization factor and instead try to perform some approximate normalization this class allows a small overflow and brings it down very close to the [0, 1] range using tanh().
    /// 
    /// Returns values in a range of [0..1]
    /// </summary>
    public class NormalizedSimplexNoise
    {
        private const double VALUE_MULTIPLIER = 1.2;

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
                value += VALUE_MULTIPLIER * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * scaledAmplitudes2D[i];
            }

            return NoiseValueCurve(value);
        }

        public double Noise(double x, double y, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes2D.Length; i++)
            {
                double val = octaves[i].Evaluate(x * frequencies[i], y * frequencies[i]) * scaledAmplitudes2D[i];
                value += VALUE_MULTIPLIER * (val > 0 ? Math.Max(0, val - thresholds[i]) : Math.Min(0, val + thresholds[i]));
            }

            return NoiseValueCurve(value);
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
                value += VALUE_MULTIPLIER * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i], z * frequencies[i]) * scaledAmplitudes3D[i];
            }

            return NoiseValueCurve(value);
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
                value += VALUE_MULTIPLIER * octaves[i].Evaluate(x * frequencies[i], y * frequencies[i], z * frequencies[i]) * amplitudes[i];
            }

            return NoiseValueCurve(value);
        }

        public double Noise(double x, double y, double z, double[] amplitudes, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes3D.Length; i++)
            {
                double freq = frequencies[i];
                
                // This looks nice on the bumplands landform
                //double val = octaves[i].Evaluate(x * freq + y / 4.0, y * freq, z * freq + y / 4.0) * amplitudes[i];

                double val = octaves[i].Evaluate(x * freq, y * freq, z * freq) * amplitudes[i];
                value += 1.2 * ApplyThresholding(val, thresholds[i]);
            }

            return NoiseValueCurve(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NoiseValueCurve(double value)
        {
            return Math.Tanh(value) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NoiseValueCurveInverse(double value) {
            if (value <= 0.0) return double.NegativeInfinity;
            if (value >= 1.0) return double.PositiveInfinity;
            return 0.5 * Math.Log(value / (1.0 - value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ApplyThresholding(double value, double threshold)
        {
            return value > 0 ?
                Math.Max(0, value - threshold) :
                Math.Min(0, value + threshold);
        }

        public ColumnNoise ForColumn(double relativeYFrequency, double[] amplitudes, double[] thresholds, double noiseX, double noiseZ)
        {
            return new ColumnNoise(this, relativeYFrequency, amplitudes, thresholds, noiseX, noiseZ);
        }

        public struct ColumnNoise
        {
            OctaveEntry[] orderedOctaveEntries;

            public double UncurvedBound { get; private set; }
            public double BoundMin { get; private set; }
            public double BoundMax { get; private set; }

            struct OctaveEntry
            {
                public SimplexNoiseOctave Octave;
                public double X, FrequencyY, Z, Amplitude, Threshold, StopBound;
            }

            public ColumnNoise(NormalizedSimplexNoise terrainNoise, double relativeYFrequency, double[] amplitudes, double[] thresholds, double noiseX, double noiseZ)
            {
                int nAvailableOctaves = terrainNoise.frequencies.Length;
                int nUsedOctaves = 0;
                double[] maxValues = new double[nAvailableOctaves];
                int[] order = new int[nAvailableOctaves];
                double bound = 0;
                for (int i = nAvailableOctaves - 1; i >= 0; i--)
                {
                    // The actual maximum value, factoring in multiplier and threshold. 
                    maxValues[i] = Math.Max(0, Math.Abs(amplitudes[i]) - thresholds[i]) * (SimplexNoiseOctave.MAX_VALUE_3D * VALUE_MULTIPLIER);
                    bound += maxValues[i];

                    // Don't generate the octave if the max value is zero.
                    if (maxValues[i] == 0) continue;

                    // Descending order: Biggest octaves first, so we can rule out layers sooner.
                    order[nUsedOctaves] = i;
                    for (int j = nUsedOctaves - 1; j >= 0; j--)
                    {
                        if (maxValues[order[j + 1]] > maxValues[order[j]])
                        {
                            int temp = order[j];
                            order[j] = order[j + 1];
                            order[j + 1] = temp;
                        }
                    }
                    nUsedOctaves++;
                }
                this.UncurvedBound = bound;
                this.BoundMin = NoiseValueCurve(-bound);
                this.BoundMax = NoiseValueCurve(bound);

                // Fill out noise generators in order
                this.orderedOctaveEntries = new OctaveEntry[nUsedOctaves];
                double uncertaintySum = 0;
                for (int j = nUsedOctaves - 1; j >= 0; j--)
                {
                    int i = order[j];
                    uncertaintySum += maxValues[i];
                    double thisOctaveFrequency = terrainNoise.frequencies[i];
                    orderedOctaveEntries[j] = new OctaveEntry
                    {
                        Octave = terrainNoise.octaves[i],
                        X = noiseX * thisOctaveFrequency,
                        Z = noiseZ * thisOctaveFrequency,
                        FrequencyY = thisOctaveFrequency * relativeYFrequency,
                        Amplitude = amplitudes[i] * VALUE_MULTIPLIER,
                        Threshold = thresholds[i] * VALUE_MULTIPLIER,
                        StopBound = uncertaintySum
                    };
                }
            }

            // You don't always need to evaluate all the octaves to know the sign of the result.
            public double NoiseSign(double y, double inverseCurvedThresholder)
            {
                double value = inverseCurvedThresholder;
                for (int j = 0; j < orderedOctaveEntries.Length; j++)
                {
                    ref readonly OctaveEntry octaveEntry = ref orderedOctaveEntries[j];

                    // Stop if no further noise calculation is necessary to know the sign of the result.
                    if (value >= octaveEntry.StopBound || value <= -octaveEntry.StopBound) break;

                    // Multiplication by VALUE_MULTIPLIER is baked into .Amplitude and .Threshold
                    double noiseValue = octaveEntry.Octave.Evaluate(octaveEntry.X, y * octaveEntry.FrequencyY, octaveEntry.Z) * octaveEntry.Amplitude;
                    value += ApplyThresholding(noiseValue, octaveEntry.Threshold);
                }
                return value;
            }

            // But if you need the full noise value, you can use this!
            public double Noise(double y) {
                double value = 0.0;
                for (int j = 0; j < orderedOctaveEntries.Length; j++) {
                    ref readonly OctaveEntry octaveEntry = ref orderedOctaveEntries[j];

                    // Multiplication by VALUE_MULTIPLIER is baked into .Amplitude and .Threshold
                    double noiseValue = octaveEntry.Octave.Evaluate(octaveEntry.X, y * octaveEntry.FrequencyY, octaveEntry.Z) * octaveEntry.Amplitude;
                    value += ApplyThresholding(noiseValue, octaveEntry.Threshold);
                }
                return NoiseValueCurve(value);
            }
        }
    }
}
