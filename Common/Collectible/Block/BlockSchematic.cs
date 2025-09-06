using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using ProperVersion;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common.Collectible.Block;

#nullable disable

namespace Vintagestory.API.Common
{
    public delegate int PlaceBlockDelegate(IBlockAccessor blockAccessor, BlockPos pos, Block newBlock, bool replaceMeta);

    public enum EnumReplaceMode
    {
        /// <summary>
        /// Replace if new block replaceable value > old block replaceable value
        /// </summary>
        Replaceable,
        /// <summary>
        /// Replace always, no matter what blocks were there previously
        /// </summary>
        ReplaceAll,
        /// <summary>
        /// Replace always, no matter what blocks were there previously, but skip air blocks in the schematic
        /// </summary>
        ReplaceAllNoAir,
        /// <summary>
        /// Replace only air blocks
        /// </summary>
        ReplaceOnlyAir
    }

    public enum EnumOrigin
    {
        StartPos = 0,
        BottomCenter = 1,
        TopCenter = 2,
        MiddleCenter = 3
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class BlockSchematic
    {
        [JsonProperty]
        public string GameVersion;
        [JsonProperty]
        public int SizeX;
        [JsonProperty]
        public int SizeY;
        [JsonProperty]
        public int SizeZ;
        [JsonProperty]
        public Dictionary<int, AssetLocation> BlockCodes = new Dictionary<int, AssetLocation>();
        [JsonProperty]
        public Dictionary<int, AssetLocation> ItemCodes = new Dictionary<int, AssetLocation>();
        [JsonProperty]
        public List<uint> Indices = new List<uint>();
        [JsonProperty]
        public List<int> BlockIds = new List<int>();
        [JsonProperty]
        public List<uint> DecorIndices = new List<uint>();
        [JsonProperty]
        public List<long> DecorIds = new List<long>();
        [JsonProperty]
        public Dictionary<uint, string> BlockEntities = new Dictionary<uint, string>();
        [JsonProperty]
        public List<string> Entities = new List<string>();

        [JsonProperty]
        public EnumReplaceMode ReplaceMode = EnumReplaceMode.ReplaceAllNoAir;

        [JsonProperty]
        public int EntranceRotation = -1;

        [JsonProperty]
        public BlockPos OriginalPos;

        public Dictionary<BlockPos, int> BlocksUnpacked = new Dictionary<BlockPos, int>();
        public Dictionary<BlockPos, int> FluidsLayerUnpacked = new Dictionary<BlockPos, int>();
        public Dictionary<BlockPos, string> BlockEntitiesUnpacked = new Dictionary<BlockPos, string>();
        public List<Entity> EntitiesUnpacked = new List<Entity>();
        public Dictionary<BlockPos, Dictionary<int,Block>> DecorsUnpacked = new Dictionary<BlockPos, Dictionary<int,Block>>();
        public FastVec3i PackedOffset;
        public List<BlockPosFacing> PathwayBlocksUnpacked;

        public static int FillerBlockId;
        public static int PathwayBlockId;
        public static int UndergroundBlockId;
        public static int AbovegroundBlockId;

        protected ushort empty = 0;
        public bool OmitLiquids = false;

        public BlockFacing[] PathwaySides;
        /// <summary>
        /// Distance positions from bottom left corner of the schematic. Only the first door block.
        /// </summary>
        public BlockPos[] PathwayStarts;
        /// <summary>
        /// Distance from the bottom left door block, so the bottom left door block is always at 0,0,0
        /// </summary>
        public BlockPos[][] PathwayOffsets;

        public BlockPos[] UndergroundCheckPositions;
        public BlockPos[] AbovegroundCheckPositions;

        /// <summary>
        /// Set by the RemapperAssistant in OnFinalizeAssets
        /// <br/>Heads up!:  This is unordered, it will iterate through the different game versions' remaps not necessarily in the order they originally appear in remaps.json config.  If any block remaps over the years have duplicate original block names, behavior for those ones may be unpredictable
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> BlockRemaps { get; set; }

        /// <summary>
        /// Set by the RemapperAssistant in OnFinalizeAssets
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> ItemRemaps { get; set; }
        static BlockPos Zero = new BlockPos(0, 0, 0);

        /// <summary>
        /// This bitmask for the position in schematics
        /// </summary>
        public const uint PosBitMask = 0x3ff;

        public BlockSchematic()
        {
            GameVersion = Config.GameVersion.ShortGameVersion;
        }

        /// <summary>
        /// Construct a schematic from a specified area in the specified world
        /// </summary>
        /// <param name="world"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="notLiquids"></param>
        public BlockSchematic(IServerWorldAccessor world, BlockPos start, BlockPos end, bool notLiquids) : this(world, world.BlockAccessor, start, end, notLiquids)
        {
        }

        public BlockSchematic(IServerWorldAccessor world, IBlockAccessor blockAccess, BlockPos start, BlockPos end, bool notLiquids)
        {
            BlockPos startPos = new BlockPos(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z));
            OmitLiquids = notLiquids;
            AddArea(world, blockAccess, start, end);
            Pack(world, startPos);
        }

        public virtual void Init(IBlockAccessor blockAccessor)
        {
            Remap();
        }

        public bool TryGetVersionFromRemapKey(string remapKey, out SemVer remapVersion)
        {
            var remapString = remapKey.Split(":");
            if (remapKey.Length < 2)
            {
                remapVersion = null;
                return false;
            }

            if (remapString[1].StartsWithFast("v"))
            {
                remapString[1] = remapString[1].Substring(1, remapString[1].Length - 1);
            }
            SemVer.TryParse(remapString[1], out remapVersion);

            return true;
        }

