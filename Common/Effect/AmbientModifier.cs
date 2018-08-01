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
        public WeightedFloat FogMin;
        public WeightedFloat FogDensity;
        public WeightedFloatArray FogColor;
        public WeightedFloatArray AmbientColor;
        public WeightedFloat SmallCloudDensity;
        public WeightedFloat LargeCloudDensity;
        public WeightedFloat CloudBrightness;
        public WeightedFloat LerpSpeed;
        //public WeightedValue<EnumWeatherPattern> WeatherPattern;


        public static AmbientModifier DefaultAmbient
        {
            get
            {
                return new AmbientModifier()
                {
                    FogColor = WeightedFloatArray.New(new float[] { 0.725f, 0.827f, 0.929f }, 1),
                    FogDensity = WeightedFloat.New(0.0025f, 1),
                    AmbientColor = WeightedFloatArray.New(new float[] { 1, 1, 1 }, 1),
                    CloudBrightness = WeightedFloat.New(0.85f, 1),
                    LargeCloudDensity = WeightedFloat.New(-1.3f, 1),
                    SmallCloudDensity = WeightedFloat.New(-1f, 1),
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
            if (SmallCloudDensity == null) SmallCloudDensity = WeightedFloat.New(0, 0);
            if (LargeCloudDensity == null) LargeCloudDensity = WeightedFloat.New(0, 0);
            if (CloudBrightness == null) CloudBrightness = WeightedFloat.New(0, 0);
            if (LerpSpeed == null) LerpSpeed = WeightedFloat.New(0, 0);
            //if (WeatherPattern == null) WeatherPattern = new WeightedValue<EnumWeatherPattern>(EnumWeatherPattern.Gusts, 0);

            return this;
        }

        public void ToBytes(BinaryWriter wrtier)
        {
            FogMin.ToBytes(wrtier);
            FogDensity.ToBytes(wrtier);
            FogColor.ToBytes(wrtier);
            AmbientColor.ToBytes(wrtier);
            SmallCloudDensity.ToBytes(wrtier);
            LargeCloudDensity.ToBytes(wrtier);
            CloudBrightness.ToBytes(wrtier);
            LerpSpeed.ToBytes(wrtier);
            //WeatherPattern.ToBytes(wrtier);
        }

        public void FromBytes(BinaryReader reader)
        {
            FogMin.FromBytes(reader);
            FogDensity.FromBytes(reader);
            FogColor.FromBytes(reader);
            AmbientColor.FromBytes(reader);
            SmallCloudDensity.FromBytes(reader);
            LargeCloudDensity.FromBytes(reader);
            CloudBrightness.FromBytes(reader);
            LerpSpeed.FromBytes(reader);
            //WeatherPattern.FromBytes(ms);
        }
    }
}
