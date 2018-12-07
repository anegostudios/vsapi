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
        public ServerAnimator(Entity entity, Animation[] Animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById) : base(entity, Animations, rootElements, jointsById)
        {
            
        }

        public ServerAnimator(Entity entity, List<ElementPose> rootPoses, Animation[] Animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById) : base(entity, rootPoses, Animations, rootElements, jointsById)
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
                    AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose() { AttachPoint = apoint, CachedPose = pose };
                }
            }
        }
    }
}
