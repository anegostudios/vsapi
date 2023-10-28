using System.Runtime.Serialization;

namespace Vintagestory.API.Common
{
    public class HeldSounds
    {
        public AssetLocation Idle = null;
        public AssetLocation Equip = null;
        public AssetLocation Unequip = null;
        public AssetLocation Attack = null;
        

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
                Attack = Attack == null ? null : Attack.Clone()
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
        }
    }
}