        public void Remap()
        {
            SemVer.TryParse(GameVersion ?? "0.0.0", out var schematicVersion);

            // now do block remapping
            foreach (var map in BlockRemaps)
            {
                if(TryGetVersionFromRemapKey(map.Key, out var remapVersion) && remapVersion <= schematicVersion)
                    continue;

                foreach (var blockCode in BlockCodes)
                {
                    if (map.Value.TryGetValue(blockCode.Value.Path, out var newBlockCode))
                    {
                        BlockCodes[blockCode.Key] = new AssetLocation(newBlockCode);
                    }
                }
            }

            // now do item remapping
            foreach (var map in ItemRemaps)
            {
                if(TryGetVersionFromRemapKey(map.Key, out var remapVersion) && remapVersion <= schematicVersion)
                    continue;

                foreach (var itemCode in ItemCodes)
                {
                    if (map.Value.TryGetValue(itemCode.Value.Path, out var newItemCode))
                    {
                        ItemCodes[itemCode.Key] = new AssetLocation(newItemCode);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the meta information for each block in the schematic.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="worldForResolve"></param>
        /// <param name="fileNameForLogging"></param>
        public void LoadMetaInformationAndValidate(IBlockAccessor blockAccessor, IWorldAccessor worldForResolve, string fileNameForLogging)
        {
            List<BlockPos> undergroundPositions = new List<BlockPos>();
            List<BlockPos> abovegroundPositions = new List<BlockPos>();
            Queue<BlockPos> pathwayPositions = new Queue<BlockPos>();

            HashSet<AssetLocation> missingBlocks = new HashSet<AssetLocation>();

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                AssetLocation blockCode = BlockCodes[storedBlockid];
                Block newBlock = blockAccessor.GetBlock(blockCode);

                if (newBlock == null)
                {
                    missingBlocks.Add(blockCode);
                }
                else
                {
                    BlockPos pos = new BlockPos(dx, dy, dz);
                    if (newBlock.Id == PathwayBlockId)
                    {
                        pathwayPositions.Enqueue(pos);
                    }
                    else if (newBlock.Id == UndergroundBlockId)
                    {
                        undergroundPositions.Add(pos);
                    }
                    else if (newBlock.Id == AbovegroundBlockId)
                    {
                        abovegroundPositions.Add(pos);
                    }
                }
            }

            for (int i = 0; i < DecorIds.Count; i++)
            {
                int storedBlockid = (int)DecorIds[i] & 0xFFFFFF;
                AssetLocation blockCode = BlockCodes[storedBlockid];
                Block newBlock = blockAccessor.GetBlock(blockCode);
                if (newBlock == null) missingBlocks.Add(blockCode);
            }

            if (missingBlocks.Count > 0)
            {
                worldForResolve.Logger.Warning("Block schematic file {0} uses blocks that could no longer be found. These will turn into air blocks! (affected: {1})", fileNameForLogging, string.Join(",", missingBlocks));
            }

            HashSet<AssetLocation> missingItems = new HashSet<AssetLocation>();
            foreach (var val in ItemCodes)
            {
                if (worldForResolve.GetItem(val.Value) == null)
                {
                    missingItems.Add(val.Value);
                }
            }

            if (missingItems.Count > 0)
            {
                worldForResolve.Logger.Warning("Block schematic file {0} uses items that could no longer be found. These will turn into unknown items! (affected: {1})", fileNameForLogging, string.Join(",", missingItems));
            }


            UndergroundCheckPositions = undergroundPositions.ToArray();
            AbovegroundCheckPositions = abovegroundPositions.ToArray();


            List<List<BlockPos>> pathwayslist = new List<List<BlockPos>>();

            if (pathwayPositions.Count == 0)
            {
                PathwayStarts = Array.Empty<BlockPos>();
                PathwayOffsets = Array.Empty<BlockPos[]>();
                PathwaySides = Array.Empty<BlockFacing>();
                return;
            }


            while (pathwayPositions.Count > 0)
            {
                List<BlockPos> pathway = new List<BlockPos>() { pathwayPositions.Dequeue() };
                pathwayslist.Add(pathway);

                int i = pathwayPositions.Count;

                while (i-- > 0)
                {
                    BlockPos pos = pathwayPositions.Dequeue();
                    bool found = false;

                    for (int j = 0; j < pathway.Count; j++)
                    {
                        BlockPos ppos = pathway[j];
                        int distance = Math.Abs(pos.X - ppos.X) + Math.Abs(pos.Y - ppos.Y) + Math.Abs(pos.Z - ppos.Z);

                        if (distance == 1)
                        {
                            found = true;
                            pathway.Add(pos);
                            break;
                        }
                    }

                    if (!found) pathwayPositions.Enqueue(pos);
                    else i = pathwayPositions.Count;
                }
            }



            PathwayStarts = new BlockPos[pathwayslist.Count];
            PathwayOffsets = new BlockPos[pathwayslist.Count][];
            PathwaySides = new BlockFacing[pathwayslist.Count];


            for (int i = 0; i < PathwayStarts.Length; i++)
            {
                // Concept to determine on which side the door is:
                // 1. Iterate over every pathway block
                // 2. Calculate the vector between the schematic center point an the pathway block
                // 3. Get the average vector by summing up + divide by count
                // => this is now basically the centerpoint of the door!
                // 4. This final vector can now be used to determine the block facing

                Vec3f dirToMiddle = new Vec3f();

                List<BlockPos> pathway = pathwayslist[i];

                for (int j = 0; j < pathway.Count; j++)
                {
                    BlockPos pos = pathway[j];
                    dirToMiddle.X += pos.X - SizeX / 2f;
                    dirToMiddle.Y += pos.Y - SizeY / 2f;
                    dirToMiddle.Z += pos.Z - SizeZ / 2f;
                }

                dirToMiddle.Normalize();

                PathwaySides[i] = BlockFacing.FromNormal(dirToMiddle);
                BlockPos start = PathwayStarts[i] = pathwayslist[i][0].Copy();

                PathwayOffsets[i] = new BlockPos[pathwayslist[i].Count];

                for (int j = 0; j < pathwayslist[i].Count; j++)
                {
                    PathwayOffsets[i][j] = pathwayslist[i][j].Sub(start);
                }
            }

        }




        /// <summary>
        /// Adds an area to the schematic.
        /// </summary>
        /// <param name="world">The world the blocks are in</param>
        /// <param name="start">The start position of all the blocks.</param>
        /// <param name="end">The end position of all the blocks.</param>
        public virtual void AddArea(IWorldAccessor world, BlockPos start, BlockPos end)
        {
            AddArea(world, world.BlockAccessor, start, end);
        }

        public virtual void AddArea(IWorldAccessor world, IBlockAccessor blockAccess, BlockPos start, BlockPos end)
        {
            BlockPos startPos = new BlockPos(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z), start.dimension);
            BlockPos finalPos = new BlockPos(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z), start.dimension);
            OriginalPos = start;

            BlockPos readPos = new BlockPos(start.dimension);   // readPos has dimensionality, keyPos does not (because keyPos will be saved in the schematic)
            using FastMemoryStream reusableMemoryStream = new FastMemoryStream();
            for (int x = startPos.X; x < finalPos.X; x++)
            {
                for (int y = startPos.Y; y < finalPos.Y; y++)
                {
                    for (int z = startPos.Z; z < finalPos.Z; z++)
                    {
                        readPos.Set(x, y, z);
                        int blockid = blockAccess.GetBlock(readPos, BlockLayersAccess.Solid).BlockId;
                        int fluidid = blockAccess.GetBlock(readPos, BlockLayersAccess.Fluid).BlockId;
                        if (fluidid == blockid) blockid = 0;
                        if (OmitLiquids) fluidid = 0;
                        if (blockid == 0 && fluidid == 0) continue;

                        BlockPos keyPos = new BlockPos(x, y, z);    // We create a new BlockPos object each time because it's going to be used as a key in the BlocksUnpacked dictionary; in future for performance a long might be a better key
                        BlocksUnpacked[keyPos] = blockid;
                        FluidsLayerUnpacked[keyPos] = fluidid;

                        BlockEntity be = blockAccess.GetBlockEntity(readPos);
                        if (be != null)
                        {
                            if (be.Api == null) be.Initialize(world.Api);
                            BlockEntitiesUnpacked[keyPos] = EncodeBlockEntityData(be, reusableMemoryStream);
                            be.OnStoreCollectibleMappings(BlockCodes, ItemCodes);
                        }

                        var decors = blockAccess.GetSubDecors(readPos);
                        if (decors != null)
                        {
                            DecorsUnpacked[keyPos] = decors;
                        }
                    }
                }
            }

            EntitiesUnpacked.AddRange(world.GetEntitiesInsideCuboid(start, end, (e) => !(e is EntityPlayer)));

            foreach (var entity in EntitiesUnpacked)
            {
                entity.OnStoreCollectibleMappings(BlockCodes, ItemCodes);
            }
        }


        public virtual bool Pack(IWorldAccessor world, BlockPos startPos)
        {
            Indices.Clear();
            BlockIds.Clear();
            BlockEntities.Clear();
            Entities.Clear();
            DecorIndices.Clear();
            DecorIds.Clear();
            SizeX = 0;
            SizeY = 0;
            SizeZ = 0;

            int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;

            foreach (var val in BlocksUnpacked)
            {
                minX = Math.Min(minX, val.Key.X);
                minY = Math.Min(minY, val.Key.Y);
                minZ = Math.Min(minZ, val.Key.Z);

                // Store relative position and the block id
                int dx = val.Key.X - startPos.X;
                int dy = val.Key.Y - startPos.Y;
                int dz = val.Key.Z - startPos.Z;

                if (dx >= 1024 || dy >= 1024 || dz >= 1024)
                {
                    world.Logger.Warning("Export format does not support areas larger than 1024 blocks in any direction. Will not pack.");
                    PackedOffset = new FastVec3i(0, 0, 0);
                    return false;
                }
            }

            foreach (var val in BlocksUnpacked)
            {
                if (!FluidsLayerUnpacked.TryGetValue(val.Key, out int fluidid)) fluidid = 0;
                int blockid = val.Value;
                if (blockid == 0 && fluidid == 0) continue;

                // Store a block mapping
                if (blockid != 0) BlockCodes[blockid] = world.BlockAccessor.GetBlock(blockid).Code;
                if (fluidid != 0) BlockCodes[fluidid] = world.BlockAccessor.GetBlock(fluidid).Code;

                // Store relative position and the block id
                int dx = val.Key.X - minX;
                int dy = val.Key.Y - minY;
                int dz = val.Key.Z - minZ;

                SizeX = Math.Max(dx, SizeX);
                SizeY = Math.Max(dy, SizeY);
                SizeZ = Math.Max(dz, SizeZ);

                Indices.Add((uint)((dy << 20) | (dz << 10) | dx));
                if (fluidid == 0)
                {
                    BlockIds.Add(blockid);
                }
                else if (blockid == 0)
                {
                    BlockIds.Add(fluidid);
                }
                else   // if both block layer and liquid layer are present (non zero), add this twice;  placing code will place the liquidid blocks in the liquids layer
                {
                    BlockIds.Add(blockid);
                    Indices.Add((uint)((dy << 20) | (dz << 10) | dx));
                    BlockIds.Add(fluidid);
                }
            }

            // also export fluid locks that do not have any other blocks in the same position
            foreach (var (pos, blockId) in FluidsLayerUnpacked)
            {
                if (BlocksUnpacked.ContainsKey(pos)) continue;

                // Store a block mapping
                if (blockId != 0) BlockCodes[blockId] = world.BlockAccessor.GetBlock(blockId).Code;

                // Store relative position and the block id
                int dx = pos.X - minX;
                int dy = pos.Y - minY;
                int dz = pos.Z - minZ;

                SizeX = Math.Max(dx, SizeX);
                SizeY = Math.Max(dy, SizeY);
                SizeZ = Math.Max(dz, SizeZ);

                Indices.Add((uint)((dy << 20) | (dz << 10) | dx));
                BlockIds.Add(blockId);
            }

            foreach (var (pos, decors) in DecorsUnpacked)
            {
                // Store relative position and the block id
                int dx = pos.X - minX;
                int dy = pos.Y - minY;
                int dz = pos.Z - minZ;

                SizeX = Math.Max(dx, SizeX);
                SizeY = Math.Max(dy, SizeY);
                SizeZ = Math.Max(dz, SizeZ);


                foreach (var (faceAndSubposition, decorBlock) in decors)
                {
                    BlockCodes[decorBlock.BlockId] = decorBlock.Code;
                    DecorIndices.Add((uint)((dy << 20) | (dz << 10) | dx));

                    //UnpackDecorPosition(packedIndex, out var decorFaceIndex, out var subPosition);
                    //decorFaceIndex += subPosition * 6;
                    DecorIds.Add(((long)faceAndSubposition << 24) + decorBlock.BlockId);
                }
            }

            // off-by-one problem as usual. A block at x=3 and x=4 means a sizex of 2
            SizeX++;
            SizeY++;
            SizeZ++;

            foreach(var val in BlockEntitiesUnpacked)
            {
                int dx = val.Key.X - minX;
                int dy = val.Key.Y - minY;
                int dz = val.Key.Z - minZ;
                BlockEntities[(uint)((dy << 20) | (dz << 10) | dx)] = val.Value;
            }

            BlockPos minPos = new BlockPos(minX, minY, minZ, startPos.dimension);
            using (FastMemoryStream ms = new FastMemoryStream())
            {
                foreach (var e in EntitiesUnpacked)
                {
                    ms.Reset();
                    BinaryWriter writer = new BinaryWriter(ms);

                    writer.Write(world.ClassRegistry.GetEntityClassName(e.GetType()));

                    e.WillExport(minPos);
                    e.ToBytes(writer, false);
                    e.DidImportOrExport(minPos);

                    Entities.Add(Ascii85.Encode(ms.ToArray()));
                }
            }

            if (PathwayBlocksUnpacked != null)
            {
                foreach (var path in PathwayBlocksUnpacked)
                {
                    path.Position.X -= minX;
                    path.Position.Y -= minY;
                    path.Position.Z -= minZ;
                }
            }

            PackedOffset = new FastVec3i(minX - startPos.X, minY - startPos.Y, minZ - startPos.Z);
            BlocksUnpacked.Clear();
            FluidsLayerUnpacked.Clear();
            DecorsUnpacked.Clear();
            BlockEntitiesUnpacked.Clear();
            EntitiesUnpacked.Clear();
            return true;
        }

        /// <summary>
        /// Will place all blocks using the configured replace mode. Note: If you use a revertable or bulk block accessor you will have to call PlaceBlockEntities() after the Commit()
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="worldForCollectibleResolve"></param>
        /// <param name="startPos"></param>
        /// <param name="replaceMetaBlocks"></param>
        /// <returns></returns>
        public virtual int Place(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos, bool replaceMetaBlocks = true)
        {
            int result = Place(blockAccessor, worldForCollectibleResolve, startPos, ReplaceMode, replaceMetaBlocks);
            PlaceDecors(blockAccessor, startPos);
            return result;
        }

        /// <summary>
        /// Will place all blocks using the supplied replace mode. Note: If you use a revertable or bulk block accessor you will have to call PlaceBlockEntities() after the Commit()
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="worldForCollectibleResolve"></param>
        /// <param name="startPos"></param>
        /// <param name="mode"></param>
        /// <param name="replaceMetaBlocks"></param>
        /// <returns></returns>
        public virtual int Place(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos, EnumReplaceMode mode, bool replaceMetaBlocks = true)
        {
            BlockPos curPos = new BlockPos(startPos.dimension);
            int placed = 0;

            PlaceBlockDelegate handler = null;
            switch (mode)
            {
                case EnumReplaceMode.ReplaceAll:
                    handler = PlaceReplaceAll;

                    for (int dx = 0; dx < SizeX; dx++)
                    {
                        for (int dy = 0; dy < SizeY; dy++)
                        {
                            for (int dz = 0; dz < SizeZ; dz++)
                            {
                                curPos.Set(dx + startPos.X, dy + startPos.Y, dz + startPos.Z);
                                if (!blockAccessor.IsValidPos(curPos)) continue;    // Deal with cases where we are at the map edge
                                blockAccessor.SetBlock(0, curPos);
                            }
                        }
                    }
                    break;

                case EnumReplaceMode.Replaceable:
                    handler = PlaceReplaceable;
                    break;

                case EnumReplaceMode.ReplaceAllNoAir:
                    handler = PlaceReplaceAllNoAir;
                    break;

                case EnumReplaceMode.ReplaceOnlyAir:
                    handler = PlaceReplaceOnlyAir;
                    break;
            }

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                AssetLocation blockCode = BlockCodes[storedBlockid];

                Block newBlock = blockAccessor.GetBlock(blockCode);

                if (newBlock == null || (replaceMetaBlocks && (newBlock.Id == UndergroundBlockId || newBlock.Id == AbovegroundBlockId))) continue;

                curPos.Set(dx + startPos.X, dy + startPos.Y, dz + startPos.Z);
                if (!blockAccessor.IsValidPos(curPos)) continue;    // Deal with cases where we are at the map edge
                placed += handler(blockAccessor, curPos, newBlock, replaceMetaBlocks);


                if (newBlock.LightHsv[2] > 0 && blockAccessor is IWorldGenBlockAccessor)
                {
                    Block oldBlock = blockAccessor.GetBlock(curPos);
                    ((IWorldGenBlockAccessor)blockAccessor).ScheduleBlockLightUpdate(curPos, oldBlock.BlockId, newBlock.BlockId);
                }
            }

            if (!(blockAccessor is IBlockAccessorRevertable))
            {
                PlaceEntitiesAndBlockEntities(blockAccessor, worldForCollectibleResolve, startPos, BlockCodes, ItemCodes, false, null, 0 , null, replaceMetaBlocks);
            }

            return placed;
        }

