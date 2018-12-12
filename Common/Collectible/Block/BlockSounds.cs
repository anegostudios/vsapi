using System;
using System.Collections.Generic;
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

        public Dictionary<EnumTool, BlockSounds> ByTool = new Dictionary<EnumTool, BlockSounds>();

        /// <summary>
        /// Clones the block sounds.
        /// </summary>
        /// <returns></returns>
        public BlockSounds Clone()
        {
            BlockSounds sounds = new BlockSounds()
            {
                Walk = Walk == null ? null : Walk.Clone(),
                Inside = Inside == null ? null : Inside.Clone(),
                Break = Break == null ? null : Break.Clone(),
                Place = Place == null ? null : Place.Clone(),
                Hit = Hit == null ? null : Hit.Clone(),
                Ambient = Ambient == null ? null : Ambient.Clone()
            };

            foreach (var val in ByTool)
            {
                sounds.ByTool[val.Key] = val.Value.Clone();
            }

            return sounds;
        }

        /// <summary>
        /// Gets the breaking sound, either provided by the tool or by this sound.
        /// </summary>
        /// <param name="byPlayer"></param>
        /// <returns></returns>
        public AssetLocation GetBreakSound(IPlayer byPlayer)
        {
            EnumTool? tool = byPlayer?.InventoryManager?.ActiveHotbarSlot?.Itemstack?.Collectible?.Tool;
            return tool == null ? Break : GetBreakSound((EnumTool)tool);
        }

        /// <summary>
        /// Gets the hit sound either provided by the tool or by the block.
        /// </summary>
        /// <param name="byPlayer"></param>
        /// <returns></returns>
        public AssetLocation GetHitSound(IPlayer byPlayer)
        {
            EnumTool? tool = byPlayer?.InventoryManager?.ActiveHotbarSlot?.Itemstack?.Collectible?.Tool;
            return tool == null ? Hit : GetHitSound((EnumTool)tool);
        }

        /// <summary>
        /// Gets the break sound either by the tool or by the block if the tool does not have a break sound.
        /// </summary>
        /// <param name="tool">The Tool used.</param>
        /// <returns>The resulting sound</returns>
        public AssetLocation GetBreakSound(EnumTool tool)
        {
            BlockSounds toolSounds;
            ByTool.TryGetValue(tool, out toolSounds);
            if (toolSounds?.Break != null) return toolSounds.Break;

            return Break;
        }

        /// <summary>
        /// Gets the hit sound either by the tool or by the block if the tool does not have a hit sound.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public AssetLocation GetHitSound(EnumTool tool)
        {
            BlockSounds toolSounds;
            ByTool.TryGetValue(tool, out toolSounds);
            if (toolSounds?.Hit != null) return toolSounds.Hit;

            return Hit;
        }



        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Walk?.WithPathPrefix("sounds/");
            Inside?.WithPathPrefix("sounds/");
            Break?.WithPathPrefix("sounds/");
            Place?.WithPathPrefix("sounds/");
            Hit?.WithPathPrefix("sounds/");
            Ambient?.WithPathPrefix("sounds/");
            
        }
    }
}