using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vintagestory.API.MathTools
{
    public static class VecSpanExt
    {
        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> AsSpan(this ref FastVec3f vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> AsSpan(this ref FastVec3d vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> AsSpan(this ref FastVec3i vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<float> AsReadOnlySpan(this in FastVec3f vec)
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(vec.X), 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<double> AsReadOnlySpan(this in FastVec3d vec)
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(vec.X), 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<int> AsReadOnlySpan(this in FastVec3i vec)
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(vec.X), 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> AsSpan(this Vec2f vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 2);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> AsSpan(this Vec2d vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 2);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> AsSpan(this Vec2i vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 2);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> AsSpan(this Vec3f vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> AsSpan(this Vec3d vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> AsSpan(this Vec3i vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 3);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> AsSpan(this Vec4f vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 4);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> AsSpan(this Vec4d vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 4);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> AsSpan(this Vec4i vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 4);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<short> AsSpan(this Vec4s vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 4);
        }

        /// <summary>
        /// Returns the span representation of the vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<ushort> AsSpan(this Vec4us vec)
        {
            return MemoryMarshal.CreateSpan(ref vec.X, 4);
        }
    }
}