        public virtual void PlaceDecors(IBlockAccessor blockAccessor, BlockPos startPos)
        {
            this.curPos.dimension = startPos.dimension;
            for (int i = 0; i < DecorIndices.Count; i++)
            {
                uint index = DecorIndices[i];
                int posX = startPos.X + (int)(index & PosBitMask);
                int posY = startPos.Y + (int)((index >> 20) & PosBitMask);
                int posZ = startPos.Z + (int)((index >> 10) & PosBitMask);
                var storedDecorId = DecorIds[i];
                PlaceOneDecor(blockAccessor, posX, posY, posZ, storedDecorId);
            }
        }

        public virtual void PlaceDecors(IBlockAccessor blockAccessor, BlockPos startPos, Rectanglei rect)
        {
            int i = -1;
            foreach (uint index in DecorIndices)
            {
                i++;   // increment i first, because we have various continue statements

                int posX = startPos.X + (int)(index & PosBitMask);
                int posZ = startPos.Z + (int)((index >> 10) & PosBitMask);
                if (!rect.Contains(posX, posZ)) continue;

                int posY = startPos.Y + (int)((index >> 20) & PosBitMask);

                var storedDecorId = DecorIds[i];
                PlaceOneDecor(blockAccessor, posX, posY, posZ, storedDecorId);
            }
        }

