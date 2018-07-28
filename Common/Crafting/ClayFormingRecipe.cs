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
    public class ClayFormingRecipe : RecipeBase<ClayFormingRecipe>, ByteSerializable
    {
        public string[][] Pattern;
        public bool[,,] Voxels = new bool[16, 16, 16];

        public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
        {
            if (Pattern == null || Ingredient == null || Output == null)
            {
                world.Logger.Error("Clay Forming Recipe with output {0} has no ingredient pattern or missing ingredient/output. Ignoring recipe.", Output);
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

        public void GenVoxels()
        {
            int length = Pattern[0][0].Length;
            int width = Pattern[0].Length;
            int height = Pattern.Length;

            // We'll center the recipe to the horizontal middle
            int startX = (16 - width) / 2;
            int startZ = (16 - length) / 2;

            for (int x = 0; x < Math.Min(width, 16); x++)
            {
                for (int y = 0; y < Math.Min(height, 16); y++)
                {
                    for (int z = 0; z < Math.Min(length, 16); z++)
                    {
                        Voxels[x + startX, y, z + startZ] = Pattern[y][x][z] != '_' && Pattern[y][x][z] != ' ';
                    }
                }
            }
        }


        /// <summary>
        /// Serialized the alloy
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            Ingredient.ToBytes(writer);

            writer.Write(Pattern.Length);
            
            for (int i = 0; i < Pattern.Length; i++)
            {
                writer.Write(Pattern[i].Length);

                for (int j = 0; j < Pattern[i].Length; j++)
                {
                    writer.Write(Pattern[i][j]);
                }
                
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

            int height = reader.ReadInt32();
            
            Pattern = new string[height][];
            for (int i = 0; i < Pattern.Length; i++)
            {
                int len = reader.ReadInt32();
                Pattern[i] = new string[len];

                for (int j = 0; j < len; j++)
                {
                    Pattern[i][j] = reader.ReadString();
                }
                
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
                    if (world.Blocks[i] == null) continue;

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
                    if (world.Items[i] == null) continue;
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



        public static bool WildCardMatch(AssetLocation wildCard, AssetLocation blockCode)
        {
            if (blockCode == null || !wildCard.Domain.Equals(blockCode.Domain)) return false;
            if (wildCard.Equals(blockCode)) return true;

            string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

            return Regex.IsMatch(blockCode.Path, @"^" + pattern + @"$");
        }




        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public override ClayFormingRecipe Clone()
        {
            ClayFormingRecipe recipe = new ClayFormingRecipe();

            recipe.Pattern = new string[Pattern.Length][];
            for (int i = 0; i < recipe.Pattern.Length; i++)
            {
                recipe.Pattern[i] = (string[])Pattern[i].Clone();
            }
            
            recipe.Ingredient = Ingredient.Clone();
            recipe.Output = Output.Clone();
            recipe.Name = Name;

            return recipe;
        }

    }
}
