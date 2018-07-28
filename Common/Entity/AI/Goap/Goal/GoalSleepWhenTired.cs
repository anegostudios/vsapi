using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalSleepWhenTired : AIGoal
    {
        public GoalSleepWhenTired(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
