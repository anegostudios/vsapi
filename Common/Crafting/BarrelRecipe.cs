using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{

    public class BarrelRecipe : RecipeBase<BarrelRecipe>, IByteSerializable
    {
        public string Code;
        public double SealHours;
        

        /// <summary>
        /// Gets the name of the output food if one exists.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="inputStacks"></param>
        /// <returns></returns>
        public string GetOutputName(IWorldAccessor worldForResolve, ItemStack[] inputStacks)
        {
            return Lang.Get("unknown");
        }



        public bool Matches(IWorldAccessor worldForResolve, ItemStack[] inputStacks, out int outputStackSize)
        {
            outputStackSize = 0;
            List<ItemStack> inputStacksList = new List<ItemStack>();
            foreach (var val in inputStacks) if (val != null) inputStacksList.Add(val);

            if (inputStacksList.Count != Ingredients.Length) return false;

            int outQuantityMul = -1;

            
            List<CraftingRecipeIngredient> ingredientList = new List<CraftingRecipeIngredient>(Ingredients);

            while (inputStacksList.Count > 0)
            {
                ItemStack inputStack = inputStacksList[0];
                inputStacksList.RemoveAt(0);
                if (inputStack == null) continue;

                bool found = false;
                for (int i = 0; !found && i < ingredientList.Count; i++)
                {
                    CraftingRecipeIngredient ingred = ingredientList[i];
                    
                    if (ingred.SatisfiesAsIngredient(inputStack))
                    {
                        // Input stack size must be equal or a multiple of the ingredient stack size
                        if ((inputStack.StackSize % ingred.Quantity) != 0) return false;
                        // All ingredients must be at the same ratio
                        if (outQuantityMul != -1 && outQuantityMul != inputStack.StackSize / ingred.Quantity) return false;

                        // First ingredient determines the desired ratio
                        outQuantityMul = inputStack.StackSize / ingred.Quantity;

                        found = true;
                        ingredientList.RemoveAt(i);
                    }
                }

                // This input stack does not fit in this cooking recipe
                if (!found) return false;
            }

            // We're missing ingredients
            if (ingredientList.Count > 0)
            {
                return false;
            }

            outputStackSize = Output.StackSize * outQuantityMul;


            return true;
        }

        
        
        


        


        /// <summary>
        /// Serialized the alloy
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Ingredients.Length);
            for (int i = 0; i < Ingredients.Length; i++)
            {
                Ingredients[i].ToBytes(writer);
            }

            Output.ToBytes(writer);

            writer.Write(SealHours);
        }

        /// <summary>
        /// Deserializes the alloy
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            Code = reader.ReadString();
            Ingredients = new CraftingRecipeIngredient[reader.ReadInt32()];

            for (int i = 0; i < Ingredients.Length; i++)
            {
                Ingredients[i] = new CraftingRecipeIngredient();
                Ingredients[i].FromBytes(reader, resolver);
                Ingredients[i].Resolve(resolver, "Barrel Recipe (FromBytes)");
            }

            Output = new JsonItemStack();
            Output.FromBytes(reader, resolver.ClassRegistry);
            Output.Resolve(resolver, "Barrel Recipe (FromBytes)");

            SealHours = reader.ReadDouble();
        }



        /// <summary>
        /// Resolves Wildcards in the ingredients
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public override Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world)
        {
            Dictionary<string, string[]> mappings = new Dictionary<string, string[]>();

            if (Ingredients == null || Ingredients.Length == 0) return mappings;

            foreach (var ingred in Ingredients)
            {
                if (!ingred.Code.Path.Contains("*")) continue;

                int wildcardStartLen = ingred.Code.Path.IndexOf("*");
                int wildcardEndLen = ingred.Code.Path.Length - wildcardStartLen - 1;

                List<string> codes = new List<string>();

                if (ingred.Type == EnumItemClass.Block)
                {
                    for (int i = 0; i < world.Blocks.Count; i++)
                    {
                        if (world.Blocks[i] == null || world.Blocks[i].IsMissing) continue;

                        if (WildcardUtil.Match(ingred.Code, world.Blocks[i].Code))
                        {
                            string code = world.Blocks[i].Code.Path.Substring(wildcardStartLen);
                            string codepart = code.Substring(0, code.Length - wildcardEndLen);
                            if (ingred.AllowedVariants != null && !ingred.AllowedVariants.Contains(codepart)) continue;

                            codes.Add(codepart);

                        }
                    }
                }
                else
                {
                    for (int i = 0; i < world.Items.Count; i++)
                    {
                        if (world.Items[i] == null || world.Items[i].IsMissing) continue;

                        if (WildcardUtil.Match(ingred.Code, world.Items[i].Code))
                        {
                            string code = world.Items[i].Code.Path.Substring(wildcardStartLen);
                            string codepart = code.Substring(0, code.Length - wildcardEndLen);
                            if (ingred.AllowedVariants != null && !ingred.AllowedVariants.Contains(codepart)) continue;

                            codes.Add(codepart);
                        }
                    }
                }

                mappings[ingred.Name] = codes.ToArray();
            }

            return mappings;
        }



        public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
        {
            bool ok = true;

            for (int i = 0; i < Ingredients.Length; i++)
            {
                ok &= Ingredients[i].Resolve(world, sourceForErrorLogging);
            }

            ok &= Output.Resolve(world, sourceForErrorLogging);

            return ok;
        }

        public override BarrelRecipe Clone()
        {
            CraftingRecipeIngredient[] ingredients = new CraftingRecipeIngredient[Ingredients.Length];
            for (int i = 0; i < Ingredients.Length; i++)
            {
                ingredients[i] = Ingredients[i].Clone();
            }

            return new BarrelRecipe()
            {
                SealHours = SealHours,
                Output = Output.Clone(),
                Code = Code,
                Enabled = Enabled,
                Name = Name,
                RecipeId = RecipeId,
                Ingredients = ingredients
            };
        }
    }
    
}
