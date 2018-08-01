using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    public class WeightedValue<T>
    {
        public T Value;
        public float Weight;

        public WeightedValue()
        {
        }

        public WeightedValue(T value, float weight)
        {
            this.Value = value;
            this.Weight = weight;
        }

        public static WeightedValue<T> New(T value, float weight)
        {
            return new WeightedValue<T>(value, weight);
        }
    }
    
    public class WeightedInt : WeightedValue<int> {
        public WeightedInt() { }

        public WeightedInt(int value, float weight)
        {
            this.Value = value;
            this.Weight = weight;
        }

        public new static WeightedInt New(int value, float weight)
        {
            return new WeightedInt(value, weight);
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Weight);
            writer.Write(Value);
        }

        public void FromBytes(BinaryReader reader)
        {
            Weight = reader.ReadSingle();
            Value = reader.ReadInt32();
        }
    }
    
    public class WeightedFloat : WeightedValue<float> {
        public WeightedFloat() { }

        public WeightedFloat(float value, float weight)
        {
            this.Value = value;
            this.Weight = weight;
        }

        public new static WeightedFloat New(float value, float weight)
        {
            return new WeightedFloat(value, weight);
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Weight);
            writer.Write(Value);
        }
        
        public void FromBytes(BinaryReader reader)
        {
            Weight = reader.ReadSingle();
            Value = reader.ReadSingle();
        }

        public WeightedFloat Clone()
        {
            return new WeightedFloat()
            {
                Weight = Weight,
                Value = Value
            };
        }
    }
    
    public class WeightedFloatArray : WeightedValue<float[]>
    {
        public WeightedFloatArray() { }

        public WeightedFloatArray(float[] value, float weight)
        {
            this.Value = value;
            this.Weight = weight;
        }

        public new static WeightedFloatArray New(float[] value, float weight)
        {
            return new WeightedFloatArray(value, weight);
        }

        public WeightedFloatArray Clone()
        {
            return new WeightedFloatArray()
            {
                Weight = Weight,
                Value = (float[])Value.Clone()
            };
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Weight);
            writer.Write(Value.Length);
            for (int i = 0; i < Value.Length; i++) writer.Write(Value[i]);
        }

        public void FromBytes(BinaryReader reader)
        {
            Weight = reader.ReadSingle();
            Value = new float[reader.ReadInt32()];
            for (int i = 0; i < Value.Length; i++) Value[i] = reader.ReadSingle();
        }
    }
}
