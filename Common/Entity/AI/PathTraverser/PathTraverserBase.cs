using System;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public abstract class PathTraverserBase
    {
        protected EntityAgent entity;
        protected Vec3d target;
        public Action OnGoalReached;
        public Action OnStuck;
        protected int stuckCounter;

        public bool Active;

        protected float movingSpeed;
        

        protected float targetDistance;

        public Vec3d CurrentTarget
        {
            get { return target; }
        }

        public PathTraverserBase(EntityAgent entity)
        {
            this.entity = entity;
        }

        public bool GoTo(Vec3d target, float movingSpeed, Action OnGoalReached, Action OnStuck)
        {
            return GoTo(target, movingSpeed, 0.12f, OnGoalReached, OnStuck);
        }

        public bool GoTo(Vec3d target, float movingSpeed, float targetDistance, Action OnGoalReached, Action OnStuck)
        {
            stuckCounter = 0;

            this.OnGoalReached = OnGoalReached;
            this.OnStuck = OnStuck;
            this.movingSpeed = movingSpeed;
            this.targetDistance = targetDistance;
            this.target = target;
            Active = true;
            return BeginGo();
        }

        public virtual void OnGameTick(float dt)
        {

        }

        protected abstract bool BeginGo();
        public abstract void Stop();
    }
}
