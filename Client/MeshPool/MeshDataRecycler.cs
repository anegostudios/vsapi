using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#nullable disable

namespace Vintagestory.API.Client
{
    /*
     * radfast thought cloud (February 2024):
     *   Reasons why this helps performance:
     *     1. MeshData used when tesselating chunks can be large in size, even non-full chunk tests with a lot of clutter and microblocks can produce 250k vertex chunks (the most complex bookshelves+books block models for example have around 300 vertices, a 32x32 wall of these would be 300k vertices).  Size in bytes is at least 34 x verticesCount, so on this example 10MB, and that's not counting xyzFaces and CustomInts etc
     *     2. Major heap memory spiking problem if such chunks are repeatedly tesselated, e.g. by player block breaking or placing actions, or grass growing etc.  Heap memory can grow by 1GB or more with 40-50 player actions
     *     3. New heap memory allocation process is also costly in CPU, can be e.g. 10ms for a large MeshData whose internal arrays are on the large object heap; the threshold MeshData size for some components being on the large object heap is around 7000 vertices
     *     4. Large MeshData objects from tesselating chunks have a short useful life in the engine; they are stored in TesselatedChunk objects, placed in the tessChunksQueue, then when they reach the head of the queue, they are uploaded to the GPU at the next opportunity, and references to them stored in MeshDataPool (whose functions are more like MeshDataIndexingPool); sometimes they are discarded before upload, e.g. if merging a new TesselatedChunk at the same coordinates
     *     5. The garbage collector is slow to collect them, for unknown reasons
     *   
     *   Technique used:
     *     6. MeshData.Dispose() is now called at all points where the MeshData is no longer required to be held in memory
     *     7. MeshData.Dispose() clears certain data fields, but retains the basic arrays (xyz, Uv, rgba, Flags, Indices) intact
     *     8. It then calls MeshDataRecycler.Recycle(). Recycle() adds the MeshData to a ConcurrentQueue of MeshData objects which are ready for recycling - "recycling" here means making available for re-use
     *     9. We do the actual recycling on the same "consumer thread" which attempts to fetch objects from the recycler, in order to avoid having to use locks everywhere
     *     10. Recycled and available MeshData objects are held in this class in one of three lists, smallSizes, mediumSizes and largeSizes
     *     11. Calling GetOrCreateMesh() on the "consumer thread" (ChunkTesselation thread) attempts first to retrieve a suitable existing MeshData object from the lists
     *     12. A "suitable" object here means the existing object has VerticesMax equal to or larger than the required VerticesMax  (but not much larger, we cap it at 25% larger to avoid wastefully using very large MeshData, and we don't allow jumping from the Small to the Medium pool in the search)
     *     13. If a suitable object is found, return that and the MeshData.CloneUsingRecycler() method will then repopulate the basic arrays without needing to do a new heap allocation
     *     14. If a suitable object is not found, we create a new MeshData object with the required characteristics; this can later be itself recycled
     *     
     *   Some fine details:
     *     15. We always create objects with a verticesCount (and therefore VerticesMax) rounded up to the nearest multiple of 4, because all or virtually all blocks in the game always have 4 verticesPerFace and therefore chunk meshdata has verticesCount as a multiple of 4; and because this ensures IndicesCount will be in the standard 6:4 ratio without rounding errors
     *     16. We use SortedList to hold the objects (sorted by a key derived from VerticesMax), this is to allow rapid binary search method when later looking for one of a suitable size
     *     17. SortedList prohibits duplicate keys (will throw an exception if Added) but there can be two MeshData of exactly the same size
     *     18. To solve this problem, we use float keys, with fraction values allowed, e.g. 87.0f, 87.125f, 87.25f are three keys for three MeshData objects all of size 87
     *     19. For VerticesMax sized in the millions (possible if spamming VertexEater objects), fractional values may reach the limits of precision of float; to mitigate this, the key is always VerticesMax/4 (because we know it must be a multiple of 4); and we test against equality due to float precision limits
     *     20. To access Array.BinarySearch on our SortedLists, we need to use reflection to access a private field in the library class :o
     *     
     *   Further implementation details
     *     21. We periodically calculate the size of the contents of all three lists, and remove the oldest element in the list if the list is over a certain size (allowing up to approx. 400MB to be used by this system, radfast arbitrary choice)
     *     22. On these periodic checks, we always remove the oldest element in the list if it has remained there unused for 3 minutes (arbitrary choice)
     *     23. Using and then recycling an element will refresh its timestamp
     *     24. A MeshData obtained from this system has its `Recyclable` field set to true; it will be set to false again when it is actually recycled; this prevents double attempts to recycle the same MeshData
     */


