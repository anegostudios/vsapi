using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public delegate int PlaceBlockDelegate(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock);

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
        TopCenter = 2
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
        public Dictionary<uint, string> BlockEntities = new Dictionary<uint, string>();
        [JsonProperty]
        public List<string> Entities = new List<string>();

        [JsonProperty]
        public EnumReplaceMode ReplaceMode = EnumReplaceMode.ReplaceAllNoAir;



        public Dictionary<BlockPos, ushort> BlocksUnpacked = new Dictionary<BlockPos, ushort>();
        public Dictionary<BlockPos, string> BlockEntitiesUnpacked = new Dictionary<BlockPos, string>();
        public List<Entity> EntitiesUnpacked = new List<Entity>();



        protected Block fillerBlock;
        protected Block pathwayBlock;
        protected Block undergroundBlock;

        protected ushort empty = 0;

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

        public virtual void Init(IBlockAccessor blockAccessor)
        {
            fillerBlock = blockAccessor.GetBlock(new AssetLocation("meta-filler"));
            pathwayBlock = blockAccessor.GetBlock(new AssetLocation("meta-pathway"));
            undergroundBlock = blockAccessor.GetBlock(new AssetLocation("meta-underground"));
        }


        public void LoadMetaInformation(IBlockAccessor blockAccessor)
        {
            BlockPos curPos = new BlockPos();

            List<BlockPos> undergroundPositions = new List<BlockPos>();
            Queue<BlockPos> pathwayPositions = new Queue<BlockPos>();

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                AssetLocation blockCode = BlockCodes[storedBlockid];

                Block newBlock = blockAccessor.GetBlock(blockCode);

                if (newBlock != pathwayBlock && newBlock != undergroundBlock) continue;
                
                BlockPos pos = new BlockPos(dx, dy, dz);                

                if (newBlock == pathwayBlock)
                {
                    pathwayPositions.Enqueue(pos);
                } else
                {
                    undergroundPositions.Add(pos);
                }
            }


            UndergroundCheckPositions = undergroundPositions.ToArray();


            List<List<BlockPos>> pathwayslist = new List<List<BlockPos>>();

            if (pathwayPositions.Count == 0)
            {
                this.PathwayStarts = new BlockPos[0];
                this.PathwayOffsets = new BlockPos[0][];
                this.PathwaySides = new BlockFacing[0];
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

                PathwaySides[i] = BlockFacing.FromVector(dirToMiddle);
                BlockPos start = PathwayStarts[i] = pathwayslist[i][0].Copy();

                PathwayOffsets[i] = new BlockPos[pathwayslist[i].Count];

                for (int j = 0; j < pathwayslist[i].Count; j++)
                {
                    PathwayOffsets[i][j] = pathwayslist[i][j].Sub(start);
                }
            }
            
        }


    


        public virtual void AddArea(IWorldAccessor world, BlockPos start, BlockPos end)
        {
            BlockPos startPos = new BlockPos(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z));
            BlockPos finalPos = new BlockPos(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z));

            for (int x = startPos.X; x < finalPos.X; x++)
            {
                for (int y = startPos.Y; y < finalPos.Y; y++)
                {
                    for (int z = startPos.Z; z < finalPos.Z; z++)
                    {
                        BlockPos pos = new BlockPos(x, y, z);
                        ushort blockid = world.BlockAccessor.GetBlockId(pos);
                        if (blockid == 0) continue;

                        BlocksUnpacked[pos] = blockid;

                        BlockEntity be = world.BlockAccessor.GetBlockEntity(pos);
                        if (be != null)
                        {
                            BlockEntitiesUnpacked[pos] = EncodeBlockEntityData(be);
                            be.OnStoreCollectibleMappings(BlockCodes, ItemCodes);
                        }
                    }
                }
            }

            EntitiesUnpacked.AddRange(world.GetEntitiesInsideCuboid(start, end));
        }


        public virtual bool Pack(IWorldAccessor world, BlockPos startPos)
        {
            Indices.Clear();
            BlockIds.Clear();
            BlockEntities.Clear();
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
                    Console.WriteLine("Export format does not support areas larger than 1024 blocks in any direction");
                    return false;
                }
            }

            foreach (var val in BlocksUnpacked)
            {
                if (val.Value == 0) continue;

                // Store a block mapping
                BlockCodes[val.Value] = world.BlockAccessor.GetBlock(val.Value).Code;

                // Store relative position and the block id
                int dx = val.Key.X - minX;
                int dy = val.Key.Y - minY;
                int dz = val.Key.Z - minZ;

                SizeX = Math.Max(dx, SizeX);
                SizeY = Math.Max(dy, SizeY);
                SizeZ = Math.Max(dz, SizeZ);

                Indices.Add((uint)((dy << 20) | (dz << 10) | dx));
                BlockIds.Add(val.Value);
            }

            foreach(var val in BlockEntitiesUnpacked)
            {
                int dx = val.Key.X - minX;
                int dy = val.Key.Y - minY;
                int dz = val.Key.Z - minZ;
                BlockEntities[(uint)((dy << 20) | (dz << 10) | dx)] = val.Value;
            }

            foreach (Entity e in EntitiesUnpacked)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(ms);

                    writer.Write(world.ClassRegistry.GetEntityClassName(e.GetType()));

                    e.WillExport(startPos);
                    e.ToBytes(writer, false);
                    e.DidImportOrExport(startPos);

                    Entities.Add(Ascii85.Encode(ms.ToArray()));
                }
            }

            return true;
        }


        /// <summary>
        /// Will place all blocks using the configured replace mode. Note: If you use a revertable or bulk block accessor you will have to call PlaceBlockEntities() after the Commit()
        /// </summary>
        /// <param name="blocAccessor"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public virtual int Place(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos, bool replaceMetaBlocks = true)
        {
            return Place(blockAccessor, worldForCollectibleResolve, startPos, ReplaceMode, replaceMetaBlocks);
        }

        /// <summary>
        /// Will place all blocks using the supplied replace mode. Note: If you use a revertable or bulk block accessor you will have to call PlaceBlockEntities() after the Commit()
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="startPos"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public virtual int Place(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos, EnumReplaceMode mode, bool replaceMetaBlocks = true)
        {
            BlockPos curPos = new BlockPos();
            int placed = 0;

            PlaceBlockDelegate handler = null;
            switch (ReplaceMode)
            {
                case EnumReplaceMode.ReplaceAll:
                    if (replaceMetaBlocks) handler = PlaceReplaceAllReplaceMeta;
                    else handler = PlaceReplaceAllKeepMeta;
                    break;

                case EnumReplaceMode.Replaceable:
                    if (replaceMetaBlocks) handler = PlaceReplaceableReplaceMeta;
                    else handler = PlaceReplaceableKeepMeta;
                    break;

                case EnumReplaceMode.ReplaceAllNoAir:
                    if (replaceMetaBlocks) handler = PlaceReplaceAllNoAirReplaceMeta;
                    else handler = PlaceReplaceAllNoAirKeepMeta;
                    break;

                case EnumReplaceMode.ReplaceOnlyAir:
                    if (replaceMetaBlocks) handler = PlaceReplaceOnlyAirReplaceMeta;
                    else handler = PlaceReplaceOnlyAirKeepMeta;
                    break;
            }

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                AssetLocation blockCode = BlockCodes[storedBlockid];

                Block newBlock = blockAccessor.GetBlock(blockCode);

                if (newBlock == null || (replaceMetaBlocks && newBlock == undergroundBlock)) continue;

                curPos.Set(dx + startPos.X, dy + startPos.Y, dz + startPos.Z);

                Block oldBlock = blockAccessor.GetBlock(curPos);
                placed += handler(blockAccessor, curPos, oldBlock, newBlock);

                if (newBlock.LightHsv[2] > 0 && blockAccessor is IWorldGenBlockAccessor)
                {
                    int chunkSize = blockAccessor.ChunkSize;
                    ((IWorldGenBlockAccessor)blockAccessor).ScheduleBlockLightUpdate(curPos.Copy(), oldBlock.BlockId, newBlock.BlockId);
                }
            }

            if (!(blockAccessor is IBlockAccessorRevertable))
            {
                PlaceEntitiesAndBlockEntities(blockAccessor, worldForCollectibleResolve, startPos);
            }

            return placed;
        }



        public virtual void RotateWhilePacked(IWorldAccessor worldForResolve, EnumOrigin aroundOrigin, int angle, bool flipVertical = false)
        {
            BlockPos curPos = new BlockPos();
            BlockPos startPos = new BlockPos(1024, 1024, 1024);

            List<uint> indicesRotated = new List<uint>();

            BlocksUnpacked.Clear();
            BlockEntitiesUnpacked.Clear();

            angle = GameMath.Mod(angle, 360);

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                AssetLocation blockCode = BlockCodes[storedBlockid];
                Block newBlock = worldForResolve.GetBlock(blockCode);
                if (newBlock == null) continue;

                if (flipVertical)
                {
                    dy = SizeY - dy;

                    AssetLocation newCode = newBlock.GetVerticallyFlippedBlockCode();
                    newBlock = worldForResolve.GetBlock(newCode);
                }

                if (angle != 0)
                {
                    AssetLocation newCode = newBlock.GetRotatedBlockCode(angle);
                    newBlock = worldForResolve.GetBlock(newCode);
                }

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

                curPos.Set(pos.X + startPos.X, pos.Y + startPos.Y, pos.Z + startPos.Z);

                BlocksUnpacked[pos] = newBlock.BlockId;
            }


            foreach (var val in BlockEntities)
            {
                uint index = val.Key;
                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                dx -= SizeX / 2;
                dz -= SizeZ / 2;

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

                pos.X += SizeX / 2;
                pos.Z += SizeZ / 2;

                curPos.Set(pos.X + startPos.X, pos.Y + startPos.Y, pos.Z + startPos.Z);

                string beData = val.Value;


                BlockEntity be = worldForResolve.ClassRegistry.CreateBlockEntity(worldForResolve.GetBlock(BlocksUnpacked[pos]).EntityClass);
                if (be is IBlockEntityRotatable)
                {
                    ITreeAttribute tree = DecodeBlockEntityData(beData);
                    (be as IBlockEntityRotatable).OnRotated(tree, angle, flipVertical);
                    beData = StringEncodeTreeAttribute(tree);
                }

                BlockEntitiesUnpacked[pos] = beData;
            }


            Pack(worldForResolve, startPos);
        }
        

        public void PlaceEntitiesAndBlockEntities(IBlockAccessor blockAccessor, IWorldAccessor worldForCollectibleResolve, BlockPos startPos)
        {
            BlockPos curPos = new BlockPos();

            foreach (var val in BlockEntities)
            {
                uint index = val.Key;
                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                curPos.Set(dx + startPos.X, dy + startPos.Y, dz + startPos.Z);

                BlockEntity be = blockAccessor.GetBlockEntity(curPos);

                // Block entities need to be manually initialized for world gen block access
                if (be == null && blockAccessor is IWorldGenBlockAccessor)
                {
                    Block block = blockAccessor.GetBlock(curPos);

                    if (block.EntityClass != null)
                    {
                        blockAccessor.SpawnBlockEntity(block.EntityClass, curPos);
                        be = blockAccessor.GetBlockEntity(curPos);
                    }
                }

                if (be != null)
                {
                    ITreeAttribute tree = DecodeBlockEntityData(val.Value);
                    tree.SetInt("posx", curPos.X);
                    tree.SetInt("posy", curPos.Y);
                    tree.SetInt("posz", curPos.Z);

                    be.FromTreeAtributes(tree, worldForCollectibleResolve);
                    be.OnLoadCollectibleMappings(worldForCollectibleResolve, BlockCodes, ItemCodes);                    
                    be.pos = curPos.Copy();
                }
            }

            foreach (string entityData in Entities)
            {
                using (MemoryStream ms = new MemoryStream(Ascii85.Decode(entityData)))
                {
                    BinaryReader reader = new BinaryReader(ms);

                    string className = reader.ReadString();
                    Entity entity = worldForCollectibleResolve.ClassRegistry.CreateEntity(className);

                    entity.FromBytes(reader, false);
                    entity.DidImportOrExport(startPos);

                    // Not ideal but whatever
                    if (blockAccessor is IWorldGenBlockAccessor)
                    {
                        (blockAccessor as IWorldGenBlockAccessor).AddEntity(entity);
                    } else
                    {
                        worldForCollectibleResolve.SpawnEntity(entity);
                    }
                    
                }
            }
        }


        public virtual BlockPos[] GetJustPositions(BlockPos origin)
        {
            BlockPos[] positions = new BlockPos[Indices.Count];

            for (int i = 0; i < Indices.Count; i++)
            {
                uint index = Indices[i];
                int storedBlockid = BlockIds[i];

                int dx = (int)(index & 0x1ff);
                int dy = (int)((index >> 20) & 0x1ff);
                int dz = (int)((index >> 10) & 0x1ff);

                BlockPos pos = new BlockPos(dx, dy, dz);
                positions[i] = pos.Add(origin);
            }

            return positions;
        }



        public virtual BlockPos GetStartPos(BlockPos pos, EnumOrigin origin)
        {
            return AdjustStartPos(pos.Copy(), origin);
        }

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

            return startpos;
        }


        public static BlockSchematic Load(string infilepath, ref string error)
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


        public virtual string Save(string outfilepath)
        {
            this.GameVersion = API.Config.GameVersion.ShortGameVersion;

            if (!outfilepath.EndsWith(".json"))
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

        
        

        public virtual string EncodeBlockEntityData(BlockEntity be)
        {
            TreeAttribute tree = new TreeAttribute();
            be.ToTreeAttributes(tree);

            return StringEncodeTreeAttribute(tree);
        }

        public virtual string StringEncodeTreeAttribute(ITreeAttribute tree)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms);
                tree.ToBytes(writer);
                data = ms.ToArray();
            }

            return Ascii85.Encode(data);
        }



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





        protected virtual int PlaceReplaceAllKeepMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            blockAccessor.SetBlock(newBlock.BlockId, pos);
            return 1;
        }

        protected virtual int PlaceReplaceableKeepMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (oldBlock.Replaceable < newBlock.Replaceable)
            {
                blockAccessor.SetBlock(newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceAllNoAirKeepMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (newBlock.BlockId != 0)
            {
                blockAccessor.SetBlock(newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceOnlyAirKeepMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (oldBlock.BlockId == 0)
            {
                blockAccessor.SetBlock(newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceAllReplaceMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            blockAccessor.SetBlock((newBlock == fillerBlock || newBlock == pathwayBlock) ? empty : newBlock.BlockId, pos);
            return 1;
        }

        protected virtual int PlaceReplaceableReplaceMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (oldBlock.Replaceable < newBlock.Replaceable)
            {
                blockAccessor.SetBlock((newBlock == fillerBlock || newBlock == pathwayBlock) ? empty : newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceAllNoAirReplaceMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (newBlock.BlockId != 0)
            {
                blockAccessor.SetBlock((newBlock == fillerBlock || newBlock == pathwayBlock) ? empty : newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }

        protected virtual int PlaceReplaceOnlyAirReplaceMeta(IBlockAccessor blockAccessor, BlockPos pos, Block oldBlock, Block newBlock)
        {
            if (oldBlock.BlockId == 0)
            {
                blockAccessor.SetBlock((newBlock == fillerBlock || newBlock == pathwayBlock) ? empty : newBlock.BlockId, pos);
                return 1;
            }
            return 0;
        }


        public BlockSchematic Clone()
        {
            BlockSchematic cloned = new BlockSchematic();
            cloned.SizeX = SizeX;
            cloned.SizeY = SizeY;
            cloned.SizeZ = SizeZ;
            cloned.BlockCodes = new Dictionary<int, AssetLocation>(BlockCodes);
            cloned.ItemCodes = new Dictionary<int, AssetLocation>(ItemCodes);
            cloned.Indices = new List<uint>(Indices);
            cloned.BlockIds = new List<int>(BlockIds);
            cloned.BlockEntities = new Dictionary<uint, string>(BlockEntities);
            cloned.ReplaceMode = ReplaceMode;
            return cloned;
        }
    }
}