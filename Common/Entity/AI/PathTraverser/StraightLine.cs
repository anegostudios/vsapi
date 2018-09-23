using System;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class StraightLinePathTraverser : PathTraverserBase
    {
        float minTurnAnglePerSec;
        float maxTurnAnglePerSec;
        float curTurnRadPerSec;
        Vec3f targetVec = new Vec3f();

        public StraightLinePathTraverser(EntityAgent entity) : base(entity)
        {
            if (entity?.Properties.Server?.Attributes != null)
            {
                minTurnAnglePerSec = (float)entity.Properties.Server.Attributes.GetTreeAttribute("pathfinder").GetFloat("minTurnAnglePerSec", 250);
                maxTurnAnglePerSec = (float)entity.Properties.Server.Attributes.GetTreeAttribute("pathfinder").GetFloat("maxTurnAnglePerSec", 450);
            } else
            {
                minTurnAnglePerSec = 250;
                maxTurnAnglePerSec = 450;
            }
        }

        protected override bool BeginGo()
        {
            entity.Controls.Forward = true;
            curTurnRadPerSec = minTurnAnglePerSec + (float)entity.World.Rand.NextDouble() * (maxTurnAnglePerSec - minTurnAnglePerSec);
            curTurnRadPerSec *= GameMath.DEG2RAD * 50 * movingSpeed;

            stuckCounter = 0;
            
            return true;
        }



        public override void OnGameTick(float dt)
        {
            if (!Active) return;

            // For land dwellers only check horizontal distance
            double sqDistToTarget = 
                entity.Properties.Habitat == API.Common.EnumHabitat.Land ?
                    target.SquareDistanceTo(entity.ServerPos.X, target.Y, entity.ServerPos.Z) :
                    target.SquareDistanceTo(entity.ServerPos.X, entity.ServerPos.Y, entity.ServerPos.Z)
                ;


            if (sqDistToTarget < targetDistance * targetDistance)
            {
                Stop();
                OnGoalReached?.Invoke();
                return;
            }

            bool stuck =
                (entity.CollidedVertically && entity.Controls.IsClimbing) ||
                (entity.ServerPos.Motion.LengthSq() < 0.001 * 0.001) ||
                (entity.CollidedHorizontally && entity.ServerPos.Motion.Y <= 0)
            ;


            stuckCounter = stuck ? (stuckCounter + 1) : 0;
            //stuckCounter = 0;
            //if (entity.Controls.IsClimbing && stuckCounter > 0) Console.WriteLine(entity.EntityId + ":" + stuckCounter);
            if (stuckCounter > 20)
            {
                Stop();
                OnStuck?.Invoke();
                return;
            }           


            EntityControls controls = entity.MountedOn == null ? entity.Controls : entity.MountedOn.Controls;
            if (controls == null) return;

            targetVec.Set(
                (float)(target.X - entity.ServerPos.X),
                (float)(target.Y - entity.ServerPos.Y),
                (float)(target.Z - entity.ServerPos.Z)
            );

            float desiredYaw = 0;
            
            if (sqDistToTarget >= 0.01)
            {
                desiredYaw = (float)Math.Atan2(targetVec.X, targetVec.Z);
            }



            float yawDist = GameMath.AngleRadDistance(entity.ServerPos.Yaw, desiredYaw);
            entity.ServerPos.Yaw += GameMath.Clamp(yawDist, -curTurnRadPerSec * dt, curTurnRadPerSec * dt);
            entity.ServerPos.Yaw = entity.ServerPos.Yaw % GameMath.TWOPI;

            

            double cosYaw = Math.Cos(entity.ServerPos.Yaw);
            double sinYaw = Math.Sin(entity.ServerPos.Yaw);
            controls.WalkVector.Set(sinYaw, GameMath.Clamp(targetVec.Y, -1, 1), cosYaw);
            controls.WalkVector.Mul(movingSpeed);

            // Make it walk along the wall, but not walk into the wall, which causes it to climb
            if (entity.Properties.RotateModelOnClimb && entity.Controls.IsClimbing && entity.ClimbingOnFace != null)
            {
                BlockFacing facing = entity.ClimbingOnFace;
                if (Math.Sign(facing.Normali.X) == Math.Sign(controls.WalkVector.X))
                {
                    controls.WalkVector.X = 0;
                }

                if (Math.Sign(facing.Normali.Z) == Math.Sign(controls.WalkVector.Z))
                {
                    controls.WalkVector.Z = 0;
                }
            }

         //   entity.World.SpawnParticles(0.3f, ColorUtil.WhiteAhsl, target, target, new Vec3f(), new Vec3f(), 0.1f, 0.1f, 3f, EnumParticleModel.Cube);


            if (entity.Swimming)
            {
                controls.FlyVector.Set(controls.WalkVector);
                controls.FlyVector.Mul(0.7f);
                if (entity.CollidedHorizontally)
                {
                    controls.FlyVector.Y = -0.05f;
                }
            }
        }

        public override void Stop()
        {
            Active = false;
            entity.Controls.Forward = false;
            entity.Controls.WalkVector.Set(0, 0, 0);
            stuckCounter = 0;
        }
        
    }
}
