using System;
using Newtonsoft.Json;
using System.IO;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public delegate float TransformFunction(float firstvalue, float factor, float sequence);

    /// <summary>
    /// A number generator whose return value changes over time, parametrized by a transform function and some constants
    /// </summary>
    [DocumentAsJson]
    [JsonObject(MemberSerialization.OptIn)]
    public class EvolvingNatFloat
    {
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
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>IDENTICAL</jsondefault>-->
        /// The type of function to use as this value changes.
        /// </summary>
        [JsonProperty]
        EnumTransformFunction transform = EnumTransformFunction.IDENTICAL;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// A scale factor for the value during the transformation function.
        /// </summary>
        [JsonProperty]
        float factor = 0;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The maximum value this random value can return.
        /// </summary>
        [JsonProperty]
        float? maxvalue;

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


        EvolvingNatFloat setMax(float? value)
        {
            this.maxvalue = value;
            return this;
        }



        /// <summary>
        /// The sequence should always run from 0 to n
        /// </summary>
        /// <param name="firstvalue"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public float nextFloat(float firstvalue, float sequence)
        {
            float result = transfuncs[(int)transform](firstvalue, factor, sequence);
            if (maxvalue != null) return Math.Min((float)maxvalue, result);
            return result;
        }


        public EvolvingNatFloat Clone()
        {
            EvolvingNatFloat copy = (EvolvingNatFloat)MemberwiseClone();
            return copy;
        }

        public void FromBytes(BinaryReader reader)
        {
            transform = (EnumTransformFunction)reader.ReadByte();
            factor = reader.ReadSingle();
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)transform);
            writer.Write(factor);
        }

       
        public static EvolvingNatFloat CreateFromBytes(BinaryReader reader)
        {
            EvolvingNatFloat evo = new EvolvingNatFloat();
            evo.FromBytes(reader);
            return evo;
        }
    }
}
