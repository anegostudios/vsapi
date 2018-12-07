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

        public PlayerHeadController(IAnimationManager animator, EntityPlayer entity, Shape entityShape) : base(animator, entity, entityShape)
        {
            this.entityPlayer = entity;
        }

        public override void OnTick(float dt)
        {
            if (this.player == null) this.player = entityPlayer.Player;

            float diff = GameMath.AngleRadDistance(entity.BodyYaw, entity.Pos.Yaw);

            if (Math.Abs(diff) > GameMath.PIHALF * 1.2f) opposite = true;
            if (opposite)
            {
                if (Math.Abs(diff) < GameMath.PIHALF * 0.9f) opposite = false;
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
                if (Math.Abs(yawDist) > 1f - (ismoving ? 0.99f : 0) || rotateTpYawNow)
                {
                    entity.BodyYaw += GameMath.Clamp(yawDist, -dt * 3, dt * 3);
                    rotateTpYawNow = Math.Abs(yawDist) > 0.01f;
                }
            }

            base.OnTick(dt);
        }
    }


    public class EntityHeadController
    {
        public ShapeElement HeadElement;

        protected bool rotateTpYawNow;
        protected float[] HeadGlobalMatrix = null;
        protected float[] HeadGlobalMatrixInverted = null;
        protected float[] HeadLocalMatrix = null;

       
        protected bool opposite;

        protected EntityAgent entity;
        
        protected IAnimationManager animManager;

        public float[] tmpMatrix = Mat4f.Create();
        //public float[] IdentityMatrix = Mat4f.Create();
        

        public EntityHeadController(IAnimationManager animator, EntityAgent entity, Shape entityShape)
        {
            this.entity = entity;
            this.animManager = animator;

            if (entityShape.Elements != null)
            {
                HeadElement = FindHead(entityShape.Elements);
            }

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
        


        ShapeElement FindHead(ShapeElement[] elements)
        {
            foreach (ShapeElement elem in elements)
            {
                if (elem.Name.ToLowerInvariant() == "head") return elem;
                if (elem.Children != null)
                {
                    ShapeElement foundElem = FindHead(elem.Children);
                    if (foundElem != null) return foundElem;
                }

            }
            return null;
        }

        
        public virtual void OnTick(float dt)
        {
            if (entity.HeadYaw != 0 || entity.HeadPitch != 0)
            {
                Mat4f.Identity(HeadLocalMatrix);
                Mat4f.RotateY(HeadLocalMatrix, HeadLocalMatrix, entity.HeadYaw);
                Mat4f.RotateZ(HeadLocalMatrix, HeadLocalMatrix, entity.HeadPitch);

                AddTransformToElement(HeadLocalMatrix, HeadElement);
            }
        }



        public virtual void AddTransformToElement(float[] matrix, ShapeElement jointElement)
        {
            AddTransformToElements(matrix, new ShapeElement[] { jointElement }, jointElement, jointElement.JointId);
        }


        protected virtual void AddTransformToElements(float[] matrix, ShapeElement[] forElems, ShapeElement jointElement, int jointId)
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

                    Mat4f.Mul(tmpMatrix, tmpMatrix, HeadGlobalMatrix);
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, matrix);
                    origin[0] = -origin[0];
                    origin[1] = -origin[1];
                    origin[2] = -origin[2];
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, HeadGlobalMatrixInverted);

                    for (int i = 0; i < 16; i++)
                    {
                        transformationMatrices[16 * elem.JointId + i] = tmpMatrix[i];
                    }
                }

                if (elem.Children != null)
                {
                    AddTransformToElements(matrix, elem.Children, jointElement, jointId);
                }
            }
        }
    }
}

