using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Util
{
    public delegate T CreateCachableObjectDelegate<T>();

    public static class ObjectCacheUtil
    {

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


    }
}
