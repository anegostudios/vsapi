using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class PlayerHeadController : EntityHeadController
    {
        protected IPlayer player = null;
        EntityPlayer entityPlayer;
        protected bool turnOpposite;
        protected bool rotateTpYawNow;

        
        public PlayerHeadController(IAnimationManager animator, EntityPlayer entity, Shape entityShape) : base(animator, entity, entityShape)
        {
            this.entityPlayer = entity;
        }

        public override void OnFrame(float dt)
        {
            if (this.player == null) this.player = entityPlayer.Player;

            float diff = GameMath.AngleRadDistance(entity.BodyYaw, entity.Pos.Yaw);

            if (Math.Abs(diff) > GameMath.PIHALF * 1.2f) turnOpposite = true;
            if (turnOpposite)
            {
                if (Math.Abs(diff) < GameMath.PIHALF * 0.9f) turnOpposite = false;
                else diff = 0;
            }

            entity.HeadYaw += (diff - entity.HeadYaw) * dt * 6;
            entity.HeadYaw = GameMath.Clamp(entity.HeadYaw, -0.75f, 0.75f);

            entity.HeadPitch = GameMath.Clamp((entity.Pos.Pitch - GameMath.PI) * 0.75f, -1.2f, 1.2f);


            if (player?.Entity == null || player.Entity.MountedOn != null || (player as IClientPlayer).CameraMode == EnumCameraMode.Overhead)
            {
                entity.BodyYaw = entity.Pos.Yaw;
            }
            else
            {
                float yawDist = GameMath.AngleRadDistance(entity.BodyYaw, entity.Pos.Yaw);
                bool ismoving = player.Entity.Controls.TriesToMove || player.Entity.ServerControls.TriesToMove;

                bool attachedToClimbWall = false;// player.Entity.Controls.IsClimbing && player.Entity.Controls.IsClimbing && (entity.BodyYaw - player.Entity.ClimbingOnFace.HorizontalAngleIndex * 90 * GameMath.DEG2RAD) < 0.02;

                float threshold = 1f - (ismoving ? 0.99f : 0) + (attachedToClimbWall ? 3 : 0);
                ICoreClientAPI capi = entity.Api as ICoreClientAPI;

                if (player.PlayerUID == capi.World.Player.PlayerUID && capi.Settings.Bool["immersiveFpMode"] && false)
                {
                    entity.BodyYaw = entity.Pos.Yaw;
                }
                else
                {

                    if (Math.Abs(yawDist) > threshold || rotateTpYawNow)
                    {
                        float speed = 0.05f + Math.Abs(yawDist) * 3.5f;
                        entity.BodyYaw += GameMath.Clamp(yawDist, -dt * speed, dt * speed);
                        rotateTpYawNow = Math.Abs(yawDist) > 0.01f;
                    }
                }
            }

            base.OnFrame(dt);
        }
    }


    public class EntityHeadController
    {
        public ShapeElement HeadElement;
        public ShapeElement NeckElement;

        protected EntityAgent entity;
        protected IAnimationManager animManager;

        protected float[] HeadGlobalMatrix = null;
        protected float[] HeadGlobalMatrixInverted = null;
        protected float[] HeadLocalMatrix = null;
        protected float[] tmpMatrix = Mat4f.Create();


        public EntityHeadController(IAnimationManager animator, EntityAgent entity, Shape entityShape)
        {
            this.entity = entity;
            this.animManager = animator;

            HeadElement = entityShape.GetElementByName("head");

            HeadGlobalMatrix = Mat4f.Create();
            HeadGlobalMatrixInverted = Mat4f.Create();
            HeadLocalMatrix = Mat4f.Create();

            List<ShapeElement> elems = HeadElement.GetParentPath();

            for (int i = 0; i < elems.Count; i++)
            {
                ShapeElement elem = elems[i];
                float[] localTransform = elem.GetLocalTransformMatrix();
                Mat4f.Mul(HeadGlobalMatrix, HeadGlobalMatrix, localTransform);
            }

            Mat4f.Mul(HeadGlobalMatrix, HeadGlobalMatrix, HeadElement.GetLocalTransformMatrix());
            Mat4f.Invert(HeadGlobalMatrixInverted, HeadGlobalMatrix);

        }
        
        
        /// <summary>
        /// The event fired when the game ticks.
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnFrame(float dt)
        {
            if (entity.HeadYaw != 0 || entity.HeadPitch != 0)
            {

                Mat4f.Identity(HeadLocalMatrix);
                Mat4f.RotateY(HeadLocalMatrix, HeadLocalMatrix, entity.HeadYaw);
                Mat4f.RotateZ(HeadLocalMatrix, HeadLocalMatrix, entity.HeadPitch);

                ApplyTransformToElement(HeadLocalMatrix, HeadGlobalMatrix, HeadGlobalMatrixInverted, HeadElement);

            }
        }

        /// <summary>
        /// Applies the transformation to the head element of the entity.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="jointElement"></param>
        public virtual void ApplyTransformToElement(float[] matrix, float[] globMatrix, float[] globMatrixInverted, ShapeElement jointElement)
        {
            ApplyTransformToElements(matrix, globMatrix, globMatrixInverted, new ShapeElement[] { jointElement }, jointElement, jointElement.JointId);
        }


        protected virtual void ApplyTransformToElements(float[] matrix, float[] globMatrix, float[] globMatrixInverted, ShapeElement[] forElems, ShapeElement jointElement, int jointId)
        {
            float[] transformationMatrices = animManager.Animator.Matrices;

            for (int k = 0; k < forElems.Length; k++)
            {
                ShapeElement elem = forElems[k];

                if (elem == jointElement || elem.JointId != jointId)
                {

                    for (int i = 0; i < 16; i++)
                    {
                        tmpMatrix[i] = transformationMatrices[16 * elem.JointId + i];
                    }

                    float[] origin = new float[] {
                        (float)jointElement.RotationOrigin[0] / 16f,
                        (float)jointElement.RotationOrigin[1] / 16f,
                        (float)jointElement.RotationOrigin[2] / 16f
                    };

                    Mat4f.Mul(tmpMatrix, tmpMatrix, globMatrix);
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, matrix);
                    origin[0] = -origin[0];
                    origin[1] = -origin[1];
                    origin[2] = -origin[2];
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, globMatrixInverted);

                    for (int i = 0; i < 16; i++)
                    {
                        transformationMatrices[16 * elem.JointId + i] = tmpMatrix[i];
                    }
                }

                if (elem.Children != null)
                {
                    ApplyTransformToElements(matrix, globMatrix, globMatrixInverted, elem.Children, jointElement, jointId);
                }
            }
        }
    }
}

