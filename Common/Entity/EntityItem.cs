using System;
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
        Vec3d tmpPos = new Vec3d();
        int counter = 0;
        bool stuckInBlock;

        public ItemStack Itemstack
        {
            get { return WatchedAttributes.GetItemstack("itemstack"); }
            set { WatchedAttributes.SetItemstack("itemstack", value); stack = value; }
        }

        public string ByPlayerUid
        {
            get { return WatchedAttributes.GetString("byPlayerUid"); }
            set { WatchedAttributes.SetString("byPlayerUid", value); }
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
            
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            base.Initialize(properties, api, chunkindex3d);

            if (Itemstack == null || !Itemstack.ResolveBlockOrItem(World))
            {
                Die();
                this.Itemstack = null;
                return;
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

            
            if (counter++ > 10 || stuckInBlock)
            {
                stuckInBlock = false;
                counter = 0;
                Properties.Habitat = EnumHabitat.Land;
                if (!Swimming)
                {
                    tmpPos.Set(LocalPos.X, LocalPos.Y, LocalPos.Z);
                    Cuboidd collbox = World.CollisionTester.GetCollidingCollisionBox(World.BlockAccessor, CollisionBox, tmpPos, false);

                    if (collbox != null)
                    {
                        PushoutOfCollisionbox(dt, collbox);
                        stuckInBlock = true;
                    }
                }
            }
        }

        private void PushoutOfCollisionbox(float dt, Cuboidd collBox)
        {
            double posX = LocalPos.X;
            double posY = LocalPos.Y;
            double posZ = LocalPos.Z;
            /// North: Negative Z
            /// East: Positive X
            /// South: Positive Z
            /// West: Negative X

            double[] distByFacing = new double[]
            {
                posZ - collBox.Z1, // N
                collBox.X2 - posX, // E
                collBox.Z2 - posZ, // S
                posX - collBox.X1, // W
                collBox.Y2 - posY, // U
                99 // D
            };

            BlockFacing pushDir = BlockFacing.UP;
            double shortestDist = 99;
            for (int i = 0; i < distByFacing.Length; i++)
            {
                BlockFacing face = BlockFacing.ALLFACES[i];
                if (distByFacing[i] < shortestDist && !World.CollisionTester.IsColliding(World.BlockAccessor, CollisionBox, tmpPos.Set(posX + face.Normali.X, posY, posZ + face.Normali.Z)))
                {
                    shortestDist = distByFacing[i];
                    pushDir = face;
                }    
            }

            dt = Math.Min(dt, 0.1f);

            LocalPos.X += pushDir.Normali.X * dt;
            LocalPos.Y += pushDir.Normali.Y * dt;
            LocalPos.Z += pushDir.Normali.Z * dt;

            LocalPos.Motion.X = pushDir.Normali.X * dt;
            LocalPos.Motion.Y = pushDir.Normali.Y * dt * 2;
            LocalPos.Motion.Z = pushDir.Normali.Z * dt;

            Properties.Habitat = EnumHabitat.Air;
        }








        public static EntityItem FromItemstack(ItemStack itemstack, Vec3d position, Vec3d velocity, IWorldAccessor world)
        {
            EntityItem item = new EntityItem();
            item.Code = GlobalConstants.EntityItemTypeCode;
            item.SimulationRange = 3 * world.BlockAccessor.ChunkSize;
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