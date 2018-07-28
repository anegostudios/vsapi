using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class EntityItem : Entity
    {
        ItemStack stack;
        public long itemSpawnedMilliseconds;

        public ItemStack Itemstack
        {
            get { return WatchedAttributes.GetItemstack("itemstack"); }
            set { WatchedAttributes.SetItemstack("itemstack", value); stack = value; }
        }

        public override float MaterialDensity
        {
            get { return (stack?.Collectible != null) ? stack.Collectible.MaterialDensity : 2000; }
        }

        public override bool IsInteractable
        {
            get { return false; }
        }

        public override byte[] LightHsv
        {
            get
            {
                return stack?.Block?.GetLightHsv(World.BlockAccessor, null, stack);
            }
        }



        public EntityItem() : base()
        {
            AddBehavior(new EntityBehaviorPassivePhysics(this));
        }

        public override void Initialize(ICoreAPI api, long chunkindex3d)
        {
            base.Initialize(api, chunkindex3d);

            if (Itemstack == null)
            {
                Die();
            }
            else
            {
                if (!Itemstack.ResolveBlockOrItem(World))
                {
                    this.Itemstack = null;
                    Die();
                }
            }

            itemSpawnedMilliseconds = World.ElapsedMilliseconds;

            Swimming = FeetInLiquid = World.BlockAccessor.GetBlock(Pos.AsBlockPos).IsLiquid();
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            if (Itemstack != null)
            {
                Itemstack.Collectible.OnGroundIdle(this);
            }
        }

        public static EntityItem FromItemstack(ItemStack itemstack, Vec3d position, Vec3d velocity, IWorldAccessor world)
        {
            EntityItem item = new EntityItem();

            item.SetType(world.GetEntityType(GlobalConstants.EntityItemTypeCode));
            item.TrackingRange = 3 * world.BlockAccessor.ChunkSize;
            item.Itemstack = itemstack;

            item.ServerPos.SetPos(position);

            if (velocity == null)
            {
                velocity = new Vec3d((float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f);
            }

            item.ServerPos.Motion = velocity;


            item.Pos.SetFrom(item.ServerPos);

            return item;
        }


        public override bool CanCollect(Entity byEntity)
        {
            return Alive && World.ElapsedMilliseconds - itemSpawnedMilliseconds > 1000;
        }

        public override ItemStack OnCollected(Entity byEntity)
        {
            return stack;
        }

        public override void OnCollideWithLiquid()
        {
            base.OnCollideWithLiquid();
            
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            return false;
        }



        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            base.FromBytes(reader, forClient);

            if (Itemstack != null)
            {
                stack = Itemstack;
            }

            if (World != null)
            {
                if (!stack.ResolveBlockOrItem(World))
                {
                    this.Itemstack = null;
                    Die();
                }
            }
        }

        public override double SwimmingOffsetY
        {
            get { return 0.25f; }
        }
    }
}