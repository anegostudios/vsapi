using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
