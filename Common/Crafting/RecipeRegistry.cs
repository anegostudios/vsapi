using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class RecipeRegistryGeneric<T> : RecipeRegistryBase where T : IByteSerializable, new()
    {
        public List<T> Recipes;

        public RecipeRegistryGeneric()
        {
            Recipes = new List<T>();
        }

        public RecipeRegistryGeneric(List<T> recipes)
        {
            this.Recipes = recipes;
        }

        public override void FromBytes(IWorldAccessor resolver, int quantity, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(ms);

                for (int j = 0; j < quantity; j++)
                {
                    T rec = new T();
                    rec.FromBytes(reader, resolver);
                    Recipes.Add(rec);
                }
            }
        }

        public override void ToBytes(IWorldAccessor resolver, out byte[] data, out int quantity)
        {
            quantity = Recipes.Count;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms);

                foreach (T recipe in Recipes)
                {
                    recipe.ToBytes(writer);
                }

                data = ms.ToArray();
            }
        }
    }

    public abstract class RecipeRegistryBase
    {
        public abstract void ToBytes(IWorldAccessor resolver, out byte[] data, out int quantity);

        public abstract void FromBytes(IWorldAccessor resolver, int quantity, byte[] data);
    }

}
