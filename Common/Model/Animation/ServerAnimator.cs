using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;

#nullable enable

namespace Vintagestory.API.Common;

public class ServerAnimator : ClientAnimator
{
    public static ServerAnimator CreateForEntity(Entity entity, List<ElementPose> rootPoses, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, bool requirePosesOnServer)
    {
        return entity is EntityAgent entityAgent
            ? new ServerAnimator(
                () => entityAgent.Controls.MovespeedMultiplier * entityAgent.GetWalkSpeedMultiplier(0.3),
                rootPoses,
                animations,
                rootElements,
                jointsById,
                entity.AnimManager.TriggerAnimationStopped,
                requirePosesOnServer
            )
            : new ServerAnimator(
                () => 1,
                rootPoses,
                animations,
                rootElements,
                jointsById,
                entity.AnimManager.TriggerAnimationStopped,
                requirePosesOnServer
            );
    }

    public static ServerAnimator CreateForEntity(Entity entity, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, bool requirePosesOnServer)
    {
        ServerAnimator animator = entity is EntityAgent entityAgent
            ? new ServerAnimator(
                () => entityAgent.Controls.MovespeedMultiplier * entityAgent.GetWalkSpeedMultiplier(0.3),
                animations,
                rootElements,
                jointsById,
                entity.AnimManager.TriggerAnimationStopped,
                requirePosesOnServer
            )
            : new ServerAnimator(
                () => 1,
                animations,
                rootElements,
                jointsById,
                entity.AnimManager.TriggerAnimationStopped,
                requirePosesOnServer
            );
        animator.entityForLogging = entity;
        return animator;
    }

    public ServerAnimator(
        WalkSpeedSupplierDelegate walkSpeedSupplier,
        Animation[] animations,
        ShapeElement[] rootElements,
        Dictionary<int, AnimationJoint> jointsById,
        Action<string>? onAnimationStoppedListener = null,
        bool loadFully = false
    ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
    {
        RootElements = rootElements;
        this.jointsById = jointsById;

        RootPoses = new List<ElementPose>();
        LoadPosesAndAttachmentPoints(rootElements, RootPoses);
        InitFields();
    }

    public ServerAnimator(
        WalkSpeedSupplierDelegate walkSpeedSupplier,
        List<ElementPose> rootPoses,
        Animation[] animations,
        ShapeElement[] rootElements,
        Dictionary<int, AnimationJoint> jointsById,
        Action<string>? onAnimationStoppedListener = null,
        bool loadFully = false
    ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
    {
        RootElements = rootElements;
        this.jointsById = jointsById;
        RootPoses = rootPoses;

        LoadAttachmentPoints(RootPoses);
        InitFields();
    }

    protected override void LoadPosesAndAttachmentPoints(ShapeElement[] elements, List<ElementPose> intoPoses)
    {
        // Tyron Oct 3, 2024: We used to only load root poses and root attachment points here server side.
        // Its just not feasible anymore with the Center AP being at non-root elements and also its inconsistent with the Client side
        // and also this optimization does not really help much I think. We only calculate matrices server side on dead creatures anyway
        // so the loadFully argument is now obsolete.
        base.LoadPosesAndAttachmentPoints(elements, intoPoses);
    }
}