        BlockPos curPos = new BlockPos();
        private void PlaceOneDecor(IBlockAccessor blockAccessor, int posX, int posY, int posZ, long storedBlockIdAndDecoPos)
        {
            var faceAndSubPosition = (int)(storedBlockIdAndDecoPos >> 24);
            storedBlockIdAndDecoPos &= 0xFFFFFF;
            AssetLocation blockCode = BlockCodes[(int)storedBlockIdAndDecoPos];

            Block newBlock = blockAccessor.GetBlock(blockCode);

            if (newBlock == null) return;

            curPos.Set(posX, posY, posZ);
            if (!blockAccessor.IsValidPos(curPos)) return;    // Deal with cases where we are at the map edge

            blockAccessor.SetDecor(newBlock, curPos, faceAndSubPosition);
        }

        /// <summary>
        /// Attempts to transform each block as they are placed in directions different from the schematic.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="aroundOrigin"></param>
        /// <param name="angle"></param>
        /// <param name="flipAxis"></param>
        /// <param name="isDungeon"></param>
        public virtual void TransformWhilePacked(IWorldAccessor worldForResolve, EnumOrigin aroundOrigin, int angle, EnumAxis? flipAxis = null, bool isDungeon = false)
        {
            BlockPos startPos = new BlockPos(1024, 1024, 1024);

            BlocksUnpacked.Clear();
            FluidsLayerUnpacked.Clear();
            BlockEntitiesUnpacked.Clear();
            DecorsUnpacked.Clear();
            EntitiesUnpacked.Clear();

            angle = GameMath.Mod(angle, 360);
            if (angle == 0) return;

            if (EntranceRotation != -1)
            {
                EntranceRotation = GameMath.Mod(EntranceRotation + angle, 360);
            }

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                AssetLocation blockCode = BlockCodes[storedBlockid];

                Block newBlock = worldForResolve.GetBlock(blockCode);
                if (newBlock == null)
                {
                    BlockEntities.Remove(index);
                    continue;
                }

                if (flipAxis != null)
                {
                    if (flipAxis == EnumAxis.Y)
                    {
                        dy = SizeY - dy;

                        AssetLocation newCode = newBlock.GetVerticallyFlippedBlockCode();
                        newBlock = worldForResolve.GetBlock(newCode);
                    }

                    if (flipAxis == EnumAxis.X)
                    {
                        dx = SizeX - dx;

                        AssetLocation newCode = newBlock.GetHorizontallyFlippedBlockCode((EnumAxis)flipAxis);
                        newBlock = worldForResolve.GetBlock(newCode);
                    }

                    if (flipAxis == EnumAxis.Z)
                    {
                        dz = SizeZ - dz;

                        AssetLocation newCode = newBlock.GetHorizontallyFlippedBlockCode((EnumAxis)flipAxis);
                        newBlock = worldForResolve.GetBlock(newCode);
                    }
                }

                if (angle != 0)
                {
                    AssetLocation newCode = newBlock.GetRotatedBlockCode(angle);
                    var rotBlock = worldForResolve.GetBlock(newCode);
                    if (rotBlock != null)
                    {
                        newBlock = rotBlock;
                    } else
                    {
                        worldForResolve.Logger.Warning("Schematic rotate: Unable to rotate block {0} - its GetRotatedBlockCode() method call returns an invalid block code: {1}! Will use unrotated variant.", blockCode, newCode);
                    }
                }

                BlockPos pos = GetRotatedPos(aroundOrigin, angle, dx, dy, dz);

                if (newBlock.ForFluidsLayer)
                {
                    FluidsLayerUnpacked[pos] = newBlock.BlockId;
                }
                else
                {
                    BlocksUnpacked[pos] = newBlock.BlockId;
                }
            }

