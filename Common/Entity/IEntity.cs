using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Represents pretty much any object that is not a block
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Server wide unique identifier of this entity
        /// </summary>
        long EntityId { get; }

        EntityType Type { get; }

        bool Alive { get; }

        EnumEntityState State { get; }

        /// <summary>
        /// The world this entity resides in
        /// </summary>
        IWorldAccessor World { get; }

        /// <summary>
        /// Position according to the client side physics simulation. For survival players this position is sent by the client to the server every 200ms.
        /// </summary>
        EntityPos Pos { get; }

        /// <summary>
        /// Position according to the server side physics simulation
        /// </summary>
        EntityPos ServerPos { get; }


        /// <summary>
        /// Position according to the local side
        /// </summary>
        EntityPos LocalPos { get; }


        /// <summary>
        /// Client: Pos
        /// Server: ServerPos
        /// </summary>
        //EntityPos LocalPos { get; }

        /// <summary>
        /// Properties that are synced to client
        /// </summary>
        ITreeAttribute WatchedAttributes { get; }

        /// <summary>
        /// Properties that are not synced and only local on client or server
        /// </summary>
        ITreeAttribute Attributes { get; }

        /// <summary>
        /// The entities boundaries with which it collides with the terrain
        /// </summary>
        Cuboidf CollisionBox { get; }

        /// <summary>
        /// Causes the entity to despawn
        /// </summary>
        void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null);

        /// <summary>
        /// Damages this entity. Returns true if damage was applied
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="damage"></param>
        bool ReceiveDamage(DamageSource damageSource, float damage);

        /// <summary>
        /// Invulernability in ms
        /// </summary>
        //long InvulnerableUntil { get; }

        /// <summary>
        /// True if the entity is in contact with something solid
        /// </summary>
        bool Collided { get; }

        /// <summary>
        /// True if the players picking ray should collide with this entity
        /// </summary>
        bool IsInteractable { get; }

        /// <summary>
        /// Return true if this entity is collectible by given entity.
        /// </summary>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        bool CanCollect(Entity byEntity);

        /// <summary>
        /// Called if CanCollect() returned true. Should return the itemstack that will land in the players inventory
        /// </summary>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        ItemStack OnCollected(Entity byEntity);

        /// <summary>
        /// Sets the type of this entity
        /// </summary>
        /// <param name="entityType"></param>
        void SetType(EntityType entityType);

        /// <summary>
        /// Plays a preconfigured sound of this entity (from entitytype config)
        /// When called on Server Side: Broadcast a sound to all clients
        /// When called on Client side: Plays on the client
        /// </summary>
        /// <param name="type"></param>
        void PlayEntitySound(string type, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 24);

        /// <summary>
        /// Client: Starts given animation
        /// Server: Sends all active anims to all connected clients then purges the ActiveAnimationsByAnimCode list
        /// </summary>
        /// <param name="animdata"></param>
        void StartAnimation(AnimationMetaData animdata);

        /// <summary>
        /// Start a new animation defined in the entity config file. If it's not defined, it won't play.
        /// Use StartAnimation(AnimationMetaData animdata) to circumvent the entity config anim data.
        /// </summary>
        /// <param name="configCode">Anim Config code, not the animation code!</param>
        void StartAnimation(string configCode);

        /// <summary>
        /// Stops given animation defined by the animation code or entity config code (both are searched for)
        /// </summary>
        /// <param name="code"></param>
        void StopAnimation(string code);
    }
}