    /// <summary>
    /// This is a recycling system for MeshData objects, so that they can be re-used: helps performance by easing memory allocation pressure, at the cost of holding typically around 300-400MB of memory for these recycled objects
    /// </summary>
    public class MeshDataRecycler
    {
        public const int MinimumSizeForRecycling = 4096;    // Prevents the overheads of this for small objects, and prevents wasteful use of larger recycled objects for small-ish arrays (120 vertices, individual arrays at most 360 elements long)
        public const int TTL = 15000;                // 15 seconds in milliseconds
        private const int smallLimit = 368;          // Contains meshes up to around 50kb in size
        private const int mediumLimit = 3072;        // Contains meshes up to around 375kb in size; anything above this we call 'large'
        private const int Four = 4;                 // The divisor we use to calculate the size key from VerticesMax

        SortedList<float, MeshData> smallSizes = new SortedList<float, MeshData>();    // key is VerticesMax / 4; keys must be unique; fractional keys indicate equal VerticesMax up to 8 the same
        SortedList<float, MeshData> mediumSizes = new SortedList<float, MeshData>();
        SortedList<float, MeshData> largeSizes = new SortedList<float, MeshData>(); 

        private IClientWorldAccessor game;
        private bool disposed;               // If set to true will fully disable further use of the recycling system (memory held will be cleared and non-recycled MeshData will always be created)
        private ConcurrentQueue<MeshData> forRecycling = new ConcurrentQueue<MeshData>();
        private FieldInfo keysAccessor;

