using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class EntityCubeParticles : CollectibleParticleProperties
    {
        public Vec3d particlePos;
        public int quantity;
        public float radius;
        public float minScale;
        public float maxScale;

        int textureSubId;
        long entityId;

        public override bool DieInLiquid() { return false; }


        public EntityCubeParticles() { }

        public EntityCubeParticles(IWorldAccessor world, long entityId, Vec3d particlePos, float radius, int quantity, float minScale, float maxScale)
        {
            this.particlePos = particlePos;
            this.entityId = entityId;
            this.quantity = quantity;
            this.radius = radius;
            this.minScale = minScale;
            this.maxScale = maxScale;

            if (world.Side == EnumAppSide.Client)
            {
                Entity entity;
                if ((world as IClientWorldAccessor).LoadedEntities.TryGetValue(entityId, out entity))
                {
                    textureSubId = entity.Properties.Client.FirstTexture.Baked.TextureSubId;
                }
            }
        }

        public override void Init(ICoreAPI api)
        {
            base.Init(api);

            if (textureSubId == 0 && api.Side == EnumAppSide.Client)
            {
                Entity entity;
                if ((api.World as IClientWorldAccessor).LoadedEntities.TryGetValue(entityId, out entity))
                {
                    textureSubId = entity.Properties.Client.FirstTexture.Baked.TextureSubId;
                }
            }
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return capi.EntityTextureAtlas.GetRandomPixel(textureSubId); ;
        }

        public override Vec3d GetPos()
        {
            return new Vec3d(particlePos.X + rand.NextDouble() * radius - radius / 2, particlePos.Y + 0.1f, particlePos.Z + rand.NextDouble() * radius - radius / 2);
        }

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());
            return distanceVector;
        }


        public override float GetSize()
        {
            return (float)(minScale + rand.NextDouble() * (maxScale - minScale));
        }

        public override EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Cube;
        }

        public override float GetQuantity()
        {
            return quantity;
        }

        public override float GetLifeLength()
        {
            return 0.75f + (float)api.World.Rand.NextDouble() / 3f;
        }

        public override byte GetGlowLevel()
        {
            return 0;
        }

        public override IParticlePropertiesProvider[] GetSecondaryParticles() { return null; }

        public override void ToBytes(BinaryWriter writer)
        {
            particlePos.ToBytes(writer);
            writer.Write(entityId);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(minScale);
            writer.Write(maxScale);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            particlePos = Vec3d.CreateFromBytes(reader);
            entityId = reader.ReadInt64();
            quantity = reader.ReadInt32();
            radius = reader.ReadSingle();
            minScale = reader.ReadSingle();
            maxScale = reader.ReadSingle();
        }

    }
}


