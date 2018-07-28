using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalKillPlayer : AIGoal
    {
        public GoalKillPlayer(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
