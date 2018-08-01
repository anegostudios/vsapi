using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AmbientModifier
    {
        public WeightedFloat FlatFogDensity = new WeightedFloat(0, 0);
        public WeightedFloat FlatFogYPos = new WeightedFloat(0, 0);

        public WeightedFloat FogMin;
        public WeightedFloat FogDensity;
        public WeightedFloatArray FogColor;
        public WeightedFloatArray AmbientColor;
        public WeightedFloat CloudDensity;
        public WeightedFloat CloudBrightness;
        public WeightedFloat LerpSpeed;

        public AmbientModifier Clone()
        {
            return new AmbientModifier()
            {
                FlatFogDensity = FlatFogDensity.Clone(),
                FlatFogYPos = FlatFogYPos.Clone(),
                FogDensity = FogDensity.Clone(),
                FogMin = FogMin.Clone(),
                FogColor = FogColor.Clone(),
                AmbientColor = AmbientColor.Clone(),
                CloudDensity = CloudDensity.Clone(),
                CloudBrightness = CloudBrightness.Clone(),
            };
        }

        public static AmbientModifier DefaultAmbient
        {
            get
            {
                return new AmbientModifier()
                {
                    FogColor = WeightedFloatArray.New(new float[] { 0.725f, 0.827f, 0.929f }, 1),
                    FogDensity = WeightedFloat.New(0.0025f, 1),
                    AmbientColor = WeightedFloatArray.New(new float[] { 1, 1, 1 }, 1),
                    CloudBrightness = WeightedFloat.New(0.95f, 1),
                    CloudDensity = WeightedFloat.New(0, 0)
                }.EnsurePopulated();
            }
        }

        public AmbientModifier EnsurePopulated()
        {
            if (FogMin == null) FogMin = WeightedFloat.New(0, 0);
            if (FogDensity == null) FogDensity = WeightedFloat.New(0, 0);
            if (FogColor == null) FogColor = WeightedFloatArray.New(new float[] { 0,0,0 }, 0);
            if (FogColor.Value == null) FogColor.Value = new float[] { 0, 0, 0 };
            if (AmbientColor == null) AmbientColor = WeightedFloatArray.New(new float[] { 0, 0, 0 }, 0);
            if (AmbientColor.Value == null) AmbientColor.Value = new float[] { 0, 0, 0 };
            if (CloudDensity == null) CloudDensity = WeightedFloat.New(0, 0);
            if (CloudBrightness == null) CloudBrightness = WeightedFloat.New(0, 0);
            if (LerpSpeed == null) LerpSpeed = WeightedFloat.New(0, 0);

            return this;
        }

        public void ToBytes(BinaryWriter writer)
        {
            FogMin.ToBytes(writer);
            FogDensity.ToBytes(writer);
            FogColor.ToBytes(writer);
            AmbientColor.ToBytes(writer);
            CloudDensity.ToBytes(writer);
            CloudBrightness.ToBytes(writer);
            LerpSpeed.ToBytes(writer);
            FlatFogDensity.ToBytes(writer);
            FlatFogYPos.ToBytes(writer);
            //WeatherPattern.ToBytes(wrtier);
        }

        public void FromBytes(BinaryReader reader)
        {
            FogMin.FromBytes(reader);
            FogDensity.FromBytes(reader);
            FogColor.FromBytes(reader);
            AmbientColor.FromBytes(reader);
            CloudDensity.FromBytes(reader);
            CloudBrightness.FromBytes(reader);
            LerpSpeed.FromBytes(reader);
            FlatFogDensity.FromBytes(reader);
            FlatFogYPos.FromBytes(reader);
            //WeatherPattern.FromBytes(ms);
        }
    }
}
