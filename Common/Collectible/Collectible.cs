using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Contains all properties shared by Blocks and Items
    /// </summary>
    public abstract class CollectibleObject : RegistryObject
    {
        /// <summary>
        /// Liquids are handled and rendered differently than solid blocks.
        /// </summary>
        public EnumMatterState MatterState = EnumMatterState.Solid;

        /// <summary>
        /// This value is set the the BlockId or ItemId-Remapper if it encounters a block/item in the savegame, 
        /// but no longer exists as a loaded block/item
        /// </summary>
        public bool IsMissing { get; set; }

        /// <summary>
        /// The block or item id
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// Block or Item?
        /// </summary>
        public abstract EnumItemClass ItemClass { get; }

        /// <summary>
        /// Max amount of collectible that one default inventory slot can hold
        /// </summary>
        public int MaxStackSize = 64;

        /// <summary>
        /// How many uses does this collectible has when being used. Item disappears at durability 0
        /// </summary>
        public int Durability = 1;

        /// <summary>
        /// When true, liquids become selectable to the player when being held in hands
        /// </summary>
        public bool LiquidSelectable;

        /// <summary>
        /// How much damage this collectible deals when used as a weapon
        /// </summary>
        public float AttackPower = 0.5f;

        /// <summary>
        /// Until how for away can you attack entities using this collectibe
        /// </summary>
        public float AttackRange = GlobalConstants.DefaultAttackRange;

        /// <summary>
        /// From which damage sources does the item takes durability damage
        /// </summary>
        public EnumItemDamageSource[] DamagedBy;

        /// <summary>
        /// Modifies how fast the player can break a block when holding this item
        /// </summary>
        public Dictionary<EnumBlockMaterial, float> MiningSpeed;

        /// <summary>
        /// What tier this block can mine when held in hands
        /// </summary>
        public int MiningTier;

        /// <summary>
        /// List of creative tabs in which this collectible should appear in
        /// </summary>
        public string[] CreativeInventoryTabs;

        /// <summary>
        /// If you want to add itemstacks with custom attributes to the creative inventory, add them to this list
        /// </summary>
        public CreativeTabAndStackList[] CreativeInventoryStacks;

        /// <summary>
        /// Alpha test value for rendering in gui, fp hand, tp hand or on the ground
        /// </summary>
        public float RenderAlphaTest = 0.01f;

        /// <summary>
        /// Used for scaling, rotation or offseting the block when rendered in guis
        /// </summary>
        public ModelTransform GuiTransform;

        /// <summary>
        /// Used for scaling, rotation or offseting the block when rendered in the first person mode hand
        /// </summary>
        public ModelTransform FpHandTransform;

        /// <summary>
        /// Used for scaling, rotation or offseting the block when rendered in the third person mode hand
        /// </summary>
        public ModelTransform TpHandTransform;

        /// <summary>
        /// Used for scaling, rotation or offseting the rendered as a dropped item on the ground
        /// </summary>
        public ModelTransform GroundTransform;

        /// <summary>
        /// Custom Attributes that's always assiociated with this item
        /// </summary>
        public JsonObject Attributes;

        /// <summary>
        /// Information about the blocks burnable states
        /// </summary>
        public CombustibleProperties CombustibleProps = null;

        /// <summary>
        /// Information about the blocks nutrition states
        /// </summary>
        public FoodNutritionProperties NutritionProps = null;

        /// <summary>
        /// Information about the blocks grinding properties
        /// </summary>
        public GrindingProperties GrindingProps = null;


        /// <summary>
        /// If set, this item will be classified as given tool
        /// </summary>
        public EnumTool? Tool;

        /// <summary>
        /// Determines in which kind of bags the item can be stored in
        /// </summary>
        public EnumItemStorageFlags StorageFlags = EnumItemStorageFlags.General;

        /// <summary>
        /// Determines on whether an object floats on liquids or not. Water has a density of 1000
        /// </summary>
        public int MaterialDensity = 2000;

        /// <summary>
        /// The animation to play in 3rd person mod when hitting with this collectible
        /// </summary>
        public string HeldTpHitAnimation = "breakhand";

        /// <summary>
        /// The animation to play in 3rd person mod when holding this collectible in the right hand
        /// </summary>
        public string HeldRightTpIdleAnimation;

        /// <summary>
        /// The animation to play in 3rd person mod when holding this collectible in the left hand
        /// </summary>
        public string HeldLeftTpIdleAnimation;


        /// <summary>
        /// The animation to play in 3rd person mod when using this collectible
        /// </summary>
        public string HeldTpUseAnimation = "placeblock";


        static int[] ItemDamageColor;

        /// <summary>
        /// The api object, assigned during OnLoaded
        /// </summary>
        protected ICoreAPI api;


        static CollectibleObject()
        {
            int[] colors = new int[]
            {
                ColorUtil.Hex2Int("#A7251F"),
                ColorUtil.Hex2Int("#F01700"),
                ColorUtil.Hex2Int("#F04900"),
                ColorUtil.Hex2Int("#F07100"),
                ColorUtil.Hex2Int("#F0D100"),
                ColorUtil.Hex2Int("#F0ED00"),
                ColorUtil.Hex2Int("#E2F000"),
                ColorUtil.Hex2Int("#AAF000"),
                ColorUtil.Hex2Int("#71F000"),
                ColorUtil.Hex2Int("#33F000"),
                ColorUtil.Hex2Int("#00F06B"),
            };

            ItemDamageColor = new int[100];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    ItemDamageColor[10 * i + j] = ColorUtil.ColorOverlay(colors[i], colors[i + 1], j / 10f);
                }
            }
        }


        // Non overridable so people don't accidently forget to call the base method for assigning the api in OnLoaded
        public void OnLoadedNative(ICoreAPI api)
        {
            this.api = api;
            OnLoaded(api);
        }

        /// <summary>
        /// Server Side: Called one the collectible has been registered
        /// Client Side: Called once the collectible has been loaded from server packet
        /// </summary>
        public virtual void OnLoaded(ICoreAPI api)
        {
        }

        /// <summary>
        /// Called when the client/server is shutting down
        /// </summary>
        /// <param name="api"></param>
        public virtual void OnUnloaded(ICoreAPI api)
        {

        }

        /// <summary>
        /// Should return the nutrition properties of the item/block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <param name="forEntity"></param>
        /// <returns></returns>
        public virtual FoodNutritionProperties GetNutritionProperties(IWorldAccessor world, ItemStack itemstack, Entity forEntity)
        {
            return NutritionProps;
        }

        /// <summary>
        /// Should return in which storage containers this item can be placed in
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual EnumItemStorageFlags GetStorageFlags(ItemStack itemstack)
        {
            // We clear the backpack flag if the backpack is empty
            if ((StorageFlags & EnumItemStorageFlags.Backpack) > 0 && IsEmptyBackPack(itemstack)) return EnumItemStorageFlags.General | EnumItemStorageFlags.Backpack;

            return StorageFlags;
        }

        /// <summary>
        /// Returns a hardcoded rgb color (green->yellow->red) that is representative for its remaining durability vs total durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual int GetItemDamageColor(ItemStack itemstack)
        {
            int p = GameMath.Clamp((100 * itemstack.Attributes.GetInt("durability")) / Durability, 0, 99);

            return ItemDamageColor[p];
        }

        /// <summary>
        /// Return true if remaining durability != total durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool ShouldDisplayItemDamage(IItemStack itemstack)
        {
            return Durability != itemstack.Attributes.GetInt("durability", Durability);
        }



        /// <summary>
        /// This method is called before rendering the item stack into GUI, first person hand, third person hand and/or on the ground
        /// The renderinfo object is pre-filled with default values. 
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="itemstack"></param>
        /// <param name="target"></param>
        /// <param name="renderinfo"></param>
        public virtual void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
        {

        }


        /// <summary>
        /// Returns the items remaining durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual float GetDurability(IItemStack itemstack)
        {
            return Durability;
        }

        /// <summary>
        /// The amount of damage dealt when used as a weapon
        /// </summary>
        /// <param name="withItemStack"></param>
        /// <returns></returns>
        public virtual float GetAttackPower(IItemStack withItemStack)
        {
            return AttackPower;
        }

        /// <summary>
        /// The the attack range when used as a weapon
        /// </summary>
        /// <param name="withItemStack"></param>
        /// <returns></returns>
        public virtual float GetAttackRange(IItemStack withItemStack)
        {
            return AttackRange;
        }



        /// <summary>
        /// Player is holding this collectible and breaks the targeted block
        /// </summary>
        /// <param name="player"></param>
        /// <param name="blockSel"></param>
        /// <param name="itemslot"></param>
        /// <param name="remainingResistance"></param>
        /// <param name="dt"></param>
        /// <param name="counter"></param>
        /// <returns></returns>
        public virtual float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
        {
            Block block = player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);

            Vec3f faceVec = blockSel.Face.Normalf;
            Random rnd = player.Entity.World.Rand;

            bool cantMine = block.RequiredMiningTier > 0 && (itemslot.Itemstack.Collectible.MiningTier < block.RequiredMiningTier || !MiningSpeed.ContainsKey(block.BlockMaterial));

            if ((counter % 5 == 0) && (rnd.NextDouble() < 0.12 || cantMine) && (block.BlockMaterial == EnumBlockMaterial.Stone || block.BlockMaterial == EnumBlockMaterial.Ore) && (Tool == EnumTool.Pickaxe || Tool == EnumTool.Hammer))
            {
                double posx = blockSel.Position.X + blockSel.HitPosition.X;
                double posy = blockSel.Position.Y + blockSel.HitPosition.Y;
                double posz = blockSel.Position.Z + blockSel.HitPosition.Z;

                player.Entity.World.SpawnParticles(new SimpleParticleProperties()
                {
                    minQuantity = 0,
                    addQuantity = 8,
                    color = ColorUtil.ToRgba(255, 255, 255, 128),
                    minPos = new Vec3d(posx + faceVec.X * 0.01f, posy + faceVec.Y * 0.01f, posz + faceVec.Z * 0.01f),
                    addPos = new Vec3d(0, 0, 0),
                    minVelocity = new Vec3f(
                        4 * faceVec.X,
                        4 * faceVec.Y,
                        4 * faceVec.Z
                    ),
                    addVelocity = new Vec3f(
                        8 * ((float)rnd.NextDouble() - 0.5f),
                        8 * ((float)rnd.NextDouble() - 0.5f),
                        8 * ((float)rnd.NextDouble() - 0.5f)
                    ),
                    lifeLength = 0.025f,
                    gravityEffect = 0f,
                    minSize = 0.03f,
                    maxSize = 0.4f,
                    model = EnumParticleModel.Cube,
                    glowLevel = 200,
                    SizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -0.15f)
                });
            }


            if (cantMine)
            {
                return remainingResistance;
            }

            return remainingResistance - GetMiningSpeed(itemslot.Itemstack, block) * dt;
        }


        /// <summary>
        /// Player has broken a block while holding this collectible. Return false if you want to cancel the block break event.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byEntity"></param>
        /// <param name="itemslot"></param>
        /// <param name="blockSel"></param>
        /// <returns></returns>
        public virtual bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel)
        {
            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            Block block = world.BlockAccessor.GetBlock(blockSel.Position);
            block.OnBlockBroken(world, blockSel.Position, byPlayer);

            if (DamagedBy != null && DamagedBy.Contains(EnumItemDamageSource.BlockBreaking))
            {
                DamageItem(world, byEntity, itemslot);
            }

            return true;
        }



        /// <summary>
        /// Called every game tick when the player breaks a block with this item in his hands. Returns the mining speed for given block.
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual float GetMiningSpeed(IItemStack itemstack, Block block)
        {
            if (MiningSpeed == null || !MiningSpeed.ContainsKey(block.BlockMaterial)) return 1f;

            return MiningSpeed[block.BlockMaterial];
        }


        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        public virtual ModelTransformKeyFrame[] GeldHeldFpHitAnimation(ItemSlot slot, Entity byEntity)
        {
            return null;
        }

        /// <summary>
        /// Called when an entity uses this item to hit something in 3rd person mode
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        public virtual string GetHeldTpHitAnimation(ItemSlot slot, Entity byEntity)
        {
            return HeldTpHitAnimation;
        }

        /// <summary>
        /// Called when an entity holds this item in hands in 3rd person mode
        /// </summary>
        /// <param name="activeHotbarSlot"></param>
        /// <param name="forEntity"></param>
        /// <returns></returns>
        public virtual string GetHeldTpIdleAnimation(ItemSlot activeHotbarSlot, Entity forEntity, EnumHand hand)
        {
            return hand == EnumHand.Left ? HeldLeftTpIdleAnimation : HeldRightTpIdleAnimation;
        }

        /// <summary>
        /// Called when an entity holds this item in hands in 3rd person mode
        /// </summary>
        /// <param name="activeHotbarSlot"></param>
        /// <param name="forEntity"></param>
        /// <returns></returns>
        public virtual string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity forEntity)
        {
            if (GetNutritionProperties(forEntity.World, activeHotbarSlot.Itemstack, forEntity) != null) return null;

            return HeldTpUseAnimation;
        }

        /// <summary>
        /// An entity used this collectibe to attack something
        /// </summary>
        /// <param name="world"></param>
        /// <param name="attackedEntity"></param>
        /// <param name="itemslot"></param>
        public virtual void OnAttackingWith(IWorldAccessor world, Entity byEntity, Entity attackedEntity, ItemSlot itemslot)
        {
            if (DamagedBy != null && DamagedBy.Contains(EnumItemDamageSource.Attacking) && attackedEntity?.Alive == true)
            {
                DamageItem(world, byEntity, itemslot);
            }
        }


        /// <summary>
        /// Called when this collectible is attempted to being used as part of a crafting recipe and should get consumed now. Return false if it doesn't match the ingredient
        /// </summary>
        /// <param name="inputStack"></param>
        /// <param name="gridRecipe"></param>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        public virtual bool MatchesForCrafting(ItemStack inputStack, GridRecipe gridRecipe, CraftingRecipeIngredient ingredient)
        {
            return true;
        }



        /// <summary>
        /// Called when this collectible is being used as part of a crafting recipe and should get consumed now
        /// </summary>
        /// <param name="allInputSlots"></param>
        /// <param name="stackInSlot"></param>
        /// <param name="gridRecipe"></param>
        /// <param name="fromIngredient"></param>
        /// <param name="byPlayer"></param>
        /// <param name="quantity"></param>
        public virtual void OnConsumedByCrafting(ItemSlot[] allInputSlots, ItemSlot stackInSlot, GridRecipe gridRecipe, CraftingRecipeIngredient fromIngredient, IPlayer byPlayer, int quantity)
        {
            if (fromIngredient.IsTool)
            {
                stackInSlot.Itemstack.Collectible.DamageItem(byPlayer.Entity.World, byPlayer.Entity, stackInSlot);
            }
            else
            {
                stackInSlot.Itemstack.StackSize -= quantity;

                if (stackInSlot.Itemstack.StackSize <= 0)
                {
                    stackInSlot.Itemstack = null;
                    stackInSlot.MarkDirty();
                }

                if (fromIngredient.ReturnedStack != null)
                {
                    byPlayer.InventoryManager.TryGiveItemstack(fromIngredient.ReturnedStack.ResolvedItemstack.Clone(), true);
                }
            }
        }



        /// <summary>
        /// Causes the item to be damaged. Will play a breaking sound and removes the itemstack if no more durability is left
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byEntity"></param>
        /// <param name="itemslot"></param>
        /// <param name="amount">Amount of damage</param>
        public virtual void DamageItem(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, int amount = 1)
        {
            IItemStack itemstack = itemslot.Itemstack;

            int leftDurability = itemstack.Attributes.GetInt("durability", Durability);
            leftDurability -= amount;
            itemstack.Attributes.SetInt("durability", leftDurability);

            if (leftDurability <= 0)
            {
                itemslot.Itemstack = null;

                if (byEntity is EntityPlayer)
                {
                    IPlayer player = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                    world.PlaySoundAt(new AssetLocation("sounds/effect/toolbreak"), player, player);
                } else
                {
                    world.PlaySoundAt(new AssetLocation("sounds/effect/toolbreak"), byEntity.Pos.X, byEntity.Pos.Y, byEntity.Pos.Z);
                }


            }

            itemslot.MarkDirty();
        }

        /// <summary>
        /// Should return the amount of tool modes this collectible has
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSelection"></param>
        /// <returns></returns>
        public virtual int GetQuantityToolModes(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection)
        {
            return 0;
        }

        /// <summary>
        /// Should draw given tool mode icon
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSelection"></param>
        /// <param name="cr"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="toolMode"></param>
        /// <param name="color"></param>
        public virtual void DrawToolModeIcon(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection, Context cr, int x, int y, int width, int height, int toolMode, int color)
        {

        }



        /// <summary>
        /// Should return the current items tool mode.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSelection"></param>
        /// <returns></returns>
        public virtual int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection)
        {
            return 0;
        }

        /// <summary>
        /// Should set given toolmode
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSelection"></param>
        /// <param name="toolMode"></param>
        public virtual void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection, int toolMode)
        {

        }

        /// <summary>
        /// This method is called during the opaque render pass when this item or block is being held in hands
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="byPlayer"></param>
        public virtual void OnHeldRenderOpaque(ItemSlot inSlot, IClientPlayer byPlayer)
        {

        }

        /// <summary>
        /// This method is called during the order independent transparency render pass when this item or block is being held in hands
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="byPlayer"></param>
        public virtual void OnHeldRenderOit(ItemSlot inSlot, IClientPlayer byPlayer)
        {

        }

        /// <summary>
        /// This method is called during the ortho (for 2D GUIs) render pass when this item or block is being held in hands
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="byPlayer"></param>
        public virtual void OnHeldRenderOrtho(ItemSlot inSlot, IClientPlayer byPlayer)
        {

        }



        /// <summary>
        /// Called every frame when the player is holding this collectible in his hands. Is not called during OnUsing() or OnAttacking()
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        public virtual void OnHeldIdle(ItemSlot slot, EntityAgent byEntity)
        {

        }

        /// <summary>
        /// Called every game tick when this collectible lies dropped on the ground
        /// </summary>
        /// <param name="entityItem"></param>
        public virtual void OnGroundIdle(EntityItem entityItem)
        {

        }

        /// <summary>
        /// Called every frame when this item is being displayed in the gui
        /// </summary>
        /// <param name="world"></param>
        /// <param name="stack"></param>
        public virtual void InGuiIdle(IWorldAccessor world, ItemStack stack)
        {

        }



        /// <summary>
        /// General begin use access. Override OnHeldAttackStart or OnHeldInteractStart to alter the behavior.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="useType"></param>
        /// <param name="handling">Whether or not to do any subsequent actions. If not set or set to NotHandled, the action will not called on the server.</param>
        /// <returns></returns>
        public void OnHeldUseStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumHandInteract useType, bool firstEvent, ref EnumHandHandling handling)
        {
            if (useType == EnumHandInteract.HeldItemAttack)
            {
                OnHeldAttackStart(slot, byEntity, blockSel, entitySel, ref handling);
                return;
            }

            if (useType == EnumHandInteract.HeldItemInteract)
            {
                OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
                return;
            }

        }

        /// <summary>
        /// General cancel use access. Override OnHeldAttackCancel or OnHeldInteractCancel to alter the behavior.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        public EnumHandInteract OnHeldUseCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            EnumHandInteract useType = byEntity.Controls.HandUse;

            bool allowCancel = useType == EnumHandInteract.HeldItemAttack ? OnHeldAttackCancel(secondsPassed, slot, byEntity, blockSel, entitySel, cancelReason) : OnHeldInteractCancel(secondsPassed, slot, byEntity, blockSel, entitySel, cancelReason);
            return allowCancel ? EnumHandInteract.None : useType;
        }

        /// <summary>
        /// General using access. Override OnHeldAttackStep or OnHeldInteractStep to alter the behavior.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <returns></returns>
        public EnumHandInteract OnHeldUseStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            EnumHandInteract useType = byEntity.Controls.HandUse;

            bool shouldContinueUse = useType == EnumHandInteract.HeldItemAttack ? OnHeldAttackStep(secondsPassed, slot, byEntity, blockSel, entitySel) : OnHeldInteractStep(secondsPassed, slot, byEntity, blockSel, entitySel);

            return shouldContinueUse ? useType : EnumHandInteract.None;
        }

        /// <summary>
        /// General use over access. Override OnHeldAttackStop or OnHeldInteractStop to alter the behavior.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="useType"></param>
        public void OnHeldUseStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumHandInteract useType)
        {
            if (useType == EnumHandInteract.HeldItemAttack)
            {
                OnHeldAttackStop(secondsPassed, slot, byEntity, blockSel, entitySel);
            } else
            {
                OnHeldInteractStop(secondsPassed, slot, byEntity, blockSel, entitySel);
            }
        }


        /// <summary>
        /// When the player has begun using this item for attacking (left mouse click). Return true to play a custom action.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling">Whether or not to do any subsequent actions. If not set or set to NotHandled, the action will not called on the server.</param>
        /// <returns></returns>
        public virtual void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
        {

        }

        /// <summary>
        /// When the player has canceled a custom attack action. Return false to deny action cancellation.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            return false;
        }

        /// <summary>
        /// Called continously when a custom attack action is playing. Return false to stop the action.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
        {
            return false;
        }

        /// <summary>
        /// Called when a custom attack action is finished
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        public virtual void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
        {

        }


        /// <summary>
        /// Called when the player right clicks while holding this block/item in his hands
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="firstEvent">True when the player pressed the right mouse button on this block. Every subsequent call, while the player holds right mouse down will be false, it gets called every second while right mouse is down</param>
        /// <param name="handling">Whether or not to do any subsequent actions. If not set or set to NotHandled, the action will not called on the server.</param>
        /// <returns>True if an interaction should happen (makes it sync to the server), false if no sync to server is required</returns>
        public virtual void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity as Entity) != null)
            {
                byEntity.World.RegisterCallback((dt) =>
                {
                    if (byEntity.Controls.HandUse == EnumHandInteract.HeldItemInteract)
                    {
                        IPlayer player = null;
                        if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                        byEntity.PlayEntitySound("eat", player);
                    }
                }, 500);

                byEntity.AnimManager?.StartAnimation("eat");

                handling = EnumHandHandling.PreventDefault;
            }
        }


        /// <summary>
        /// Called every frame while the player is using this collectible. Return false to stop the interaction.
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <returns>False if the interaction should be stopped. True if the interaction should continue</returns>
        public virtual bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity as Entity) == null) return false;

            

            Vec3d pos = byEntity.Pos.AheadCopy(0.4f).XYZ;
            pos.Y += byEntity.EyeHeight - 0.4f;

            if (secondsUsed > 0.5f && (int)(30 * secondsUsed) % 7 == 1)
            {
                byEntity.World.SpawnCubeParticles(pos, slot.Itemstack, 0.3f, 4, 0.5f, (byEntity as EntityPlayer)?.Player);
            }


            if (byEntity.World is IClientWorldAccessor)
            {
                ModelTransform tf = new ModelTransform();

                tf.EnsureDefaultValues();

                tf.Origin.Set(0f, 0, 0f);

                if (secondsUsed > 0.5f)
                {
                    tf.Translation.Y = Math.Min(0.02f, GameMath.Sin(20 * secondsUsed) / 10);
                }

                tf.Translation.X -= Math.Min(1f, secondsUsed * 4 * 1.57f);
                tf.Translation.Y -= Math.Min(0.05f, secondsUsed * 2);

                tf.Rotation.X += Math.Min(30f, secondsUsed * 350);
                tf.Rotation.Y += Math.Min(80f, secondsUsed * 350);

                byEntity.Controls.UsingHeldItemTransformAfter = tf;

                return secondsUsed <= 1f;
            }

            // Let the client decide when he is done eating
            return true;
        }

        /// <summary>
        /// Called when the player successfully completed the using action, always called once an interaction is over
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        public virtual void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            FoodNutritionProperties nutriProps = GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity as Entity);

            if (byEntity.World is IServerWorldAccessor && nutriProps != null && secondsUsed >= 0.95f)
            {
                byEntity.ReceiveSaturation(nutriProps.Saturation, nutriProps.FoodCategory);

                if (nutriProps.EatenStack != null)
                {
                    IPlayer player = null;
                    if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                    if (player == null || !player.InventoryManager.TryGiveItemstack(nutriProps.EatenStack.ResolvedItemstack.Clone(), true))
                    {
                        byEntity.World.SpawnItemEntity(nutriProps.EatenStack.ResolvedItemstack.Clone(), byEntity.LocalPos.XYZ);
                    }
                }

                slot.Itemstack.StackSize--;

                if (nutriProps.Health != 0)
                {
                    byEntity.ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Internal, Type = nutriProps.Health > 0 ? EnumDamageType.Heal : EnumDamageType.Poison }, Math.Abs(nutriProps.Health));
                }

                slot.MarkDirty();
            }
        }

        /// <summary>
        /// When the player released the right mouse button. Return false to deny the cancellation (= will keep using the item until OnHeldInteractStep returns false).
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        public virtual bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            return true;
        }


        /// <summary>
        /// Callback when the player dropped this item from his inventory. You can set handling to PreventDefault to prevent dropping this item.
        /// You can also check if the entityplayer of this player is dead to check if dropping of this item was due the players death
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="slot"></param>
        /// <param name="quantity">Amount of items the player wants to drop</param>
        /// <param name="handling"></param>
        public virtual void OnHeldDropped(IWorldAccessor world, IPlayer byPlayer, ItemSlot slot, int quantity, ref EnumHandling handling)
        {

        }


        /// <summary>
        /// Called by the inventory system when you hover over an item stack. This is the item stack name that is getting displayed.
        /// </summary>
        /// <param name="itemStack"></param>
        /// <returns></returns>
        public virtual string GetHeldItemName(ItemStack itemStack)
        {
            string type = ItemClass == EnumItemClass.Block ? "block" : "item";

            return Lang.GetMatching(Code?.Domain + AssetLocation.LocationSeparator + type + "-" + Code?.Path);
        }


        /// <summary>
        /// Called by the inventory system when you hover over an item stack. This is the text that is getting displayed.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="dsc"></param>
        /// <param name="world"></param>
        /// <param name="withDebugInfo"></param>
        public virtual void GetHeldItemInfo(ItemStack stack, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            string descLangCode = Code?.Domain + AssetLocation.LocationSeparator + ItemClass.ToString().ToLowerInvariant() + "desc-" + Code?.Path;
            string descText = Lang.GetMatching(descLangCode);
            if (descText == descLangCode) descText = "";
            else descText = descText + "\n";

            dsc.Append((withDebugInfo ? "Id: " + Id + "\n" : ""));
            dsc.Append((withDebugInfo ? "Code: " + Code + "\n" : ""));

            float temp = GetTemperature(world, stack);
            if (temp > 20)
            {
                dsc.AppendLine(Lang.Get("Temperature: {0}°C", (int)temp));
            }

            if (Durability > 1)
            {
                dsc.AppendLine(Lang.Get("Durability: {0} / {1}", stack.Attributes.GetInt("durability", Durability), Durability));
            }


            if (MiningSpeed != null && MiningSpeed.Count > 0)
            {
                dsc.AppendLine(Lang.Get("Tool Tier: {0}", MiningTier));

                dsc.Append(Lang.Get("item-tooltip-miningspeed"));
                int i = 0;
                foreach (var val in MiningSpeed)
                {
                    if (val.Value < 1.1) continue;

                    if (i > 0) dsc.Append(", ");
                    dsc.Append(Lang.Get(val.Key.ToString()) + " " + val.Value.ToString("#.#") + "x");
                    i++;
                }

                dsc.Append("\n");

            }

            if (IsBackPack(stack))
            {
                dsc.AppendLine(Lang.Get("Quantity Slots: {0}", QuantityBackPackSlots(stack)));
                ITreeAttribute backPackTree = stack.Attributes.GetTreeAttribute("backpack");
                if (backPackTree != null)
                {
                    bool didPrint = false;

                    ITreeAttribute slotsTree = backPackTree.GetTreeAttribute("slots");

                    foreach (var val in slotsTree)
                    {
                        ItemStack cstack = (ItemStack)val.Value?.GetValue();
                        
                        if (cstack != null && cstack.StackSize > 0)
                        {
                            if (!didPrint)
                            {
                                dsc.AppendLine(Lang.Get("Contents: "));
                                didPrint = true;
                            }
                            cstack.ResolveBlockOrItem(world);
                            dsc.AppendLine("- " + cstack.StackSize + "x " + cstack.GetName());
                        }
                    }

                    if (!didPrint)
                    {
                        dsc.AppendLine(Lang.Get("Empty"));
                    }

                }
            }

            FoodNutritionProperties nutriProps = GetNutritionProperties(world, stack, world.Side == EnumAppSide.Client ? (world as IClientWorldAccessor).Player.Entity : null);
            if (nutriProps != null)
            {
                if (Math.Abs(nutriProps.Health) > 0.001f)
                {
                    dsc.Append(Lang.Get("When eaten: {0} sat, {1} hp\n", nutriProps.Saturation, nutriProps.Health));
                }
                else
                {
                    dsc.Append(Lang.Get("When eaten: {0} sat\n", nutriProps.Saturation));
                }

                dsc.AppendLine(Lang.Get("Food Category: {0}", nutriProps.FoodCategory));
            }

            if (GrindingProps != null)
            {
                dsc.Append(Lang.Get("When ground: Turns into {0}x {1}", GrindingProps.GrindedStack.ResolvedItemstack.StackSize, GrindingProps.GrindedStack.ResolvedItemstack.GetName()));
            }

            if (GetAttackPower(stack) > 0.5f)
            {
                dsc.Append(Lang.Get("Attack power: -{0} hp\n", GetAttackPower(stack).ToString("0.#")));
            }

            if (GetAttackRange(stack) > GlobalConstants.DefaultAttackRange)
            {
                dsc.Append(Lang.Get("Attack range: {0} m\n", GetAttackRange(stack).ToString("0.#")));
            }

            if (CombustibleProps != null)
            {
                if (CombustibleProps.BurnTemperature > 0)
                {
                    dsc.AppendLine(Lang.Get("Burn temperature: {0}°C", CombustibleProps.BurnTemperature));
                    dsc.AppendLine(Lang.Get("Burn duration: {0}s", CombustibleProps.BurnDuration));
                }


                string smelttype = CombustibleProps.SmeltingType.ToString().ToLowerInvariant();
                if (CombustibleProps.MeltingPoint > 0)
                {
                    dsc.AppendLine(Lang.Get("game:smeltpoint-" + smelttype, CombustibleProps.MeltingPoint));
                }

                if (CombustibleProps.SmeltedStack != null)
                {
                    int instacksize = CombustibleProps.SmeltedRatio;
                    int outstacksize = CombustibleProps.SmeltedStack.ResolvedItemstack.StackSize;


                    string str = instacksize == 1 ?
                        Lang.Get("game:smeltdesc-" + smelttype + "-singular", outstacksize, CombustibleProps.SmeltedStack.ResolvedItemstack.GetName()) :
                        Lang.Get("game:smeltdesc-" + smelttype + "-plural", instacksize, outstacksize, CombustibleProps.SmeltedStack.ResolvedItemstack.GetName())
                    ;

                    dsc.AppendLine(str);
                }
            }

            if (descText.Length > 0 && dsc.Length > 0) dsc.Append("\n");
            dsc.Append(descText);

            if (Attributes?["pigment"]?["color"].Exists == true)
            {
                dsc.AppendLine(Lang.Get("Pigment: {0}", Lang.Get(Attributes["pigment"]["name"].AsString())));
            }

        }


        public virtual List<ItemStack> GetHandBookStacks(ICoreClientAPI capi)
        {
            if (Code == null) return null;

            bool inCreativeTab = CreativeInventoryTabs != null && CreativeInventoryTabs.Length > 0;
            bool inCreativeTabStack = CreativeInventoryStacks != null && CreativeInventoryStacks.Length > 0;
            bool explicitlyIncluded = Attributes?["handbook"]?["include"].AsBool() == true;
            bool explicitlyExcluded = Attributes?["handbook"]?["exclude"].AsBool() == true;

            if (explicitlyExcluded) return null;
            if (!explicitlyIncluded && !inCreativeTab && !inCreativeTabStack) return null;

            List<ItemStack> stacks = new List<ItemStack>();

            if (inCreativeTabStack)
            {
                for (int i = 0; i < CreativeInventoryStacks.Length; i++)
                {
                    for (int j = 0; j < CreativeInventoryStacks[i].Stacks.Length; j++)
                    {
                        ItemStack stack = CreativeInventoryStacks[i].Stacks[j].ResolvedItemstack;
                        stack.ResolveBlockOrItem(capi.World);

                        stack = stack.Clone();
                        stack.StackSize = stack.Collectible.MaxStackSize;

                        if (!stacks.Any((stack1) => stack1.Equals(stack)))
                        {
                            stacks.Add(stack);
                        }
                    }
                }
            }
            else
            {
                ItemStack stack = new ItemStack(this);
                stack.StackSize = stack.Collectible.MaxStackSize;

                stacks.Add(stack);
            }

            return stacks;
        }

        

        /// <summary>
        /// Detailed information on this block/item to be displayed in the handbook
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="capi"></param>
        /// <param name="allStacks">An itemstack for every block and item that should be considered during information display</param>
        /// <param name="openDetailPageFor">Callback when someone clicks a displayed itemstack</param>
        /// <returns></returns>
        public virtual RichTextComponentBase[] GetHandbookInfo(ItemStack stack, ICoreClientAPI capi, ItemStack[] allStacks, Action<string> openDetailPageFor)
        {
            List<RichTextComponentBase> components = new List<RichTextComponentBase>();

            components.Add(new ItemstackTextComponent(capi, stack, 100, 10, EnumFloat.Left));
            components.Add(new RichTextComponent(capi, stack.GetName() + "\n", CairoFont.WhiteSmallishText()));
            components.Add(new RichTextComponent(capi, stack.GetDescription(capi.World), CairoFont.WhiteSmallText()));


            components.Add(new ClearFloatTextComponent(capi, 10));


            if (stack.Class == EnumItemClass.Block)
            {
                BlockDropItemStack[] stacks = stack.Block.GetDropsForHandbook(capi.World, capi.World.Player.Entity.Pos.AsBlockPos, capi.World.Player);

                if (stacks != null && stacks.Length > 0)
                {
                    Dictionary<AssetLocation, ItemStack> drops = new Dictionary<AssetLocation, ItemStack>();
                    foreach (var val in stacks)
                    {
                        if (val.ResolvedItemstack == null) continue;
                        drops[val.ResolvedItemstack.Collectible.Code] = val.ResolvedItemstack;
                    }

                    components.Add(new RichTextComponent(capi, Lang.Get("Drops when broken") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                    while (drops.Count > 0)
                    {
                        ItemStack rstack = drops.First().Value;
                        SlideshowItemstackTextComponent comp = new SlideshowItemstackTextComponent(capi, rstack, drops, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
                        components.Add(comp);
                    }

                    components.Add(new ClearFloatTextComponent(capi, 10));
                }
            }



            // Obtained through...
            // * Killing drifters
            // * From flax crops
            List<string> killCreatures = new List<string>();

            foreach (var val in capi.World.EntityTypes)
            {
                if (val.Drops == null) continue;

                for (int i = 0; i < val.Drops.Length; i++)
                {
                    if (val.Drops[i].ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                    {
                        killCreatures.Add(Lang.Get(val.Code.Domain + ":item-creature-" + val.Code.Path));
                    }
                }
            }

            Dictionary<AssetLocation, ItemStack> breakBlocks = new Dictionary<AssetLocation, ItemStack>();

            foreach (var val in allStacks)
            {
                if (val.Block == null) continue;

                ItemStack[] stacks = val.Block.GetDrops(capi.World, capi.World.Player.Entity.Pos.AsBlockPos, capi.World.Player, 1);
                if (stacks == null) continue;

                for (int i = 0; i < stacks.Length; i++)
                {
                    if (stacks[i].Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                    {
                        breakBlocks[val.Block.Code] = new ItemStack(val.Block);
                    }
                }
            }

            bool haveText = false;

            if (killCreatures.Count > 0)
            {
                components.Add(new RichTextComponent(capi, Lang.Get("Obtained by killing") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                components.Add(new RichTextComponent(capi, string.Join(", ", killCreatures) + "\n", CairoFont.WhiteSmallText()));
                haveText = true;
            }



            if (breakBlocks.Count > 0)
            {
                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Obtained by breaking") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                while (breakBlocks.Count > 0)
                {
                    ItemStack rstack = breakBlocks.First().Value;
                    SlideshowItemstackTextComponent comp = new SlideshowItemstackTextComponent(capi, rstack, breakBlocks, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
                    components.Add(comp);
                }
                haveText = true;
            }


            // Found in....
            string customFoundIn = stack.Collectible.Attributes?["handbook"]?["foundIn"]?.AsString(null);
            if (customFoundIn != null)
            {
                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Found in") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                components.Add(new RichTextComponent(capi, Lang.Get(customFoundIn), CairoFont.WhiteSmallText()));
                haveText = true;
            }


            if (Attributes?["hostRockFor"].Exists == true)
            {
                ushort[] blockids = Attributes?["hostRockFor"].AsArray<ushort>();

                OrderedDictionary<string, List<ItemStack>> blocks = new OrderedDictionary<string, List<ItemStack>>();

                for (int i = 0; i < blockids.Length; i++)
                {
                    Block block = api.World.Blocks[blockids[i]];

                    string key = block.Code.ToString();
                    if (block.Attributes?["handbook"]["groupBy"].Exists == true)
                    {
                        key = block.Attributes["handbook"]["groupBy"].AsArray<string>()[0];
                    }

                    if (!blocks.ContainsKey(key))
                    {
                        blocks[key] = new List<ItemStack>();
                    }

                    blocks[key].Add(new ItemStack(block));
                }

                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Host rock for") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));

                foreach (var val in blocks)
                {
                    components.Add(new SlideshowItemstackTextComponent(capi, val.Value.ToArray(), 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))));
                }

                haveText = true;
            }


            if (Attributes?["hostRock"].Exists == true)
            {
                ushort[] blockids = Attributes?["hostRock"].AsArray<ushort>();

                OrderedDictionary<string, List<ItemStack>> blocks = new OrderedDictionary<string, List<ItemStack>>();

                for (int i = 0; i < blockids.Length; i++)
                {
                    Block block = api.World.Blocks[blockids[i]];

                    string key = block.Code.ToString();
                    if (block.Attributes?["handbook"]["groupBy"].Exists == true)
                    {
                        key = block.Attributes["handbook"]["groupBy"].AsArray<string>()[0];
                    }

                    if (!blocks.ContainsKey(key))
                    {
                        blocks[key] = new List<ItemStack>();
                    }

                    blocks[key].Add(new ItemStack(block));
                }

                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Occurs in host rock") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));

                foreach (var val in blocks)
                {
                    components.Add(new SlideshowItemstackTextComponent(capi, val.Value.ToArray(), 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))));
                }

                haveText = true;
            }



            // Alloy for...


            Dictionary<AssetLocation, ItemStack> alloyables = new Dictionary<AssetLocation, ItemStack>();
            foreach (var val in capi.World.Alloys)
            {
                foreach (var ing in val.Ingredients)
                {
                    if (ing.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                    {
                        alloyables[val.Output.ResolvedItemstack.Collectible.Code] = val.Output.ResolvedItemstack;
                    }
                }
            }

            if (alloyables.Count > 0)
            {
                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Alloy for") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                foreach (var val in alloyables)
                {
                    components.Add(new ItemstackTextComponent(capi, val.Value, 40, 10, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))));
                }

                haveText = true;
            }

            // Smelts into
            if (CombustibleProps?.SmeltedStack?.ResolvedItemstack != null && !CombustibleProps.SmeltedStack.ResolvedItemstack.Equals(api.World, stack, GlobalConstants.IgnoredStackAttributes))
            {
                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Smelts into") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                components.Add(new ItemstackTextComponent(capi, CombustibleProps.SmeltedStack.ResolvedItemstack, 40, 10, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))));
                haveText = true;
            }


            // Alloyable from

            Dictionary<AssetLocation, MetalAlloyIngredient[]> alloyableFrom = new Dictionary<AssetLocation, MetalAlloyIngredient[]>();
            foreach (var val in capi.World.Alloys)
            {
                if (val.Output.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    List<MetalAlloyIngredient> ingreds = new List<MetalAlloyIngredient>();
                    foreach (var ing in val.Ingredients) ingreds.Add(ing);
                    alloyableFrom[val.Output.ResolvedItemstack.Collectible.Code] = ingreds.ToArray();
                }
            }

            if (alloyableFrom.Count > 0)
            {
                components.Add(new RichTextComponent(capi, (haveText ? "\n" : "") + Lang.Get("Alloyed from") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                foreach (var val in alloyableFrom)
                {
                    foreach (var ingred in val.Value) {
                        string ratio = " " + Lang.Get("alloy-ratio-from-to", (int)(ingred.MinRatio * 100), (int)(ingred.MaxRatio * 100));
                        components.Add(new RichTextComponent(capi, ratio, CairoFont.WhiteSmallText()));
                        ItemstackComponentBase comp = new ItemstackTextComponent(capi, ingred.ResolvedItemstack, 30, 5, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
                        comp.offY = 8;
                        components.Add(comp);
                    }

                    components.Add(new RichTextComponent(capi, "\n", CairoFont.WhiteSmallText()));
                }

                components.Add(new RichTextComponent(capi, "\n", CairoFont.WhiteSmallText()));

                haveText = true;
            }

            // Ingredient for...
            // Pickaxe
            // Axe

            Dictionary<AssetLocation, ItemStack> recipestacks = new Dictionary<AssetLocation, ItemStack>();

            foreach (var recval in capi.World.GridRecipes)
            {
                foreach (var val in recval.resolvedIngredients)
                {
                    CraftingRecipeIngredient ingred = val;

                    if (ingred != null && ingred.SatisfiesAsIngredient(stack))
                    {
                        recipestacks[recval.Output.ResolvedItemstack.Collectible.Code] = recval.Output.ResolvedItemstack;
                    }
                }
                
            }


            foreach (var val in capi.World.SmithingRecipes)
            {
                if (val.Ingredient.SatisfiesAsIngredient(stack))
                {
                    recipestacks[val.Output.ResolvedItemstack.Collectible.Code] = val.Output.ResolvedItemstack;
                }
            }


            foreach (var val in capi.World.ClayFormingRecipes)
            {
                if (val.Ingredient.SatisfiesAsIngredient(stack))
                {
                    recipestacks[val.Output.ResolvedItemstack.Collectible.Code] = val.Output.ResolvedItemstack;
                }
            }


            foreach (var val in capi.World.KnappingRecipes)
            {
                if (val.Ingredient.SatisfiesAsIngredient(stack))
                {
                    recipestacks[val.Output.ResolvedItemstack.Collectible.Code] = val.Output.ResolvedItemstack;
                }
            }

            if (recipestacks.Count > 0)
            {
                components.Add(new ClearFloatTextComponent(capi, 10));
                components.Add(new RichTextComponent(capi, Lang.Get("Ingredient for") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                while (recipestacks.Count > 0)
                {
                    ItemStack rstack = recipestacks.First().Value;
                    SlideshowItemstackTextComponent comp = new SlideshowItemstackTextComponent(capi, rstack, recipestacks, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
                    components.Add(comp);
                }
            }


            // Created by....
            // * Smithing
            // * Grid crafting:
            //   x x x
            //   x x x
            //   x x x

            bool smithable = false;
            bool knappable = false;
            bool clayformable = false;

            foreach (var val in capi.World.SmithingRecipes)
            {
                if (val.Output.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    smithable = true;
                    break;
                }
            }

            foreach (var val in capi.World.KnappingRecipes)
            {
                if (val.Output.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    knappable = true;
                    break;
                }
            }


            foreach (var val in capi.World.ClayFormingRecipes)
            {
                if (val.Output.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    clayformable = true;
                    break;
                }
            }


            List<GridRecipe> recipes = new List<GridRecipe>();

            foreach (var val in capi.World.GridRecipes)
            {
                if (val.Output.ResolvedItemstack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    recipes.Add(val);
                }
            }


            Dictionary<AssetLocation, ItemStack> bakables = new Dictionary<AssetLocation, ItemStack>();
            foreach (var val in allStacks)
            {
                ItemStack smeltedStack = val.Collectible.CombustibleProps?.SmeltedStack?.ResolvedItemstack;

                if (smeltedStack != null && smeltedStack.Equals(capi.World, stack, GlobalConstants.IgnoredStackAttributes))
                {
                    bakables[val.Collectible.Code] = val;
                }
            }




            string customCreatedBy = stack.Collectible.Attributes?["handbook"]?["createdBy"]?.AsString(null);


            if (recipes.Count > 0 || smithable || knappable || clayformable || customCreatedBy != null || bakables.Count > 0)
            {
                components.Add(new ClearFloatTextComponent(capi, 10));
                components.Add(new RichTextComponent(capi, Lang.Get("Created by") + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
                if (smithable) components.Add(new LinkTextComponent(capi, Lang.Get("Smithing") + "\n", CairoFont.WhiteSmallText(), (cs) => { openDetailPageFor("craftinginfo-smithing"); }));
                if (knappable) components.Add(new LinkTextComponent(capi, Lang.Get("Knapping") + "\n", CairoFont.WhiteSmallText(), (cs) => { openDetailPageFor("craftinginfo-knapping"); }));
                if (clayformable) components.Add(new LinkTextComponent(capi, Lang.Get("Clay forming") + "\n", CairoFont.WhiteSmallText(), (cs) => { openDetailPageFor("craftinginfo-clayforming"); }));
                if (customCreatedBy != null) components.Add(new RichTextComponent(capi, "• " + Lang.Get(customCreatedBy) + "\n", CairoFont.WhiteSmallText()));


                if (bakables.Count > 0)
                {
                    components.Add(new RichTextComponent(capi, "• " + Lang.Get("Cooking/Smelting/Baking") + "\n", CairoFont.WhiteSmallText()));
                    while (bakables.Count > 0)
                    {
                        ItemStack rstack = bakables.First().Value;
                        SlideshowItemstackTextComponent comp = new SlideshowItemstackTextComponent(capi, rstack, bakables, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
                        components.Add(comp);
                    }
                }


                if (recipes.Count > 0)
                {
                    if (knappable) components.Add(new RichTextComponent(capi, "• " + Lang.Get("Crafting") + "\n", CairoFont.WhiteSmallText()));

                    components.Add(new SlideshowGridRecipeTextComponent(capi, recipes.ToArray(), 40, EnumFloat.None, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)), allStacks));
                }
            }

            JsonObject obj = stack.Collectible.Attributes?["handbook"]?["extraSections"];
            if (obj != null && obj.Exists)
            {
                ExtraSection[] sections = obj?.AsObject<ExtraSection[]>();
                for (int i = 0; i < sections.Length; i++)
                {
                    components.Add(new ClearFloatTextComponent(capi, 10));
                    components.Add(new RichTextComponent(capi, Lang.Get(sections[i].Title) + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));

                    components.AddRange(VtmlUtil.Richtextify(capi, Lang.Get(sections[i].Text) + "\n", CairoFont.WhiteSmallText()));
                }
            }

            return components.ToArray();
        }


        class ExtraSection { public string Title=null; public string Text=null; }

        /// <summary>
        /// Should return true if the stack can be placed into given slot
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public virtual bool CanBePlacedInto(ItemStack stack, ItemSlot slot)
        {
            return slot.StorageType == 0 || (slot.StorageType & GetStorageFlags(stack)) > 0;
        }

        /// <summary>
        /// Should return the max. number of items that can be placed from sourceStack into the sinkStack
        /// </summary>
        /// <param name="sinkStack"></param>
        /// <param name="sourceStack"></param>
        /// <returns></returns>
        public virtual int GetMergableQuantity(ItemStack sinkStack, ItemStack sourceStack)
        {
            if (Equals(sinkStack, sourceStack, GlobalConstants.IgnoredStackAttributes) && sinkStack.StackSize < MaxStackSize)
            {
                return Math.Min(MaxStackSize - sinkStack.StackSize, sourceStack.StackSize);
            }

            return 0;
        }

        /// <summary>
        /// Is always called on the sink slots item
        /// </summary>
        /// <param name="op"></param>
        public virtual void TryMergeStacks(ItemStackMergeOperation op)
        {
            op.MovableQuantity = GetMergableQuantity(op.SinkSlot.Itemstack, op.SourceSlot.Itemstack);
            if (op.MovableQuantity == 0) return;
            if (!op.SinkSlot.CanTakeFrom(op.SourceSlot)) return;

            op.MovedQuantity = GameMath.Min(op.SinkSlot.RemainingSlotSpace, op.MovableQuantity, op.RequestedQuantity);

            if (HasTemperature(op.SinkSlot.Itemstack) || HasTemperature(op.SourceSlot.Itemstack))
            {
                if (op.CurrentPriority < EnumMergePriority.DirectMerge)
                {
                    float tempDiff = Math.Abs(GetTemperature(op.World, op.SinkSlot.Itemstack) - GetTemperature(op.World, op.SourceSlot.Itemstack));
                    if (tempDiff > 30)
                    {
                        op.MovedQuantity = 0;
                        op.MovableQuantity = 0;
                        op.RequiredPriority = EnumMergePriority.ConfirmedMerge;
                        return;
                    }
                }
                
                SetTemperature(
                    op.World,
                    op.SinkSlot.Itemstack,
                    (op.SinkSlot.StackSize * GetTemperature(op.World, op.SinkSlot.Itemstack) + op.MovedQuantity * GetTemperature(op.World, op.SourceSlot.Itemstack)) / (op.SinkSlot.StackSize + op.MovedQuantity)
                );
            }


            op.SinkSlot.Itemstack.StackSize += op.MovedQuantity;
            op.SourceSlot.Itemstack.StackSize -= op.MovedQuantity;

            if (op.SourceSlot.Itemstack.StackSize <= 0)
            {
                op.SourceSlot.Itemstack = null;
            }
        }


        /// <summary>
        /// If the item is smeltable, this is the time it takes to smelt at smelting point
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cookingSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public virtual float GetMeltingDuration(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return CombustibleProps == null ? 0 : CombustibleProps.MeltingDuration;
        }

        /// <summary>
        /// If the item is smeltable, this is its melting point
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cookingSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public virtual float GetMeltingPoint(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot)
        {
            return CombustibleProps == null ? 0 : CombustibleProps.MeltingPoint;
        }


        /// <summary>
        /// Should return true if this collectible is smeltable
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cookingSlotsProvider"></param>
        /// <param name="inputStack"></param>
        /// <param name="outputStack"></param>
        /// <returns></returns>
        public virtual bool CanSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemStack inputStack, ItemStack outputStack)
        {
            ItemStack smeltedStack = CombustibleProps?.SmeltedStack?.ResolvedItemstack;

            return
                smeltedStack != null
                && inputStack.StackSize >= CombustibleProps.SmeltedRatio
                && CombustibleProps.MeltingPoint > 0
                && (outputStack == null || outputStack.Collectible.GetMergableQuantity(outputStack, smeltedStack) >= smeltedStack.StackSize)
            ;
        }

        /// <summary>
        /// Transform the item to it's smelted variant
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cookingSlotsProvider"></param>
        /// <param name="inputSlot"></param>
        /// <param name="outputSlot"></param>
        public virtual void DoSmelt(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot)
        {
            if (!CanSmelt(world, cookingSlotsProvider, inputSlot.Itemstack, outputSlot.Itemstack)) return;

            ItemStack smeltedStack = CombustibleProps.SmeltedStack.ResolvedItemstack;

            int batchSize = 1;

            if (outputSlot.Itemstack == null)
            {
                outputSlot.Itemstack = smeltedStack.Clone();
                outputSlot.Itemstack.StackSize = batchSize * smeltedStack.StackSize;
            }
            else
            {
                outputSlot.Itemstack.StackSize += batchSize * smeltedStack.StackSize;
            }

            inputSlot.Itemstack.StackSize -= batchSize * CombustibleProps.SmeltedRatio;

            if (inputSlot.Itemstack.StackSize <= 0)
            {
                inputSlot.Itemstack = null;
            }

            outputSlot.MarkDirty();
        }



        /// <summary>
        /// Returns true if the stack has a temperature attribute
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool HasTemperature(IItemStack itemstack)
        {
            if (itemstack == null || itemstack.Attributes == null) return false;
            return itemstack.Attributes.HasAttribute("temperature");
        }

        /// <summary>
        /// Returns the stacks item temperature in degree celsius
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual float GetTemperature(IWorldAccessor world, ItemStack itemstack)
        {
            if (
                itemstack == null
                || itemstack.Attributes == null
                || itemstack.Attributes["temperature"] == null
                || !(itemstack.Attributes["temperature"] is ITreeAttribute)
            )
            {
                return 20;
            }

            ITreeAttribute attr = ((ITreeAttribute)itemstack.Attributes["temperature"]);

            double nowHours = world.Calendar.TotalHours;
            double lastUpdateHours = attr.GetDouble("temperatureLastUpdate");

            double hourDiff = nowHours - lastUpdateHours;

            // 1.5 deg per irl second
            // 1 game hour = irl 60 seconds
            if (hourDiff > 1/85f)
            {
                float temp = Math.Max(0, attr.GetFloat("temperature", 20) - Math.Max(0, (float)(nowHours - lastUpdateHours) * attr.GetFloat("cooldownSpeed", 90)));
                SetTemperature(world, itemstack, temp);
                return temp;
            }

            return attr.GetFloat("temperature", 20);
        }

        /// <summary>
        /// Sets the stacks item temperature in degree celsius
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <param name="temperature"></param>
        /// <param name="delayCooldown"></param>
        public virtual void SetTemperature(IWorldAccessor world, ItemStack itemstack, float temperature, bool delayCooldown = true)
        {
            if (itemstack == null) return;

            ITreeAttribute attr = ((ITreeAttribute)itemstack.Attributes["temperature"]);

            if (attr == null)
            {
                itemstack.Attributes["temperature"] = attr = new TreeAttribute();
            }

            double nowHours = world.Calendar.TotalHours;
            // If the colletible gets heated, retain the heat for 1.5 ingame hours
            if (delayCooldown && attr.GetFloat("temperature") < temperature) nowHours += 1.5f;

            attr.SetDouble("temperatureLastUpdate", nowHours);
            attr.SetFloat("temperature", temperature);
        }

        /// <summary>
        /// Returns true if this stack is an empty backpack
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public static bool IsEmptyBackPack(IItemStack itemstack)
        {
            if (!IsBackPack(itemstack)) return false;

            ITreeAttribute backPackTree = itemstack.Attributes.GetTreeAttribute("backpack");
            if (backPackTree == null) return true;
            ITreeAttribute slotsTree = backPackTree.GetTreeAttribute("slots");

            foreach (var val in slotsTree)
            {
                IItemStack stack = (IItemStack)val.Value?.GetValue();
                if (stack != null && stack.StackSize > 0) return false;
            }
            return true;
        }


        /// <summary>
        /// Returns true if this stack is a backpack that can hold other items/blocks
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public static bool IsBackPack(IItemStack itemstack)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return false;
            return itemstack.Collectible.Attributes["backpack"]["quantitySlots"].AsInt() > 0;
        }

        /// <summary>
        /// If the stack is a backpack, this returns the amount of slots it has
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public static int QuantityBackPackSlots(IItemStack itemstack)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return 0;
            return itemstack.Collectible.Attributes["backpack"]["quantitySlots"].AsInt();
        }

        /// <summary>
        /// Should return true if given stacks are equal, ignoring their stack size.
        /// </summary>
        /// <param name="thisStack"></param>
        /// <param name="otherStack"></param>
        /// <param name="ignoreAttributeSubTrees"></param>
        /// <returns></returns>
        public virtual bool Equals(ItemStack thisStack, ItemStack otherStack, params string[] ignoreAttributeSubTrees)
        {
            return 
                thisStack.Class == otherStack.Class &&
                thisStack.Id == otherStack.Id &&
                thisStack.Attributes.Equals(api.World, otherStack.Attributes, ignoreAttributeSubTrees)
            ;
        }

        /// <summary>
        /// Should return true if thisStack is a satisfactory replacement of otherStack. It's bascially an Equals() test, but it ignores any additional attributes that exist in otherStack
        /// </summary>
        /// <param name="thisStack"></param>
        /// <param name="otherStack"></param>
        /// <returns></returns>
        public virtual bool Satisfies(ItemStack thisStack, ItemStack otherStack)
        {
            return
                thisStack.Class == otherStack.Class &&
                thisStack.Id == otherStack.Id &&
                thisStack.Attributes.IsSubSetOf(api.World, otherStack.Attributes)
            ;
        }

        /// <summary>
        /// This method is for example called by chests when they are being exported as part of a block schematic. Has to store all the currents block/item id mappings so it can be correctly imported again. By default it puts itself into the mapping and searches the itemstack attributes for attributes of type ItemStackAttribute and adds those to the mapping as well.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inSlot"></param>
        /// <param name="blockIdMapping"></param>
        /// <param name="itemIdMapping"></param>
        public virtual void OnStoreCollectibleMappings(IWorldAccessor world, ItemSlot inSlot, Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            if (this is Item)
            {
                itemIdMapping[Id] = Code;
            }
            else
            {
                blockIdMapping[Id] = Code;
            }

            OnStoreCollectibleMappings(world, inSlot.Itemstack.Attributes, blockIdMapping, itemIdMapping);
        }

        /// <summary>
        /// This method is called after a block/item like this has been imported as part of a block schematic. Has to restore fix the block/item id mappings as they are probably different compared to the world from where they were exported. By default iterates over all the itemstacks attributes and searches for attribute sof type ItenStackAttribute and calls .FixMapping() on them.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="inSlot"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, ItemSlot inSlot, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {
            OnLoadCollectibleMappings(worldForResolve, inSlot.Itemstack.Attributes, oldBlockIdMapping, oldItemIdMapping);
        }

        private void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, ITreeAttribute tree, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {
            foreach (var val in tree)
            {
                if (val.Value is ITreeAttribute)
                {
                    OnLoadCollectibleMappings(worldForResolve, val.Value as ITreeAttribute, oldBlockIdMapping, oldItemIdMapping);
                    continue;
                }

                if (val.Value is ItemstackAttribute)
                {
                    ItemStack stack = (val.Value as ItemstackAttribute).value;
                    stack?.FixMapping(oldBlockIdMapping, oldItemIdMapping, worldForResolve);
                }
            }
        }

        void OnStoreCollectibleMappings(IWorldAccessor world, ITreeAttribute tree, Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            foreach (var val in tree)
            {
                if (val.Value is ITreeAttribute)
                {
                    OnStoreCollectibleMappings(world, val.Value as ITreeAttribute, blockIdMapping, itemIdMapping);
                    continue;
                }

                if (val.Value is ItemstackAttribute)
                {
                    ItemStack stack = (val.Value as ItemstackAttribute).value;
                    if (stack == null) continue;

                    if (stack.Collectible == null) stack.ResolveBlockOrItem(world);

                    if (stack.Class == EnumItemClass.Item)
                    {
                        itemIdMapping[stack.Id] = stack.Collectible.Code;
                    } else
                    {
                        blockIdMapping[stack.Id] = stack.Collectible.Code;
                    }
                }
            }

        }

        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="withInheritance"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(Type type, bool withInheritance)
        {
            return false;
        }

        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasBehavior(Type type)
        {
            return HasBehavior(type, false);
        }

        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classRegistry"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(string type, IClassRegistryAPI classRegistry)
        {
            return false;
        }

        /// <summary>
        /// Should return a random pixel within the items/blocks texture
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public virtual int GetRandomColor(ICoreClientAPI capi, ItemStack stack)
        {
            return 0;
        }



        /// <summary>
        /// Returns true if this blocks matterstate is liquid
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLiquid()
        {
            return MatterState == EnumMatterState.Liquid;
        }

    }
}
