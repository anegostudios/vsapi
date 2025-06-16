using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class ServerAnimator : ClientAnimator
    {

        public static ServerAnimator CreateForEntity(Entity entity, List<ElementPose> rootPoses, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, bool requirePosesOnServer)
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
                    entity.AnimManager.TriggerAnimationStopped,
                    requirePosesOnServer
                );
            } else
            {
                return new ServerAnimator(
                    () => 1,
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    entity.AnimManager.TriggerAnimationStopped,
                    requirePosesOnServer
                );
            }
        }

        public static ServerAnimator CreateForEntity(Entity entity, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, bool requirePosesOnServer)
        {
            ServerAnimator animator;
            if (entity is EntityAgent)
            {
                EntityAgent entityag = entity as EntityAgent;

                animator = new ServerAnimator(
                    () => entityag.Controls.MovespeedMultiplier * entityag.GetWalkSpeedMultiplier(0.3),
                    animations,
                    rootElements,
                    jointsById,
                    entity.AnimManager.TriggerAnimationStopped,
                    requirePosesOnServer
                );
            } else {
                animator = new ServerAnimator(
                    () => 1,
                    animations,
                    rootElements,
                    jointsById,
                    entity.AnimManager.TriggerAnimationStopped,
                    requirePosesOnServer
                );
            }

            animator.entityForLogging = entity;
            return animator;
        }


        public ServerAnimator(
            WalkSpeedSupplierDelegate walkSpeedSupplier,
            Animation[] animations,
            ShapeElement[] rootElements,
            Dictionary<int, AnimationJoint> jointsById,
            Action<string> onAnimationStoppedListener = null,
            bool loadFully = false
        ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.RootElements = rootElements;
            this.jointsById = jointsById;

            RootPoses = new List<ElementPose>();
            LoadPosesAndAttachmentPoints(rootElements, RootPoses);
            initFields();
        }


        public ServerAnimator(
            WalkSpeedSupplierDelegate walkSpeedSupplier,
            List<ElementPose> rootPoses,
            Animation[] animations,
            ShapeElement[] rootElements,
            Dictionary<int, AnimationJoint> jointsById,
            Action<string> onAnimationStoppedListener = null,
            bool loadFully = false
        ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.RootElements = rootElements;
            this.jointsById = jointsById;
            this.RootPoses = rootPoses;

            LoadAttachmentPoints(RootPoses);
            initFields();
        }

        protected override void LoadPosesAndAttachmentPoints(ShapeElement[] elements, List<ElementPose> intoPoses)
        {
            // Tyron Oct 3, 2024: We used to only load root poses and root attachment points here server side
            // Its just not feasible anymore with the Center AP being at non-root elements and also its inconsistent with the Client side
            // And also this optimization does not really help much i think. We only calculate matrices server side on dead creatures anyway
            // So the loadFully argument is now obsolete
            base.LoadPosesAndAttachmentPoints(elements, intoPoses);
        }
    }
}
