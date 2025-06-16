
#nullable disable
namespace Vintagestory.API.Common.Entities
{
    public class EntityDespawnData
    {
        /// <summary>
        /// The reason this entity despawned.
        /// </summary>
        public EnumDespawnReason Reason;

        /// <summary>
        /// In the case of death, this was the damage source.
        /// </summary>
        public DamageSource DamageSourceForDeath;

    }
}
