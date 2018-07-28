using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalAvoidPredators : AIGoal
    {
        public GoalAvoidPredators(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
