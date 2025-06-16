using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

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

        public override bool DieInLiquid => false;

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
                if ((world as IClientWorldAccessor).LoadedEntities.TryGetValue(entityId, out Entity entity))
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
                if ((api.World as IClientWorldAccessor).LoadedEntities.TryGetValue(entityId, out Entity entity))
                {
                    textureSubId = entity.Properties.Client.FirstTexture.Baked.TextureSubId;
                }
            }
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return capi.EntityTextureAtlas.GetRandomColor(textureSubId);
        }

        public override Vec3d Pos => new Vec3d(particlePos.X + rand.NextDouble() * radius - radius / 2, particlePos.Y + 0.1f, particlePos.Z + rand.NextDouble() * radius - radius / 2);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());
            return distanceVector;
        }


        public override float Size => (float)(minScale + rand.NextDouble() * (maxScale - minScale));

        public override EnumParticleModel ParticleModel => EnumParticleModel.Cube;

        public override float Quantity => quantity;

        public override float LifeLength => 0.75f + (float)api.World.Rand.NextDouble() / 3f;

        public override int VertexFlags => 0;

        public override IParticlePropertiesProvider[] SecondaryParticles => null;
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


