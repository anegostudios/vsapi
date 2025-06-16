using System.IO;
using Vintagestory.API.MathTools;

#nullable disable

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
        public WeightedFloat SceneBrightness;

        public WeightedFloat FogBrightness;

        public void SetLerped(AmbientModifier left, AmbientModifier right, float w)
        {
            FlatFogDensity.SetLerped(left.FlatFogDensity, right.FlatFogDensity, w);
            FlatFogYPos.SetLerped(left.FlatFogYPos, right.FlatFogYPos, w);
            FogMin.SetLerped(left.FogMin, right.FogMin, w);
            FogDensity.SetLerped(left.FogDensity, right.FogDensity, w);

            FogColor.SetLerped(left.FogColor, right.FogColor, w);
            AmbientColor.SetLerped(left.AmbientColor, right.AmbientColor, w);

            CloudDensity.SetLerped(left.CloudDensity, right.CloudDensity, w);
            CloudBrightness.SetLerped(left.CloudBrightness, right.CloudBrightness, w);
            LerpSpeed.SetLerped(left.LerpSpeed, right.LerpSpeed, w);
            SceneBrightness.SetLerped(left.SceneBrightness, right.SceneBrightness, w);
            FogBrightness.SetLerped(left.FogBrightness, right.FogBrightness, w);
        }



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
                SceneBrightness = SceneBrightness.Clone(),
                FogBrightness = FogBrightness.Clone()
            };
        }

        public static AmbientModifier DefaultAmbient
        {
            get
            {
                return new AmbientModifier()
                {
                    FogColor = WeightedFloatArray.New(new float[] { 201/255f, 211/255f, 219/255f }, 1),
                    FogDensity = WeightedFloat.New(0.00125f, 1),
                    AmbientColor = WeightedFloatArray.New(new float[] { 1, 1, 1 }, 1),
                    CloudBrightness = WeightedFloat.New(1, 1),
                    CloudDensity = WeightedFloat.New(0, 0),
                    SceneBrightness = WeightedFloat.New(1, 0),
                    FogBrightness = WeightedFloat.New(1, 0)
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
            if (SceneBrightness == null) SceneBrightness = WeightedFloat.New(1, 0);
            if (FogBrightness == null) FogBrightness = WeightedFloat.New(1, 0);

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
            SceneBrightness.ToBytes(writer);
            FogBrightness.ToBytes(writer);
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
            SceneBrightness.FromBytes(reader);
            FogBrightness.FromBytes(reader);
        }
    }
}
