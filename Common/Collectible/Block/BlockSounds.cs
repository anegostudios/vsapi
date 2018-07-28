using System;
using System.Runtime.Serialization;

namespace Vintagestory.API.Common
{
    public class BlockSounds
    {
        public AssetLocation Walk = null;
        public AssetLocation Inside = null;
        public AssetLocation Break = null;
        public AssetLocation Place = null;
        public AssetLocation Hit = null;
        public AssetLocation Ambient = null;


        public BlockSounds Clone()
        {
            return new BlockSounds()
            {
                Walk = Walk == null ? null : Walk.Clone(),
                Inside = Inside == null ? null : Inside.Clone(),
                Break = Break == null ? null : Break.Clone(),
                Place = Place == null ? null : Place.Clone(),
                Hit = Hit == null ? null : Hit.Clone(),
                Ambient = Ambient == null ? null : Ambient.Clone()
            };
        }


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Walk?.WithPathPrefix("sounds/");
            Inside?.WithPathPrefix("sounds/");
            Break?.WithPathPrefix("sounds/");
            Place?.WithPathPrefix("sounds/");
            Ambient?.WithPathPrefix("sounds/");
        }
    }
}