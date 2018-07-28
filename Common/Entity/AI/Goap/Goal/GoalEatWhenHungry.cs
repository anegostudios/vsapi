using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class GoalEatWhenHungry : AIGoal
    {
        public GoalEatWhenHungry(BehaviorGoalOriented behavior, IWorldAccessor world) : base(behavior, world)
        {
        }
    }
}
