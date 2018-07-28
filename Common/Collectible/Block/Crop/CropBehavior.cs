using System;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public abstract class CropBehavior
    {
        public Block block;

        public CropBehavior(Block block)
        {
            this.block = block;
        }

        public virtual void Initialize(JsonObject properties) { }

        public virtual bool TryGrowCrop(ICoreAPI api, IFarmlandBlockEntity farmland, double currentTotalHours, int newGrowthStage, ref EnumHandling handling)
        {
            handling = EnumHandling.NotHandled;
            return false;
        }

        public virtual void OnPlanted(ICoreAPI api)
        {
            
        }
        
    }
}