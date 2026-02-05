
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Caches results of wildcard pre search among collectibles.
    /// Use it to get list of collectibles before applying wildcard search, to reduce number of wildcard evaluations on collectibles codes.
    /// </summary>
    public sealed class CollectiblePreSearchResultsCache
    {
        public List<CollectibleObject> GetOrCreate(IWorldAccessor world, AssetLocation code, EnumItemClass collectibleType)
        {
            string codeStartsWith = getCodeBeforeWildcard(code.Path);
            string cacheKey = $"{code.Domain}:{codeStartsWith}:{collectibleType}";

            List<CollectibleObject> collectibles = cache.GetOrAdd(cacheKey, _ => search(world, collectibleType, codeStartsWith));

            return collectibles;
        }

        public void Clear() => cache.Clear();

        private readonly ConcurrentDictionary<string, List<CollectibleObject>> cache = new();

        private static string getCodeBeforeWildcard(string codePath)
        {
            if (string.IsNullOrEmpty(codePath))
            {
                return codePath;
            }

            int wildcardStartIndex = codePath.IndexOfAny(['*', '{', '?']);
            if (wildcardStartIndex >= 0)
            {
                return codePath.Substring(0, wildcardStartIndex);
            }

            return codePath;
        }

        private static List<CollectibleObject> search(IWorldAccessor world, EnumItemClass collectibleType, string codeStartsWith)
        {
            IEnumerable<CollectibleObject> collectibles = collectibleType switch
            {
                EnumItemClass.Block => world.Blocks,
                EnumItemClass.Item => world.Items,
                _ => throw new NotImplementedException()
            };

            return collectibles
                .Where(collectible => !collectible.IsMissing)
                .Where(collectible => collectible.Code?.Path != null && collectible.Code.Path.Contains(codeStartsWith))
                .ToList();
        }
    }
}
