using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EventMusicTrack : SurfaceMusicTrack
    {
        [JsonProperty]
        public string SchematicCode;

        public override void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine)
        {
            Priority = 3;
            base.Initialize(assetManager, capi, musicEngine);
        }

        public override bool ShouldPlay(TrackedPlayerProperties props, ClimateCondition conds, BlockPos pos)
        {
            if (IsActive || !ShouldPlayMusic) return false;
            if (capi.World.ElapsedMilliseconds < globalCooldownUntilMs) return false;
            if (musicEngine.LastPlayedTrack == this) return false;

            tracksCooldownUntilMs.TryGetValue(Name, out long trackCoolDownMs);
            if (capi.World.ElapsedMilliseconds < trackCoolDownMs)
            {
                return false;
            }

            if (SchematicCode != null)
            {
                int regionX = pos.X / capi.World.BlockAccessor.RegionSize;
                int regionZ = pos.Z / capi.World.BlockAccessor.RegionSize;
                var region = capi.World.BlockAccessor.GetMapRegion(regionX, regionZ);
                if (region == null) return false;

                bool isInStructure = false;
                foreach (var structure in region.GeneratedStructures)
                {
                    if (structure.Code.Contains(SchematicCode) && structure.Location.Contains(pos))
                    {
                        isInStructure = true;
                        break;
                    }
                }

                if (!isInStructure) return false;
            }

            return true;
        }
    }
}
