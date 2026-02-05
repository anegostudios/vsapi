using System;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public delegate float TransformFunction(float firstvalue, float factor, float sequence);

    /// <summary>
    /// A number generator whose return value changes over time, parametrized by a transform function and some constants
    /// </summary>
    [DocumentAsJson]
    [JsonObject(MemberSerialization.OptIn)]
    public struct EvolvingNatFloat
    {
        public static readonly EvolvingNatFloat NoValueSet = new EvolvingNatFloat(EnumTransformFunction.UNSPECIFIED, 0f);

        static TransformFunction[] transfuncs;

        static EvolvingNatFloat()
        {
            transfuncs = new TransformFunction[20];

            transfuncs[(int)EnumTransformFunction.IDENTICAL] = (firstval, factor, seq) => { return firstval; };

            transfuncs[(int)EnumTransformFunction.LINEAR] = (firstval, factor, seq) => { return firstval + factor * seq; };

            transfuncs[(int)EnumTransformFunction.INVERSELINEAR] = (firstval, factor, seq) => { return firstval + 1f / (1f + factor * seq); };

            transfuncs[(int)EnumTransformFunction.LINEARNULLIFY] = (firstval, factor, seq) => {
                return
                    factor > 0 ?
                    Math.Min(0, firstval + factor * seq) :
                    Math.Max(0, firstval + factor * seq)
                ;
            };

            transfuncs[(int)EnumTransformFunction.LINEARREDUCE] = (firstval, factor, seq) => { return firstval - firstval / Math.Abs(firstval) * factor * seq; };

            transfuncs[(int)EnumTransformFunction.LINEARINCREASE] = (firstval, factor, seq) => { return firstval + firstval / Math.Abs(firstval) * factor * seq; };

            transfuncs[(int)EnumTransformFunction.QUADRATIC] = (firstval, factor, seq) => { return firstval + Math.Sign(factor) * (factor * seq) * (factor * seq); };

            transfuncs[(int)EnumTransformFunction.ROOT] = (firstval, factor, seq) => { return firstval + (float)Math.Sqrt(factor * seq); };

            transfuncs[(int)EnumTransformFunction.SINUS] = (firstval, factor, seq) => { return firstval + GameMath.FastSin(factor * seq); };

            transfuncs[(int)EnumTransformFunction.CLAMPEDPOSITIVESINUS] = (firstval, factor, seq) => { return firstval * GameMath.Min(5 * Math.Abs(GameMath.FastSin(factor * seq)), 1); };

            transfuncs[(int)EnumTransformFunction.COSINUS] = (firstval, factor, seq) => { return firstval + GameMath.FastCos(factor * seq); };

            transfuncs[(int)EnumTransformFunction.SMOOTHSTEP] = (firstval, factor, seq) => { return firstval + GameMath.SmoothStep(factor * seq); };
        }

        /// <summary>
        /// The type of function to use as this value changes.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "IDENTICAL")]
        EnumTransformFunction transform = EnumTransformFunction.IDENTICAL;

        /// <summary>
        /// A scale factor for the value during the transformation function.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Recommended", "0")]
        float factor = 0;

        /// <summary>
        /// The maximum value this random value can return.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        float? maxvalue = null;

        public float Factor
        {
            get { return factor; }
        }

        public float? MaxValue
        {
            get { return maxvalue; }
        }

        public EnumTransformFunction Transform
        {
            get { return transform; }
        }

        public EvolvingNatFloat()
        {
        }

        public EvolvingNatFloat(EnumTransformFunction transform, float factor)
        {
            this.transform = transform;
            this.factor = factor;
        }

        public static EvolvingNatFloat createIdentical(float factor)
        {
            return new EvolvingNatFloat(EnumTransformFunction.IDENTICAL, factor);
        }

        public static EvolvingNatFloat create(EnumTransformFunction function, float factor)
        {
            return new EvolvingNatFloat(function, factor);
        }



        /// <summary>
        /// The sequence should always run from 0 to n
        /// </summary>
        /// <param name="firstvalue"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public float nextFloat(float firstvalue, float sequence)
        {
            if (transform == EnumTransformFunction.UNSPECIFIED) return 0f;
            float result = transfuncs[(int)transform](firstvalue, factor, sequence);
            if (maxvalue != null) return Math.Min(maxvalue.Value, result);
            return result;
        }


        public EvolvingNatFloat Clone()
        {
            return new EvolvingNatFloat()
            {
                factor = factor,
                maxvalue = maxvalue,
                transform = transform
            };
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)transform);
            writer.Write(factor);
        }


        public static EvolvingNatFloat CreateFromBytes(BinaryReader reader)
        {
            EvolvingNatFloat evo = new EvolvingNatFloat()
            {
                transform = (EnumTransformFunction)reader.ReadByte(),
                factor = reader.ReadSingle()
            };
            return evo;
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is not EvolvingNatFloat enf) return false;
            return enf.Transform == Transform && enf.factor == factor;
        }

        public static bool operator ==(EvolvingNatFloat left, EvolvingNatFloat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EvolvingNatFloat left, EvolvingNatFloat right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return factor.GetHashCode() + (int)Transform * 269023;
        }
    }
}
