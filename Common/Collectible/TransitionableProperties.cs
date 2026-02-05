using System.IO;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types of transition for items.
    /// </summary>
    [DocumentAsJson]
    public enum EnumTransitionType
    {
        /// <summary>
        /// For food, animals or non-organic materials.
        /// </summary>
        Perish,

        /// <summary>
        /// Can be dried.
        /// </summary>
        Dry,

        /// <summary>
        /// Can be burned.
        /// </summary>
        Burn,

        /// <summary>
        /// Can be cured, for meat.
        /// </summary>
        Cure,

        /// <summary>
        /// Generic 'other' conversion.
        /// </summary>
        Convert,

        /// <summary>
        /// Cheese ripening.
        /// </summary>
        Ripen,

        /// <summary>
        /// Snow/ice melting.
        /// </summary>
        Melt,

        /// <summary>
        /// Glue hardening.
        /// </summary>
        Harden,

        /// <summary>
        /// Used for cooking recipes where the output has no perishableprops, but we still need a non-null TransitionableProperties  (e.g. sulfuric acid in 1.20)
        /// </summary>
        None
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

    /// <summary>
    /// This is a versatile way of allowing a collectible to change to another after a certain time in the inventory.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"transitionablePropsByType": {
	///	"*-long-raw": [
	///		{
	///			"type": "Dry",
	///			"freshHours": { "avg": 0 },
	///			"transitionHours": { "avg": 168 },
	///			"transitionedStack": {
	///				"type": "item",
	///				"code": "bowstave-long-dry"
	///			},
	///			"transitionRatio": 1
	///		}
	///	]
	///},
    /// </code>
    /// <code language="json">
    ///"transitionableProps": [
	///	{
	///		"type": "Perish",
	///		"freshHours": { "avg": 120 },
	///		"transitionHours": { "avg": 24 },
	///		"transitionedStack": {
	///			"type": "item",
	///			"code": "rot"
	///		},
	///		"transitionRatio": 0.5
	///	}
	///],
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class TransitionableProperties
    {
        /// <summary>
        /// What kind of transition can it make?
        /// </summary>
        [DocumentAsJson("Recommended", "None")]
        public EnumTransitionType Type = EnumTransitionType.None;

        /// <summary>
        /// The amount of hours before this item starts the transitioning process.
        /// </summary>
        [DocumentAsJson("Recommended", "36")]
        public NatFloat FreshHours = NatFloat.createUniform(36f, 0);

        /// <summary>
        /// The amount of hours it takes for the item to transition, after <see cref="FreshHours"/> has elapsed.
        /// </summary>
        [DocumentAsJson("Recommended", "12")]
        public NatFloat TransitionHours = NatFloat.createUniform(12f, 0);

        /// <summary>
        /// The itemstack the collectible turns into upon transitioning.
        /// </summary>
        [DocumentAsJson("Required")]
        public JsonItemStack TransitionedStack;

        /// <summary>
        /// Conversion ratio of fresh stacksize to transitioned stack size
        /// </summary>
        [DocumentAsJson("Optional", "1")]
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
