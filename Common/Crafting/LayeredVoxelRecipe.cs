using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Creates a recipe using a 3D voxel-based system. Used for recipes types such as clayforming, smithing, or stone-knapping.
    /// </summary>
    [DocumentAsJson]
    public abstract class LayeredVoxelRecipe<T> : RecipeBase<T>
    {

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// A 2D array of strings that are layered together to form the recipe. Use "#" for solid, and "_" or " " for a gap.
        /// </summary>
        [DocumentAsJson] public string[][] Pattern;

        /// <summary>
        /// An array of voxels, created from <see cref="Pattern"/> during loading. This array is cloned when a player starts creating the recipe.
        /// </summary>
        public bool[,,] Voxels;

        /// <summary>
        /// The number of layers in this recipe, in the Y-axis.
        /// </summary>
        public abstract int QuantityLayers { get; }

        /// <summary>
        /// A category code for this recipe type. Used for error logging.
        /// </summary>
        public abstract string RecipeCategoryCode { get; }

        /// <summary>
        /// If true, the recipe is rotated 90 degrees in the Y axis.
        /// </summary>
        protected virtual bool RotateRecipe { get; set; } = false;

        public LayeredVoxelRecipe()
        {
            Voxels = new bool[16, QuantityLayers, 16];
        }

        /// <summary>
        /// Resolves the recipe.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="sourceForErrorLogging"></param>
        /// <returns></returns>
        public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
        {
            if (Pattern == null || Ingredient == null || Output == null)
            {
                world.Logger.Error("{1} Recipe with output {0} has no ingredient pattern or missing ingredient/output. Ignoring recipe.", Output, RecipeCategoryCode);
                return false;
            }

            if (!Ingredient.Resolve(world, RecipeCategoryCode + " recipe"))
            {
                world.Logger.Error("{1} Recipe with output {0}: Cannot resolve ingredient in {1}.", Output, sourceForErrorLogging, RecipeCategoryCode);
                return false;
            }

            if (!Output.Resolve(world, sourceForErrorLogging, Ingredient.Code))
            {
                return false;
            }

            GenVoxels();
            return true;
        }

        /// <summary>
        /// Generates the voxels for the recipe.
        /// </summary>
        public void GenVoxels()
        {
            int length = Pattern[0][0].Length;
            int width = Pattern[0].Length;
            int height = Pattern.Length;

            if (width > 16 || height > QuantityLayers || length > 16)
            {
                throw new Exception(string.Format("Invalid {1} recipe {0}! Either Width or length is beyond 16 voxels or height is beyond {2} voxels", this.Name, RecipeCategoryCode, QuantityLayers));
            }

            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Pattern[i].Length != width)
                {
                    throw new Exception(string.Format("Invalid {4} recipe {3}! Layer {0} has a width of {1}, " +
                        "which is not the same as the first layer width of {2}. All layers need to be sized equally.", i, Pattern[i].Length, width, this.Name, RecipeCategoryCode));
                }

                for (int j = 0; j < Pattern[i].Length; j++)
                {
                    if (Pattern[i][j].Length != length)
                    {
                        throw new Exception(string.Format("Invalid {5} recipe {3}! Layer {0}, line {4} has a length of {1}, " +
                        "which is not the same as the first layer length of {2}. All layers need to be sized equally.", i, Pattern[i][j].Length, length, this.Name, j, RecipeCategoryCode));
                    }
                }
            }

            // We'll center the recipe to the horizontal middle
            int startX = (16 - width) / 2;
            int startZ = (16 - length) / 2;

            for (int x = 0; x < Math.Min(width, 16); x++)
            {
                for (int y = 0; y < Math.Min(height, QuantityLayers); y++)
                {
                    for (int z = 0; z < Math.Min(length, 16); z++)
                    {
                        if (RotateRecipe)
                        {
                            Voxels[z + startZ, y, x + startX] = Pattern[y][x][z] != '_' && Pattern[y][x][z] != ' ';
                        } else
                        {
                            Voxels[x + startX, y, z + startZ] = Pattern[y][x][z] != '_' && Pattern[y][x][z] != ' ';
                        }
                        
                    }
                }
            }
        }


        /// <summary>
        /// Serialized the recipe
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(RecipeId);
            Ingredient.ToBytes(writer);

            writer.Write(Pattern.Length);
            for (int i = 0; i < Pattern.Length; i++)
            {
                writer.WriteArray(Pattern[i]);
            }

            writer.Write(Name.ToShortString());

            Output.ToBytes(writer);


        }

        /// <summary>
        /// Deserializes the alloy
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            Ingredient = new CraftingRecipeIngredient();
            RecipeId = reader.ReadInt32();

            Ingredient.FromBytes(reader, resolver);

            int len = reader.ReadInt32();
            Pattern = new string[len][];
            for (int i = 0; i < Pattern.Length; i++)
            {
                Pattern[i] = reader.ReadStringArray();
            }

            Name = new AssetLocation(reader.ReadString());

            Output = new JsonItemStack();
            Output.FromBytes(reader, resolver.ClassRegistry);
            Output.Resolve(resolver, "[Voxel recipe FromBytes]", Ingredient.Code);
            GenVoxels();
        }



        /// <summary>
        /// Resolves Wildcards in the ingredients
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public override Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world)
        {
            Dictionary<string, string[]> mappings = new Dictionary<string, string[]>();

            if (Ingredient.Name == null || Ingredient.Name.Length == 0) return mappings;
            if (!Ingredient.Code.Path.Contains('*')) return mappings;
            int wildcardStartLen = Ingredient.Code.Path.IndexOf('*');
            int wildcardEndLen = Ingredient.Code.Path.Length - wildcardStartLen - 1;

            List<string> codes = new List<string>();

            if (Ingredient.Type == EnumItemClass.Block)
            {
                foreach (Block block in world.Blocks)
                {
                    if (block.IsMissing) continue;    // BlockList already performs the null check for us, in its enumerator

                    if (WildcardUtil.Match(Ingredient.Code, block.Code))
                    {
                        string code = block.Code.Path.Substring(wildcardStartLen);
                        string codepart = code.Substring(0, code.Length - wildcardEndLen);
                        if (Ingredient.AllowedVariants != null && !Ingredient.AllowedVariants.Contains(codepart)) continue;

                        codes.Add(codepart);

                    }
                }
            }
            else
            {
                foreach (Item item in world.Items)
                {
                    if (item?.Code == null || item.IsMissing) continue;

                    if (WildcardUtil.Match(Ingredient.Code, item.Code))
                    {
                        string code = item.Code.Path.Substring(wildcardStartLen);
                        string codepart = code.Substring(0, code.Length - wildcardEndLen);
                        if (Ingredient.AllowedVariants != null && !Ingredient.AllowedVariants.Contains(codepart)) continue;

                        codes.Add(codepart);
                    }
                }
            }

            mappings[Ingredient.Name] = codes.ToArray();

            return mappings;
        }



        /// <summary>
        /// Matches the wildcards for the clay recipe.
        /// </summary>
        /// <param name="wildCard"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public static bool WildCardMatch(AssetLocation wildCard, AssetLocation blockCode)
        {
            if (blockCode == null || !wildCard.Domain.Equals(blockCode.Domain)) return false;
            if (wildCard.Equals(blockCode)) return true;

            string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

            return Regex.IsMatch(blockCode.Path, @"^" + pattern + @"$");
        }


    }

}




