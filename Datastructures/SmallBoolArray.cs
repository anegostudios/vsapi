using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{

    /// <summary>
    /// Suitable for up to 32 bool values, though normally used only for 6.  Offers most of the methods available for a bool[], so can be dropped in to existing code
    /// </summary>
    public struct SmallBoolArray : IEquatable<int>
    {
        public const int OnAllSides = 0x3F;
        int bits;

        public static implicit operator int(SmallBoolArray a)     // For backwards compatibility and not too much change to API, we convert this to an (int) for use elsewhere
        {
            return a.bits;
        }

        public SmallBoolArray(int values)
        {
            bits = values;
        }

        public SmallBoolArray(int[] values)
        {
            bits = 0;
            for (int i = 0; i < values.Length; i++) if (values[i] != 0) bits |= 1 << i;
        }

        public SmallBoolArray(bool[] values)
        {
            bits = 0;
            for (int i = 0; i < values.Length; i++) if (values[i]) bits |= 1 << i;
        }

        public bool this[int i]
        {
            get
            {
                return (bits & 1 << i) != 0;
            }

            set
            {
                if (value) bits |= 1 << i;
                else bits &= ~(1 << i);
            }
        }

        public bool Equals(int other) { return bits == other; }
        public override bool Equals(Object o) { if (o is int other) return bits == other; return (o is SmallBoolArray ob) && bits == ob.bits; }

        public static bool operator ==(SmallBoolArray left, int right) => right == left.bits;
        public static bool operator !=(SmallBoolArray left, int right) => right != left.bits;

        public void Fill(bool b)
        {
            bits = b ? OnAllSides : 0;
        }

        public int[] ToIntArray(int size)
        {
            int[] result = new int[size];
            int b = bits;
            for (int i = 0; i < result.Length; i++) { result[i] = b & 1; b >>= 1; }
            return result;
        }

        public bool Opposite(int i)
        {
            return (bits & 1 << (i ^ (2 - i / 4))) != 0;   // radfast's magic formula to find the opposite of our NESWUD sides 0-5, without conditional branches or array lookup; if we don't want this, should have indexed the sides N=0 S=1 W=2 E=3 D=4 U=5 :p
        }

        public bool OnSide(BlockFacing face)
        {
            return (bits & 1 << face.Index) != 0;
        }

        public bool Any => bits != 0;

        public bool All {
            get { return bits == OnAllSides; }
            set { bits = value ? OnAllSides : 0; }
        }

        public int Value()
        {
            return bits;
        }

        public bool SidesAndBase => (bits & 0x2F) == 0x2F;

        public bool Horizontals => (bits & 0xF) == 0xF;

        public bool Verticals => (bits & 0x30) == 0x30;

        public override int GetHashCode()
        {
            return 1537853281 + bits.GetHashCode();
        }
    }
}
