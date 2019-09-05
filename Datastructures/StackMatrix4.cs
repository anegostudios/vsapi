using System;
using Vintagestory.API.MathTools;

namespace Vintagestory.API
{
    public class StackMatrix4
    {
        double[][] values;
        const int max = 1024;
        int count;

        public double[] Top
        {
            get { return values[count - 1]; }
        }

        public int Count
        {
            get { return count; }
        }

        public StackMatrix4(int max = 1024)
        {
            values = new double[max][];
            for (int i = 0; i < max; i++)
            {
                values[i] = Mat4d.Create();
            }
        }

        public void Push(double[] p)
        {
            Mat4d.Copy(values[count], p);
            count++;

            if (count >= values.Length) throw new Exception("Stack matrix overflow");
        }

        public void Push()
        {
            Mat4d.Copy(values[count], Top);
            count++;

            if (count >= values.Length) throw new Exception("Stack matrix overflow");
        }


        public double[] Pop()
        {
            double[] ret = values[count - 1];
            count--;
            if (count < 0) throw new Exception("Stack matrix underflow");
            return ret;
        }

        public void Clear()
        {
            count = 0;
        }

        double[] triple = new double[3];

        public void Rotate(double rad, double x, double y, double z)
        {
            triple[0] = x;
            triple[1] = y;
            triple[2] = z;

            Mat4d.Rotate(Top, Top, rad, triple);
        }

        public void Translate(double x, double y, double z)
        {
            triple[0] = x;
            triple[1] = y;
            triple[2] = z;

            Mat4d.Translate(Top, Top, triple);
        }

        public void Scale(double x, double y, double z)
        {
            triple[0] = x;
            triple[1] = y;
            triple[2] = z;

            Mat4d.Scale(Top, Top, triple);
        }

        public void Translate(double[] rotationOrigin)
        {
            Mat4d.Translate(Top, Top, rotationOrigin);
        }
    }
}