            for (int i = 0; i < DecorIndices.Count; i++)
            {
                uint index = DecorIndices[i];
                var storedBlockIdAndDecoPos = DecorIds[i];

                var faceAndSubPosition = (int)(storedBlockIdAndDecoPos >> 24);
                var faceIndex = faceAndSubPosition % 6;

                storedBlockIdAndDecoPos &= 0xFFFFFF;
                var blockId = (int)storedBlockIdAndDecoPos;
                BlockFacing face = BlockFacing.ALLFACES[faceIndex];

                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                AssetLocation blockCode = BlockCodes[blockId];

                Block newBlock = worldForResolve.GetBlock(blockCode);
                if (newBlock == null)
                {
                    continue;
                }

                if (flipAxis != null)
                {
                    if (flipAxis == EnumAxis.Y)
                    {
                        dy = SizeY - dy;

                        AssetLocation newCode = newBlock.GetVerticallyFlippedBlockCode();
                        newBlock = worldForResolve.GetBlock(newCode);
                        if (face.IsVertical) face = face.Opposite;
                    }

                    if (flipAxis == EnumAxis.X)
                    {
                        dx = SizeX - dx;

                        AssetLocation newCode = newBlock.GetHorizontallyFlippedBlockCode((EnumAxis)flipAxis);
                        newBlock = worldForResolve.GetBlock(newCode);
                        if (face.Axis == EnumAxis.X) face = face.Opposite;
                    }

                    if (flipAxis == EnumAxis.Z)
                    {
                        dz = SizeZ - dz;

                        AssetLocation newCode = newBlock.GetHorizontallyFlippedBlockCode((EnumAxis)flipAxis);
                        newBlock = worldForResolve.GetBlock(newCode);
                        if (face.Axis == EnumAxis.Z) face = face.Opposite;
                    }
                }

                if (angle != 0)
                {
                    AssetLocation newCode = newBlock.GetRotatedBlockCode(angle);
                    newBlock = worldForResolve.GetBlock(newCode);
                }

                BlockPos pos = GetRotatedPos(aroundOrigin, angle, dx, dy, dz);

                DecorsUnpacked.TryGetValue(pos, out var decorsTmp);
                if (decorsTmp == null)
                {
                    decorsTmp = new();
                    DecorsUnpacked[pos] = decorsTmp;
                }

                decorsTmp[faceAndSubPosition / 6 * 6 + face.GetHorizontalRotated(angle).Index] = newBlock;
            }

            using FastMemoryStream reusableMemoryStream = new FastMemoryStream();
            foreach (var val in BlockEntities)
            {
                uint index = val.Key;
                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                if (flipAxis == EnumAxis.Y) dy = SizeY - dy;
                if (flipAxis == EnumAxis.X) dx = SizeX - dx;
                if (flipAxis == EnumAxis.Z) dz = SizeZ - dz;
                BlockPos pos = GetRotatedPos(aroundOrigin, angle, dx, dy, dz);

                string beData = val.Value;

                var block = worldForResolve.GetBlock(BlocksUnpacked[pos]);
                string entityclass = block.EntityClass;

                if (entityclass != null)
                {
                    BlockEntity be = worldForResolve.ClassRegistry.CreateBlockEntity(entityclass);
                    ITreeAttribute tree = DecodeBlockEntityData(beData);
                    if (be is IRotatable rotatable)
                    {
                        be.Pos = pos;
                        be.CreateBehaviors(block, worldForResolve);
                        rotatable.OnTransformed(worldForResolve ,tree, angle, BlockCodes, ItemCodes, flipAxis);
                    }
                    tree.SetString("blockCode", block.Code.ToShortString());
                    beData = StringEncodeTreeAttribute(tree, reusableMemoryStream);
                    BlockEntitiesUnpacked[pos] = beData;
                }
            }


