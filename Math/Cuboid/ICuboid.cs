using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public enum EnumPushDirection
    {
        None,
        Positive,
        Negative
    }

    interface ICuboid<T, C> : IEquatable<C>
    {
        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        C Set(T x1, T y1, T z1, T x2, T y2, T z2);

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        C Set(IVec3 min, IVec3 max);

        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        C Translate(T posX, T posY, T posZ);

        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        C Translate(IVec3 vec);
        
        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        bool ContainsOrTouches(T x, T y, T z);

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        bool ContainsOrTouches(IVec3 vec);

        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        C GrowToInclude(int x, int y, int z);

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        C GrowToInclude(IVec3 vec);

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        double ShortestDistanceFrom(T x, T y, T z);

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        double ShortestDistanceFrom(IVec3 vec);

        /// <summary>
        /// Returns a new x coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        double pushOutX(C from, T x, ref EnumPushDirection direction);

        /// <summary>
        /// Returns a new y coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        double pushOutY(C from, T y, ref EnumPushDirection direction);

        /// <summary>
        /// Returns a new z coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        double pushOutZ(C from, T z, ref EnumPushDirection directione);

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        C RotatedCopy(T degX, T degY, T degZ, Vec3d origin);

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        C RotatedCopy(IVec3 vec, Vec3d origin);

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        C OffsetCopy(T x, T y, T z);

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        C OffsetCopy(IVec3 vec);
        
        /// <summary>
        /// Creates a copy of the cuboid
        /// </summary>
        C Clone();
    }
}
