using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class AIGoal
    {
        IWorldAccessor world;
        BehaviorGoalOriented behavior;

        public AIGoal(BehaviorGoalOriented behavior, IWorldAccessor world)
        {
            this.world = world;
            this.behavior = behavior;
        }

        public virtual bool ShouldExecute()
        {
            return false;
        }
    }
}
