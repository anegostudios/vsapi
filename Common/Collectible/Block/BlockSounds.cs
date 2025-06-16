using System.Collections.Generic;
using System.Runtime.Serialization;
using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A set of sounds that are defined for a block. All fields use default or empty sounds if not set.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"sounds": {
	///	"place": "block/dirt",
	///	"break": "block/dirt",
	///	"hit": "block/dirt",
	///	"walk": "walk/grass"
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class BlockSounds
    {
        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played when an entity walks on this block.
        /// </summary>
        [DocumentAsJson] public virtual AssetLocation Walk { get; set; } = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played when an entity moves inside this block. Primarily used for liquids.
        /// </summary>
        [DocumentAsJson] public virtual AssetLocation Inside { get; set; } = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played when this block is broken.
        /// </summary>
        [DocumentAsJson] public virtual AssetLocation Break { get; set; } = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played when this block is placed.
        /// </summary>
        [DocumentAsJson] public virtual AssetLocation Place { get; set; } = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played when this block is hit. Will be overridden by <see cref="ByTool"/> if an appropriate tool is set.
        /// </summary>
        [DocumentAsJson] public virtual AssetLocation Hit { get; set; } = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Played in ambience for this block.
        /// </summary>
        [DocumentAsJson] public AssetLocation Ambient = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Ambient</jsondefault>-->
        /// The type of sound for this block's ambient sound.
        /// </summary>
        [DocumentAsJson] public EnumSoundType AmbientSoundType = EnumSoundType.Ambient;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>3</jsondefault>-->
        /// Adjacent ambient sound sources are merged to avoid playing too many sounds too loudly. This is the maximum distance a sound source can be from another to allow a merge.
        /// </summary>
        [DocumentAsJson] public float AmbientMaxDistanceMerge = 3;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>10</jsondefault>-->
        /// Amount of nearby ambient sound blocks in order to reach full ambient sound volume
        /// </summary>
        [DocumentAsJson] public float AmbientBlockCount = 10f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Gets the sound that occurs when a specific tool hits a block.  (Note for coders: if none specified in the JSON, this will be null from version 1.20.4 onwards)
        /// </summary>
        [DocumentAsJson] public virtual Dictionary<EnumTool, BlockSounds> ByTool { get; set; }

        /// <summary>
        /// Clones the block sounds.
        /// </summary>
        /// <returns></returns>
        public BlockSounds Clone()
        {
            BlockSounds sounds = new BlockSounds()
            {
                Walk = Walk?.PermanentClone(),
                Inside = Inside?.PermanentClone(),
                Break = Break?.PermanentClone(),
                Place = Place?.PermanentClone(),
                Hit = Hit?.PermanentClone(),
                Ambient = Ambient?.PermanentClone(),
                AmbientBlockCount = AmbientBlockCount,
                AmbientSoundType = AmbientSoundType,
                AmbientMaxDistanceMerge = AmbientMaxDistanceMerge
            };

            if (ByTool != null)
            {
                sounds.ByTool = new(ByTool.Count);
                foreach (var val in ByTool)
                {
                    sounds.ByTool[val.Key] = val.Value.Clone();
                }
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
            if (ByTool != null)
            {
                ByTool.TryGetValue(tool, out BlockSounds toolSounds);
                if (toolSounds?.Break != null) return toolSounds.Break;
            }

            return Break;
        }

        /// <summary>
        /// Gets the hit sound either by the tool or by the block if the tool does not have a hit sound.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public AssetLocation GetHitSound(EnumTool tool)
        {
            if (ByTool != null)
            {
                ByTool.TryGetValue(tool, out BlockSounds toolSounds);
                if (toolSounds?.Hit != null) return toolSounds.Hit;
            }

            return Hit;
        }



        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            Walk?.WithPathPrefixOnce("sounds/");
            Inside?.WithPathPrefixOnce("sounds/");
            Break?.WithPathPrefixOnce("sounds/");
            Place?.WithPathPrefixOnce("sounds/");
            Hit?.WithPathPrefixOnce("sounds/");
            Ambient?.WithPathPrefixOnce("sounds/");
        }
    }
}
