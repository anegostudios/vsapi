using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common.Entities
{
    public class EntityDespawnReason
    {
        /// <summary>
        /// The reason this entity despawned.
        /// </summary>
        public EnumDespawnReason reason;

        /// <summary>
        /// In the case of death, this was the damage source.
        /// </summary>
        public DamageSource damageSourceForDeath;

    }
}
