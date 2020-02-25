using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class ServerAnimator : ClientAnimator
    {
        public new static ServerAnimator CreateForEntity(Entity entity, List<ElementPose> rootPoses, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById)
        {
            if (entity is EntityAgent)
            {
                EntityAgent entityag = entity as EntityAgent;
                return new ServerAnimator(
                    () => entityag.Controls.MovespeedMultiplier * entityag.GetWalkSpeedMultiplier(0.3),
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            } else
            {
                return new ServerAnimator(
                    () => 1,
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            }
        }

        public new static ServerAnimator CreateForEntity(Entity entity, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById)
        {
            if (entity is EntityAgent)
            {
                EntityAgent entityag = entity as EntityAgent;

                return new ServerAnimator(
                    () => entityag.Controls.MovespeedMultiplier * entityag.GetWalkSpeedMultiplier(0.3),
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            } else {
                return new ServerAnimator(
                    () => 1,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            }
        }


        public ServerAnimator(WalkSpeedSupplierDelegate walkSpeedSupplier, Animation[] Animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, Action<string> onAnimationStoppedListener = null) : base(walkSpeedSupplier, Animations, rootElements, jointsById, onAnimationStoppedListener)
        {
            
        }


        public ServerAnimator(WalkSpeedSupplierDelegate walkSpeedSupplier, List<ElementPose> rootPoses, Animation[] Animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, Action<string> onAnimationStoppedListener = null) : base(walkSpeedSupplier, rootPoses, Animations, rootElements, jointsById, onAnimationStoppedListener)
        {
        }

        protected override void LoadedPosesAndAttachmentPoints(ShapeElement[] elements, List<ElementPose> intoPoses)
        {
            // Only load root pose and only the ones that have attachment points

            ElementPose pose;
            for (int i = 0; i < elements.Length; i++)
            {
                ShapeElement elem = elements[i];
                if (elem.AttachmentPoints == null) continue;

                intoPoses.Add(pose = new ElementPose());
                pose.AnimModelMatrix = Mat4f.Create();
                pose.ForElement = elem;

                for (int j = 0; j < elem.AttachmentPoints.Length; j++)
                {
                    AttachmentPoint apoint = elem.AttachmentPoints[j];
                    AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose() {
                        AttachPoint = apoint,
                        CachedPose = pose
                    };
                }
            }
        }
    }
}
