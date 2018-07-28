using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalFleeWhenHurt : AIGoal
    {
        public GoalFleeWhenHurt(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
