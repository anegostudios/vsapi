using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API
{
    public class RotatableCube : Cuboidf
    {
        public float RotateX = 0;
        public float RotateY = 0;
        public float RotateZ = 0;

        public Vec3d Origin = new Vec3d(0.5, 0.5, 0.5);


        public RotatableCube()
        {

        }

        public RotatableCube(float MinX, float MinY, float MinZ, float MaxX, float MaxY, float MaxZ) : base(MinX, MinY, MinZ, MaxX, MaxY, MaxZ)
        {

        }

        public Cuboidf RotatedCopy()
        {
            return RotatedCopy(RotateX, RotateY, RotateZ, Origin);
        }

        public new RotatableCube Clone()
        {
            RotatableCube cloned = (RotatableCube)MemberwiseClone();
            return cloned;
        }
    }
}
