using Vintagestory.API.Common;

namespace Vintagestory.API.Util
{
    public delegate T CreateCachableObjectDelegate<T>();

    public static class ObjectCacheUtil
    {
        public static T TryGet<T>(ICoreAPI api, string key)
        {
            object obj;
            if (api.ObjectCache.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            return default(T);
        }

        public static T GetOrCreate<T>(ICoreAPI api, string key, CreateCachableObjectDelegate<T> onRequireCreate)
        {
            object obj;
            if (!api.ObjectCache.TryGetValue(key, out obj) || obj == null)
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
            return api.ObjectCache.Remove(key);
        }

    }
}
