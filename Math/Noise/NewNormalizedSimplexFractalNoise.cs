using System;
using System.Runtime.CompilerServices;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class NewNormalizedSimplexFractalNoise
    {
        private const double ValueMultiplier = 1.2 * SimplexNoiseOctave.MAX_VALUE_3D;
        private const double ThresholdRescaleOldToNew = 1.0 / SimplexNoiseOctave.MAX_VALUE_3D;
        private const double AmpAndFreqToThresholdSmoothing = 3.5;

        public double[] scaledAmplitudes2D;
        public double[] scaledAmplitudes3D;

        public double[] inputAmplitudes;
        public double[] frequencies;

        public long[] octaveSeeds;


        public NewNormalizedSimplexFractalNoise(double[] inputAmplitudes, double[] frequencies, long seed)
        {
            this.frequencies = frequencies;
            this.inputAmplitudes = inputAmplitudes;

            octaveSeeds = new long[inputAmplitudes.Length];
            for (int i = 0; i < octaveSeeds.Length; i++)
            {
                octaveSeeds[i] = seed * 65599 + i;
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
        public static NewNormalizedSimplexFractalNoise FromDefaultOctaves(int quantityOctaves, double baseFrequency, double persistence, long seed)
        {
            double[] frequencies = new double[quantityOctaves];
            double[] amplitudes = new double[quantityOctaves];

            for (int i = 0; i < quantityOctaves; i++)
            {
                frequencies[i] = Math.Pow(2, i) * baseFrequency;
                amplitudes[i] = Math.Pow(persistence, i);
            }

            return new NewNormalizedSimplexFractalNoise(amplitudes, frequencies, seed);
        }

        // TODO deduce meaning of constants
        internal virtual void CalculateAmplitudes(double[] inputAmplitudes)
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
        public double Noise(double x, double y, double z, double[] amplitudes, double[] thresholds)
        {
            double value = 0;

            for (int i = 0; i < scaledAmplitudes3D.Length; i++)
            {
                double freq = frequencies[i];

                double val = NewSimplexNoiseLayer.Evaluate_ImprovedXZ(octaveSeeds[i], x * freq, y * freq, z * freq) * amplitudes[i];
                double smoothingFactor = amplitudes[i] * freq * AmpAndFreqToThresholdSmoothing;
                value += ValueMultiplier * ApplyThresholding(val, thresholds[i] * ThresholdRescaleOldToNew, smoothingFactor);
            }

            return NoiseValueCurve(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NoiseValueCurve(double value)
        {
            return value / Math.Sqrt(1 + value * value) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NoiseValueCurveInverse(double value)
        {
            if (value <= 0.0) return double.NegativeInfinity;
            if (value >= 1.0) return double.PositiveInfinity;
            value = (value * 2.0 - 1.0);
            return value / Math.Sqrt(1 - value * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ApplyThresholding(double value, double threshold, double smoothingFactor)
        {
            return GameMath.SmoothMax(0, value - threshold, smoothingFactor) + GameMath.SmoothMin(0, value + threshold, smoothingFactor);
        }

        public ColumnNoise ForColumn(double relativeYFrequency, double[] amplitudes, double[] thresholds, double noiseX, double noiseZ)
        {
            return new ColumnNoise(this, relativeYFrequency, amplitudes, thresholds, noiseX, noiseZ);
        }

        public struct ColumnNoise
        {
            // Thread-isolated static arrays (TLS).
            // Size 48 covers any possible world height.
            [ThreadStatic] private static OctaveEntry[] _threadOctaves;
            [ThreadStatic] private static PastEvaluation[] _threadPastEvals;
            [ThreadStatic] private static double[] _threadMaxValues;
            [ThreadStatic] private static int[] _threadOrder;

            // References to TLS arrays for quick access within methods
            private OctaveEntry[] orderedOctaveEntries;
            private PastEvaluation[] pastEvaluations;

            // Strict limitation for loops since arrays can be longer
            private int nUsedOctaves;

            public double UncurvedBound { get; private set; }
            public double BoundMin { get; private set; }
            public double BoundMax { get; private set; }

            struct OctaveEntry
            {
                public long Seed;
                public double X, FrequencyY, Z, Amplitude, Threshold, SmoothingFactor, StopBound;
            }

            struct PastEvaluation
            {
                public double Value, Y;
            }

            public ColumnNoise(NewNormalizedSimplexFractalNoise terrainNoise, double relativeYFrequency, double[] amplitudes, double[] thresholds, double noiseX, double noiseZ)
            {
                int nAvailableOctaves = terrainNoise.frequencies.Length;

                // Initialize or expand TLS arrays on first call in a thread
                if (_threadOctaves == null || _threadOctaves.Length < nAvailableOctaves)
                {
                    int newSize = Math.Max(48, nAvailableOctaves);
                    _threadOctaves = new OctaveEntry[newSize];
                    _threadPastEvals = new PastEvaluation[newSize];
                    _threadMaxValues = new double[newSize];
                    _threadOrder = new int[newSize];
                }

                // Binding local links to TLS arrays
                double[] maxValues = _threadMaxValues;
                int[] order = _threadOrder;

                int nUsedOctavesLocal = 0;
                double bound = 0;
                for (int i = nAvailableOctaves - 1; i >= 0; i--)
                {
                    // The actual maximum value, factoring in multiplier and threshold. 
                    maxValues[i] = Math.Max(0, Math.Abs(amplitudes[i]) - thresholds[i]) * ValueMultiplier; // TODO verify is same with smooth max/min
                    bound += maxValues[i];

                    // Don't generate the octave if the max value is zero.
                    if (maxValues[i] == 0) continue;

                    // Descending order: Biggest octaves first, so we can rule out layers sooner.
                    order[nUsedOctavesLocal] = i;
                    for (int j = nUsedOctavesLocal - 1; j >= 0; j--)
                    {
                        if (maxValues[order[j + 1]] > maxValues[order[j]])
                        {
                            int temp = order[j];
                            order[j] = order[j + 1];
                            order[j + 1] = temp;
                        }
                    }
                    nUsedOctavesLocal++;
                }

                this.UncurvedBound = bound;
                this.BoundMin = NoiseValueCurve(-bound);
                this.BoundMax = NoiseValueCurve(bound);
                this.nUsedOctaves = nUsedOctavesLocal;

                // reuse _threadOctaves and _threadPastEvals
                this.orderedOctaveEntries = _threadOctaves;
                this.pastEvaluations = _threadPastEvals;

                double uncertaintySum = 0;
                for (int j = nUsedOctavesLocal - 1; j >= 0; j--)
                {
                    int i = order[j];
                    uncertaintySum += maxValues[i];
                    double thisOctaveFrequency = terrainNoise.frequencies[i];
                    orderedOctaveEntries[j] = new OctaveEntry
                    {
                        Seed = terrainNoise.octaveSeeds[i],
                        X = noiseX * thisOctaveFrequency,
                        Z = noiseZ * thisOctaveFrequency,
                        FrequencyY = thisOctaveFrequency * relativeYFrequency,
                        Amplitude = amplitudes[i] * ValueMultiplier,
                        Threshold = thresholds[i] * (ValueMultiplier * ThresholdRescaleOldToNew),
                        SmoothingFactor = amplitudes[i] * thisOctaveFrequency * AmpAndFreqToThresholdSmoothing,
                        StopBound = uncertaintySum
                    };
                    pastEvaluations[j] = new PastEvaluation
                    {
                        Y = double.NaN
                    };
                }
            }

            // You don't always need to evaluate all the octaves to know the sign of the result.
            public double NoiseSign(double y, double inverseCurvedThresholder)
            {
                double value = inverseCurvedThresholder;

                const double maxYSlope = NewSimplexNoiseLayer.MaxYSlope_ImprovedXZ;
                double valueTempMin = inverseCurvedThresholder;
                double valueTempMax = inverseCurvedThresholder;

                // Iterate strictly up to nUsedOctaves
                for (int j = 0; j < nUsedOctaves; j++)
                {
                    // Exit if we couldn't possibly trigger an early return within this loop.
                    if (!(valueTempMax <= 0) && !(valueTempMin >= 0)) break;

                    ref readonly OctaveEntry octaveEntry = ref orderedOctaveEntries[j];

                    // Stop if no further noise calculation is necessary to know the sign of the result.
                    if (valueTempMin >= octaveEntry.StopBound) return valueTempMin;
                    if (valueTempMax <= -octaveEntry.StopBound) return valueTempMax;

                    // Extrapolate most-recently calculated noise values for each octave.
                    double evalY = y * octaveEntry.FrequencyY;
                    double deltaY = Math.Abs(pastEvaluations[j].Y - evalY);
                    valueTempMin += ApplyThresholding(Math.Max(-1, pastEvaluations[j].Value - deltaY * maxYSlope) * octaveEntry.Amplitude, octaveEntry.Threshold, octaveEntry.SmoothingFactor);
                    valueTempMax += ApplyThresholding(Math.Min(1, pastEvaluations[j].Value + deltaY * maxYSlope) * octaveEntry.Amplitude, octaveEntry.Threshold, octaveEntry.SmoothingFactor);
                }

                for (int j = 0; j < nUsedOctaves; j++)
                {
                    ref readonly OctaveEntry octaveEntry = ref orderedOctaveEntries[j];

                    // Stop if no further noise calculation is necessary to know the sign of the result.
                    if (value >= octaveEntry.StopBound || value <= -octaveEntry.StopBound) break;

                    // Multiplication by ValueMultiplier is baked into .Amplitude and .Threshold
                    double evalY = y * octaveEntry.FrequencyY;
                    double noiseValue = NewSimplexNoiseLayer.Evaluate_ImprovedXZ(octaveEntry.Seed, octaveEntry.X, evalY, octaveEntry.Z);
                    pastEvaluations[j].Value = noiseValue;
                    pastEvaluations[j].Y = evalY;
                    value += ApplyThresholding(noiseValue * octaveEntry.Amplitude, octaveEntry.Threshold, octaveEntry.SmoothingFactor);
                }

                return value;
            }

            // But if you need the full noise value, you can use this!
            public double Noise(double y)
            {
                double value = 0.0;

                // Iterate strictly up to nUsedOctaves
                for (int j = 0; j < nUsedOctaves; j++)
                {
                    ref readonly OctaveEntry octaveEntry = ref orderedOctaveEntries[j];

                    // Multiplication by VALUE_MULTIPLIER is baked into .Amplitude and .Threshold
                    double noiseValue = NewSimplexNoiseLayer.Evaluate_ImprovedXZ(octaveEntry.Seed, octaveEntry.X, y * octaveEntry.FrequencyY, octaveEntry.Z) * octaveEntry.Amplitude;
                    value += ApplyThresholding(noiseValue, octaveEntry.Threshold, octaveEntry.SmoothingFactor);
                }
                return NoiseValueCurve(value);
            }


        }
    }
}
