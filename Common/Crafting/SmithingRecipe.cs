using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class SmithingRecipe : SingleLayerVoxelRecipe<SmithingRecipe>, ByteSerializable
    {
        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public override SmithingRecipe Clone()
        {
            SmithingRecipe recipe = new SmithingRecipe();

            recipe.Pattern = (string[])Pattern.Clone();
            recipe.Ingredient = Ingredient.Clone();
            recipe.Output = Output.Clone();
            recipe.Name = Name;

            return recipe;
        }
    }

    public abstract class SingleLayerVoxelRecipe<T> : RecipeBase<T>
    {
        public string[] Pattern;
        public bool[,] Voxels = new bool[16, 16];

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
                world.Logger.Error("Recipe with output {0} has no ingredient pattern or missing ingredient/output. Ignoring recipe.", Output);
                return false;
            }

            if (!Ingredient.Resolve(world))
            {
                world.Logger.Error("Recipe with output {0}: Cannot resolve ingredient in {1}.", Output, sourceForErrorLogging);
                return false;
            }
            if (!Output.Resolve(world, sourceForErrorLogging))
            {
                return false;
            }

            GenVoxels();
            return true;
        }

        /// <summary>
        /// Generates the voxels for the recipe.
        /// </summary>
        public void GenVoxels() { 
            int width = 0;
            for (int i = 0; i < Pattern.Length; i++) width = Math.Max(width, Pattern[i].Length);
            int height = Pattern.Length;

            if (width > 16 || height > 16)
            {
                throw new Exception(string.Format("Invalid smithing recipe {0}! Width or height is beyond 16 voxels", this.Name));
            }


            int startX = (16 - width) / 2;
            int startY = (16 - height) / 2;

            for (int x = 0; x < Math.Min(width, 16); x++)
            {
                for (int y = 0; y < Math.Min(height, 16); y++)
                {
                    Voxels[x + startX, y + startY] = Pattern[y][x] != '_' && Pattern[y][x] != ' ';
                }
            }
        }


        /// <summary>
        /// Serialized the recipe
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            Ingredient.ToBytes(writer);

            writer.Write(Pattern.Length);
            for (int i = 0; i < Pattern.Length; i++)
            {
                writer.Write(Pattern[i]);
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
            Ingredient.FromBytes(reader, resolver);

            int len = reader.ReadInt32();
            Pattern = new string[len];
            for (int i = 0; i < Pattern.Length; i++)
            {
                Pattern[i] = reader.ReadString();
            }

            Name = new AssetLocation(reader.ReadString());

            Output = new JsonItemStack();
            Output.FromBytes(reader, resolver.ClassRegistry);
            Output.Resolve(resolver, "[FromBytes]");
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
            if (!Ingredient.Code.Path.Contains("*")) return mappings;
            int wildcardStartLen = Ingredient.Code.Path.IndexOf("*");
            int wildcardEndLen = Ingredient.Code.Path.Length - wildcardStartLen - 1;

            List<string> codes = new List<string>();

            if (Ingredient.Type == EnumItemClass.Block)
            {
                for (int i = 0; i < world.Blocks.Length; i++)
                {
                    if (world.Blocks[i] == null || world.Blocks[i].IsMissing) continue;

                    if (WildCardMatch(Ingredient.Code, world.Blocks[i].Code))
                    {
                        string code = world.Blocks[i].Code.Path.Substring(wildcardStartLen);
                        string codepart = code.Substring(0, code.Length - wildcardEndLen);
                        if (Ingredient.AllowedVariants != null && !Ingredient.AllowedVariants.Contains(codepart)) continue;

                        codes.Add(codepart);

                    }
                }
            }
            else
            {
                for (int i = 0; i < world.Items.Length; i++)
                {
                    if (world.Items[i] == null || world.Items[i].IsMissing) continue;

                    if (WildCardMatch(Ingredient.Code, world.Items[i].Code))
                    {
                        string code = world.Items[i].Code.Path.Substring(wildcardStartLen);
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
        /// Checks to see whether or not the wildcard matches the smithing recipe.
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
