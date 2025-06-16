using System;
using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.MathTools
{
    // North: Negative Z
    // NorthEast: Positive X, negative Z
    // East: Positive X
    // SouthEast: Positive X, positive Z
    // South: Positive Z
    // SouthWest: Negative X, positive Z
    // West: Negative X
    // NorthWest: Negative X, negative Z
    public class Cardinal
    {
        private static Dictionary<Vec3i, Cardinal> byNormali = new Dictionary<Vec3i, Cardinal>();
        private static Dictionary<string, Cardinal> byInitial = new Dictionary<string, Cardinal>();

        /// <summary>
        /// Negative Z
        /// </summary>
        public static readonly Cardinal North = new Cardinal("north", "n", new Vec3i(0,0,-1), 0, 4, false);
        /// <summary>
        /// Positive X, negative Z
        /// </summary>
        public static readonly Cardinal NorthEast = new Cardinal("northeast", "ne", new Vec3i(1, 0, -1), 1, 5, true);
        /// <summary>
        /// Positive X
        /// </summary>
        public static readonly Cardinal East = new Cardinal("east", "e", new Vec3i(1, 0, 0), 2, 6, false);
        /// <summary>
        /// Positive X, positive Z
        /// </summary>
        public static readonly Cardinal SouthEast = new Cardinal("southeast", "se", new Vec3i(1, 0, 1), 3, 7, true);
        /// <summary>
        /// Positive Z
        /// </summary>
        public static readonly Cardinal South = new Cardinal("south", "s", new Vec3i(0, 0, 1), 4, 0, false);
        /// <summary>
        /// Negative X, positive Z
        /// </summary>
        public static readonly Cardinal SouthWest = new Cardinal("southwest", "sw", new Vec3i(-1, 0, 1), 5, 1, true);
        /// <summary>
        /// Negative X
        /// </summary>
        public static readonly Cardinal West = new Cardinal("west", "w", new Vec3i(-1, 0, 0), 6, 2, false);
        /// <summary>
        /// Negative X, negative Z
        /// </summary>
        public static readonly Cardinal NorthWest = new Cardinal("northwest", "nw", new Vec3i(-1, 0, -1), 7, 3, true);

        public static readonly Cardinal[] ALL = new Cardinal[] { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

        public Vec3i Normali {get; private set;}
        public Vec3f Normalf { get; private set; }
        public Cardinal Opposite { get { return ALL[OppositeIndex]; } }
        public int Index { get; private set; }
        public string Initial { get; private set; }
        public string Code { get; private set; }

        public bool IsDiagnoal { get; private set; }

        public int OppositeIndex { get; private set; }

        public Cardinal(string code, string initial, Vec3i normali, int index, int oppositeIndex, bool isDiagonal)
        {
            this.Code = code;
            this.Initial = initial;
            this.Normali = normali;
            this.Normalf = new Vec3f(normali);
            this.Index = index;
            this.IsDiagnoal = isDiagonal;
            this.OppositeIndex = oppositeIndex;
            byNormali.Add(normali, this);
            byInitial.Add(initial, this);
        }

        public static implicit operator int(Cardinal card)
        {
            return card.Index;
        }

        public static Cardinal FromNormali(Vec3i normali)
        {
            byNormali.TryGetValue(normali, out Cardinal card);
            return card;
        }

        public static Cardinal FromInitial(string initials)
        {
            byInitial.TryGetValue(initials, out Cardinal card);
            return card;
        }


        public static Cardinal FromVector(double x, double y, double z)
        {
            float smallestAngle = GameMath.PI;
            Cardinal cardinal = null;

            double len = GameMath.Sqrt(x * x + y * y + z * z);
            x /= len;
            y /= len;
            z /= len;

            for (int i = 0; i < ALL.Length; i++)
            {
                Cardinal f = ALL[i];
                float angle = (float)Math.Acos(f.Normalf.X * x + f.Normalf.Y * y + f.Normalf.Z * z);

                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    cardinal = f;
                }
            }

            return cardinal;
        }

    }
}
