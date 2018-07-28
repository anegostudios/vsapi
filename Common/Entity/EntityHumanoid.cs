using System;

namespace Vintagestory.API.Common
{
    public class EntityHumanoid : EntityAgent
    {
        public override double EyeHeight()
        {
            return controls.Sneak ? 1.5 : 1.6;
        }


        
    }
}