            foreach (string entityData in Entities)
            {
                using (MemoryStream ms = new MemoryStream(Ascii85.Decode(entityData)))
                {
                    BinaryReader reader = new BinaryReader(ms);

                    string className = reader.ReadString();
                    Entity entity = worldForResolve.ClassRegistry.CreateEntity(className);

                    entity.FromBytes(reader, false);

                    var pos = entity.ServerPos;

                    double offx = 0;
                    double offz = 0;

                    if (aroundOrigin != EnumOrigin.StartPos)
                    {
                        offx = SizeX / 2.0;
                        offz = SizeZ / 2.0;
                    }

                    pos.X -= offx;
                    pos.Z -= offz;

                    var x = pos.X;
                    var z = pos.Z;

                    switch (angle)
                    {
                        case 90:
                            pos.X = -z; // I have no idea why i need to add offz/offx flipped here for entities, but not for blocks (－‸ლ)
                            pos.Z = x;
                            break;
                        case 180:
                            pos.X = -x;
                            pos.Z = -z;
                            break;
                        case 270:
                            pos.X = z;
                            pos.Z = -x;
                            break;
                    }
                    if (aroundOrigin != EnumOrigin.StartPos)
                    {
                        pos.X += offx;
                        pos.Z += offz;
                    }

                    pos.Yaw -= angle * GameMath.DEG2RAD;

                    entity.Pos.SetPos(pos);
                    entity.ServerPos.SetPos(pos);
                    entity.PositionBeforeFalling.X = pos.X;
                    entity.PositionBeforeFalling.Z = pos.Z;

                    EntitiesUnpacked.Add(entity);
                }
            }

            Pack(worldForResolve, startPos);
        }

        public BlockPos GetRotatedPos(EnumOrigin aroundOrigin, int angle, int dx, int dy, int dz)
        {
            if (aroundOrigin != EnumOrigin.StartPos)
            {
                dx -= SizeX / 2;
                dz -= SizeZ / 2;
            }

            BlockPos pos = new BlockPos(dx, dy, dz);

            // 90 deg:
            // xNew = -yOld
            // yNew = xOld

            // 180 deg:
            // xNew = -xOld
            // yNew = -yOld

            // 270 deg:
            // xNew = yOld
            // yNew = -xOld

            switch (angle)
            {
                case 90:
                    pos.Set(-dz, dy, dx);
                    break;
                case 180:
                    pos.Set(-dx, dy, -dz);
                    break;
                case 270:
                    pos.Set(dz, dy, -dx);
                    break;
            }

            if (aroundOrigin != EnumOrigin.StartPos)
            {
                pos.X += SizeX / 2;
                pos.Z += SizeZ / 2;
            }

            return pos;
        }


