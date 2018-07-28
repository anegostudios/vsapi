using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

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

        /// <summary>
        /// The type of the collectible object
        /// </summary>
        public override EnumItemClass ItemClass { get { return EnumItemClass.Item; } }

        /// <summary>
        /// The unique number of the item, dynamically assigned by the game
        /// </summary>
        public int ItemId;

        /// <summary>
        /// The items shape. Null for automatic shape based on the texture.
        /// </summary>
        public CompositeShape Shape = null;

        /// <summary>
        /// Default textures to be used for this block
        /// </summary>
        public Dictionary<string, CompositeTexture> Textures = new Dictionary<string, CompositeTexture>();

        /// <summary>
        /// Returns the first texture in Textures
        /// </summary>
        public CompositeTexture FirstTexture { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; }}
        
        /// <summary>
        /// Instantiate a new item with default model transforms
        /// </summary>
        public Item()
        {
            GuiTransform = ModelTransform.ItemDefaultGui();
            FpHandTransform = ModelTransform.ItemDefault();
            TpHandTransform = ModelTransform.ItemDefaultTp();
            GroundTransform = ModelTransform.ItemDefault();
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
        /// Fills in placeholders in the composite texture (called by the VSGameContent mod during item loading)
        /// </summary>
        /// <param name="searchReplace"></param>
        public void FillPlaceHolders(Dictionary<string, string> searchReplace)
        {
            foreach (CompositeTexture tex in Textures.Values)
            {
                tex.FillPlaceHolders(searchReplace);
            }

            Shape?.FillPlaceHolders(searchReplace);
            foreach (var val in searchReplace)
            {
                Attributes?.FillPlaceHolder(val.Key, val.Value);
            }
            
            if (CombustibleProps != null && CombustibleProps.SmeltedStack != null)
            {
                CombustibleProps.SmeltedStack.Code = Block.FillPlaceHolder(CombustibleProps.SmeltedStack.Code, searchReplace);
            }

            if (NutritionProps != null && NutritionProps.EatenStack != null)
            {
                NutritionProps.EatenStack.Code = Block.FillPlaceHolder(NutritionProps.EatenStack.Code, searchReplace);
            }

            if (GrindingProps != null && GrindingProps.GrindedStack != null)
            {
                GrindingProps.GrindedStack.Code = Block.FillPlaceHolder(GrindingProps.GrindedStack.Code, searchReplace);
            }
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
    }
}