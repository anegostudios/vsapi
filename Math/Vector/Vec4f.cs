using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    public class Vec4f
    {
        public float X;
        public float Y;
        public float Z;
        public float W;


        /// <summary>
        /// Synonum for X
        /// </summary>
        public float R { get { return X; } set { X = value; } }
        /// <summary>
        /// Synonum for Y
        /// </summary>
        public float G { get { return Y; } set { Y = value; } }
        /// <summary>
        /// Synonum for Z
        /// </summary>
        public float B { get { return Z; } set { Z = value; } }
        /// <summary>
        /// Synonum for W
        /// </summary>
        public float A { get { return W; } set { W = value; } }


        public Vec4f()
        {

        }

        public Vec4f(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Returns the n-th coordinate
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get { return index == 0 ? X : (index == 1 ? Y : (index == 2 ? Z : W)); }
            set { if (index == 0) X = value; else if (index == 1) Y = value; else if (index == 2) Z = value; else W = value; }
        }

        public Vec4f Set(float[] vec)
        {
            this.X = vec[0];
            this.Y = vec[1];
            this.Z = vec[2];
            this.W = vec[3];
            return this;
        }

        public Vec4f Set(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
            return this;
        }

        public Vec4f Clone()
        {
            return new Vec4f(X, Y, Z, W);
        }
    }
}
