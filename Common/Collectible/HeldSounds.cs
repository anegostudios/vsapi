using System.Runtime.Serialization;

#nullable disable

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
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The path to a sound played when this item is being held.
        /// </summary>
        [DocumentAsJson] public AssetLocation Idle = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The path to a sound played when this item is equipped.
        /// </summary>
        [DocumentAsJson] public AssetLocation Equip = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The path to a sound played when this item is unequipped.
        /// </summary>
        [DocumentAsJson] public AssetLocation Unequip = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The path to a sound played when this item is used to attack.
        /// </summary>
        [DocumentAsJson] public AssetLocation Attack = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>"player/clayformhi"</jsondefault>-->
        /// The path to a sound played when this item is picked up in the inventory using the mouse.
        /// </summary>
        [DocumentAsJson] public AssetLocation InvPickup = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>"player/clayform"</jsondefault>-->
        /// The path to a sound played when this item is placed in the inventory using the mouse.
        /// </summary>
        [DocumentAsJson] public AssetLocation InvPlace = null;

        public static AssetLocation InvPickUpDefault = new AssetLocation("sounds/player/clayformhi");
        public static AssetLocation InvPlaceDefault = new AssetLocation("sounds/player/clayform");


        /// <summary>
        /// Clones the held sounds.
        /// </summary>
        /// <returns></returns>
        public HeldSounds Clone()
        {
            HeldSounds sounds = new HeldSounds()
            {
                Idle = Idle == null ? null : Idle.Clone(),
                Equip = Equip == null ? null : Equip.Clone(),
                Unequip = Unequip == null ? null : Unequip.Clone(),
                Attack = Attack == null ? null : Attack.Clone(),
                InvPickup = InvPickup == null ? null : InvPickup.Clone(),
                InvPlace = InvPlace == null ? null : InvPlace.Clone()
            };

            return sounds;
        }



        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            Idle?.WithPathPrefixOnce("sounds/");
            Equip?.WithPathPrefixOnce("sounds/");
            Unequip?.WithPathPrefixOnce("sounds/");
            Attack?.WithPathPrefixOnce("sounds/");
            InvPickup?.WithPathPrefixOnce("sounds/");
            InvPlace?.WithPathPrefixOnce("sounds/");
        }
    }
}
