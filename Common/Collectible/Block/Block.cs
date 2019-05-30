﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public delegate void BehaviorDelegate(BlockBehavior behavior, ref EnumHandling handling);

    /// <summary>
    /// Basic class for a placeable block
    /// </summary>
    public class Block : CollectibleObject
    {
        /// <summary>
        /// Returns the block id
        /// </summary>
        public override int Id { get { return BlockId; } }

        /// <summary>
        /// Returns EnumItemClass.Block
        /// </summary>
        public override EnumItemClass ItemClass { get { return EnumItemClass.Block; } }

        /// <summary>
        /// Default Full Block Collision Box
        /// </summary>
        public static Cuboidf DefaultCollisionBox = new Cuboidf(0, 0, 0, 1, 1, 1);

        /// <summary>
        /// Unique number of the block. This number depends on the order in which the blocks are order. The numbering is however always ensured to remain the same on a per world basis.
        /// </summary>
        public ushort BlockId;
        
        /// <summary>
        /// If not set to JSON it will use an efficient hardcoded model
        /// </summary>
        public EnumDrawType DrawType = EnumDrawType.JSON;

        /// <summary>
        /// During which render pass this block should be rendered
        /// </summary>
        public EnumChunkRenderPass RenderPass = EnumChunkRenderPass.Opaque;

        /// <summary>
        /// Currently not used
        /// </summary>
        public bool Ambientocclusion = true;

        /// <summary>
        /// Walk speed when standing or inside this block
        /// </summary>
        public float WalkSpeedMultiplier = 1;

        /// <summary>
        /// Drag multiplier applied to entities standing on it
        /// </summary>
        public float DragMultiplier = 1;

        /// <summary>
        /// If true, players can target individual selection boxes of the block
        /// </summary>
        public bool PartialSelection;

        /// <summary>
        /// The sounds played for this block during step, break, build and walk
        /// </summary>
        public BlockSounds Sounds;



        /// <summary>
        /// Data thats passed on to the graphics card for every vertex of the blocks model
        /// </summary>
        public VertexFlags VertexFlags;

        /// <summary>
        /// For light emitting blocks: hue, saturation and brightness value
        /// </summary>
        public byte[] LightHsv = new byte[3];

        /// <summary>
        /// For light blocking blocks. Any value above 32 will completely block all light.
        /// </summary>
        public int LightAbsorption;

        /// <summary>
        /// A value usually between 0-9999 that indicates which blocks may be replaced with others.
        /// - Any block with replaceable value above 5000 will be washed away by water
        /// - Any block with replaceable value above 6000 will replaced when the player tries to place a block
        /// Examples:
        /// 0 = Bedrock
        /// 6000 = Tallgrass
        /// 9000 = Lava
        /// 9500 = Water
        /// 9999 = Air
        /// </summary>
        public int Replaceable;


        /// <summary>
        /// 0 = nothing can grow, 10 = some tallgrass and small trees can be grow on it, 100 = all grass and trees can grow on it
        /// </summary>
        public int Fertility;

        /// <summary>
        /// The mining tier required to break this block
        /// </summary>
        public int RequiredMiningTier;

        /// <summary>
        /// How long it takes to break this block in seconds
        /// </summary>
        public float Resistance = 2f;

        /// <summary>
        /// A way to categorize blocks. Used for getting the mining speed for each tool type, amongst other things
        /// </summary>
        public EnumBlockMaterial BlockMaterial = EnumBlockMaterial.Stone;

        /// <summary>
        /// Random texture selection - whether or not to use the Y axis during randomization (for multiblock plants)
        /// </summary>
        public EnumRandomizeAxes RandomizeAxes = EnumRandomizeAxes.XYZ;

        /// <summary>
        /// If true then the block will be randomly offseted by 1/3 of a block when placed
        /// </summary>
        public bool RandomDrawOffset;
        
        /// <summary>
        /// The block shape to be used when displayed in the inventory gui, held in hand or dropped on the ground
        /// </summary>
        public CompositeShape ShapeInventory = null;

        /// <summary>
        /// The default json block shape to be used when drawtype==JSON
        /// </summary>
        public CompositeShape Shape = new CompositeShape() { Base = new AssetLocation("block/basic/cube") };

        /// <summary>
        /// Particles that should spawn in regular intervals from this block
        /// </summary>
        public AdvancedParticleProperties[] ParticleProperties = null;

        /// <summary>
        /// Default textures to be used for this block
        /// </summary>
        public Dictionary<string, CompositeTexture> Textures = new Dictionary<string, CompositeTexture>();

        /// <summary>
        /// Textures to be used for this block in the inventory gui, held in hand or dropped on the ground
        /// </summary>
        public Dictionary<string, CompositeTexture> TexturesInventory = new Dictionary<string, CompositeTexture>();

        /// <summary>
        /// Returns the first textures in the TexturesInventory dictionary
        /// </summary>
        public CompositeTexture FirstTextureInventory { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; } }

        /// <summary>
        /// Defines which of the 6 block sides are completely opaque. Used to determine which block faces can be culled during tesselation.
        /// </summary>
        public bool[] SideOpaque = new bool[] { true, true, true, true, true, true};

        /// <summary>
        /// Defines which of the 6 block side are solid. Used to determine if attachable blocks can be attached to this block. Also used to determine if snow can rest on top of this block.
        /// </summary>
        public bool[] SideSolid = new bool[] { true, true, true, true, true, true };

        /// <summary>
        /// Defines which of the 6 block side should be shaded with ambient occlusion
        /// </summary>
        public bool[] SideAo = new bool[] { true, true, true, true, true, true };

        /// <summary>
        /// Defines which of the 6 block neighbours should receive AO if this block is in front of them
        /// </summary>
        public bool[] NeighbourSideAo = new bool[] { false, false, false, false, false, false };

        /// <summary>
        /// Defines what creature groups may spawn on this block
        /// </summary>
        public string[] AllowSpawnCreatureGroups = new string[] { "*" };

        /// <summary>
        /// Created on the client to cache the side ao flags by blockfacing flags plus every face with every face combined (e.g. south|west). Havin these values cached speeds up chunk tesselation.
        /// </summary>
        public bool[] SideAoByFlags;


        /// <summary>
        /// Determines which sides of the blocks should be rendered
        /// </summary>
        public EnumFaceCullMode FaceCullMode;

        /// <summary>
        /// 0 for no tint, 1 for plant climate tint, 2 for water climate tint
        /// </summary>
        public int TintIndex = 0;

        /// <summary>
        /// Internal value that's set during if the block shape has any tint indexes for use in chunk tesselation and stuff O_O
        /// </summary>
        public bool ShapeHasClimateTint;

        /// <summary>
        /// Internal value that's set during if the block shape has any tint indexes for use in chunk tesselation and stuff O_O
        /// </summary>
        public bool ShapeHasWaterTint;

        /// <summary>
        /// Defines the area with which the player character collides with.
        /// </summary>
        public Cuboidf[] CollisionBoxes = new Cuboidf[] { DefaultCollisionBox.Clone() };

        /// <summary>
        /// Defines the area which the players mouse pointer collides with for selection.
        /// </summary>
        public Cuboidf[] SelectionBoxes = new Cuboidf[] { DefaultCollisionBox.Clone() };

        /// <summary>
        /// Used for ladders. If true, walking against this blocks collisionbox will make the player climb
        /// </summary>
        public bool Climbable;

        /// <summary>
        /// Will be used for not rendering rain below this block
        /// </summary>
        public bool RainPermeable;

        /// <summary>
        /// Whether snow may rest on top of this block
        /// </summary>
        public bool? SnowCoverage = null;

        /// <summary>
        /// Value between 0..7 for Liquids to determine the height of the liquid
        /// </summary>
        public int LiquidLevel;

        /// <summary>
        /// If this block is or contains a liquid, this should be the code (or "identifier") of the liquid
        /// </summary>
        /// <returns></returns>
        public string LiquidCode;

        /// <summary>
        /// A flag set during texture block shape tesselation
        /// </summary>
        public bool HasAlternates;

        /// <summary>
        /// Modifiers that can alter the behavior of a block, particularly when being placed or removed
        /// </summary>
        public BlockBehavior[] BlockBehaviors = new BlockBehavior[0];

        /// <summary>
        /// The items that should drop from breaking this block
        /// </summary>
        public BlockDropItemStack[] Drops;

        /// <summary>
        /// If true, a blocks drops will be split into stacks of stacksize 1 for more game juice. This field is only used in OnBlockBroken() and OnBlockExploded()
        /// </summary>
        public bool SplitDropStacks = true;
        
        /// <summary>
        /// Information about the blocks as a crop 
        /// </summary>
        public BlockCropProperties CropProps = null;

        /// <summary>
        /// If this block has a block entity attached to it, this will store it's code 
        /// </summary>
        public string EntityClass;

        /// <summary>
        /// Creates a new instance of a block with default model transforms
        /// </summary>
        public Block()
        {
            GuiTransform = ModelTransform.BlockDefaultGui();
            FpHandTransform = ModelTransform.BlockDefaultFp();
            TpHandTransform = ModelTransform.BlockDefaultTp();
            GroundTransform = ModelTransform.BlockDefaultGround();
        }

        /// <summary>
        /// Called when this block was loaded by the server or the client
        /// </summary>
        /// <param name="api"></param>
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                behavior.OnLoaded(api);
            }
        }


        /// <summary>
        /// Sets the whole SideOpaque array to true 
        /// </summary>
        public bool AllSidesOpaque
        {
            set
            {
                SideOpaque[0] = value;
                SideOpaque[1] = value;
                SideOpaque[2] = value;
                SideOpaque[3] = value;
                SideOpaque[4] = value;
                SideOpaque[5] = value;
            }
            get
            {
                return SideOpaque[0] && SideOpaque[1] && SideOpaque[2] && SideOpaque[3] && SideOpaque[4] && SideOpaque[5];
            }
        }

        /// <summary>
        /// The cuboid used to determine where to spawn particles when breaking the block
        /// </summary>
        /// <param name="blockAccess"></param>
        /// <param name="pos"></param>
        /// <param name="facing"></param>
        /// <returns></returns>
        public virtual Cuboidf GetParticleBreakBox(IBlockAccessor blockAccess, BlockPos pos, BlockFacing facing)
        {
            if (facing == null)
            {
                Cuboidf[] selectionBoxes = GetSelectionBoxes(blockAccess, pos);
                Cuboidf box = (selectionBoxes != null && selectionBoxes.Length > 0) ? selectionBoxes[0] : Block.DefaultCollisionBox;
                return box;
            } else {
                Vec3i face = facing.Normali;
                Cuboidf[] boxes = GetCollisionBoxes(blockAccess, pos);
                if (boxes == null || boxes.Length == 0) boxes = GetSelectionBoxes(blockAccess, pos);

                return boxes != null && boxes.Length > 0 ? boxes[0] : null;
            }
        }

        /// <summary>
        /// Returns the blocks selection boxes at this position in the world
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual Cuboidf[] GetSelectionBoxes(IBlockAccessor world, BlockPos pos)
        {
            if (RandomDrawOffset)
            {
                float x = (GameMath.oaatHash(pos.X, 0, pos.Z) % 12) / 36f;
                float z = (GameMath.oaatHash(pos.X, 1, pos.Z) % 12) / 36f;

                return new Cuboidf[] { SelectionBoxes[0].OffsetCopy(x, 0, z) };
            }

            return SelectionBoxes;
        }

        /// <summary>
        /// Returns the blocks collision box
        /// </summary>
        /// <returns></returns>
        public virtual Cuboidf[] GetCollisionBoxes(IBlockAccessor world, BlockPos pos)
        {
            return CollisionBoxes;
        }

        /// <summary>
        /// Should return the light HSV values. 
        /// Warning: This method is likely to get called in a background thread. Please make sure your code in here is thread safe.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos">May be null</param>
        /// <param name="stack">Set if its an itemstack for which the engine wants to check the light level</param>
        /// <returns></returns>
        public virtual byte[] GetLightHsv(IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
        {
            return LightHsv;
        }

        /// <summary>
        /// If this block is or contains a liquid, it should return the code of it. Used for example by farmland to check if a nearby block is water
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual string GetLiquidCode(IBlockAccessor blockAccessor, BlockPos pos)
        {
            return LiquidCode;
        }
        

        /// <summary>
        /// Called before a decal is created.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="decalTexSource"></param>
        /// <param name="decalModelData">The block model which need UV values for the decal texture</param>
        /// <param name="blockModelData">The original block model</param>
        public virtual void GetDecal(IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
        {
            
        }



        /// <summary>
        /// Used by torches and other blocks to check if it can attach itself to that block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="block"></param>
        /// <param name="pos"></param>
        /// <param name="blockFace"></param>
        /// <returns></returns>
        public virtual bool CanAttachBlockAt(IBlockAccessor world, Block block, BlockPos pos, BlockFacing blockFace)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = behavior.CanAttachBlockAt(world, block, pos, blockFace, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return SideSolid[blockFace.Index];
        }


        /// <summary>
        /// Should return if supplied entitytype is allowed to spawn on this block
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool CanCreatureSpawnOn(IBlockAccessor blockAccessor, BlockPos pos, EntityProperties type, BaseSpawnConditions sc)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = behavior.CanCreatureSpawnOn(blockAccessor, pos, type, sc, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    result &= behaviorResult;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            bool allowedGroup =
                AllowSpawnCreatureGroups != null &&
                AllowSpawnCreatureGroups.Length > 0 &&
                (AllowSpawnCreatureGroups.Contains(sc.Group) || AllowSpawnCreatureGroups.Contains("*"))
            ;

            return allowedGroup && (!sc.RequireSolidGround || SideSolid[BlockFacing.UP.Index]);
        }




        /// <summary>
        /// Currently used for wildvines and saguaro cactus
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="onBlockFace"></param>
        /// <returns></returns>
        public virtual bool TryPlaceBlockForWorldGen(IBlockAccessor blockAccessor, BlockPos pos, BlockFacing onBlockFace, Random worldgenRandom)
        {
            Block block = blockAccessor.GetBlock(pos);

            if (block.IsReplacableBy(this))
            {
                if (block.EntityClass != null)
                {
                    blockAccessor.RemoveBlockEntity(pos);
                }

                blockAccessor.SetBlock(BlockId, pos);

                if (EntityClass != null)
                {
                    blockAccessor.SpawnBlockEntity(EntityClass, pos);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the player attempts to place this block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="itemstack"></param>
        /// <param name="blockSel"></param>
        /// <param name="failureCode">If you return false, set this value to a code why it cannot be placed. Its used for the ingame error overlay. Set to "__ignore__" to not trigger an error</param>
        /// <returns></returns>
        public virtual bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            bool result = true;

            if (byPlayer != null && !world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                // Probably good idea to do so, so lets do it :P
                byPlayer.InventoryManager.ActiveHotbarSlot.MarkDirty();

                failureCode = "claimed";
                return false;
            }

            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                bool behaviorResult = behavior.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref handled, ref failureCode);

                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            if (IsSuitablePosition(world, blockSel.Position, ref failureCode))
            {
                DoPlaceBlock(world, blockSel.Position, blockSel.Face, itemstack);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Checks if this block does not intersect with something at given position
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        public virtual bool IsSuitablePosition(IWorldAccessor world, BlockPos pos, ref string failureCode)
        {
            if (!world.BlockAccessor.GetBlock(pos).IsReplacableBy(this))
            {
                failureCode = "notreplaceable";
                return false;
            }
            if (world.GetIntersectingEntities(pos, this.GetCollisionBoxes(world.BlockAccessor, pos), e => !(e is EntityItem)).Length != 0)
            {
                failureCode = "entityintersecting";
                return false;
            }

            return true;
        }



        /// <summary>
        /// Delegates the event to the block behaviors and calls the base method if the event was not handled
        /// </summary>
        /// <param name="slot">The item the entity currently has in its hands</param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <returns></returns>
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            EnumHandHandling bhHandHandling = EnumHandHandling.NotHandled;
            WalkBehaviors(
                (BlockBehavior bh, ref EnumHandling hd) => bh.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref bhHandHandling, ref hd),
                () => base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref bhHandHandling)
            );
            handHandling = bhHandHandling;
        }


        /// <summary>
        /// Delegates the event to the block behaviors and calls the base method if the event was not handled
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot">The item the entity currently has in its hands</param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <returns></returns>
        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                bool behaviorResult = behavior.OnHeldInteractStep(secondsUsed, slot, byEntity, blockSel, entitySel, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return base.OnHeldInteractStep(secondsUsed, slot, byEntity, blockSel, entitySel);
        }


        /// <summary>
        /// Delegates the event to the block behaviors and calls the base method if the event was not handled
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot">The item the entity currently has in its hands</param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel, ref handled);
                if (handled != EnumHandling.PassThrough) preventDefault = true;

                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
        }


        /// <summary>
        /// Called by TryPlaceBlock if placement is possible 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="onBlockFace"></param>
        /// <param name="byItemStack">Might be null</param>
        public virtual void DoPlaceBlock(IWorldAccessor world, BlockPos pos, BlockFacing onBlockFace, ItemStack byItemStack)
        {
            world.BlockAccessor.SetBlock(BlockId, pos, byItemStack);
        }




        /// <summary>
        /// Player is breaking this block. Has to reduce remainingResistance by the amount of time it should be broken. This method is called only client side, every 40ms during breaking.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="blockSel"></param>
        /// <param name="itemslot">The item the player currently has in his hands</param>
        /// <param name="remainingResistance">how many seconds was left until the block breaks fully</param>
        /// <param name="dt">seconds passed since last render frame</param>
        /// <param name="counter">Total count of hits (every 40ms)</param>
        /// <returns>how many seconds now left until the block breaks fully. If a value equal to or below 0 is returned, OnBlockBroken() will get called.</returns>
        public virtual float OnGettingBroken(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
        {
            IItemStack stack = player.InventoryManager.ActiveHotbarSlot.Itemstack;
            float resistance = RequiredMiningTier == 0 ? remainingResistance - dt : remainingResistance;

            if (stack != null)
            {
                resistance = stack.Collectible.OnBlockBreaking(player, blockSel, itemslot, remainingResistance, dt, counter);
            }

            if (counter % 5 == 0 || resistance <= 0)
            {
                double posx = blockSel.Position.X + blockSel.HitPosition.X;
                double posy = blockSel.Position.Y + blockSel.HitPosition.Y;
                double posz = blockSel.Position.Z + blockSel.HitPosition.Z;
                player.Entity.World.PlaySoundAt(resistance > 0 ? Sounds.GetHitSound(player) : Sounds.GetBreakSound(player), posx, posy, posz, player, true, 16, 1);
            }

            return resistance;
        }


        /// <summary>
        /// Called when a survival player has broken the block. This method needs to remove the block.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <param name="dropQuantityMultiplier"></param>
        /// <returns></returns>
        public virtual void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            bool preventDefault = false;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnBlockBroken(world, pos, byPlayer, ref handled);
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            if (world.Side == EnumAppSide.Server && (byPlayer == null || byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative))
            {
                ItemStack[] drops = GetDrops(world, pos, byPlayer, dropQuantityMultiplier);

                if (drops != null)
                {
                    for (int i = 0; i < drops.Length; i++)
                    {
                        if (SplitDropStacks)
                        {
                            for (int k = 0; k < drops[i].StackSize; k++)
                            {
                                ItemStack stack = drops[i].Clone();
                                stack.StackSize = 1;
                                world.SpawnItemEntity(stack, new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5), null);
                            }
                        }
                        
                    }
                }

                world.PlaySoundAt(Sounds?.GetBreakSound(byPlayer), pos.X, pos.Y, pos.Z, byPlayer);
            }

            if (EntityClass != null)
            {
                BlockEntity entity = world.BlockAccessor.GetBlockEntity(pos);
                if (entity != null)
                {
                    entity.OnBlockBroken();
                }
            }

            world.BlockAccessor.SetBlock(0, pos);
        }




        public virtual BlockDropItemStack[] GetDropsForHandbook(IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
        {
            return Drops;
        }

        protected BlockDropItemStack[] GetHandbookDropsFromBreakDrops(IWorldAccessor world, BlockPos pos, IPlayer player)
        {
            ItemStack[] stacks = GetDrops(world, pos, player);
            if (stacks == null) return new BlockDropItemStack[0];

            BlockDropItemStack[] drops = new BlockDropItemStack[stacks.Length];
            for (int i = 0; i < stacks.Length; i++)
            {
                drops[i] = new BlockDropItemStack(stacks[i]);
            }

            return drops;
        }

        /// <summary>
        /// Is called before a block is broken, should return what items this block should drop. Return null or empty array for no drops.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <param name="dropQuantityMultiplier"></param>
        /// <returns></returns>
        public virtual ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            bool preventDefault = false;
            List<ItemStack> dropStacks = new List<ItemStack>();

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                ItemStack[] stacks = behavior.GetDrops(world, pos, byPlayer, dropQuantityMultiplier, ref handled);
                if (stacks != null) dropStacks.AddRange(stacks);
                if (handled == EnumHandling.PreventSubsequent) return stacks;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
            }

            if (preventDefault) return dropStacks.ToArray();

            if (Drops == null) return null;
            List<ItemStack> todrop = new List<ItemStack>();

            for (int i = 0; i < Drops.Length; i++)
            {
                if (Drops[i].Tool != null && (byPlayer == null || Drops[i].Tool != byPlayer.InventoryManager.ActiveTool)) continue;

                ItemStack stack = Drops[i].GetNextItemStack(dropQuantityMultiplier);
                if (stack == null) continue;
                
                todrop.Add(stack);
                if (Drops[i].LastDrop) break;
            }

            return todrop.ToArray();
        }


        /// <summary>
        /// Called while the given entity attempts to ignite this block
        /// </summary>
        /// <param name="byEntity"></param>
        /// <param name="pos"></param>
        /// <param name="secondsIgniting"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnTryIgniteBlock(EntityAgent byEntity, BlockPos pos, float secondsIgniting, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called after the given entity has attempted to ignite this block
        /// </summary>
        /// <param name="byEntity"></param>
        /// <param name="pos"></param>
        /// <param name="secondsIgniting"></param>
        /// <param name="handling"></param>
        public virtual void OnTryIgniteBlockOver(EntityAgent byEntity, BlockPos pos, float secondsIgniting, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            
        }


        /// <summary>
        /// When the player has presed the middle mouse click on the block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
        {
            EnumHandling handled = EnumHandling.PassThrough;
            ItemStack stack = null;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                stack = behavior.OnPickBlock(world, pos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return stack;
            }

            if (handled == EnumHandling.PreventDefault) return stack;

            return new ItemStack(this, 1);
        }

        /// <summary>
        /// Always called when a block has been removed through whatever method, except during worldgen or via ExchangeBlock()
        /// For Worldgen you might be able to use TryPlaceBlockForWorldGen() to attach custom behaviors during placement/removal
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual void OnBlockRemoved(IWorldAccessor world, BlockPos pos)
        {
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnBlockRemoved(world, pos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
            }

            if (preventDefault) return;

            if (EntityClass != null)
            {
                world.BlockAccessor.RemoveBlockEntity(pos);
            }
        }

        /// <summary>
        /// Always called when a block has been placed through whatever method, except during worldgen or via ExchangeBlock()
        /// For Worldgen you might be able to use TryPlaceBlockForWorldGen() to attach custom behaviors during placement/removal
        /// </summary>
        /// <param name="world"></param>
        /// <param name="blockPos"></param>
        /// <param name="byItemStack">May be null!</param>
        public virtual void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
        {
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnBlockPlaced(world, blockPos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
            }

            if (preventDefault) return;

            if (EntityClass != null)
            {
                world.BlockAccessor.SpawnBlockEntity(EntityClass, blockPos, byItemStack);
            }
        }



        /// <summary>
        /// Called when any of it's 6 neighbour blocks has been changed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="neibpos"></param>
        public virtual void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                behavior.OnNeighbourBlockChange(world, pos, neibpos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return;
            }
        }

        /// <summary>
        /// When a player does a right click while targeting this placed block. Should return true if the event is handled, so that other events can occur, e.g. eating a held item if the block is not interactable with.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <returns>False if the interaction should be stopped. True if the interaction should continue. If you return false, the interaction will not be synced to the server.</returns>
        public virtual bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            bool result = true;

            if (!world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
            {
                return false;
            }

            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = behavior.OnBlockInteractStart(world, byPlayer, blockSel, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return false;
        }


        /// <summary>
        /// Called every frame while the player is using this block. Return false to stop the interaction.
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <returns></returns>
        public virtual bool OnBlockInteractStep(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            return false;
        }

        /// <summary>
        /// Called when the player successfully completed the using action, always called once an interaction is over
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        public virtual void OnBlockInteractStop(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            
        }

        /// <summary>
        /// When the player released the right mouse button. Return false to deny the cancellation (= will keep using the block until OnBlockInteractStep returns false).
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        public virtual bool OnBlockInteractCancel(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, EnumItemUseCancelReason cancelReason)
        {
            return true;
        }

        /// <summary>
        /// When an entity is inside a block (can only occur if collision box is smaller than 1x1x1)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        public virtual void OnEntityInside(IWorldAccessor world, Entity entity, BlockPos pos)
        {

        }



        /// <summary>
        /// Whenever an entity collides with the collision box of the block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <param name="facing"></param>
        /// <param name="isImpact"></param>
        public virtual void OnEntityCollide(IWorldAccessor world, Entity entity, BlockPos pos, BlockFacing facing, Vec3d collideSpeed, bool isImpact)
        {
            if (entity.Properties.CanClimb == true && (Climbable || entity.Properties.CanClimbAnywhere) && facing.IsHorizontal && entity is EntityAgent)
            {
                EntityAgent ea = entity as EntityAgent;
                bool? isSneaking = ea.Controls.Sneak;
                if (isSneaking != true)
                {
                    ea.LocalPos.Motion.Y = 0.04;
                }
            }
        }

        /// <summary>
        /// Everytime the player moves by 8 blocks (or rather leaves the current 8-grid), a scan of all blocks 32x32x32 blocks around the player is initiated
        /// and this method is called. If the method returns true, the block is registered to a client side game ticking for spawning particles and such.
        /// This method will be called everytime the player left his current 8-grid area. 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool ShouldReceiveClientGameTicks(IWorldAccessor world, IPlayer player, BlockPos pos)
        {
            bool result = true;
            bool preventDefault = false;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                bool behaviorResult = behavior.ShouldReceiveClientGameTicks(world, player, pos, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return ParticleProperties != null && ParticleProperties.Length > 0;
        }


        public virtual bool ShouldPlayAmbientSound(IWorldAccessor world, BlockPos pos)
        {
            return true;
        }


        /// <summary>
        /// Called evey 25ms if the block is in range (32 blocks) and block returned true on ShouldReceiveClientGameTicks(). Takes a few seconds for the game to register the block.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        public virtual void OnClientGameTick(IWorldAccessor world, BlockPos pos, float secondsTicking)
        {
            if (ParticleProperties != null && ParticleProperties.Length > 0)
            {
                for (int i = 0; i < ParticleProperties.Length; i++)
                {
                    AdvancedParticleProperties bps = ParticleProperties[i];
                    bps.basePos.X = pos.X + TopMiddlePos.X;
                    bps.basePos.Y = pos.Y + TopMiddlePos.Y;
                    bps.basePos.Z = pos.Z + TopMiddlePos.Z;
                    world.SpawnParticles(bps);
                }
            }
        }

        /// <summary>
        /// Called every interval specified in Server.Config.RandomTickInterval. Defaults to 50ms. This method
        /// is called on a separate server thread. This should be considered when deciding how to access blocks.
        /// If true is returned, the server will call OnServerGameTick on the main thread passing the BlockPos
        /// and the 'extra' object if specified. The 'extra' parameter is meant to prevent duplicating lookups
        /// and other calculations when OnServerGameTick is called. 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos">The position of this block</param>
        /// <param name="offThreadRandom">If you do anything with random inside this method, don't use world.Rand because <see cref="Random"/> its not thread safe, use this or create your own instance</param>
        /// <param name="extra">Optional parameter to set if you need to pass additional data to the OnServerGameTick method</param>
        /// <returns></returns>
        public virtual bool ShouldReceiveServerGameTicks(IWorldAccessor world, BlockPos pos, Random offThreadRandom, out object extra)
        {
            extra = null;
            return false;
        }

        /// <summary>
        /// Called by the main server thread if and only if this block returned true in ShouldReceiveServerGameTicks.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos">The position of this block</param>
        /// <param name="extra">The value set for the 'extra' parameter when ShouldReceiveGameTicks was called.</param>
        public virtual void OnServerGameTick(IWorldAccessor world, BlockPos pos, object extra = null)
        {
        }

        /// <summary>
        /// When the item is being held in hands without using it 
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        public override void OnHeldIdle(ItemSlot slot, EntityAgent byEntity)
        {
            // Would be nice, doesn't look nice :/
            /*if (ParticleProperties != null && ParticleProperties.Length > 0 && byEntity.World is IClientWorldAccessor && byEntity.World.Rand.NextDouble() > 0.5)
            {
                Vec3d pos =
                        byEntity.Pos.XYZ.Add(0, byEntity.EyeHeight() - 0.5f + FpHandTransform.Translation.Y, 0)
                        .Ahead(0.6f, byEntity.Pos.Pitch, byEntity.Pos.Yaw)
                        .Ahead(0.5f + FpHandTransform.Translation.X, 0, byEntity.Pos.Yaw + GameMath.PIHALF)
                    ;

                for (int i = 0; i < ParticleProperties.Length; i++)
                {
                    AdvancedParticleProperties bps = ParticleProperties[i];
                    bps.basePos.X = pos.X;
                    bps.basePos.Y = pos.Y;
                    bps.basePos.Z = pos.Z;
                    byEntity.World.SpawnParticles(bps);
                }
            }*/

            base.OnHeldIdle(slot, byEntity);
        }
        

        /// <summary>
        /// The origin point from which particles are being spawned
        /// </summary>
        public Vec3f TopMiddlePos = new Vec3f(0.5f, 1, 0.5f);


        /// <summary>
        /// Used as base position for particles.
        /// </summary>
        public virtual void DetermineTopMiddlePos()
        {
            if (CollisionBoxes != null && CollisionBoxes.Length > 0)
            {
                TopMiddlePos.X = (CollisionBoxes[0].X1 + CollisionBoxes[0].X2) / 2;
                TopMiddlePos.Y = CollisionBoxes[0].Y2;
                TopMiddlePos.Z = (CollisionBoxes[0].Z1 + CollisionBoxes[0].Z2) / 2;

                for (int i = 1; i < CollisionBoxes.Length; i++)
                {
                    TopMiddlePos.Y = Math.Max(TopMiddlePos.Y, CollisionBoxes[0].Y2);
                }

                return;
            }

            if (SelectionBoxes != null && SelectionBoxes.Length > 0)
            {
                TopMiddlePos.X = (SelectionBoxes[0].X1 + SelectionBoxes[0].X2) / 2;
                TopMiddlePos.Y = SelectionBoxes[0].Y2;
                TopMiddlePos.Z = (SelectionBoxes[0].Z1 + SelectionBoxes[0].Z2) / 2;

                for (int i = 1; i < SelectionBoxes.Length; i++)
                {
                    TopMiddlePos.Y = Math.Max(TopMiddlePos.Y, SelectionBoxes[0].Y2);
                }

                return;
            }
        }



        /// <summary>
        /// Used to determine if a block should be treated like air when placing blocks. (e.g. used for tallgrass)
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual bool IsReplacableBy(Block block)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = behavior.IsReplacableBy(block, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return (IsLiquid() || Replaceable >= 6000) && block.Replaceable < Replaceable;
        }

        /// <summary>
        /// Returns a horizontal and vertical orientation which should be used for oriented blocks like stairs during placement.
        /// </summary>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <returns></returns>
        public static BlockFacing[] SuggestedHVOrientation(IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockPos targetPos = blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.GetOpposite()) : blockSel.Position;

            double dx = byPlayer.Entity.Pos.X - (targetPos.X + blockSel.HitPosition.X);
            double dy = (float)byPlayer.Entity.Pos.Y + (float)(byPlayer.Entity.EyeHeight - (targetPos.Y + blockSel.HitPosition.Y));
            double dz = (float)byPlayer.Entity.Pos.Z - (targetPos.Z + blockSel.HitPosition.Z);
            float radius = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);

            float angleHor = (float)Math.Atan2(dx, dz) + GameMath.PIHALF;

            double a = dy;
            float b = (float)Math.Sqrt(dx * dx + dz * dz);

            float angleVer = (float)Math.Atan2(a, b);
            

            BlockFacing verticalFace = angleVer < -Math.PI / 4 ? BlockFacing.DOWN : (angleVer > Math.PI / 4 ? BlockFacing.UP : null);
            BlockFacing horizontalFace = BlockFacing.HorizontalFromAngle(angleHor);

            return new BlockFacing[] { horizontalFace, verticalFace };
        }


        /// <summary>
        /// Simple string representation for debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code.Domain + AssetLocation.LocationSeparator + "block " + Code.Path + "/" + BlockId;
        }

        /// <summary>
        /// For any block that can be rotated, this method should be implemented to return the correct rotated block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public virtual AssetLocation GetRotatedBlockCode(int angle)
        {
            bool preventDefault = false;
            AssetLocation result = Code;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                AssetLocation bhresult;

                EnumHandling handled = EnumHandling.PassThrough;
                bhresult = behavior.GetRotatedBlockCode(angle, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    result = bhresult;
                }
                if (handled == EnumHandling.PreventSubsequent) return bhresult;
            }

            if (preventDefault) return result;

            return Code;
        }

        /// <summary>
        /// For any block that can be flipped upside down, this method should be implemented to return the correctly flipped block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <returns></returns>
        public virtual AssetLocation GetVerticallyFlippedBlockCode()
        {
            bool preventDefault = false;
            
            AssetLocation result = Code;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                AssetLocation bhresult;

                EnumHandling handled = EnumHandling.PassThrough;
                bhresult = behavior.GetVerticallyFlippedBlockCode(ref handled);
                if (handled == EnumHandling.PreventSubsequent) return bhresult;
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    result = bhresult;
                }
            }

            if (preventDefault) return result;

            return Code;
        }

        /// <summary>
        /// For any block that can be flipped vertically, this method should be implemented to return the correctly flipped block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public virtual AssetLocation GetHorizontallyFlippedBlockCode(EnumAxis axis)
        {
            AssetLocation result = Code;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                AssetLocation bhresult;
                EnumHandling handled = EnumHandling.PassThrough;
                bhresult = behavior.GetHorizontallyFlippedBlockCode(axis, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return bhresult;
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    result = bhresult;
                }
            }

            if (preventDefault) return result;

            return Code;
        }



        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BlockBehavior GetBehavior(Type type, bool withInheritance)
        {
            for (int i = 0; i < BlockBehaviors.Length; i++)
            {
                if (withInheritance && BlockBehaviors[i].GetType().IsAssignableFrom(type))
                {
                    return BlockBehaviors[i];
                }
                if (BlockBehaviors[i].GetType() == type)
                {
                    return BlockBehaviors[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BlockBehavior GetBehavior(Type type)
        {
            return GetBehavior(type, false);
        }

        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetBehavior<T>() where T : BlockBehavior
        {
            return (T)GetBehavior(typeof(T), false);
        }


        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasBehavior<T>(bool withInheritance = false) where T : BlockBehavior
        {
            return (T)GetBehavior(typeof(T), withInheritance) != null;
        }

        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool HasBehavior(Type type, bool withInheritance = false)
        {
            return GetBehavior(type, withInheritance) != null;
        }

        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classRegistry"></param>
        /// <returns></returns>
        public override bool HasBehavior(string type, IClassRegistryAPI classRegistry)
        {
            return GetBehavior(classRegistry.GetBlockBehaviorClass(type)) != null;
        }


        /// <summary>
        /// Called by the block info HUD for displaying the blocks name
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual string GetPlacedBlockName(IClientWorldAccessor world, BlockPos pos)
        {
            return OnPickBlock(world, pos)?.GetName();
        }

        /// <summary>
        /// Called by the block info HUD for display the interaction help besides the crosshair
        /// </summary>
        /// <param name="world"></param>
        /// <param name="selection"></param>
        /// <param name="forPlayer"></param>
        /// <returns></returns>
        public virtual WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            EnumHandling handled = EnumHandling.PassThrough;
            WorldInteraction[] interactions = new WorldInteraction[0];

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                WorldInteraction[] bhi = behavior.GetPlacedBlockInteractionHelp(world, selection, forPlayer, ref handled);

                interactions = interactions.Append(bhi);

                if (handled == EnumHandling.PreventSubsequent) break;
            }

            return interactions;
        }


        /// <summary>
        /// Called by the block info HUD for displaying additional information
        /// </summary>
        /// <returns></returns>
        public virtual string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            if (EntityClass != null)
            {
                BlockEntity be = world.BlockAccessor.GetBlockEntity(pos);
                if (be != null)
                {
                    string text = be.GetBlockInfo(forPlayer);
                    if (text != null) return text;
                }
            }

            if (Code == null)
            {
                return "Unknown Block with ID " + BlockId;
            }

            string descLangCode = Code.Domain + AssetLocation.LocationSeparator + ItemClass.ToString().ToLowerInvariant() + "desc-" + Code.Path;

            string desc = Lang.GetMatching(descLangCode);

            desc = desc != descLangCode ? desc : "";

            string[] tiers = new string[] { Lang.Get("tier_hands"), Lang.Get("tier_stone"), Lang.Get("tier_copper"), Lang.Get("tier_bronze"), Lang.Get("tier_iron"), Lang.Get("tier_steel"), Lang.Get("tier_titanium") };

            if (RequiredMiningTier > 0)
            {
                string tierName = "?";

                if (RequiredMiningTier < tiers.Length)
                {
                    tierName = tiers[RequiredMiningTier];
                }

                desc += "\n" + Lang.Get("Requires tool tier {0} ({1}) to break", RequiredMiningTier, tierName);
            }

            foreach (BlockBehavior bh in BlockBehaviors)
            {
                desc += bh.GetPlacedBlockInfo(world, pos, forPlayer);
            }

            return desc;
        }

        /// <summary>
        /// Called by the inventory system when you hover over an item stack. This is the text that is getting displayed.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="dsc"></param>
        /// <param name="world"></param>
        /// <param name="withDebugInfo"></param>
        public override void GetHeldItemInfo(ItemStack stack, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            dsc.Append(Lang.Get("Material: ") + Lang.Get("blockmaterial-" + BlockMaterial) + "\n");
            //dsc.Append("Replaceable: " + Replaceable + "\n");
            dsc.Append((Fertility > 0 ? (Lang.Get("Fertility: ") + Fertility + "\n") : ""));

            byte[] lightHsv = GetLightHsv(world.BlockAccessor, null, stack);

            dsc.Append((withDebugInfo ? (lightHsv[2] > 0 ? Lang.Get("light-hsv") + lightHsv[0] + ", " + lightHsv[1] + ", " + lightHsv[2] + "\n" : "") : ""));
            dsc.Append((!withDebugInfo ? (lightHsv[2] > 0 ? Lang.Get("light-level") + lightHsv[2] + "\n" : "") : ""));
            if (LightAbsorption > 0 && LightAbsorption < 33) dsc.Append(Lang.Get("light-absorb") + LightAbsorption + "\n");

            if (WalkSpeedMultiplier != 1)
            {
                dsc.Append(Lang.Get("walk-multiplier") + WalkSpeedMultiplier + "\n");
            }

            base.GetHeldItemInfo(stack, dsc, world, withDebugInfo);
        }

        /// <summary>
        /// If true, the player can select invdividual selection boxes of this block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool DoParticalSelection(IWorldAccessor world, BlockPos pos)
        {
            return PartialSelection;
        }


        /// <summary>
        /// Called by the texture atlas manager when building up the block atlas. Has to add all of the blocks texture
        /// </summary>
        /// <param name="api"></param>
        /// <param name="textureDict"></param>
        public virtual void OnCollectTextures(ICoreAPI api, IBlockTextureLocationDictionary textureDict)
        {
            BakeAndCollect(api, Textures, textureDict);
            BakeAndCollect(api, TexturesInventory, textureDict);
        }

        /// <summary>
        /// Called by the texture atlas manager when building up the block atlas. Has to add all of the blocks texture
        /// </summary>
        /// <param name="api"></param>
        /// <param name="dict"></param>
        /// <param name="textureDict"></param>
        protected virtual void BakeAndCollect(ICoreAPI api, Dictionary<string, CompositeTexture> dict, IBlockTextureLocationDictionary textureDict)
        {
            foreach (var val in dict)
            {
                CompositeTexture ct = val.Value;
                ct.Bake(api.Assets);

                if (ct.Baked.BakedVariants != null)
                {
                    for (int i = 0; i < ct.Baked.BakedVariants.Length; i++)
                    {
                        textureDict.AddTextureLocation(new AssetLocationAndSource(ct.Baked.BakedVariants[i].BakedName, "Baked variant of block " + Code));
                    }
                    continue;
                }

                textureDict.AddTextureLocation(new AssetLocationAndSource(ct.Baked.BakedName, "Baked variant of block " + Code));
            }
        }




        /// <summary>
        /// Returns a new AssetLocation with the wildcards (*) being filled with the blocks other Code parts, if the wildcard matches. 
        /// Example this block is trapdoor-up-north. search is *-up-*, replace is *-down-*, in this case this method will return trapdoor-down-north.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public AssetLocation WildCardReplace(AssetLocation search, AssetLocation replace)
        {
            if (search == Code) return search;

            if (Code == null || search.Domain != Code.Domain) return null;

            string pattern = Regex.Escape(search.Path).Replace(@"\*", @"(.*)");

            Match match = Regex.Match(Code.Path, @"^" + pattern + @"$");
            if (!match.Success) return null;

            string outCode = replace.Path;

            for (int i = 1; i < match.Groups.Count; i++)
            {
                Group g = match.Groups[i];
                CaptureCollection cc = g.Captures;
                for (int j = 0; j < cc.Count; j++)
                {
                    Capture c = cc[j];

                    int pos = outCode.IndexOf('*');
                    outCode = outCode.Remove(pos, 1).Insert(pos, c.Value);
                }
            }

            return new AssetLocation(Code.Domain, outCode);
        }


        /// <summary>
        /// Should return the blocks blast resistance. Default behavior is to return BlockMaterialUtil.MaterialBlastResistance(blastType, BlockMaterial);
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="blastDirectionVector"></param>
        /// <param name="blastType"></param>
        /// <returns></returns>
        public virtual double GetBlastResistance(IWorldAccessor world, BlockPos pos, Vec3f blastDirectionVector, EnumBlastType blastType)
        {
            return BlockMaterialUtil.MaterialBlastResistance(blastType, BlockMaterial);
        }

        /// <summary>
        /// Should return the chance of the block dropping its upon upon being exploded. Default behavior is to return BlockMaterialUtil.MaterialBlastDropChances(blastType, BlockMaterial);
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="blastType"></param>
        /// <returns></returns>
        public virtual double ExplosionDropChance(IWorldAccessor world, BlockPos pos, EnumBlastType blastType) {
            return BlockMaterialUtil.MaterialBlastDropChances(blastType, BlockMaterial);
        }

        /// <summary>
        /// Called when the block was blown up by explosives
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="explosionCenter"></param>
        /// <param name="blastType"></param>
        public virtual void OnBlockExploded(IWorldAccessor world, BlockPos pos, BlockPos explosionCenter, EnumBlastType blastType)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                behavior.OnBlockExploded(world, pos, explosionCenter, blastType, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }

            if (handled == EnumHandling.PreventDefault) return;

            

            double dropChancce = ExplosionDropChance(world, pos, blastType);

            if (world.Rand.NextDouble() < dropChancce)
            {
                ItemStack[] drops = GetDrops(world, pos, null);

                if (drops == null) return;
                
                for (int i = 0; i < drops.Length; i++)
                {
                    if (SplitDropStacks)
                    {
                        for (int k = 0; k < drops[i].StackSize; k++)
                        {
                            ItemStack stack = drops[i].Clone();
                            stack.StackSize = 1;
                            world.SpawnItemEntity(stack, new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5), null);
                        }
                    }
                }
            }

            if (EntityClass != null)
            {
                BlockEntity entity = world.BlockAccessor.GetBlockEntity(pos);
                if (entity != null)
                {
                    entity.OnBlockBroken();
                }
            }

        }

        /// <summary>
        /// Should return the color to be used for the block particle coloring
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="pos"></param>
        /// <param name="facing"></param>
        /// <returns></returns>
        public virtual int GetRandomColor(ICoreClientAPI capi, BlockPos pos, BlockFacing facing)
        {
            if (Textures == null || Textures.Count == 0) return 0;
            CompositeTexture tex;
            if (!Textures.TryGetValue(facing.Code, out tex))
            {
                tex = Textures.First().Value;
            }
            if (tex?.Baked == null) return 0;

            int color = capi.BlockTextureAtlas.GetRandomPixel(tex.Baked.TextureSubId);

            

            // TODO: FIXME
            // We probably need to pre-generate like a list of 50 random colors per block face 
            // not by probing the first texture but by probing all elmenent faces that are facing that block face
            // Has to include the info if that pixel has to be biome tinted or not and then tinted as below
            // Result:
            // - No more white block pixels
            // - Properly biome tinted
            if (TintIndex > 0)
            {
                color = capi.ApplyColorTintOnRgba(TintIndex, color, pos.X, pos.Y, pos.Z);
            }
            
            return color;
        }
        
        /// <summary>
        /// Should return a random pixel within the items/blocks texture
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="facing"></param>
        /// <param name="tintIndex"></param>
        /// <returns></returns>
        public override int GetRandomColor(ICoreClientAPI capi, ItemStack stack)
        {
            if (Textures == null || Textures.Count == 0) return 0;

            BakedCompositeTexture tex = Textures?.First().Value?.Baked;
            return tex == null ? 0 : capi.BlockTextureAtlas.GetRandomPixel(tex.TextureSubId);
        }



        /// <summary>
        /// Should return an RGB color for this block. Current use: In the world map. Default behavior: The 2 averaged pixels at 40%/40% ad 60%/60% position
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        public virtual int GetColor(ICoreClientAPI capi, BlockPos pos)
        {
            int color = GetColorWithoutTint(capi, pos);

            if (TintIndex > 0)
            {
                color = capi.ApplyColorTintOnRgba(TintIndex, color, pos.X, pos.Y, pos.Z, false);
            }

            return color;
        }

        public virtual int GetColorWithoutTint(ICoreClientAPI capi, BlockPos pos)
        {
            int? textureSubId = null;

            if (Textures.ContainsKey("up"))
            {
                textureSubId = Textures["up"].Baked.TextureSubId;
            }
            else
            {
                textureSubId = Textures?.First().Value?.Baked.TextureSubId;
            }

            if (textureSubId == null) return ColorUtil.WhiteArgb;

            int color = ColorUtil.ReverseColorBytes(
                ColorUtil.ColorOverlay(
                    capi.BlockTextureAtlas.GetPixelAt((int)textureSubId, 0.4f, 0.4f),
                    capi.BlockTextureAtlas.GetPixelAt((int)textureSubId, 0.6f, 0.6f),
                    0.5f
                )
            );

            return color;
        }
        


        /// <summary>
        /// Creates a deep copy of the block
        /// </summary>
        /// <returns></returns>
        public Block Clone()
        {
            Block cloned = (Block)MemberwiseClone();

            cloned.Code = this.Code.Clone();

            if (MiningSpeed != null) cloned.MiningSpeed = new Dictionary<EnumBlockMaterial, float>(MiningSpeed);

            cloned.Textures = new Dictionary<string, CompositeTexture>();
            foreach (var var in Textures)
            {
                cloned.Textures[var.Key] = var.Value.Clone();
            }
            cloned.TexturesInventory = new Dictionary<string, CompositeTexture>();
            foreach (var var in TexturesInventory)
            {
                cloned.TexturesInventory[var.Key] = var.Value.Clone();
            }

            cloned.Shape = Shape.Clone();

            if (LightHsv != null)
            {
                cloned.LightHsv = (byte[])LightHsv.Clone();
            }

            if (ParticleProperties != null)
            {
                cloned.ParticleProperties = new AdvancedParticleProperties[ParticleProperties.Length];
                for (int i = 0; i < ParticleProperties.Length; i++)
                {
                    cloned.ParticleProperties[i] = ParticleProperties[i].Clone();
                }
            }

            if (Drops != null)
            {
                cloned.Drops = new BlockDropItemStack[Drops.Length];
                for (int i = 0; i < Drops.Length; i++)
                {
                    cloned.Drops[i] = Drops[i].Clone();
                }
            }

            if (SideOpaque != null)
            {
                cloned.SideOpaque = (bool[])SideOpaque.Clone();
            }

            if (SideSolid != null)
            {
                cloned.SideSolid = (bool[])SideSolid.Clone();
            }

            if (SideAo != null)
            {
                cloned.SideAo = (bool[])SideAo.Clone();
            }

            if (CombustibleProps != null)
            {
                cloned.CombustibleProps = CombustibleProps.Clone();
            }

            if (NutritionProps != null)
            {
                cloned.NutritionProps = NutritionProps.Clone();
            }

            if (GrindingProps != null)
            {
                cloned.GrindingProps = GrindingProps.Clone();
            }

            if (Attributes != null) cloned.Attributes = Attributes.Clone();

            return cloned;
        }



        void WalkBehaviors(BehaviorDelegate onBehavior, Action defaultAction)
        {
            bool executeDefault = true;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                onBehavior(behavior, ref handling);

                if (handling == EnumHandling.PreventSubsequent) return;
                if (handling == EnumHandling.PreventDefault) executeDefault = false;
            }

            if (executeDefault) defaultAction();
        }
    }
}