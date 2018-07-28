using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalAvoidPlayer : AIGoal
    {
        public GoalAvoidPlayer(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
