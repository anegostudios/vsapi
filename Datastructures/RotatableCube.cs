using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// A rotatable version of a cuboid. 
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"selectionboxbytype": {
	///	"*-up": {
	///		"x1": 0,
	///		"y1": 0,
	///		"z1": 0,
	///		"x2": 1,
	///		"y2": 0.4,
	///		"z2": 1
	///	},
	///	"*-north": {
	///		"x1": 0,
	///		"y1": 0,
	///		"z1": 0,
	///		"x2": 1,
	///		"y2": 0.4,
	///		"z2": 1,
	///		"rotateZ": 90,
	///		"rotateY": 270
    ///	},
	///	...
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class RotatableCube : Cuboidf
    {
        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The cube's rotation around the X axis.
        /// </summary>
        [DocumentAsJson] public float RotateX = 0;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The cube's rotation around the Y axis.
        /// </summary>
        [DocumentAsJson] public float RotateY = 0;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The cube's rotation around the Z axis.
        /// </summary>
        [DocumentAsJson] public float RotateZ = 0;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>(0.5, 0.5, 0.5)</jsondefault>-->
        /// The origin point for the object to rotate around. Measured in meters from zero, not percent.
        /// </summary>
        [DocumentAsJson] public Vec3d Origin = new Vec3d(0.5, 0.5, 0.5);

        public RotatableCube()
        {

        }

        public Cuboidi ToHitboxCuboidi(float rotateY, Vec3d origin = null)
        {
            return RotatedCopy(0, rotateY, 0, origin ?? new Vec3d(8,8,8)).ConvertToCuboidi();
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
