using Newtonsoft.Json;
using System.Runtime.Serialization;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Defines a set of sounds for a collectible object.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"heldSoundsbyType": {
	///	"*-lit-*": {
	///		"idle": "held/torch-idle",
	///		"equip": "held/torch-equip",
	///		"unequip": "held/torch-unequip",
	///		"attack": "held/torch-attack"
	///	}
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class HeldSounds
    {
        /// <summary>
        /// The path to a sound played when this item is being held.
        /// </summary>
        [DocumentAsJson("Optional", "None"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes Idle = new SoundAttributes();

        /// <summary>
        /// The path to a sound played when this item is equipped.
        /// </summary>
        [DocumentAsJson("Optional", "None"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes Equip = new SoundAttributes();

        /// <summary>
        /// The path to a sound played when this item is unequipped.
        /// </summary>
        [DocumentAsJson("Optional", "None"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes Unequip = new SoundAttributes();

        /// <summary>
        /// The path to a sound played when this item is used to attack.
        /// </summary>
        [DocumentAsJson("Optional", "None"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes Attack = new SoundAttributes();

        /// <summary>
        /// The path to a sound played when this item is picked up in the inventory using the mouse.
        /// </summary>
        [DocumentAsJson("Optional", "player/clayformhi"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes InvPickup = new SoundAttributes();

        /// <summary>
        /// The path to a sound played when this item is placed in the inventory using the mouse.
        /// </summary>
        [DocumentAsJson("Optional", "player/clayform"), JsonConverter(typeof(SoundAttributeConverter), false)]
        public SoundAttributes InvPlace = new SoundAttributes();

        public static SoundAttributes InvPickUpDefault = new SoundAttributes(new AssetLocation("sounds/player/clayformhi"), false);
        public static SoundAttributes InvPlaceDefault = new SoundAttributes(new AssetLocation("sounds/player/clayform"), false);
        public static AssetLocation ToolBreak = new AssetLocation("sounds/effect/toolbreak");
        public static NatFloat SemirandomPitchDefault = new NatFloat(1f, 0.1f, EnumDistribution.UNIFORM);

        public HeldSounds()
        {
            InvPickup = InvPickUpDefault;
            InvPlace = InvPlaceDefault;
            Attack.Pitch = SemirandomPitchDefault;
            Equip.Pitch = SemirandomPitchDefault;
            Unequip.Pitch = SemirandomPitchDefault;
            Idle.Pitch = SemirandomPitchDefault;
        }

        /// <summary>
        /// Clones the held sounds.
        /// </summary>
        /// <returns></returns>
        public HeldSounds Clone()
        {
            HeldSounds sounds = new HeldSounds()
            {
                Idle = Idle.Clone(),
                Equip = Equip.Clone(),
                Unequip = Unequip.Clone(),
                Attack = Attack.Clone(),
                InvPickup = InvPickup.Clone(),
                InvPlace = InvPlace.Clone()
            };

            return sounds;
        }
    }
}
