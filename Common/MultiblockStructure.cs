using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    public class BlockOffsetAndNumber : Vec4i
    {
        public BlockOffsetAndNumber()
        {
        }

        public BlockOffsetAndNumber(int x, int y, int z, int w) : base(x, y, z, w)
        {
        }

        public int BlockNumber => W;
    }

    public delegate void PositionMismatchDelegate(Block haveBlock, AssetLocation wantBlockCode);

    /// <summary>
    /// You can export one of these by making a selection with worldedit, looking at the center block (which should be your controller/master) then typing /we mgencode. Please note, air blocks are not exported
    /// </summary>
    public class MultiblockStructure
    {
        public static int HighlightSlotId = 23;

        public Dictionary<AssetLocation, int> BlockNumbers = new Dictionary<AssetLocation, int>();

        public List<BlockOffsetAndNumber> Offsets = new List<BlockOffsetAndNumber>();

        public string OffsetsOrientation;

        public int GetOrCreateBlockNumber(Block block)
        {
            if (!BlockNumbers.TryGetValue(block.Code, out int blockNum))
            {
                blockNum = BlockNumbers[block.Code] = 1 + BlockNumbers.Count;
            }
            return blockNum;
        }


        Dictionary<int, AssetLocation> BlockCodes;
        List<BlockOffsetAndNumber> TransformedOffsets;

        public void InitForUse(float rotateYDeg)
        {
            Matrixf mat = new Matrixf();
            mat.RotateYDeg(rotateYDeg);

            BlockCodes = new Dictionary<int, AssetLocation>();
            TransformedOffsets = new List<BlockOffsetAndNumber>();

            foreach (var val in BlockNumbers)
            {
                BlockCodes[val.Value] = val.Key;
            }

            for (int i = 0; i < Offsets.Count; i++)
            {
                Vec4i offset = Offsets[i];
                Vec4f offsetTf = new Vec4f(offset.X, offset.Y, offset.Z, 0);
                Vec4f tfedOffset = mat.TransformVector(offsetTf);

                TransformedOffsets.Add(new BlockOffsetAndNumber() { X = (int)Math.Round(tfedOffset.X), Y = (int)Math.Round(tfedOffset.Y), Z = (int)Math.Round(tfedOffset.Z), W = offset.W });
            }
        }


        public void WalkMatchingBlocks(IWorldAccessor world, BlockPos centerPos, Action<Block, BlockPos> onBlock)
        {
            if (TransformedOffsets == null)
            {
                throw new InvalidOperationException("call InitForUse() first");
            }

            BlockPos pos = new BlockPos();

            for (int i = 0; i < TransformedOffsets.Count; i++)
            {
                Vec4i offset = TransformedOffsets[i];

                pos.Set(centerPos.X + offset.X, centerPos.Y + offset.Y, centerPos.Z + offset.Z);
                Block block = world.BlockAccessor.GetBlock(pos);

                if (WildcardUtil.Match(BlockCodes[offset.W], block.Code))
                {
                    onBlock?.Invoke(block, pos);
                }
            }
        }

        /// <summary>
        /// Check if the multiblock structure is complete. Ignores air blocks
        /// </summary>
        /// <param name="world"></param>
        /// <param name="centerPos"></param>
        /// <param name="onMismatch"></param>
        /// <returns></returns>
        public int InCompleteBlockCount(IWorldAccessor world, BlockPos centerPos, PositionMismatchDelegate onMismatch = null)
        {
            if (TransformedOffsets == null)
            {
                throw new InvalidOperationException("call InitForUse() first");
            }

            int qinc = 0;

            for (int i = 0; i < TransformedOffsets.Count; i++)
            {
                Vec4i offset = TransformedOffsets[i];

                Block block = world.BlockAccessor.GetBlockRaw(centerPos.X + offset.X, centerPos.InternalY + offset.Y, centerPos.Z + offset.Z);

                if (!WildcardUtil.Match(BlockCodes[offset.W], block.Code))
                {
                    onMismatch?.Invoke(block, BlockCodes[offset.W]);
                    //world.Logger.Notification("Expected: {0}, Is: {1}", BlockCodes[offset.W], block.Code);
                    qinc++;
                }
            }

            return qinc;
        }


        public void ClearHighlights(IWorldAccessor world, IPlayer player)
        {
            world.HighlightBlocks(player, HighlightSlotId, new List<BlockPos>(), new List<int>());
        }

        public void HighlightIncompleteParts(IWorldAccessor world, IPlayer player, BlockPos centerPos)
        {
            List<BlockPos> blocks = new List<BlockPos>();
            List<int> colors = new List<int>();

            for (int i = 0; i < TransformedOffsets.Count; i++)
            {
                Vec4i offset = TransformedOffsets[i];

                Block block = world.BlockAccessor.GetBlockRaw(centerPos.X + offset.X, centerPos.InternalY + offset.Y, centerPos.Z + offset.Z);
                AssetLocation desireBlockLoc = BlockCodes[offset.W];

                if (!WildcardUtil.Match(BlockCodes[offset.W], block.Code))
                {
                    blocks.Add(new BlockPos(offset.X, offset.Y, offset.Z).Add(centerPos));

                    if (block.Id != 0)
                    {
                        colors.Add(ColorUtil.ColorFromRgba(215, 94, 94, 64));
                    } else
                    {
                        int col = world.SearchBlocks(desireBlockLoc)[0].GetColor(world.Api as ICoreClientAPI, centerPos);

                        col &= ~(255 << 24);
                        col |= 96 << 24;

                        colors.Add(col);
                    }


                }
            }

            world.HighlightBlocks(player, HighlightSlotId, blocks, colors);
        }
    }
}
