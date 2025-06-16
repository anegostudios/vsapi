using System;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds arbitrary mesh data for meshes to be used in a shader
    /// </summary>
    /// <typeparam name="T">The arbitrary type.</typeparam>
    public abstract class CustomMeshDataPart<T>
    {
        bool customAllocationSize = false;
        int allocationSize;

        /// <summary>
        /// The arbitrary data to be uploaded to the graphics card
        /// </summary>
        public T[] Values;

        /// <summary>
        /// Amout of values currently added
        /// </summary>
        public int Count;

        /// <summary>
        /// Size of the Values array
        /// </summary>
        public int BufferSize { get { return Values == null ? 0 : Values.Length; } }

        /// <summary>
        /// Size of the array that should be allocated on the graphics card.
        /// </summary>
        public int AllocationSize
        {
            get { return customAllocationSize ? allocationSize : Count; }
        }

        /// <summary>
        /// Amount of variable components for variable (i.e. 2, 3 for a vec2 and a vec3), valid values are 1, 2, 3 and 4 (limited by glVertexAttribPointer)
        /// </summary>
        public int[] InterleaveSizes = null;

        /// <summary>
        /// Stride - Size in bytes of all values for one vertex
        /// </summary>
        public int InterleaveStride = 0;

        /// <summary>
        /// Offset in bytes for each variable 
        /// </summary>
        public int[] InterleaveOffsets = null;

        /// <summary>
        /// For instanced rendering
        /// </summary>
        public bool Instanced = false;

        /// <summary>
        /// Set to false if you intend to update the buffer very often (i.e. every frame)
        /// </summary>
        public bool StaticDraw = true;

        /// <summary>
        /// Used as offset when doing a partial update on an existing buffer
        /// </summary>
        public int BaseOffset = 0;

        /// <summary>
        /// Blank constructor.
        /// </summary>
        public CustomMeshDataPart()
        {
        }

        /// <summary>
        /// Array initialization constructor.
        /// </summary>
        /// <param name="arraySize">the size of the typed Array.</param>
        public CustomMeshDataPart(int arraySize) {
            Values = new T[arraySize];
        }

        /// <summary>
        /// Grows the buffer with a minimum.
        /// </summary>
        /// <param name="growAtLeastBy">The minimum amount to grow the buffer by.</param>
        public void GrowBuffer(int growAtLeastBy = 1)
        {
            if (Values == null)
            {
                Values = new T[Math.Max(growAtLeastBy, Count * 2)];
                return;
            }
            Array.Resize(ref Values, Math.Max(Values.Length + growAtLeastBy, Count * 2));
        }

        /// <summary>
        /// Adds a value to the buffer.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(T value)
        {
            if (Count >= BufferSize)
            {
                GrowBuffer();
            }
            Values[Count++] = value;
        }

        /// <summary>
        /// Adds the same value to the buffer 4 times - coded for performance.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add4(T value)
        {
            int count = Count;
            if (count + 4 > BufferSize)
            {
                GrowBuffer();
            }
            T[] values = this.Values;
            values[count++] = value;
            values[count++] = value;
            values[count++] = value;
            values[count++] = value;
            this.Count = count;
        }

        /// <summary>
        /// Adds multiple values to the buffer.
        /// </summary>
        /// <param name="values">The values being added.</param>
        public void Add(params T[] values)
        {
            if (Count + values.Length >= BufferSize)
            {
                GrowBuffer(values.Length);
            }

            for (int i = 0; i < values.Length; i++)
            {
                Values[Count++] = values[i];
            }
        }

        /// <summary>
        /// Lets you define your a self defined size to be allocated on the graphics card.
        /// If not called the allocation size will be the count of the Elements in the Array
        /// </summary>
        public void SetAllocationSize(int size)
        {
            customAllocationSize = true;
            allocationSize = size;
        }

        /// <summary>
        /// Use the element count as the allocation size (default behavior)
        /// </summary>
        public void AutoAllocationSize()
        {
            customAllocationSize = false;
        }

        /// <summary>
        /// Sets a value from a given mesh data part.
        /// </summary>
        /// <param name="meshdatapart">the mesh data part for this type.</param>
        public void SetFrom(CustomMeshDataPart<T> meshdatapart)
        {
            customAllocationSize = meshdatapart.customAllocationSize;
            allocationSize = meshdatapart.allocationSize;
            Count = meshdatapart.Count;

            if (meshdatapart.Values != null)
            {
                Values = (T[])meshdatapart.Values.Clone();
            }
            if (meshdatapart.InterleaveSizes != null)
            {
                InterleaveSizes = (int[])meshdatapart.InterleaveSizes.Clone();
            }
            if (meshdatapart.InterleaveOffsets != null)
            {
                InterleaveOffsets = (int[])meshdatapart.InterleaveOffsets.Clone();
            }

            InterleaveStride = meshdatapart.InterleaveStride;
            Instanced = meshdatapart.Instanced;
            StaticDraw = meshdatapart.StaticDraw;
            BaseOffset = meshdatapart.BaseOffset;
        }


        /// <summary>
        /// Sets a value from a given mesh data part.
        /// </summary>
        /// <param name="cloned">the mesh data part for this type.</param>
        protected CustomMeshDataPart<T> EmptyClone(CustomMeshDataPart<T> cloned)
        {
            cloned.customAllocationSize = customAllocationSize;
            cloned.allocationSize = allocationSize;
            cloned.Count = 0;

            if (Values != null)
            {
                cloned.GrowBuffer(Values.Length);
            }
            if (InterleaveSizes != null)
            {
                cloned.InterleaveSizes = (int[])InterleaveSizes.Clone();
            }
            if (InterleaveOffsets != null)
            {
                cloned.InterleaveOffsets = (int[])InterleaveOffsets.Clone();
            }

            cloned.InterleaveStride = InterleaveStride;
            cloned.Instanced = Instanced;
            cloned.StaticDraw = StaticDraw;
            cloned.BaseOffset = BaseOffset;

            return cloned;
        }
    }
}