        /// <summary>
        /// Places all the entities and blocks in the schematic at the position.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="worldForCollectibleResolve"></param>
        /// <param name="startPos"></param>
        /// <param name="blockCodes"></param>
        /// <param name="itemCodes"></param>
        /// <param name="replaceBlockEntities"></param>
        /// <param name="replaceBlocks"></param>
        /// <param name="centerrockblockid"></param>
        /// <param name="layerBlockForBlockEntities"></param>
        /// <param name="resolveImports">Turn it off to spawn structures as they are. For example, in this mode, instead of traders, their meta spawners will spawn</param>
        public void PlaceEntitiesAndBlockEntities(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos, Dictionary<int, AssetLocation> blockCodes, Dictionary<int, AssetLocation> itemCodes, bool replaceBlockEntities = false, Dictionary<int, Dictionary<int, int>> replaceBlocks = null, int centerrockblockid = 0, Dictionary<BlockPos, Block> layerBlockForBlockEntities = null, bool resolveImports = true)
        {
            BlockPos curPos = startPos.Copy();

            int schematicSeed = worldForCollectibleResolve.Rand.Next();

            foreach (var val in BlockEntities)
            {
                uint index = val.Key;
                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                curPos.Set(dx + startPos.X, dy + startPos.Y, dz + startPos.Z);
                if (!blockAccessor.IsValidPos(curPos)) continue;

                BlockEntity be = blockAccessor.GetBlockEntity(curPos);

                // Block entities need to be manually initialized for world gen block access
                if ((be == null || replaceBlockEntities) && blockAccessor is IWorldGenBlockAccessor)
                {
                    Block block = blockAccessor.GetBlock(curPos, BlockLayersAccess.Solid);

                    if (block.EntityClass != null)
                    {
                        blockAccessor.SpawnBlockEntity(block.EntityClass, curPos);
                        be = blockAccessor.GetBlockEntity(curPos);
                    }
                }

                if (be != null)
                {
                    if (!replaceBlockEntities)
                    {
                        Block block = blockAccessor.GetBlock(curPos, BlockLayersAccess.Solid);
                        if (block.EntityClass != worldForCollectibleResolve.ClassRegistry.GetBlockEntityClass(be.GetType()))
                        {
                            worldForCollectibleResolve.Logger.Warning("Could not import block entity data for schematic at {0}. There is already {1}, expected {2}. Probably overlapping ruins.", curPos, be.GetType(), block.EntityClass);
                            continue;
                        }
                    }

                    ITreeAttribute tree = DecodeBlockEntityData(val.Value);

                    // Check that blockCode in the stored tree matches the block at the current position, otherwise it's unlikely that this tree belongs to this BlockEntity at all!
                    string treeBlockCode = tree.GetString("blockCode");
                    if (be.Block != null && treeBlockCode != null)
                    {
                        Block encodedBlock = worldForCollectibleResolve.GetBlock(new AssetLocation(treeBlockCode));
                        if (encodedBlock != null && encodedBlock.GetType() != be.Block.GetType())   // It should be the same type at least, though it might have been rotated or had some other state change eg. snow to free or vice-versa
                        {
                            foreach (var map in BlockRemaps)
                            {
                                if (map.Value.TryGetValue(treeBlockCode, out var newBlockCode))
                                {
                                    encodedBlock = worldForCollectibleResolve.GetBlock(new AssetLocation(newBlockCode));
                                    break;
                                }
                            }
                            if (encodedBlock != null && encodedBlock.GetType() != be.Block.GetType())   // Sanity check again, after the remapping
                            {
                                worldForCollectibleResolve.Logger.Warning("Could not import block entity data for schematic at {0}. There is already {1}, expected {2}. Possibly overlapping ruins, or is this schematic from an old game version?", curPos, be.Block, treeBlockCode);
                                continue;
                            }
                        }
                    }

                    tree.SetInt("posx", curPos.X);
                    tree.SetInt("posy", curPos.InternalY);
                    tree.SetInt("posz", curPos.Z);

                    be.FromTreeAttributes(tree, worldForCollectibleResolve);
                    be.OnLoadCollectibleMappings(worldForCollectibleResolve, blockCodes, itemCodes, schematicSeed, resolveImports);
                    Block layerBlock = null;
                    layerBlockForBlockEntities?.TryGetValue(curPos, out layerBlock);
                    if (layerBlock != null && layerBlock.Id == 0) layerBlock = null;   // Necessary legacy fix for some positions in the Arctic where rockBlockId is 0, due to an older bug
                    be.OnPlacementBySchematic(worldForCollectibleResolve.Api as ICoreServerAPI, blockAccessor, curPos, replaceBlocks, centerrockblockid, layerBlock, resolveImports);
                    if (!(blockAccessor is IWorldGenBlockAccessor)) be.MarkDirty();
                }
            }

            if(blockAccessor is IMiniDimension) return;

            foreach (string entityData in Entities)
            {
                using (MemoryStream ms = new MemoryStream(Ascii85.Decode(entityData)))
                {
                    BinaryReader reader = new BinaryReader(ms);

                    string className = reader.ReadString();
                    try
                    {
                        Entity entity = worldForCollectibleResolve.ClassRegistry.CreateEntity(className);

                        entity.FromBytes(reader, false, ((IServerWorldAccessor)worldForCollectibleResolve).RemappedEntities);
                        entity.DidImportOrExport(startPos);
                        if (OriginalPos != null)
                        {
                            var prevOffset = entity.WatchedAttributes.GetBlockPos("importOffset", Zero);
                            entity.WatchedAttributes.SetBlockPos("importOffset", startPos - OriginalPos + prevOffset);
                        }
                        if (worldForCollectibleResolve.GetEntityType(entity.Code) != null) // Can be null if its a no longer existent mob type
                        {
                            // Not ideal but whatever
                            if (blockAccessor is IWorldGenBlockAccessor accessor)
                            {
                                accessor.AddEntity(entity);
                                if (!entity.TryEarlyLoadCollectibleMappings(worldForCollectibleResolve, BlockCodes, ItemCodes, schematicSeed, resolveImports))
                                {
                                    entity.OnInitialized += () => entity.OnLoadCollectibleMappings(worldForCollectibleResolve, BlockCodes, ItemCodes, schematicSeed, resolveImports);
                                }
                            } else
                            {
                                worldForCollectibleResolve.SpawnEntity(entity);
                                if (blockAccessor is IBlockAccessorRevertable re)
                                {
                                    re.StoreEntitySpawnToHistory(entity);
                                }
                                entity.OnLoadCollectibleMappings(worldForCollectibleResolve, BlockCodes, ItemCodes, schematicSeed, resolveImports);
                            }
                        }
                        else
                        {
                            worldForCollectibleResolve.Logger.Error("Couldn't import entity {0} with id {1} and code {2} - it's Type is null! Maybe from an older game version or a missing mod.", entity.GetType(), entity.EntityId, entity.Code);
                        }
                    }
                    catch (Exception)
                    {
                        worldForCollectibleResolve.Logger.Error("Couldn't import entity with classname {0} - Maybe from an older game version or a missing mod.", className);
                    }
                }
            }
        }

        /// <summary>
        /// Gets just the positions of the blocks.
        /// </summary>
        /// <param name="origin">The origin point to start from</param>
        /// <returns>An array containing the BlockPos of each block in the area.</returns>
        public virtual BlockPos[] GetJustPositions(BlockPos origin)
        {
            BlockPos[] positions = new BlockPos[Indices.Count];

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];

                int dx = (int)(index & PosBitMask);
                int dy = (int)((index >> 20) & PosBitMask);
                int dz = (int)((index >> 10) & PosBitMask);

