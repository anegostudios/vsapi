using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public class EntityStats
    {
        Dictionary<string, EntityFloatStats> floatStats = new Dictionary<string, EntityFloatStats>();
        Entity entity;

        bool ignoreChange = false;
        public EntityStats(Entity entity)
        {
            this.entity = entity;
            entity.WatchedAttributes.RegisterModifiedListener("stats", onStatsChanged);
        }

        private void onStatsChanged()
        {
            if (entity.World?.Side == EnumAppSide.Client && !ignoreChange)
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

        public void ToTreeAttributes(ITreeAttribute tree, bool forClient)
        {
            TreeAttribute statstree = new TreeAttribute();

            foreach (var stats in floatStats)
            {
                TreeAttribute subtree = new TreeAttribute();
                stats.Value.ToTreeAttributes(subtree, forClient);
                statstree[stats.Key] = subtree;
            }

            tree["stats"] = statstree;
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


        public EntityStats Set(string category, string code, float value, bool persistent = false)
        {
            ignoreChange = true;

            EntityFloatStats stats;
            if (!floatStats.TryGetValue(category, out stats))
            {
                floatStats[category] = stats = new EntityFloatStats();
            }

            stats.Set(code, value, persistent);

            ToTreeAttributes(entity.WatchedAttributes, true);
            entity.WatchedAttributes.MarkPathDirty("stats");

            ignoreChange = false;

            return this;
        }

        public EntityStats Remove(string category, string code)
        {
            ignoreChange = true;

            EntityFloatStats stats = null;
            if (floatStats.TryGetValue(category, out stats))
            {
                stats.Remove(code);
            }

            ToTreeAttributes(entity.WatchedAttributes, true);
            entity.WatchedAttributes.MarkPathDirty("stats");


            ignoreChange = false;

            return this;
        }

        public float GetBlended(string category)
        {
            EntityFloatStats stats = null;
            if (floatStats.TryGetValue(category, out stats))
            {
                return stats.GetBlended();
            }

            return 1;
        }
    }

    public enum EnumBlendType
    {
        FlatSum,
        FlatMultiply,
        WeightedSum,
        WeightedOverlay
    }

    public class EntityFloatStats
    {
        public OrderedDictionary<string, EntityStat<float>> Stats = new OrderedDictionary<string, EntityStat<float>>();
        public EnumBlendType BlendType = EnumBlendType.WeightedSum;
        public EntityFloatStats()
        {
            Stats["base"] = new EntityStat<float>() { Value = 1, Persistent = true };
        }

        public float GetBlended()
        {
            float blended = 0;
            bool first = true; 

            switch (BlendType)
            {
                case EnumBlendType.FlatMultiply:
                    foreach (var stat in Stats.Values)
                    {
                        if (first)
                        {
                            blended = stat.Value;
                            first = false;
                        }

                        blended *= stat.Value;
                    }
                    break;

                case EnumBlendType.FlatSum:
                    foreach (var stat in Stats.Values) blended += stat.Value;
                    break;

                case EnumBlendType.WeightedSum:
                    foreach (var stat in Stats.Values) blended += stat.Value * stat.Weight;
                    break;

                case EnumBlendType.WeightedOverlay:
                    foreach (var stat in Stats.Values)
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
            Stats[code] = new EntityStat<float>() { Value = value, Persistent = persistent };
        }
        public void Remove(string code)
        {
            Stats.Remove(code);
        }

        public void ToTreeAttributes(ITreeAttribute tree, bool forClient)
        {
            foreach (var stat in Stats)
            {
                if (!stat.Value.Persistent && !forClient) continue;
                tree.SetFloat(stat.Key, stat.Value.Value);
            }
        }

        public void FromTreeAttributes(ITreeAttribute tree)
        {
            foreach (var val in tree)
            {
                Stats[val.Key] = new EntityStat<float>()
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
