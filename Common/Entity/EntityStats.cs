using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    public class EntityStats : IEnumerable<KeyValuePair<string, EntityFloatStats>>, IEnumerable
    {
        IDictionary<string, EntityFloatStats> floatStats = new FastSmallDictionary<string, EntityFloatStats>(0);
        Entity entity;

        bool ignoreChange = false;
        public EntityStats(Entity entity)
        {
            this.entity = entity;
        }

        public void Initialize(ICoreAPI api)
        {
            // We only register the listener client-side, because only client-side will we call FromTreeAtttributes() in the onStatsChanged method
            if (api.Side == EnumAppSide.Client) entity.WatchedAttributes.RegisterModifiedListener("stats", onStatsChanged);
        }

        private void onStatsChanged()
        {
            if (!ignoreChange)
            {
                FromTreeAttributes(entity.WatchedAttributes);
            }
        }

        public EntityFloatStats this[string key]
        {
            get
            {
                return floatStats[key];
            }
            set
            {
                floatStats[key] = value;
            }
        }

        public IEnumerator<KeyValuePair<string, EntityFloatStats>> GetEnumerator()
        {
            return floatStats.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, EntityFloatStats>> IEnumerable<KeyValuePair<string, EntityFloatStats>>.GetEnumerator()
        {
            return floatStats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return floatStats.GetEnumerator();
        }





        public void ToTreeAttributes(ITreeAttribute tree, bool forClient)
        {
            if (floatStats.Count == 0) return;

            if (tree.TryGetAttribute("stats", out IAttribute attr) && attr is TreeAttribute statstree)
            {
                statstree.Clear();
            }
            else
            {
                statstree = new TreeAttribute();
                tree["stats"] = statstree;
            }

            foreach (var stats in floatStats)
            {
                TreeAttribute subtree = new TreeAttribute();            // radfast 14.3.25: costly to create a new TreeAttribute with its Dictionary for each floatStats, each time this method is called, but hey what can you do...
                stats.Value.ToTreeAttributes(subtree, forClient);
                statstree[stats.Key] = subtree;
            }
        }

        public void FromTreeAttributes(ITreeAttribute tree)
        {
            ITreeAttribute subtree = tree["stats"] as ITreeAttribute;
            if (subtree == null) return;

            foreach (var val in subtree)
            {
                EntityFloatStats stats = new EntityFloatStats();
                stats.FromTreeAttributes(val.Value as ITreeAttribute);
                floatStats[val.Key] = stats;
            }
        }

        /// <summary>
        /// Set up a stat. Its not required to call this method, you can go straight to doing .Set() if your blend type is weighted sum. Also initializes a base value of 1.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="blendType"></param>
        /// <returns></returns>
        public EntityStats Register(string category, EnumStatBlendType blendType = EnumStatBlendType.WeightedSum)
        {
            EntityFloatStats stats;
            floatStats[category] = stats = new EntityFloatStats();
            stats.BlendType = blendType;
            return this;
        }


        /// <summary>
        /// Set a stat value, if the stat catgory does not exist, it will create a new default one. Initializes a base value of 1 when creating a new stat.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="persistent"></param>
        /// <returns></returns>
        public EntityStats Set(string category, string code, float value, bool persistent = false)
        {
            ignoreChange = true;

            if (!floatStats.TryGetValue(category, out EntityFloatStats stats))
            {
                floatStats[category] = stats = new EntityFloatStats();
            }

            stats.Set(code, value, persistent);

            ToTreeAttributes(entity.WatchedAttributes, true);
            entity.WatchedAttributes.MarkPathDirty("stats");

            ignoreChange = false;

            return this;
        }

        /// <summary>
        /// Remove a stat value
        /// </summary>
        /// <param name="category"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public EntityStats Remove(string category, string code)
        {
            ignoreChange = true;

            if (floatStats.TryGetValue(category, out EntityFloatStats stats))
            {
                stats.Remove(code);
            }

            ToTreeAttributes(entity.WatchedAttributes, true);
            entity.WatchedAttributes.MarkPathDirty("stats");


            ignoreChange = false;

            return this;
        }

        /// <summary>
        /// Get the final stat value, blended by the stats blend type
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public float GetBlended(string category)
        {
            if (floatStats.TryGetValue(category, out EntityFloatStats stats))
            {
                return stats.GetBlended();
            }

            return 1;
        }
    }

    public enum EnumStatBlendType
    {
        FlatSum,
        FlatMultiply,
        WeightedSum,
        WeightedOverlay
    }

    public class EntityFloatStats
    {
        public Datastructures.OrderedDictionary<string, EntityStat<float>> ValuesByKey = new ();
        public EnumStatBlendType BlendType = EnumStatBlendType.WeightedSum;
        public EntityFloatStats()
        {
            ValuesByKey["base"] = new EntityStat<float>() { Value = 1, Persistent = true };
        }

        public float GetBlended()
        {
            float blended = 0;
            bool first = true;

            switch (BlendType)
            {
                case EnumStatBlendType.FlatMultiply:
                    foreach (var stat in ValuesByKey.Values)
                    {
                        if (first)
                        {
                            blended = stat.Value;
                            first = false;
                        }

                        blended *= stat.Value;
                    }
                    break;

                case EnumStatBlendType.FlatSum:
                    foreach (var stat in ValuesByKey.Values) blended += stat.Value;
                    break;

                case EnumStatBlendType.WeightedSum:
                    foreach (var stat in ValuesByKey.Values) blended += stat.Value * stat.Weight;
                    break;

                case EnumStatBlendType.WeightedOverlay:
                    foreach (var stat in ValuesByKey.Values)
                    {
                        if (first)
                        {
                            blended = stat.Value;
                            first = false;
                        }

                        blended = stat.Value * stat.Weight + blended * (1 - stat.Weight);
                    }
                    break;
            }

            return blended;
        }

        public void Set(string code, float value, bool persistent = false)
        {
            ValuesByKey[code] = new EntityStat<float>() { Value = value, Persistent = persistent };
        }
        public void Remove(string code)
        {
            ValuesByKey.Remove(code);
        }

        public void ToTreeAttributes(ITreeAttribute tree, bool forClient)
        {
            foreach (var stat in ValuesByKey)
            {
                if (!stat.Value.Persistent && !forClient) continue;
                tree.SetFloat(stat.Key, stat.Value.Value);
            }
        }

        public void FromTreeAttributes(ITreeAttribute tree)
        {
            foreach (var val in tree)
            {
                ValuesByKey[val.Key] = new EntityStat<float>()
                {
                    Value = (val.Value as FloatAttribute).value,
                    Persistent = true
                };
            }
        }
    }

    public class EntityStat<T>
    {
        public T Value;
        public float Weight = 1;
        public bool Persistent;
    }
}
