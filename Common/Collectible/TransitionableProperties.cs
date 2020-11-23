using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public enum EnumTransitionType
    {
        /// <summary>
        /// For food, animals or non-organic materials
        /// </summary>
        Perish,
        /// <summary>
        /// Can be dried
        /// </summary>
        Dry,
        /// <summary>
        /// Can be burned to a crisp :D
        /// </summary>
        Burn,
        Cure,
        /// <summary>
        /// something else o.O
        /// </summary>
        Convert,

        /// <summary>
        /// Cheese ripening
        /// </summary>
        Ripen
    }


    public class TransitionState
    {
        public TransitionableProperties Props;

        public float FreshHoursLeft;
        public float TransitionLevel;

        public float TransitionedHours;

        public float TransitionHours;
        public float FreshHours;
    }

    public class TransitionableProperties
    {
        /// <summary>
        /// What kind of transition can it make?
        /// </summary>
        public EnumTransitionType Type;

        /// <summary>
        /// The amount of hours this item stays fresh / untransitioned
        /// </summary>
        public NatFloat FreshHours = NatFloat.createUniform(36f, 0);

        /// <summary>
        /// The amount of hours it takes for the item to transition
        /// </summary>
        public NatFloat TransitionHours = NatFloat.createUniform(12f, 0);

        /// <summary>
        /// The itemstack the item/block turns into upon transitioning
        /// </summary>
        public JsonItemStack TransitionedStack;

        /// <summary>
        /// Conversion ratio of fresh stacksize to transitioned stack size
        /// </summary>
        public float TransitionRatio = 1;
        
        /// <summary>
        /// Duplicates the properties, which includes cloning the stack that was eaten.
        /// </summary>
        /// <returns></returns>
        public TransitionableProperties Clone()
        {
            return new TransitionableProperties()
            {
                FreshHours = FreshHours.Clone(),
                TransitionHours = TransitionHours.Clone(),
                TransitionRatio = TransitionRatio,
                TransitionedStack = TransitionedStack?.Clone(),
                Type = Type
            };
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write((ushort)Type);
            FreshHours.ToBytes(writer);
            TransitionHours.ToBytes(writer);
            TransitionedStack.ToBytes(writer);
            writer.Write(TransitionRatio);
        }

        public void FromBytes(BinaryReader reader, IClassRegistryAPI instancer)
        {
            Type = (EnumTransitionType)reader.ReadUInt16();
            FreshHours = NatFloat.createFromBytes(reader);
            TransitionHours = NatFloat.createFromBytes(reader);
            TransitionedStack = new JsonItemStack();
            TransitionedStack.FromBytes(reader, instancer);
            TransitionRatio = reader.ReadSingle();
        }

    }
}