        public MeshDataRecycler(IClientWorldAccessor clientMain)
        {
            this.game = clientMain;

            // Annoyingly, the keys field in SortedList which we need to do a proper BinarySearch is private
            Type t = typeof(SortedList<float, MeshData>);
            keysAccessor = t.GetField("keys", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        // ===== Public methods =====

        /// <summary>
        /// Gets or creates a MeshData with basic data fields already allocated (may contain junk data) and capacity (VerticesMax) at least equal to minimumVertices; in MeshData created/recycled using this system, IndicesMax will be fixed equal to VerticesMax * 6 / 4
        /// </summary>
        /// <param name="minimumVertices"></param>
        /// <returns></returns>
        public MeshData GetOrCreateMesh(int minimumVertices)
        {
            minimumVertices = ((minimumVertices + Four - 1) / Four) * Four;    // Round up to next whole multiple of 4

            MeshData newMesh = disposed ? null : GetRecycled(minimumVertices);
            if (newMesh == null)
            {
                if (!disposed) minimumVertices = ((minimumVertices * 41 / 40 + Four - 1) / Four) * Four;    // Create one 2.5% larger than requested, this makes for better recyclability if vertices are being added (player building, lake ice forming, grass growing etc)

                //if (minimumVertices >= 21250)
                //{
                //    newMesh = new MeshData(minimumVertices);
                //    game.Logger.VerboseDebug("Allocated new large MeshData from LOH: size " + minimumVertices * 34);
                //}
                //else
                {
                    newMesh = new MeshData(minimumVertices);
                }
            }
            else
            {
                //if (minimumVertices >= 21250) game.Logger.VerboseDebug("Recycle large MeshData, saving LOH: size " + minimumVertices * 34);
                if (newMesh.IndicesMax != newMesh.VerticesMax * MeshData.StandardIndicesPerFace / MeshData.StandardVerticesPerFace)  // Check Indices is still in the standard 6:4 ratio
                {
                    newMesh.Indices = new int[newMesh.VerticesMax * MeshData.StandardIndicesPerFace / MeshData.StandardVerticesPerFace];
                    newMesh.IndicesMax = newMesh.Indices.Length;
                }
            }
            newMesh.Recyclable = true;
            return newMesh;
        }

        /// <summary>
        /// Call this periodically on the same thread which will call GetOrCreateMesh, this is required to ensure the Recycling system is up to date
        /// </summary>
        public void DoRecycling()
        {
            if (disposed)
            {
                // Do the disposal action on the same thread which accesses these lists, to avoid any race-condition
                forRecycling.Clear();
                smallSizes.Clear();
                mediumSizes.Clear();
                largeSizes.Clear();
            }

            if (forRecycling.IsEmpty) return;

            ControlSizeOfLists();   // Only control the size of the lists if we are going to add something to them - is this controlled often enough, will they get out of control? Let's see?

            while (!forRecycling.IsEmpty && forRecycling.TryDequeue(out MeshData recycled))
            {
                int entrySize = recycled.VerticesMax / Four;   // round up to multiple of 4; then divide by 4 to get the size for the purposes of our lists where the key is VerticesMax/4

                if (entrySize < smallLimit) TryAdd(smallSizes, entrySize, recycled);
                else if (entrySize < mediumLimit) TryAdd(mediumSizes, entrySize, recycled);
                else TryAdd(largeSizes, entrySize, recycled);

                recycled.RecyclingTime = game.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Offer this MeshData to the recycling system: it will first be queued for recycling, and later processed to be either recycled or disposed of
        /// </summary>
        /// <param name="meshData"></param>
        public void Recycle(MeshData meshData)
        {
            if (!disposed)     // Don't recycle or retain anything after client stop
            {
                forRecycling.Enqueue(meshData);
            }
        }

        /// <summary>
        /// Dispose of the MeshDataRecycler (normally on game exit, but can also be used to disable further use of it)
        /// </summary>
        public void Dispose()
        {
            this.disposed = true;
        }


        // ===== Private methods =====

        private void ControlSizeOfLists()
        {
            RemoveOldest(smallSizes, 300000);   //multiplied by sizeinbytes of 136 gives 40MB  max, about 1000 entries of average size;  note key in the lists is VerticesMax * 4, so each +1 in key is approximately 136 bytes
            RemoveOldest(mediumSizes, 900000);   //multiplied by sizeinbytes of 136 gives 120MB  max, about 800 entries of average size
            RemoveOldest(largeSizes, 2240000);   //multiplied by sizeinbytes of 136 gives 300MB  max, about 200 entries of arbitrary large size
        }

        private void RemoveOldest(SortedList<float, MeshData> list, int maxSize)
        {
            if (list.Count == 0) return;   // Nothing to do and logic fails on empty list

            int totalsize = 0;
            int indexOldest = 0;
            long timeOldest = list.GetValueAtIndex(0).RecyclingTime;
            int index = 0;
            foreach (var entry in list)
            {
                totalsize += (int)entry.Key;
                if (entry.Value.RecyclingTime < timeOldest)
                {
                    indexOldest = index;
                    timeOldest = entry.Value.RecyclingTime;
                }
                index++;
            }
            if (totalsize > maxSize || timeOldest < game.ElapsedMilliseconds - TTL)    // Always remove one if more than 45 seconds in queue unused
            {
                list.GetValueAtIndex(indexOldest).DisposeBasicData();     // Help to release its memory to the GC
                list.RemoveAt(indexOldest);
            }
        }

        private MeshData GetRecycled(int minimumCapacity)
        {
            if (disposed) return null;

            int entrySize = minimumCapacity / Four;
            if (entrySize < smallLimit) return TryGet(smallSizes, entrySize);
            else if (entrySize < mediumLimit) return TryGet(mediumSizes, entrySize);
            return TryGet(largeSizes, entrySize);
        }

        private void TryAdd(SortedList<float, MeshData> list, int intkey, MeshData entry)
        {
            float key = intkey;
            while (key < intkey + 1)
            {
                if (list.TryAdd(key, entry)) return;    // Did we successfully add the entry with this key?

                float newkey = key + 0.25f;      // try up to 4 fractional values, to allow several list entries of the same size  (SortedList is not allowed to have duplicate keys);
                if (newkey == key) newkey = key + 0.5f;    // can happen due to float precision
                if (newkey == key) break;     // prevent endless looping
                key = newkey;
            }

            entry.DisposeBasicData();    // we failed to add it for recycling, therefore clear its fields to help the GC
        }

        private MeshData TryGet(SortedList<float, MeshData> list, int entrySize)
        {
            if (list.Count == 0) return null;
            object keys = keysAccessor.GetValue(list);

            int index = Array.BinarySearch<float>((float[])keys, 0, list.Count, entrySize, null);
            if (index < 0)
            {
                index = ~index;    // Array.BinarySearch returns a twos-complement value if the key is not in the array, but one above it is
                if (index >= list.Count) return null;  // Required size exceeds the size of the largest in the list

                int foundSize = (int)list.GetKeyAtIndex(index);
                if (foundSize > entrySize * 5 / 4 + 64) return null;   // We allow up to 25% larger sizes (or even more for small sizes), as a smaller VerticesCount can still fit in to a larger MeshData
            }

            MeshData result = list.GetValueAtIndex(index);
            list.RemoveAt(index);
            return result;
        }

    }


}
