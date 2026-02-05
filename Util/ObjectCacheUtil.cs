using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Util
{
    public delegate T CreateCachableObjectDelegate<T>();

    public static class ObjectCacheUtil
    {
        public static T? TryGet<T>(ICoreAPI api, string key)
        {
            if (api.ObjectCache.TryGetValue(key, out object? obj))
            {
                return (T)obj;
            }
            return default(T);
        }

        public static T GetOrCreate<T>(ICoreAPI api, string key, CreateCachableObjectDelegate<T> onRequireCreate)
        {
            if (!api.ObjectCache.TryGetValue(key, out object? obj) || obj == null)
            {
                T typedObj = onRequireCreate();
                api.ObjectCache[key] = typedObj;
                return typedObj;
            }
            else
            {
                return (T)obj;
            }
        }


        public static bool Delete(ICoreAPI api, string key)
        {
            if (key == null) return false;
            return api.ObjectCache.Remove(key);
        }


        public static ItemStack[] GetToolStacks(ICoreAPI api, EnumTool tool)
        {
            return GetOrCreate<ItemStack[]>(api, tool.ToString()!.ToLowerInvariant() + "ToolStacks", () =>
            {
                return [..api.World.Items.Where(item => item.Tool == tool)
                                         .Select(item => new ItemStack(item))];
            });
        }
    }
}
