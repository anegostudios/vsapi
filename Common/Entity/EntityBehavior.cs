using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Defines a basic entity behavior that can be attached to entities
    /// </summary>
    public abstract class EntityBehavior
    {
        public Entity entity;

        public EntityBehavior(Entity entity)
        {
            this.entity = entity;
        }

        public virtual void Initialize(EntityType entityType, JsonObject attributes)
        {
            
        }

        public virtual void OnGameTick(float deltaTime) { }

        public virtual void OnEntitySpawn() { }

        public virtual void OnEntityDespawn(EntityDespawnReason despawn) { }

        public abstract string PropertyName();

        public virtual void OnEntityReceiveDamage(DamageSource damageSource, float damage)
        {
            
        }

        public virtual void OnFallToGround(Vec3d lastTerrainContact, double withYMotion)
        {
        }

        public virtual void OnSetEntityName(string playername)
        {
            
        }

        public virtual void OnEntityReceiveSaturation(float saturation)
        {
            
        }

        public virtual void OnReceivedServerPos(ref EnumHandling handled)
        {
            
        }

        public virtual ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            handling = EnumHandling.NotHandled;

            return null;
        }

        public virtual void OnStateChanged(EnumEntityState beforeState, ref EnumHandling handled)
        {
            
        }

        /// <summary>
        /// The notify method bubbled up from entity.Notify()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public virtual void Notify(string key, object data)
        {
            
        }
    }
}
