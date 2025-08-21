using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents an in game Item of Vintage Story
    /// </summary>
    public class Item : CollectibleObject
    {
        /// <summary>
        /// The unique number of the item, dynamically assigned by the game
        /// </summary>
        public override int Id { get { return ItemId; } }

        public ItemTagArray Tags = ItemTagArray.Empty;

        /// <summary>
        /// The type of the collectible object
        /// </summary>
        public override EnumItemClass ItemClass { get { return EnumItemClass.Item; } }

        /// <summary>
        /// The unique number of the item, dynamically assigned by the game
        /// </summary>
        public int ItemId;

        /// <summary>
        /// The item's shape. Null for automatic shape based on the texture.
        /// </summary>
        public CompositeShape Shape = null;

        /// <summary>
        /// Default textures to be used for this item. The Dictionary keys are the texture short names, as referenced in this item's shape ShapeElementFaces
        /// <br/>(may be null on clients, prior to receipt of server assets)
        /// <br/>Note: from game version 1.20.4, this is <b>null on server-side</b> (except during asset loading start-up stage)
        /// </summary>
        public Dictionary<string, CompositeTexture> Textures = new Dictionary<string, CompositeTexture>();

        /// <summary>
        /// Returns the first texture in Textures
        /// </summary>
        public CompositeTexture FirstTexture { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; }}

        /// <summary>
        /// Instantiate a new item with null model transforms; ItemTypeNet will add default transforms client-side if they are null in the ItemType packet; transforms should not be needed on a server
        /// </summary>
        public Item()
        {
        }

        /// <summary>
        /// Instantiates a new item with given item id and stacksize = 1
        /// </summary>
        /// <param name="itemId"></param>
        public Item(int itemId)
        {
            this.ItemId = itemId;
            MaxStackSize = 1;
        }

        /// <summary>
        /// Should return a random pixel within the items/blocks texture
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public override int GetRandomColor(ICoreClientAPI capi, ItemStack stack)
        {
            if (Textures == null || Textures.Count == 0) return 0;

            BakedCompositeTexture tex = Textures?.First().Value?.Baked;
            return tex == null ? 0 : capi.ItemTextureAtlas.GetRandomColor(tex.TextureSubId);
        }

        /// <summary>
        /// Creates a deep copy of the item
        /// </summary>
        /// <returns></returns>
        public Item Clone()
        {
            Item cloned = (Item)MemberwiseClone();

            cloned.Code = this.Code.Clone();

            if (MiningSpeed != null) cloned.MiningSpeed = new Dictionary<EnumBlockMaterial, float>(MiningSpeed);

            cloned.Textures = new Dictionary<string, CompositeTexture>();

            if (Textures != null)
            {
                foreach (var val in Textures)
                {
                    cloned.Textures[val.Key] = val.Value.Clone();
                }
            }
            if (Shape != null)
            {
                cloned.Shape = Shape.Clone();
            }
            if (Attributes != null) cloned.Attributes = Attributes.Clone();

            if (CombustibleProps != null)
            {
                cloned.CombustibleProps = CombustibleProps.Clone();
            }

            if (NutritionProps != null)
            {
                cloned.NutritionProps = NutritionProps.Clone();
            }

            if (GrindingProps != null)
            {
                cloned.GrindingProps = GrindingProps.Clone();
            }

            return cloned;
        }

        internal void CheckTextures(ILogger logger)
        {
            List<string> toRemove = null;
            int i = 0;
            foreach (var val in Textures)
            {
                if (val.Value.Base == null)
                {
                    logger.Error("The texture definition {0} for #{2} in item with code {1} is invalid. The base property is null. Will skip.", i, Code, val.Key);
                    if (toRemove == null) toRemove = new List<string>();
                    toRemove.Add(val.Key);
                }
                i++;
            }

            if (toRemove != null)
            {
                foreach (var val in toRemove)
                {
                    Textures.Remove(val);
                }
            }
        }

        public virtual void FreeRAMServer()
        {
            // Shape = null;     // We need to keep the Shape for any block which is IAttachableToEntity or has an AttachableToEntity attribute
            Textures = null;
            GuiTransform = null;
            FpHandTransform = null;
            TpHandTransform = null;
            TpOffHandTransform = null;
            GroundTransform = null;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI clientApi && clientApi.Settings.Bool["extendedDebugInfo"])
            {
                IEnumerable<string> tags = GetTags(inSlot.Itemstack).ToArray().Select(clientApi.TagRegistry.ItemTagIdToTag).Order();
                if (tags.Any())
                {
                    dsc.AppendLine($"<font color=\"#bbbbbb\">Tags: {tags.Aggregate((first, second) => $"{first}, {second}")}</font>");
                }
            }

            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        }

        public virtual ItemTagArray GetTags(ItemStack stack) => Tags;
    }
}
