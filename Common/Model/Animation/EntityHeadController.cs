using System;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public interface IHeadController
    {
        float YawOffset { get; set; }
        float PitchOffset { get; set; }

        void OnFrame(float dt);
    }

    public class PlayerHeadController : EntityHeadController
    {
        protected IClientPlayer? player => entityPlayer.Player as IClientPlayer;
        protected readonly EntityPlayer entityPlayer;
        protected ICoreClientAPI clientApi;

        protected bool turnOpposite;
        protected bool rotateTpYawNow;

        protected float upperTorsoYOffsetFactor = 0.3f;
        protected float upperTorsoZOffsetFactor = 0.2f;
        protected float lowerTorsoZOffsetFactor = 0.1f;
        protected float upperFootROffsetFactor = 0.1f;
        protected float upperFootLOffsetFactor = 0.1f;

        protected float bodyFollowSpeedFactor = 1.0f;
        protected float bodyFollowThresholdDeg = 69f;
        protected float headYawLimitsDeg = 43f;
        protected float headPitchLimitsDeg = 69f;
        protected float headFollowEntityPitchFactor = 0.75f;

        protected readonly ElementPose upperTorsoPose;
        protected readonly ElementPose lowerTorsoPose;
        protected readonly ElementPose upperFootLPose;
        protected readonly ElementPose upperFootRPose;

        public PlayerHeadController(IAnimationManager animator, EntityPlayer entity, Shape entityShape) : base(animator, entity, entityShape)
        {
            entityPlayer = entity;
            clientApi = entity.Api as ICoreClientAPI ?? throw new InvalidOperationException("PlayerHeadController have to be created client side");

            upperTorsoPose = GetPose(entity.GetBoneName("upperTorsoBoneName", "UpperTorso"));
            lowerTorsoPose = GetPose(entity.GetBoneName("lowerTorsoBoneName", "LowerTorso"));
            upperFootRPose = GetPose(entity.GetBoneName("upperFootRBoneName", "UpperFootR"));
            upperFootLPose = GetPose(entity.GetBoneName("upperFootLBoneName", "UpperFootL"));
        }

        public override void OnFrame(float dt)
        {
            if (player == null) return;

            upperTorsoPose.degOffY = 0;
            upperTorsoPose.degOffZ = 0;
            lowerTorsoPose.degOffZ = 0;
            upperFootRPose.degOffZ = 0;
            upperFootLPose.degOffZ = 0;

            if (!IsSelf())
            {
                base.OnFrame(dt);

                if (entity.BodyYawServer == 0) // Why?
                {
                    entity.BodyYaw = entity.Pos.Yaw;
                }

                return;
            }

            if (clientApi.Input.MouseGrabbed)
            {
                AdjustAngles(dt);
            }

            base.OnFrame(dt);

            SetTorsoOffsets(dt);
        }

        protected virtual void AdjustAngles(float dt)
        {
            if (player == null) return;

            EnumCameraMode cameraMode = player.CameraMode;
            EnumMountAngleMode mountAngleMode = player.Entity.MountedOn?.AngleMode ?? EnumMountAngleMode.Unaffected;
            bool bodyFollowExact = mountAngleMode == EnumMountAngleMode.Fixate || mountAngleMode == EnumMountAngleMode.FixateYaw || cameraMode == EnumCameraMode.Overhead || cameraMode == EnumCameraMode.FirstPerson;

            AdjustHeadAngles(cameraMode, dt);

            if (bodyFollowExact)
            {
                entity.BodyYaw = entity.Pos.Yaw;
            }
            else
            {
                AdjustBodyAngles(dt);
            }
        }

        protected virtual void AdjustHeadAngles(EnumCameraMode cameraMode, float dt) // @REFACTOR
        {
            float diff = GameMath.AngleRadDistance(entity.BodyYaw, entity.Pos.Yaw);

            if (Math.Abs(diff) > GameMath.PIHALF * 1.2f)
            {
                turnOpposite = true;
            }

            if (turnOpposite)
            {
                if (Math.Abs(diff) < GameMath.PIHALF * 0.9f)
                {
                    turnOpposite = false;
                }
                else
                {
                    diff = 0;
                }
            }

            bool overheadLookAtMode = clientApi.Settings.Bool["overheadLookAt"] && cameraMode == EnumCameraMode.Overhead;
            if (overheadLookAtMode)
            {
                TryLookIntoCamera(dt);
                return;
            }

            entity.Pos.HeadYaw += (diff - entity.Pos.HeadYaw) * dt * 6;
            entity.Pos.HeadYaw = GameMath.Clamp(entity.Pos.HeadYaw, -headYawLimitsDeg * GameMath.DEG2RAD, headYawLimitsDeg * GameMath.DEG2RAD);
            entity.Pos.HeadPitch = GameMath.Clamp(
                (entity.Pos.Pitch - GameMath.PI) * headFollowEntityPitchFactor,
                -headPitchLimitsDeg * GameMath.DEG2RAD,
                headPitchLimitsDeg * GameMath.DEG2RAD);
        }

        protected virtual void TryLookIntoCamera(float dt)
        {
            float yawDistance = -GameMath.AngleRadDistance(clientApi.Input.MouseYaw, entity.Pos.Yaw);
            float targetHeadYaw = GameMath.PI + yawDistance;
            float targetPitch = GameMath.Clamp(-entity.Pos.Pitch - GameMath.PI + GameMath.TWOPI, -1, +0.8f);

            if (targetHeadYaw > GameMath.PI) targetHeadYaw -= GameMath.TWOPI;

            float pitchOffset = 0;

            if (targetHeadYaw < -1f || targetHeadYaw > 1f)
            {
                targetHeadYaw = 0;
                pitchOffset = (GameMath.Clamp((entity.Pos.Pitch - GameMath.PI) * 0.75f, -1.2f, 1.2f) - entity.Pos.HeadPitch) * dt * 6;
            }
            else
            {
                pitchOffset = (targetPitch - entity.Pos.HeadPitch) * dt * 6;
            }

            entity.Pos.HeadPitch += pitchOffset;
            entity.Pos.HeadYaw += (targetHeadYaw - entity.Pos.HeadYaw) * dt * 6;
        }

        protected virtual void AdjustBodyAngles(float dt)
        {
            if (!entityPlayer.Alive || player == null) return;

            bool isMoving = player.Entity.Controls.TriesToMove || player.Entity.ServerControls.TriesToMove;
            float threshold = isMoving ? 0.01f : bodyFollowThresholdDeg * GameMath.DEG2RAD;
            if (entity.Controls.Gliding) threshold = 0;

            float yawDistance = GameMath.AngleRadDistance(entity.BodyYaw, entity.Pos.Yaw);

            if (Math.Abs(yawDistance) > threshold || rotateTpYawNow)
            {
                float speed = 0.05f + Math.Abs(yawDistance) * 3.5f * bodyFollowSpeedFactor;
                entity.BodyYaw += GameMath.Clamp(yawDistance, -dt * speed, dt * speed);
                rotateTpYawNow = Math.Abs(yawDistance) > 0.01f;
            }
        }

        protected virtual void SetTorsoOffsets(float dt)
        {
            if (!IsSelfImmersiveFirstPerson()) return;

            (float yOffsetDeg, float zOffsetDeg) = GetOffsets(dt);

            upperTorsoPose.degOffZ = zOffsetDeg * upperTorsoYOffsetFactor;
            upperTorsoPose.degOffY = yOffsetDeg * upperTorsoZOffsetFactor;
            lowerTorsoPose.degOffZ = zOffsetDeg * lowerTorsoZOffsetFactor;
            upperFootRPose.degOffZ = -zOffsetDeg * upperFootROffsetFactor;
            upperFootLPose.degOffZ = -zOffsetDeg * upperFootLOffsetFactor;
        }

        protected bool IsSelf()
        {
            return clientApi?.World.Player.PlayerUID == player?.PlayerUID;
        }

        protected bool IsSelfImmersiveFirstPerson()
        {
            return IsSelf() && player?.ImmersiveFpMode == true;
        }
    }

    public class EntityHeadController : IHeadController
    {
        public float YawOffset { get; set; } = 0;
        public float PitchOffset { get; set; } = 0;

        protected readonly EntityAgent entity;
        protected readonly IAnimationManager animationManager;
        protected readonly ElementPose headPose;
        protected readonly ElementPose neckPose;

        protected float headYOffsetFactor = 0.45f;
        protected float headZOffsetFactor = 0.35f;
        protected float neckYOffsetFactor = 0.35f;
        protected float neckZOffsetFactor = 0.4f;

        public EntityHeadController(IAnimationManager animationManager, EntityAgent entity, Shape entityShape)
        {
            this.entity = entity;
            this.animationManager = animationManager;

            headPose = GetPose("Head");
            neckPose = GetPose("Neck");
        }

        /// <summary>
        /// Called from AnimationManager each frame before frame.
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnFrame(float dt)
        {
            headPose.degOffY = 0;
            headPose.degOffZ = 0;
            neckPose.degOffZ = 0;

            if (entity.Pos.HeadYaw == 0 && entity.Pos.HeadPitch == 0) return;

            (float yOffsetDeg, float zOffsetDeg) = GetOffsets(dt);

            headPose.degOffY = yOffsetDeg * headYOffsetFactor;
            headPose.degOffZ = zOffsetDeg * headZOffsetFactor;
            neckPose.degOffY = yOffsetDeg * neckYOffsetFactor;
            neckPose.degOffZ = zOffsetDeg * neckZOffsetFactor;
        }

        protected virtual (float yOffsetDeg, float zOffsetDeg) GetOffsets(float dt) => ((entity.Pos.HeadYaw + YawOffset) * GameMath.RAD2DEG, (entity.Pos.HeadPitch + PitchOffset) * GameMath.RAD2DEG);

        protected ElementPose GetPose(string name) => animationManager.Animator.GetPosebyName(name) ?? throw new InvalidOperationException($"[Head Controller] Entity '{entity.Code}' shape does not have '{name}' element.");
    }
}