                BlockPos pos = new BlockPos(dx, dy, dz);
                positions[i] = pos.Add(origin);
            }

            return positions;
        }


        /// <summary>
        /// Gets the starting position of the schematic.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public virtual BlockPos GetStartPos(BlockPos pos, EnumOrigin origin)
        {
            return AdjustStartPos(pos.Copy(), origin);
        }

        /// <summary>
        /// Adjusts the starting position of the schemtic.
        /// </summary>
        /// <param name="startpos"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public virtual BlockPos AdjustStartPos(BlockPos startpos, EnumOrigin origin)
        {
            if (origin == EnumOrigin.TopCenter)
            {
                startpos.X -= SizeX / 2;
                startpos.Y -= SizeY;
                startpos.Z -= SizeZ / 2;
            }

            if (origin == EnumOrigin.BottomCenter)
            {
                startpos.X -= SizeX / 2;
                startpos.Z -= SizeZ / 2;
            }

            if (origin == EnumOrigin.MiddleCenter)
            {
                startpos.X -= SizeX / 2;
                startpos.Y -= SizeY / 2;
                startpos.Z -= SizeZ / 2;
            }

            return startpos;
        }

        /// <summary>
        /// Loads the schematic from a file.
        /// </summary>
        /// <param name="infilepath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static BlockSchematic LoadFromFile(string infilepath, ref string error)
        {
            if (!File.Exists(infilepath) && File.Exists(infilepath + ".json"))
            {
                infilepath += ".json";
            }

            if (!File.Exists(infilepath))
            {
                error = "Can't import " + infilepath + ", it does not exist";
                return null;
            }

            BlockSchematic blockdata = null;

            try
            {
                using (TextReader textReader = new StreamReader(infilepath))
                {
                    blockdata = JsonConvert.DeserializeObject<BlockSchematic>(textReader.ReadToEnd());
                    textReader.Close();
                }
            }
            catch (Exception e)
            {
                error = "Failed loading " + infilepath + " : " + e.Message;
                return null;
            }

            return blockdata;
        }

        /// <summary>
        /// Loads a schematic from a string.
        /// </summary>
        /// <param name="jsoncode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static BlockSchematic LoadFromString(string jsoncode, ref string error)
        {
            try
            {
                return JsonConvert.DeserializeObject<BlockSchematic>(jsoncode);
            }
            catch (Exception e)
            {
                error = "Failed loading schematic from json code : " + e.Message;
                return null;
            }
        }

        /// <summary>
        /// Saves a schematic to a file.
        /// </summary>
        /// <param name="outfilepath"></param>
        /// <returns></returns>
        public virtual string Save(string outfilepath)
        {
            if (!outfilepath.EndsWithOrdinal(".json"))
            {
                outfilepath += ".json";
            }

            try
            {
                using (TextWriter textWriter = new StreamWriter(outfilepath))
                {
                    textWriter.Write(JsonConvert.SerializeObject(this, Formatting.None));
                    textWriter.Close();
                }
            }
            catch (IOException e)
            {
                return "Failed exporting: " + e.Message;
            }

            return null;
        }


        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }



        /// <summary>
        /// Exports the block entity data to a string.
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        public virtual string EncodeBlockEntityData(BlockEntity be)
        {
            using FastMemoryStream ms = new FastMemoryStream();
            return EncodeBlockEntityData(be, ms);
        }

        public virtual string EncodeBlockEntityData(BlockEntity be, FastMemoryStream ms)
        {
            TreeAttribute tree = new TreeAttribute();
            be.ToTreeAttributes(tree);

            return StringEncodeTreeAttribute(tree, ms);
        }

        /// <summary>
        /// Exports the tree attribute data to a string.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public virtual string StringEncodeTreeAttribute(ITreeAttribute tree)
        {
            using FastMemoryStream ms = new FastMemoryStream();
            return StringEncodeTreeAttribute(tree, ms);
        }

        public virtual string StringEncodeTreeAttribute(ITreeAttribute tree, FastMemoryStream ms)
        {
            ms.Reset();
            BinaryWriter writer = new BinaryWriter(ms);
            tree.ToBytes(writer);
            return Ascii85.Encode(ms.ToArray());
        }


        /// <summary>
        /// Imports the tree data from a string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual TreeAttribute DecodeBlockEntityData(string data)
        {
            byte[] bedata = Ascii85.Decode(data);

            TreeAttribute tree = new TreeAttribute();

            using (MemoryStream ms = new MemoryStream(bedata))
            {
                BinaryReader reader = new BinaryReader(ms);
                tree.FromBytes(reader);
            }

            return tree;
        }

        public bool IsFillerOrPath(Block newBlock)
        {
            return (newBlock.Id == FillerBlockId || newBlock.Id == PathwayBlockId);
        }

        protected virtual int PlaceReplaceAll(IBlockAccessor blockAccessor, BlockPos pos, Block newBlock, bool replaceMeta)
        {
            var layer = newBlock.ForFluidsLayer ? BlockLayersAccess.Fluid : BlockLayersAccess.Solid;
            blockAccessor.SetBlock(replaceMeta && IsFillerOrPath(newBlock) ? empty : newBlock.BlockId, pos, layer);
            return 1;
        }

        protected virtual int PlaceReplaceable(IBlockAccessor blockAccessor, BlockPos pos, Block newBlock, bool replaceMeta)
        {
            if (newBlock.ForFluidsLayer || blockAccessor.GetBlock(pos, BlockLayersAccess.MostSolid).Replaceable > newBlock.Replaceable)
            {
                var layer = newBlock.ForFluidsLayer ? BlockLayersAccess.Fluid : BlockLayersAccess.Solid;
                blockAccessor.SetBlock(replaceMeta && IsFillerOrPath(newBlock) ? empty : newBlock.BlockId, pos, layer);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceAllNoAir(IBlockAccessor blockAccessor, BlockPos pos, Block newBlock, bool replaceMeta)
        {
            if (newBlock.BlockId != 0)
            {
                var layer = newBlock.ForFluidsLayer ? BlockLayersAccess.Fluid : BlockLayersAccess.Solid;
                blockAccessor.SetBlock(replaceMeta && IsFillerOrPath(newBlock) ? empty : newBlock.BlockId, pos, layer);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceOnlyAir(IBlockAccessor blockAccessor, BlockPos pos, Block newBlock, bool replaceMeta)
        {
            Block oldBlock = blockAccessor.GetMostSolidBlock(pos);
            if (oldBlock.BlockId == 0)
            {
                var layer = newBlock.ForFluidsLayer ? BlockLayersAccess.Fluid : BlockLayersAccess.Solid;
                blockAccessor.SetBlock(replaceMeta && IsFillerOrPath(newBlock) ? empty : newBlock.BlockId, pos, layer);
                return 1;
            }
            return 0;
        }



        /// <summary>
        /// Makes a deep copy of the packed schematic. Unpacked data and loaded meta information is not cloned.
        /// </summary>
        /// <returns></returns>
        public virtual BlockSchematic ClonePacked()
        {
            BlockSchematic cloned = new BlockSchematic();
            cloned.SizeX = SizeX;
            cloned.SizeY = SizeY;
            cloned.SizeZ = SizeZ;

            cloned.GameVersion = GameVersion;

            cloned.BlockCodes = new Dictionary<int, AssetLocation>(BlockCodes);
            cloned.ItemCodes = new Dictionary<int, AssetLocation>(ItemCodes);
            cloned.Indices = new List<uint>(Indices);
            cloned.BlockIds = new List<int>(BlockIds);

            cloned.BlockEntities = new Dictionary<uint, string>(BlockEntities);
            cloned.Entities = new List<string>(Entities);

            cloned.DecorIndices = new List<uint>(DecorIndices);
            cloned.DecorIds = new List<long>(DecorIds);

            cloned.ReplaceMode = ReplaceMode;
            cloned.EntranceRotation = EntranceRotation;
            cloned.OriginalPos = OriginalPos;
            return cloned;
        }

        public void PasteToMiniDimension(ICoreServerAPI sapi, IBlockAccessor blockAccess, IMiniDimension miniDimension, BlockPos originPos, bool replaceMetaBlocks)
        {
            this.Init(blockAccess);

            this.Place(miniDimension, sapi.World, originPos, EnumReplaceMode.ReplaceAll, replaceMetaBlocks);
            this.PlaceDecors(miniDimension, originPos);
        }
    }
}
