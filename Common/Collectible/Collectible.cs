using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{

    public delegate void CollectibleBehaviorDelegate(CollectibleBehavior behavior, ref EnumHandling handling);
    public class ExtraHandbookSection { public string Title = null; public string Text = null; public string[] TextParts; }

    /// <summary>
    /// Contains all properties shared by Blocks and Items
    /// </summary>
    public abstract class CollectibleObject : RegistryObject
    {
        // ---- Some default objects which are common to many Block and Item objects
        public readonly static Size3f DefaultSize = new Size3f(0.5f, 0.5f, 0.5f);

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
        /// For blocks and items, the hashcode is the id - useful when building HashSets
        /// </summary>
        public override int GetHashCode()
        {
            return Id;
        }

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
        /// Physical size of this collectible when held or (notionally) in a container. 0.5 x 0.5 x 0.5 meters by default.
        /// <br/>Note, if all three dimensions are set to zero, the default will be used.
        /// </summary>
        public Size3f Dimensions = DefaultSize;

        /// <summary>
        /// When true, liquids become selectable to the player when being held in hands
        /// </summary>
        public bool LiquidSelectable;

        /// <summary>
        /// How much damage this collectible deals when used as a weapon
        /// </summary>
        public float AttackPower = 0.5f;

        /// <summary>
        /// If true, when the player holds the sneak key and right clicks with this item in hand, calls OnHeldInteractStart first. Without it, the order is reversed. Takes precedence over priority interact placed blocks.
        /// </summary>
        public bool HeldPriorityInteract;


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
        public int ToolTier;

        [Obsolete("Use tool tier")]
        public int MiningTier { get { return ToolTier; } set { ToolTier = value; } }

        public HeldSounds HeldSounds;

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
        public float RenderAlphaTest = 0.05f;

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
        /// Used for scaling, rotation or offseting the block when rendered in the third person mode offhand
        /// </summary>
        public ModelTransform TpOffHandTransform;

        /// <summary>
        /// Used for scaling, rotation or offseting the rendered as a dropped item on the ground
        /// </summary>
        public ModelTransform GroundTransform;

        /// <summary>
        /// Custom Attributes that's always assiociated with this item
        /// </summary>
        public JsonObject Attributes;

        /// <summary>
        /// Information about the burnable states
        /// </summary>
        public CombustibleProperties CombustibleProps = null;

        /// <summary>
        /// Information about the nutrition states
        /// </summary>
        public FoodNutritionProperties NutritionProps = null;

        /// <summary>
        /// Information about the transitionable states
        /// </summary>
        public TransitionableProperties[] TransitionableProps = null;

        /// <summary>
        /// If set, the collectible can be ground into something else
        /// </summary>
        public GrindingProperties GrindingProps = null;

        /// <summary>
        /// If set, the collectible can be crushed into something else
        /// </summary>
        public CrushingProperties CrushingProps = null;

        /// <summary>
        /// Particles that should spawn in regular intervals from this block or item when held in hands
        /// </summary>
        public AdvancedParticleProperties[] ParticleProperties = null;

        /// <summary>
        /// The origin point from which particles are being spawned
        /// </summary>
        public Vec3f TopMiddlePos = new Vec3f(0.5f, 1, 0.5f);


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
        ///
        /// </summary>
        public string HeldLeftReadyAnimation;

        /// <summary>
        ///
        /// </summary>
        public string HeldRightReadyAnimation;


        /// <summary>
        /// The animation to play in 3rd person mod when using this collectible
        /// </summary>
        public string HeldTpUseAnimation = "interactstatic";



        /// <summary>
        /// The api object, assigned during OnLoaded
        /// </summary>
        protected ICoreAPI api;


        /// <summary>
        /// Modifiers that can alter the behavior of the item or block, mostly for held interaction
        /// </summary>
        public CollectibleBehavior[] CollectibleBehaviors = Array.Empty<CollectibleBehavior>();

        /// <summary>
        /// For light emitting collectibles: hue, saturation and brightness value
        /// </summary>
        public ThreeBytes LightHsv = new ThreeBytes();



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
            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                behavior.OnLoaded(api);
            }
        }

        /// <summary>
        /// Called when the client/server is shutting down
        /// </summary>
        /// <param name="api"></param>
        public virtual void OnUnloaded(ICoreAPI api)
        {
            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                behavior.OnUnloaded(api);
            }
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
        /// Should return the transition properties of the item/block when in itemstack form
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <param name="forEntity"></param>
        /// <returns></returns>
        public virtual TransitionableProperties[] GetTransitionableProperties(IWorldAccessor world, ItemStack itemstack, Entity forEntity)
        {
            return TransitionableProps;
        }


        /// <summary>
        /// Should returns true if this collectible requires UpdateAndGetTransitionStates() to be called when ticking.
        /// <br/>Typical usage: true if this collectible itself has transitionable properties, or true for collectibles which hold other itemstacks with transitionable properties (for example, a cooked food container)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool RequiresTransitionableTicking(IWorldAccessor world, ItemStack itemstack)
        {
            return TransitionableProps != null && TransitionableProps.Length > 0;
        }


        /// <summary>
        /// Should return in which storage containers this item can be placed in
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual EnumItemStorageFlags GetStorageFlags(ItemStack itemstack)
        {
            bool preventDefault = false;
            EnumItemStorageFlags storageFlags = StorageFlags;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                var bhFlags = behavior.GetStorageFlags(itemstack, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    storageFlags = bhFlags;
                }

                if (handled == EnumHandling.PreventSubsequent) return storageFlags;
            }

            if (preventDefault) return storageFlags;

            var bag = GetCollectibleInterface<IHeldBag>();
            // We clear the backpack flag if the backpack is empty
            if (bag != null && (storageFlags & EnumItemStorageFlags.Backpack) > 0 && bag.IsEmpty(itemstack)) return EnumItemStorageFlags.General | EnumItemStorageFlags.Backpack;

            return storageFlags;
        }

        /// <summary>
        /// Returns a hardcoded rgb color (green->yellow->red) that is representative for its remaining durability vs total durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual int GetItemDamageColor(ItemStack itemstack)
        {
            int maxdura = GetMaxDurability(itemstack);
            if (maxdura == 0) return 0;

            int p = GameMath.Clamp(100 * itemstack.Collectible.GetRemainingDurability(itemstack) / maxdura, 0, 99); ;

            return GuiStyle.DamageColorGradient[p];
        }

        /// <summary>
        /// Return true if remaining durability != total durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool ShouldDisplayItemDamage(ItemStack itemstack)
        {
            return GetMaxDurability(itemstack) != GetRemainingDurability(itemstack);
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
            for (int i = 0; i < CollectibleBehaviors.Length; i++)
            {
                CollectibleBehaviors[i].OnBeforeRender(capi, itemstack, target, ref renderinfo);
            }
        }


        [Obsolete("Use GetMaxDurability instead")]
        public virtual int GetDurability(IItemStack itemstack) => GetMaxDurability(itemstack as ItemStack);

        /// <summary>
        /// Returns the items total durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual int GetMaxDurability(ItemStack itemstack)
        {
            int durability = 0;

            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    int additionalDurability = bh.OnGetMaxDurability(itemstack, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough)
                    {
                        durability += additionalDurability;
                    }
                },
                () => {
                    durability += Durability;
                }
            );

            return durability;
        }

        public virtual int GetRemainingDurability(ItemStack itemstack)
        {
            int durability = 0;

            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    int additionalDurability = bh.OnGetMaxDurability(itemstack, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough)
                    {
                        durability += additionalDurability;
                    }
                },
                () => {
                    durability += (int)itemstack.Attributes.GetDecimal("durability", GetMaxDurability(itemstack));
                }
            );

            return durability;
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
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                float remainingResistanceBh = behavior.OnBlockBreaking(player, blockSel, itemslot, remainingResistance, dt, counter, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    remainingResistance = remainingResistanceBh;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return remainingResistance;
            }

            if (preventDefault) return remainingResistance;


            Block block = player.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
            var mat = block.GetBlockMaterial(api.World.BlockAccessor, blockSel.Position);

            Vec3f faceVec = blockSel.Face.Normalf;
            Random rnd = player.Entity.World.Rand;

            bool cantMine = block.RequiredMiningTier > 0 && itemslot.Itemstack?.Collectible != null && (itemslot.Itemstack.Collectible.ToolTier < block.RequiredMiningTier || (MiningSpeed == null || !MiningSpeed.ContainsKey(mat)));

            double chance = mat == EnumBlockMaterial.Ore ? 0.72 : 0.12;

            if ((counter % 5 == 0) && (rnd.NextDouble() < chance || cantMine) && (mat == EnumBlockMaterial.Stone || mat == EnumBlockMaterial.Ore) && (Tool == EnumTool.Pickaxe || Tool == EnumTool.Hammer))
            {
                double posx = blockSel.Position.X + blockSel.HitPosition.X;
                double posy = blockSel.Position.Y + blockSel.HitPosition.Y;
                double posz = blockSel.Position.Z + blockSel.HitPosition.Z;

                player.Entity.World.SpawnParticles(new SimpleParticleProperties()
                {
                    MinQuantity = 0,
                    AddQuantity = 8,
                    Color = ColorUtil.ToRgba(255, 255, 255, 128),
                    MinPos = new Vec3d(posx + faceVec.X * 0.01f, posy + faceVec.Y * 0.01f, posz + faceVec.Z * 0.01f),
                    AddPos = new Vec3d(0, 0, 0),
                    MinVelocity = new Vec3f(
                        4 * faceVec.X,
                        4 * faceVec.Y,
                        4 * faceVec.Z
                    ),
                    AddVelocity = new Vec3f(
                        8 * ((float)rnd.NextDouble() - 0.5f),
                        8 * ((float)rnd.NextDouble() - 0.5f),
                        8 * ((float)rnd.NextDouble() - 0.5f)
                    ),
                    LifeLength = 0.025f,
                    GravityEffect = 0f,
                    MinSize = 0.03f,
                    MaxSize = 0.4f,
                    ParticleModel = EnumParticleModel.Cube,
                    VertexFlags = 200,
                    SizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -0.15f)
                }, player);
            }


            if (cantMine)
            {
                return remainingResistance;
            }

            return remainingResistance - GetMiningSpeed(itemslot.Itemstack, blockSel, block, player) * dt;
        }


        /// <summary>
        /// Whenever the collectible was modified while inside a slot, usually when it was moved, split or merged.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="slot">The slot the item is or was in</param>
        /// <param name="extractedStack">Non null if the itemstack was removed from this slot</param>
        public virtual void OnModifiedInInventorySlot(IWorldAccessor world, ItemSlot slot, ItemStack extractedStack = null)
        {
        }


        /// <summary>
        /// Player has broken a block while holding this collectible. Return false if you want to cancel the block break event.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byEntity"></param>
        /// <param name="itemslot"></param>
        /// <param name="blockSel"></param>
        /// <param name="dropQuantityMultiplier"></param>
        /// <returns></returns>
        public virtual bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = behavior.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;


            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = world.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            Block block = blockSel.Block ?? world.BlockAccessor.GetBlock(blockSel.Position);
            block.OnBlockBroken(world, blockSel.Position, byPlayer, dropQuantityMultiplier);

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
        /// <param name="blockSel"></param>
        /// <param name="block"></param>
        /// <param name="forPlayer"></param>
        /// <returns></returns>
        public virtual float GetMiningSpeed(IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
        {
            float traitRate = 1f;

            EnumBlockMaterial material = block.GetBlockMaterial(api.World.BlockAccessor, blockSel.Position);

            if (material == EnumBlockMaterial.Ore || material == EnumBlockMaterial.Stone) {
                traitRate = forPlayer.Entity.Stats.GetBlended("miningSpeedMul");
            }

            float toolMiningSpeed = 1;

            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    float miningSpeedMultiplier = bh.OnGetMiningSpeed(itemstack, blockSel, block, forPlayer, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough)
                    {
                        toolMiningSpeed *= miningSpeedMultiplier;
                    }
                },
                () => {
                    if (MiningSpeed == null || !MiningSpeed.ContainsKey(material))
                    {
                        toolMiningSpeed *= traitRate;
                    }
                    else
                    {
                        toolMiningSpeed *= MiningSpeed[material] * traitRate * GlobalConstants.ToolMiningSpeedModifier;
                    }
                }
            );

            return toolMiningSpeed;
        }


        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        [Obsolete]
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
            EnumHandling bhHandling = EnumHandling.PassThrough;
            string anim = null;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    string bhanim = bh.GetHeldTpHitAnimation(slot, byEntity, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough) anim = bhanim;
                },
                () => { anim = HeldTpHitAnimation; }
            );

            return anim;
        }

        /// <summary>
        /// Called when an entity holds this item in hands in 3rd person mode
        /// </summary>
        /// <param name="activeHotbarSlot"></param>
        /// <param name="forEntity"></param>
        /// <param name="hand"></param>
        /// <returns></returns>
        public virtual string GetHeldReadyAnimation(ItemSlot activeHotbarSlot, Entity forEntity, EnumHand hand)
        {
            EnumHandling bhHandling = EnumHandling.PassThrough;
            string anim = null;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    string bhanim = bh.GetHeldReadyAnimation(activeHotbarSlot, forEntity, hand, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough) anim = bhanim;
                },
                () => { anim = hand == EnumHand.Left ? HeldLeftReadyAnimation : HeldRightReadyAnimation; }
            );

            return anim;
        }


        /// <summary>
        /// Called when an entity holds this item in hands in 3rd person mode
        /// </summary>
        /// <param name="activeHotbarSlot"></param>
        /// <param name="forEntity"></param>
        /// <param name="hand"></param>
        /// <returns></returns>
        public virtual string GetHeldTpIdleAnimation(ItemSlot activeHotbarSlot, Entity forEntity, EnumHand hand)
        {
            EnumHandling bhHandling = EnumHandling.PassThrough;
            string anim = null;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    string bhanim = bh.GetHeldTpIdleAnimation(activeHotbarSlot, forEntity, hand, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough) anim = bhanim;
                },
                () => { anim = hand == EnumHand.Left ? HeldLeftTpIdleAnimation : HeldRightTpIdleAnimation; }
            );

            return anim;
        }

        /// <summary>
        /// Called when an entity holds this item in hands in 3rd person mode
        /// </summary>
        /// <param name="activeHotbarSlot"></param>
        /// <param name="forEntity"></param>
        /// <returns></returns>
        public virtual string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity forEntity)
        {
            EnumHandling bhHandling = EnumHandling.PassThrough;
            string anim = null;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    string bhanim = bh.GetHeldTpUseAnimation(activeHotbarSlot, forEntity, ref bhHandling);
                    if (bhHandling != EnumHandling.PassThrough) anim = bhanim;
                },
                () => {
                    if (GetNutritionProperties(forEntity.World, activeHotbarSlot.Itemstack, forEntity) == null)
                    {
                        anim = HeldTpUseAnimation;
                    }
                }
            );

            return anim;
        }

        /// <summary>
        /// An entity used this collectibe to attack something
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byEntity"></param>
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
            if (ingredient.IsTool && ingredient.ToolDurabilityCost > inputStack.Collectible.GetRemainingDurability(inputStack)) return false;
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
            if (Attributes?["noConsumeOnCrafting"].AsBool(false) == true) return;

            if (fromIngredient.IsTool)
            {
                if (fromIngredient.ToolDurabilityCost > 0) stackInSlot.Itemstack.Collectible.DamageItem(byPlayer.Entity.World, byPlayer.Entity, stackInSlot, fromIngredient.ToolDurabilityCost);
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
                    var returnedStack = fromIngredient.ReturnedStack.ResolvedItemstack.Clone();
                    if (!byPlayer.InventoryManager.TryGiveItemstack(returnedStack, true))
                    {
                        api.World.SpawnItemEntity(returnedStack, byPlayer.Entity.Pos.XYZ);
                    }
                }
            }
        }


        /// <summary>
        /// Called when a matching grid recipe has been found and an item is placed into the crafting output slot (which is still before the player clicks on the output slot to actually craft the item and consume the ingredients)
        /// </summary>
        /// <param name="allInputslots"></param>
        /// <param name="outputSlot"></param>
        /// <param name="byRecipe"></param>
        public virtual void OnCreatedByCrafting(ItemSlot[] allInputslots, ItemSlot outputSlot, GridRecipe byRecipe)
        {
            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    bh.OnCreatedByCrafting(allInputslots, outputSlot, byRecipe, ref bhHandling);
                },
                () => {

                    float pSum = 0f;
                    float q = 0;

                    if (byRecipe.AverageDurability)
                    {
                        var ingreds = byRecipe.resolvedIngredients;

                        foreach (ItemSlot slot in allInputslots)
                        {
                            if (slot.Empty) continue;
                            ItemStack stack = slot.Itemstack;

                            int maxDurability = stack.Collectible.GetMaxDurability(stack);
                            if (maxDurability == 0)
                            {
                                // An item with no durability only improves the average by 12.5%
                                pSum += 0.125f;
                                q += 0.125f;
                                continue;
                            }

                            bool skip = false;
                            foreach (var ingred in ingreds)
                            {
                                if (ingred != null && ingred.IsTool && ingred.SatisfiesAsIngredient(stack))
                                {
                                    skip = true;
                                    break;
                                }
                            }
                            if (skip) continue;

                            q++;
                            int leftDurability = stack.Collectible.GetRemainingDurability(stack);
                            pSum += (float)leftDurability / maxDurability;

                        }

                        float pFinal = pSum / q;
                        if (pFinal < 1)
                        {
                            outputSlot.Itemstack.Attributes.SetInt("durability", (int)Math.Max(1, pFinal * outputSlot.Itemstack.Collectible.GetMaxDurability(outputSlot.Itemstack)));
                        }
                    }


                    TransitionableProperties[] tprops = outputSlot.Itemstack.Collectible.GetTransitionableProperties(api.World, outputSlot.Itemstack, null);
                    var perishProps = tprops?.FirstOrDefault(p => p.Type == EnumTransitionType.Perish);
                    if (perishProps != null)
                    {
                        perishProps.TransitionedStack.Resolve(api.World, "oncrafted perished stack", Code);
                        CarryOverFreshness(api, allInputslots, new ItemStack[] { outputSlot.Itemstack }, perishProps);
                    }

                }
            );
        }

        /// <summary>
        /// Called after the player has taken out the item from the output slot
        /// </summary>
        /// <param name="slots"></param>
        /// <param name="outputSlot"></param>
        /// <param name="matchingRecipe"></param>
        /// <returns>true to prevent default ingredient consumption</returns>
        public virtual bool ConsumeCraftingIngredients(ItemSlot[] slots, ItemSlot outputSlot, GridRecipe matchingRecipe)
        {
            return false;
        }



        /// <summary>
        /// Sets the items durability
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="amount"></param>
        public virtual void SetDurability(ItemStack itemstack, int amount)
        {
            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    bh.OnSetDurability(itemstack, ref amount, ref bhHandling);
                },
                () => {
                    itemstack.Attributes.SetInt("durability", amount);
                }
            );
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
            EnumHandling bhHandling = EnumHandling.PassThrough;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    bh.OnDamageItem(world, byEntity, itemslot, ref amount, ref bhHandling);
                },
                () => {
                    ItemStack itemstack = itemslot.Itemstack;

                    int leftDurability = itemstack.Collectible.GetRemainingDurability(itemstack);
                    leftDurability -= amount;
                    itemstack.Attributes.SetInt("durability", leftDurability);

                    if (leftDurability <= 0)
                    {
                        itemslot.Itemstack = null;

                        IPlayer player = (byEntity as EntityPlayer)?.Player;

                        if (player != null)
                        {
                            if (Tool != null)
                            {
                                string ident = Attributes?["slotRefillIdentifier"].ToString();
                                RefillSlotIfEmpty(itemslot, byEntity as EntityAgent, (stack) => {
                                    return ident != null ? stack.ItemAttributes?["slotRefillIdentifier"]?.ToString() == ident : stack.Collectible.Tool == Tool;
                                });

                                if (!itemslot.Empty && Attributes?.IsTrue("rememberToolModeWhenBroken") == true)
                                {
                                    itemslot.Itemstack.Collectible.SetToolMode(itemslot, player, null, GetToolMode(new DummySlot(itemstack), player, null));
                                }
                            }

                            if (itemslot.Itemstack != null && !itemslot.Itemstack.Attributes.HasAttribute("durability"))
                            {
                                itemstack = itemslot.Itemstack;
                                itemstack.Attributes.SetInt("durability", itemstack.Collectible.GetMaxDurability(itemstack));
                                // This forces update of durability when slot is marked dirty (otherwise the durability attribute would be unset, therefore not updated, for a new item)
                            }

                            world.PlaySoundAt(new AssetLocation("sounds/effect/toolbreak"), player, null);
                        }
                        else
                        {
                            world.PlaySoundAt(new AssetLocation("sounds/effect/toolbreak"), byEntity.SidedPos.X, byEntity.SidedPos.Y, byEntity.SidedPos.Z, null, 1, 16);
                        }

                        world.SpawnCubeParticles(byEntity.SidedPos.XYZ.Add(byEntity.SelectionBox.Y2 / 2), itemstack, 0.25f, 30, 1, player);
                    }
                }
            );

            itemslot.MarkDirty();
        }



        public virtual void RefillSlotIfEmpty(ItemSlot slot, EntityAgent byEntity, ActionConsumable<ItemStack> matcher)
        {
            if (!slot.Empty) return;

            byEntity.WalkInventory((invslot) =>
            {
                if (invslot is ItemSlotCreative) return true;

                InventoryBase inv = invslot.Inventory;
                if (!(inv is InventoryBasePlayer) && !inv.HasOpened((byEntity as EntityPlayer).Player)) return true;

                if (invslot.Itemstack != null && matcher(invslot.Itemstack))
                {
                    invslot.TryPutInto(byEntity.World, slot);
                    invslot.Inventory?.PerformNotifySlot(invslot.Inventory.GetSlotId(invslot));
                    slot.Inventory?.PerformNotifySlot(slot.Inventory.GetSlotId(slot));

                    slot.MarkDirty();
                    invslot.MarkDirty();

                    return false;
                }

                return true;
            });
        }


        public virtual SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            for (int i = 0; i < CollectibleBehaviors.Length; i++)
            {
                SkillItem[] result = CollectibleBehaviors[i].GetToolModes(slot, forPlayer, blockSel);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Should return the current items tool mode.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSelection"></param>
        /// <returns>The tool mode to display or -1 to not display the current mode</returns>
        public virtual int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection)
        {
            for (int i = 0; i < CollectibleBehaviors.Length; i++)
            {
                int result = CollectibleBehaviors[i].GetToolMode(slot, byPlayer, blockSelection);
                if (result != 0) return result;
            }
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
            for (int i = 0; i < CollectibleBehaviors.Length; i++)
            {
                CollectibleBehaviors[i].SetToolMode(slot, byPlayer, blockSelection, toolMode);
            }
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

        public virtual void OnHeldActionAnimStart(ItemSlot slot, EntityAgent byEntity, EnumHandInteract type)
        {

        }

        /// <summary>
        /// Called every game tick when this collectible is in dropped form in the world (i.e. as EntityItem)
        /// </summary>
        /// <param name="entityItem"></param>
        public virtual void OnGroundIdle(EntityItem entityItem)
        {
            if (entityItem.Swimming && api.Side == EnumAppSide.Server && Attributes?.IsTrue("dissolveInWater") == true)
            {
                if (api.World.Rand.NextDouble() < 0.01)
                {
                    api.World.SpawnCubeParticles(entityItem.ServerPos.XYZ, entityItem.Itemstack.Clone(), 0.1f, 80, 0.3f);
                    entityItem.Die();
                } else
                {
                    if (api.World.Rand.NextDouble() < 0.2)
                    {
                        api.World.SpawnCubeParticles(entityItem.ServerPos.XYZ, entityItem.Itemstack.Clone(), 0.1f, 2, 0.2f + (float)api.World.Rand.NextDouble() / 5f);
                    }
                }
            }
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
        /// Called when this item was collected by an entity
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="entity"></param>
        public virtual void OnCollected(ItemStack stack, Entity entity)
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
        /// <param name="firstEvent">True on first mouse down</param>
        /// <param name="handling">Whether or not to do any subsequent actions. If not set or set to NotHandled, the action will not called on the server.</param>
        /// <returns></returns>
        public virtual void OnHeldUseStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumHandInteract useType, bool firstEvent, ref EnumHandHandling handling)
        {
            if (useType == EnumHandInteract.HeldItemAttack)
            {
                OnHeldAttackStart(slot, byEntity, blockSel, entitySel, ref handling);
                return;
            }

            if (useType == EnumHandInteract.HeldItemInteract)
            {
                OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
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
            EnumHandHandling bhHandHandling = EnumHandHandling.NotHandled;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => bh.OnHeldAttackStart(slot, byEntity, blockSel, entitySel, ref bhHandHandling, ref hd),
                () =>
                {
                    if (HeldSounds?.Attack != null && api.World.Side == EnumAppSide.Client)
                    {
                        api.World.PlaySoundAt(HeldSounds.Attack, 0, 0, 0, null, 0.9f + (float)api.World.Rand.NextDouble() * 0.2f);
                    }
                }
            );
            handling = bhHandHandling;
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
            bool retval = false;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    var bhretval = bh.OnHeldAttackCancel(secondsPassed, slot, byEntity, blockSelection, entitySel, cancelReason, ref hd);
                    if (hd != EnumHandling.PassThrough) retval = bhretval;
                },
                () => { }
            );

            return retval;
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
            bool retval = false;
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => {
                    var bhretval = bh.OnHeldAttackStep(secondsPassed, slot, byEntity, blockSelection, entitySel, ref hd);
                    if (hd != EnumHandling.PassThrough) retval = bhretval;
                },
                () => { }
            );

            return retval;
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
            WalkBehaviors(
                (CollectibleBehavior bh, ref EnumHandling hd) => bh.OnHeldAttackStop(secondsPassed, slot, byEntity, blockSelection, entitySel, ref hd),
                () => { }
            );
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
        public virtual void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            EnumHandHandling bhHandHandling = EnumHandHandling.NotHandled;
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling bhHandling = EnumHandling.PassThrough;

                behavior.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref bhHandHandling, ref bhHandling);
                if (bhHandling != EnumHandling.PassThrough)
                {
                    handling = bhHandHandling;
                    preventDefault = true;
                }

                if (bhHandling == EnumHandling.PreventSubsequent) return;
            }

            if (!preventDefault)
            {
                tryEatBegin(slot, byEntity, ref bhHandHandling);
                handling = bhHandHandling;
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
            bool result = true;
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
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

            return tryEatStep(secondsUsed, slot, byEntity);
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
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel, ref handled);
                if (handled != EnumHandling.PassThrough) preventDefault = true;

                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            tryEatStop(secondsUsed, slot, byEntity);
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
            bool result = true;
            bool preventDefault = false;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                bool behaviorResult = behavior.OnHeldInteractCancel(secondsUsed, slot, byEntity, blockSel, entitySel, cancelReason, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return true;
        }


        /// <summary>
        /// Tries to eat the contents in the slot, first call
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="handling"></param>
        /// <param name="eatSound"></param>
        /// <param name="eatSoundRepeats"></param>
        protected virtual void tryEatBegin(ItemSlot slot, EntityAgent byEntity, ref EnumHandHandling handling, string eatSound = "eat", int eatSoundRepeats = 1)
        {
            if (!slot.Empty && GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity) != null)
            {
                byEntity.World.RegisterCallback((dt) => playEatSound(byEntity, eatSound, eatSoundRepeats), 500);

                byEntity.AnimManager?.StartAnimation("eat");

                handling = EnumHandHandling.PreventDefault;
            }
        }


        protected void playEatSound(EntityAgent byEntity, string eatSound = "eat", int eatSoundRepeats = 1)
        {
            if (byEntity.Controls.HandUse != EnumHandInteract.HeldItemInteract) return;

            IPlayer player = null;
            if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            byEntity.PlayEntitySound(eatSound, player);

            eatSoundRepeats--;
            if (eatSoundRepeats > 0)
            {
                byEntity.World.RegisterCallback((dt) => playEatSound(byEntity, eatSound, eatSoundRepeats), 300);
            }
        }

        /// <summary>
        /// Tries to eat the contents in the slot, eat step call
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="spawnParticleStack"></param>
        protected virtual bool tryEatStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, ItemStack spawnParticleStack = null)
        {
            if (GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity) == null) return false;

            Vec3d pos = byEntity.Pos.AheadCopy(0.4f).XYZ;
            pos.X += byEntity.LocalEyePos.X;
            pos.Y += byEntity.LocalEyePos.Y - 0.4f;
            pos.Z += byEntity.LocalEyePos.Z;

            if (secondsUsed > 0.5f && (int)(30 * secondsUsed) % 7 == 1)
            {
                byEntity.World.SpawnCubeParticles(pos, spawnParticleStack ?? slot.Itemstack, 0.3f, 4, 0.5f, (byEntity as EntityPlayer)?.Player);
            }

            if (byEntity.World is IClientWorldAccessor)
            {
                return secondsUsed <= 1f;
            }

            // Let the client decide when he is done eating
            return true;
        }

        /// <summary>
        /// Finished eating the contents in the slot, final call
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        protected virtual void tryEatStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity)
        {
            FoodNutritionProperties nutriProps = GetNutritionProperties(byEntity.World, slot.Itemstack, byEntity);

            if (byEntity.World is IServerWorldAccessor && nutriProps != null && secondsUsed >= 0.95f)
            {
                TransitionState state = UpdateAndGetTransitionState(api.World, slot, EnumTransitionType.Perish);
                float spoilState = state != null ? state.TransitionLevel : 0;

                float satLossMul = GlobalConstants.FoodSpoilageSatLossMul(spoilState, slot.Itemstack, byEntity);
                float healthLossMul = GlobalConstants.FoodSpoilageHealthLossMul(spoilState, slot.Itemstack, byEntity);

                byEntity.ReceiveSaturation(nutriProps.Satiety * satLossMul, nutriProps.FoodCategory);

                IPlayer player = null;
                if (byEntity is EntityPlayer) player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                slot.TakeOut(1);

                if (nutriProps.EatenStack != null)
                {
                    if (slot.Empty)
                    {
                        slot.Itemstack = nutriProps.EatenStack.ResolvedItemstack.Clone();
                    }
                    else
                    {
                        if (player == null || !player.InventoryManager.TryGiveItemstack(nutriProps.EatenStack.ResolvedItemstack.Clone(), true))
                        {
                            byEntity.World.SpawnItemEntity(nutriProps.EatenStack.ResolvedItemstack.Clone(), byEntity.SidedPos.XYZ);
                        }
                    }
                }

                float healthChange = nutriProps.Health * healthLossMul;

                float intox = byEntity.WatchedAttributes.GetFloat("intoxication");
                byEntity.WatchedAttributes.SetFloat("intoxication", Math.Min(1.1f, intox + nutriProps.Intoxication));

                if (healthChange != 0)
                {
                    float durationSec = slot.Itemstack?.Collectible?.Attributes?["eatHealthEffectDurationSec"].AsFloat(0) ?? 0;
                    int ticks = slot.Itemstack?.Collectible?.Attributes?["eatHealthEffectTicks"].AsInt(1) ?? 1;

                    byEntity.ReceiveDamage(new DamageSource()
                    {
                        Source = EnumDamageSource.Internal,
                        Type = healthChange > 0 ? EnumDamageType.Heal : EnumDamageType.Poison,
                        Duration = TimeSpan.FromSeconds(durationSec),
                        TicksPerDuration = ticks,
                        DamageOverTimeTypeEnum = healthChange > 0 ? EnumDamageOverTimeEffectType.Unknown : EnumDamageOverTimeEffectType.Poison
                    }, Math.Abs(healthChange));
                }

                slot.MarkDirty();
                player.InventoryManager.BroadcastHotbarSlot();
            }
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
            if (Code == null) return "Invalid block, id " + this.Id;

            string type = ItemClass.Name();
            StringBuilder sb = new StringBuilder();
            sb.Append(Lang.GetMatching(Code?.Domain + AssetLocation.LocationSeparator + type + "-" + Code?.Path));

            foreach (var bh in CollectibleBehaviors)
            {
                bh.GetHeldItemName(sb, itemStack);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Called by the inventory system when you hover over an item stack. This is the text that is getting displayed.
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="dsc"></param>
        /// <param name="world"></param>
        /// <param name="withDebugInfo"></param>
        public virtual void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            ItemStack stack = inSlot.Itemstack;

            string descText = GetItemDescText();

            if (withDebugInfo)
            {
                dsc.AppendLine("<font color=\"#bbbbbb\">Id:" + Id + "</font>");
                dsc.AppendLine("<font color=\"#bbbbbb\">Code: " + Code + "</font>");
                if (api?.Side == EnumAppSide.Client && (api as ICoreClientAPI).Input.KeyboardKeyStateRaw[(int)GlKeys.ShiftLeft])
                {
                    dsc.AppendLine("<font color=\"#bbbbbb\">Attributes: " + inSlot.Itemstack.Attributes.ToJsonToken() + "</font>\n");
                }
            }

            int durability = GetMaxDurability(stack);

            if (durability > 1)
            {
                dsc.AppendLine(Lang.Get("Durability: {0} / {1}", stack.Collectible.GetRemainingDurability(stack), durability));
            }


            if (MiningSpeed != null && MiningSpeed.Count > 0)
            {
                dsc.AppendLine(Lang.Get("Tool Tier: {0}", ToolTier));

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

            var bag = GetCollectibleInterface<IHeldBag>();
            if (bag != null)
            {
                dsc.AppendLine(Lang.Get("Storage Slots: {0}", bag.GetQuantitySlots(stack)));

                bool didPrint = false;
                var stacks = bag.GetContents(stack, world);
                if (stacks != null)
                {
                    foreach (var cstack in stacks)
                    {
                        if (cstack == null || cstack.StackSize == 0) continue;

                        if (!didPrint)
                        {
                            dsc.AppendLine(Lang.Get("Contents: "));
                            didPrint = true;
                        }
                        cstack.ResolveBlockOrItem(world);
                        dsc.AppendLine("- " + cstack.StackSize + "x " + cstack.GetName());
                    }

                    if (!didPrint)
                    {
                        dsc.AppendLine(Lang.Get("Empty"));
                    }
                }
            }

            EntityPlayer entity = world.Side == EnumAppSide.Client ? (world as IClientWorldAccessor).Player.Entity : null;

            float spoilState = AppendPerishableInfoText(inSlot, dsc, world);

            FoodNutritionProperties nutriProps = GetNutritionProperties(world, stack, entity);
            if (nutriProps != null)
            {
                float satLossMul = GlobalConstants.FoodSpoilageSatLossMul(spoilState, stack, entity);
                float healthLossMul = GlobalConstants.FoodSpoilageHealthLossMul(spoilState, stack, entity);

                if (Math.Abs(nutriProps.Health * healthLossMul) > 0.001f)
                {
                    dsc.AppendLine(Lang.Get(MatterState == EnumMatterState.Liquid ? "liquid-when-drunk-saturation-hp" : "When eaten: {0} sat, {1} hp", Math.Round(nutriProps.Satiety * satLossMul), Math.Round(nutriProps.Health * healthLossMul, 2)));
                }
                else
                {
                    dsc.AppendLine(Lang.Get(MatterState == EnumMatterState.Liquid ? "liquid-when-drunk-saturation" : "When eaten: {0} sat", Math.Round(nutriProps.Satiety * satLossMul)));
                }

                dsc.AppendLine(Lang.Get("Food Category: {0}", Lang.Get("foodcategory-" + nutriProps.FoodCategory.ToString().ToLowerInvariant())));
            }



            if (GrindingProps?.GroundStack?.ResolvedItemstack != null)
            {
                dsc.AppendLine(Lang.Get("When ground: Turns into {0}x {1}", GrindingProps.GroundStack.ResolvedItemstack.StackSize, GrindingProps.GroundStack.ResolvedItemstack.GetName()));
            }

            if (CrushingProps != null)
            {
                float quantity = CrushingProps.Quantity.avg * CrushingProps.CrushedStack.ResolvedItemstack.StackSize;
                dsc.AppendLine(Lang.Get("When pulverized: Turns into {0:0.#}x {1}", quantity, CrushingProps.CrushedStack.ResolvedItemstack.GetName()));
                dsc.AppendLine(Lang.Get("Requires Pulverizer tier: {0}", CrushingProps.HardnessTier));
            }

            if (GetAttackPower(stack) > 0.5f)
            {
                dsc.AppendLine(Lang.Get("Attack power: -{0} hp", GetAttackPower(stack).ToString("0.#")));
                dsc.AppendLine(Lang.Get("Attack tier: {0}", ToolTier));
            }

            if (GetAttackRange(stack) > GlobalConstants.DefaultAttackRange)
            {
                dsc.AppendLine(Lang.Get("Attack range: {0} m", GetAttackRange(stack).ToString("0.#")));
            }

            if (CombustibleProps != null)
            {
                string smelttype = CombustibleProps.SmeltingType.ToString().ToLowerInvariant();
                if (smelttype == "fire")
                {
                    // Custom for clay items - do not show firing temperature as that is irrelevant to Pit kilns

                    dsc.AppendLine(Lang.Get("itemdesc-fireinkiln"));
                }
                else
                {
                    if (CombustibleProps.BurnTemperature > 0)
                    {
                        dsc.AppendLine(Lang.Get("Burn temperature: {0}°C", CombustibleProps.BurnTemperature));
                        dsc.AppendLine(Lang.Get("Burn duration: {0}s", CombustibleProps.BurnDuration));
                    }

                    if (CombustibleProps.MeltingPoint > 0)
                    {
                        dsc.AppendLine(Lang.Get("game:smeltpoint-" + smelttype, CombustibleProps.MeltingPoint));
                    }
                }

                if (CombustibleProps.SmeltedStack?.ResolvedItemstack != null)
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

            foreach (var bh in CollectibleBehaviors)
            {
                bh.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            }

            if (descText.Length > 0 && dsc.Length > 0) dsc.Append("\n");
            dsc.Append(descText);

            float temp = GetTemperature(world, stack);
            if (temp > 20)
            {
                dsc.AppendLine(Lang.Get("Temperature: {0}°C", (int)temp));
            }

            if (Code != null && Code.Domain != "game")
            {
                var mod = api.ModLoader.GetMod(Code.Domain);
                dsc.AppendLine(Lang.Get("Mod: {0}", mod?.Info.Name ?? Code.Domain));
            }
        }

        public virtual string GetItemDescText()
        {
            string descLangCode = Code?.Domain + AssetLocation.LocationSeparator + ItemClass.ToString().ToLowerInvariant() + "desc-" + Code?.Path;
            string descText = Lang.GetMatching(descLangCode);
            if (descText == descLangCode) descText = "";
            else descText = descText + "\n";
            return descText;
        }


        /// <summary>
        /// Interaction help thats displayed above the hotbar, when the player puts this item/block in his active hand slot
        /// </summary>
        /// <param name="inSlot"></param>
        /// <returns></returns>
        public virtual WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            WorldInteraction[] interactions;

            if (GetNutritionProperties(api.World, inSlot.Itemstack, null) != null)
            {
                interactions = new WorldInteraction[]
                {
                    new WorldInteraction()
                    {
                        ActionLangCode = "heldhelp-eat",
                        MouseButton = EnumMouseButton.Right
                    }
                };
            } else
            {
                interactions = Array.Empty<WorldInteraction>();
            }

            EnumHandling handled = EnumHandling.PassThrough;

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                WorldInteraction[] bhi = behavior.GetHeldInteractionHelp(inSlot, ref handled);

                interactions = interactions.Append(bhi);

                if (handled == EnumHandling.PreventSubsequent) break;
            }

            return interactions;
        }



        public virtual float AppendPerishableInfoText(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
        {
            float spoilState = 0;
            TransitionState[] transitionStates = UpdateAndGetTransitionStates(api.World, inSlot);

            bool nowSpoiling = false;

            if (transitionStates == null) return 0;

            for (int i = 0; i < transitionStates.Length; i++)
            {
                spoilState = Math.Max(spoilState, AppendPerishableInfoText(inSlot, dsc, world, transitionStates[i], nowSpoiling));
                nowSpoiling |= spoilState > 0;
            }

            return spoilState;
        }

        protected virtual float AppendPerishableInfoText(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, TransitionState state, bool nowSpoiling)
        {
            TransitionableProperties prop = state.Props;
            float transitionRate = GetTransitionRateMul(world, inSlot, prop.Type);
            if (inSlot.Inventory is CreativeInventoryTab) transitionRate = 1f;
            float transitionLevel = state.TransitionLevel;
            float freshHoursLeft = state.FreshHoursLeft / transitionRate;

            switch (prop.Type)
            {
                case EnumTransitionType.Perish:
                    if (transitionLevel > 0)
                    {
                        dsc.AppendLine(Lang.Get("itemstack-perishable-spoiling", (int)Math.Round(transitionLevel * 100)));
                        return transitionLevel;
                    }
                    else
                    {
                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-perishable"));
                        }
                        else
                        {

                            float hoursPerday = api.World.Calendar.HoursPerDay;
                            float years = freshHoursLeft / hoursPerday / api.World.Calendar.DaysPerYear;

                            if (years >= 1.0f)
                            {
                                if (years <= 1.05f)
                                {
                                    dsc.AppendLine(Lang.Get("itemstack-perishable-fresh-one-year"));
                                }
                                else
                                {
                                    dsc.AppendLine(Lang.Get("itemstack-perishable-fresh-years", Math.Round(years, 1)));
                                }
                            }
                            else if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-perishable-fresh-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-perishable-fresh-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;

                case EnumTransitionType.Cure:
                    if (nowSpoiling) break;

                    if (transitionLevel > 0 || (freshHoursLeft <= 0 && transitionRate > 0))
                    {
                        dsc.AppendLine(Lang.Get("itemstack-curable-curing", (int)Math.Round(transitionLevel * 100)));
                    }
                    else
                    {
                        double hoursPerday = api.World.Calendar.HoursPerDay;

                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-curable"));
                        }
                        else
                        {

                            if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-curable-duration-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-curable-duration-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;
                /*

            case EnumTransitionType.Ferment:
                if (transitionLevel > 0)
                {
                    dsc.AppendLine(Lang.Get("<font color=\"olive\">Fermentable.</font> {0}% fermented", (int)Math.Round(transitionLevel * 100)));
                }
                else
                {
                    double hoursPerday = api.World.Calendar.HoursPerDay;
                    if (freshHoursLeft > hoursPerday)
                    {
                        dsc.AppendLine(Lang.Get("<font color=\"olive\">Fermentable.</font> Duration: {0} days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                    }
                    else
                    {
                        dsc.AppendLine(Lang.Get("<font color=\"olive\">Fermentable.</font> Duration: {0} hours", Math.Round(freshHoursLeft, 1)));
                    }
                }
                break;
                */

                case EnumTransitionType.Ripen:
                    if (nowSpoiling) break;

                    if (transitionLevel > 0 || (freshHoursLeft <= 0 && transitionRate > 0))
                    {
                        dsc.AppendLine(Lang.Get("itemstack-ripenable-ripening", (int)Math.Round(transitionLevel * 100)));
                    }
                    else
                    {
                        double hoursPerday = api.World.Calendar.HoursPerDay;

                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-ripenable"));
                        }
                        else
                        {
                            if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-ripenable-duration-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-ripenable-duration-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;

                case EnumTransitionType.Dry:
                    if (nowSpoiling) break;

                    if (transitionLevel > 0)
                    {
                        dsc.AppendLine(Lang.Get("itemstack-dryable-dried", (int)Math.Round(transitionLevel * 100)));
                        dsc.AppendLine(Lang.Get("Drying rate in this container: {0:0.##}x", transitionRate));
                    }
                    else
                    {
                        double hoursPerday = api.World.Calendar.HoursPerDay;

                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-dryable"));
                        }
                        else
                        {
                            if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-dryable-duration-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-dryable-duration-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;

                case EnumTransitionType.Melt:
                    if (nowSpoiling) break;

                    if (transitionLevel > 0 || freshHoursLeft <= 0)
                    {
                        dsc.AppendLine(Lang.Get("itemstack-meltable-melted", (int)Math.Round(transitionLevel * 100)));
                        dsc.AppendLine(Lang.Get("Melting rate in this container: {0:0.##}x", transitionRate));
                    }
                    else
                    {
                        double hoursPerday = api.World.Calendar.HoursPerDay;

                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-meltable"));
                        }
                        else
                        {
                            if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-meltable-duration-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-meltable-duration-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;

                case EnumTransitionType.Harden:
                    if (nowSpoiling) break;

                    if (transitionLevel > 0 || freshHoursLeft <= 0)
                    {
                        dsc.AppendLine(Lang.Get("itemstack-hardenable-hardened", (int)Math.Round(transitionLevel * 100)));
                    }
                    else
                    {
                        double hoursPerday = api.World.Calendar.HoursPerDay;

                        if (transitionRate <= 0)
                        {
                            dsc.AppendLine(Lang.Get("itemstack-hardenable"));
                        }
                        else
                        {
                            if (freshHoursLeft > hoursPerday)
                            {
                                dsc.AppendLine(Lang.Get("itemstack-hardenable-duration-days", Math.Round(freshHoursLeft / hoursPerday, 1)));
                            }
                            else
                            {
                                dsc.AppendLine(Lang.Get("itemstack-hardenable-duration-hours", Math.Round(freshHoursLeft, 1)));
                            }
                        }
                    }
                    break;
            }

            return 0;
        }

        public virtual void OnHandbookRecipeRender(ICoreClientAPI capi, GridRecipe recipe, ItemSlot slot, double x, double y, double z, double size)
        {
            capi.Render.RenderItemstackToGui(
                slot,
                x,
                y,
                z, (float)size * 0.58f, ColorUtil.WhiteArgb,
                true, false, true
            );

            for (int i = 0; i < CollectibleBehaviors.Length; i++)
            {
                CollectibleBehaviors[i].OnHandbookRecipeRender(capi, recipe, slot, x, y, z, size);
            }
        }



        public virtual List<ItemStack> GetHandBookStacks(ICoreClientAPI capi)
        {
            if (Code == null) return null;
            var handbookAttributes = Attributes?["handbook"];
            if (handbookAttributes?["exclude"].AsBool() == true) return null;

            bool inCreativeTab = CreativeInventoryTabs != null && CreativeInventoryTabs.Length > 0;
            bool inCreativeTabStack = CreativeInventoryStacks != null && CreativeInventoryStacks.Length > 0;
            if (!inCreativeTab && !inCreativeTabStack)
            {
                if (true != handbookAttributes?["include"].AsBool()) return null;
            }

            List<ItemStack> stacks = new List<ItemStack>();

            if (inCreativeTabStack && handbookAttributes?["ignoreCreativeInvStacks"].AsBool() != true)
            {
                for (int i = 0; i < CreativeInventoryStacks.Length; i++)
                {
                    JsonItemStack[] creativeStacks = CreativeInventoryStacks[i].Stacks;
                    for (int j = 0; j < creativeStacks.Length; j++)
                    {
                        ItemStack stack = creativeStacks[j].ResolvedItemstack;
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
                stacks.Add(stack);
            }

            return stacks;
        }



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
        /// <param name="priority"></param>
        /// <returns></returns>
        public virtual int GetMergableQuantity(ItemStack sinkStack, ItemStack sourceStack, EnumMergePriority priority)
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
            op.MovableQuantity = GetMergableQuantity(op.SinkSlot.Itemstack, op.SourceSlot.Itemstack, op.CurrentPriority);
            if (op.MovableQuantity == 0) return;
            if (!op.SinkSlot.CanTakeFrom(op.SourceSlot, op.CurrentPriority)) return;

            bool doTemperatureAveraging = false;
            bool doTransitionAveraging = false;

            op.MovedQuantity = GameMath.Min(op.SinkSlot.GetRemainingSlotSpace(op.SourceSlot.Itemstack), op.MovableQuantity, op.RequestedQuantity);

            if (HasTemperature(op.SinkSlot.Itemstack) || HasTemperature(op.SourceSlot.Itemstack))
            {
                if (op.CurrentPriority < EnumMergePriority.DirectMerge)
                {
                    float tempDiff = Math.Abs(GetTemperature(op.World, op.SinkSlot.Itemstack) - GetTemperature(op.World, op.SourceSlot.Itemstack));
                    if (tempDiff > 30)
                    {
                        op.MovedQuantity = 0;
                        op.MovableQuantity = 0;
                        op.RequiredPriority = EnumMergePriority.DirectMerge;
                        return;
                    }
                }

                doTemperatureAveraging = true;
            }


            TransitionState[] sourceTransitionStates = UpdateAndGetTransitionStates(op.World, op.SourceSlot);
            TransitionState[] targetTransitionStates = UpdateAndGetTransitionStates(op.World, op.SinkSlot);

            Dictionary<EnumTransitionType, TransitionState> targetStatesByType=null;

            if (sourceTransitionStates != null)
            {
                bool canDirectStack = true;
                bool canAutoStack = true;


                if (targetTransitionStates == null)
                {
                    op.MovedQuantity = 0;
                    op.MovableQuantity = 0;
                    return;
                }

                targetStatesByType = new Dictionary<EnumTransitionType, TransitionState>();
                foreach (var state in targetTransitionStates) targetStatesByType[state.Props.Type] = state;


                foreach (var sourceState in sourceTransitionStates)
                {
                    if (!targetStatesByType.TryGetValue(sourceState.Props.Type, out TransitionState targetState))
                    {
                        canAutoStack = false;
                        canDirectStack = false;
                        break;
                    }

                    if (Math.Abs(targetState.TransitionedHours - sourceState.TransitionedHours) > 4 && Math.Abs(targetState.TransitionedHours - sourceState.TransitionedHours) / sourceState.FreshHours > 0.03f)
                    {
                        canAutoStack = false;
                    }
                }

                if ((!canAutoStack && op.CurrentPriority < EnumMergePriority.DirectMerge))
                {
                    op.MovedQuantity = 0;
                    op.MovableQuantity = 0;
                    op.RequiredPriority = EnumMergePriority.DirectMerge;
                    return;
                }

                if (!canDirectStack)
                {
                    op.MovedQuantity = 0;
                    op.MovableQuantity = 0;
                    return;
                }

                doTransitionAveraging = true;
            }

            if (op.SourceSlot.Itemstack == null)   //The earlier UpdateAndGetTransitionStates() may have set the Itemstack to null, e.g. if it was a 50% probability Rot producing item
            {
                op.MovedQuantity = 0;
                return;
            }

            if (op.MovedQuantity <= 0) return;  //Some of the following code could error (e.g. divide by zero) if this is 0 and the sinkSlot.stackSize is also 0

            if (op.SinkSlot.Itemstack == null)   //The earlier UpdateAndGetTransitionStates() may have set the Itemstack to null, as above: in this case create a dummy Itemstack so that no NPE in the following code
            {
                op.SinkSlot.Itemstack = new ItemStack(op.SourceSlot.Itemstack.Collectible, 0);
            }


            if (doTemperatureAveraging)
            {
                SetTemperature(
                    op.World,
                    op.SinkSlot.Itemstack,
                    (op.SinkSlot.StackSize * GetTemperature(op.World, op.SinkSlot.Itemstack) + op.MovedQuantity * GetTemperature(op.World, op.SourceSlot.Itemstack)) / (op.SinkSlot.StackSize + op.MovedQuantity)
                );
            }

            if (doTransitionAveraging)
            {
                float t = (float)op.MovedQuantity / (op.MovedQuantity + op.SinkSlot.StackSize);

                foreach (var sourceState in sourceTransitionStates)
                {
                    TransitionState targetState = targetStatesByType[sourceState.Props.Type];
                    SetTransitionState(op.SinkSlot.Itemstack, sourceState.Props.Type, sourceState.TransitionedHours * t + targetState.TransitionedHours * (1-t));
                }
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
        /// Should return true if this collectible is smeltable in an open fire
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
                && (CombustibleProps.SmeltingType != EnumSmeltType.Fire || world.Config.GetString("allowOpenFireFiring").ToBool(false))
                && (outputStack == null || outputStack.Collectible.GetMergableQuantity(outputStack, smeltedStack, EnumMergePriority.AutoMerge) >= smeltedStack.StackSize)
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

            ItemStack smeltedStack = CombustibleProps.SmeltedStack.ResolvedItemstack.Clone();

            // Copy over spoilage values but reduce them by a bit
            TransitionState state = UpdateAndGetTransitionState(world, new DummySlot(inputSlot.Itemstack), EnumTransitionType.Perish);

            if (state != null)
            {
                TransitionState smeltedState = smeltedStack.Collectible.UpdateAndGetTransitionState(world, new DummySlot(smeltedStack), EnumTransitionType.Perish);

                float nowTransitionedHours = (state.TransitionedHours / (state.TransitionHours + state.FreshHours)) * 0.8f * (smeltedState.TransitionHours + smeltedState.FreshHours) - 1;

                smeltedStack.Collectible.SetTransitionState(smeltedStack, EnumTransitionType.Perish, Math.Max(0, nowTransitionedHours));
            }

            int batchSize = 1;

            if (outputSlot.Itemstack == null)
            {
                outputSlot.Itemstack = smeltedStack;
                outputSlot.Itemstack.StackSize = batchSize * smeltedStack.StackSize;
            }
            else
            {
                smeltedStack.StackSize = batchSize * smeltedStack.StackSize;

                // use TryMergeStacks to average spoilage rate and temperature
                ItemStackMergeOperation op = new ItemStackMergeOperation(world, EnumMouseButton.Left, 0, EnumMergePriority.ConfirmedMerge, batchSize * smeltedStack.StackSize);
                op.SourceSlot = new DummySlot(smeltedStack);
                op.SinkSlot = new DummySlot(outputSlot.Itemstack);
                outputSlot.Itemstack.Collectible.TryMergeStacks(op);
                outputSlot.Itemstack = op.SinkSlot.Itemstack;
            }

            inputSlot.Itemstack.StackSize -= batchSize * CombustibleProps.SmeltedRatio;

            if (inputSlot.Itemstack.StackSize <= 0)
            {
                inputSlot.Itemstack = null;
            }

            outputSlot.MarkDirty();
        }

        /// <summary>
        /// Returns true if the stack can spoil
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool CanSpoil(ItemStack itemstack)
        {
            if (itemstack == null || itemstack.Attributes == null) return false;
            return itemstack.Collectible.NutritionProps != null && itemstack.Attributes.HasAttribute("spoilstate");
        }


        /// <summary>
        /// Returns the transition state of given transition type
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inslot"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TransitionState UpdateAndGetTransitionState(IWorldAccessor world, ItemSlot inslot, EnumTransitionType type)
        {
            TransitionState[] states = UpdateAndGetTransitionStates(world, inslot);
            if (states == null) return null;

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Props.Type == type) return states[i];
            }

            return null;
        }


        public virtual void SetTransitionState(ItemStack stack, EnumTransitionType type, float transitionedHours)
        {
            ITreeAttribute attr = (ITreeAttribute)stack.Attributes["transitionstate"];

            if (attr == null)
            {
                UpdateAndGetTransitionState(api.World, new DummySlot(stack), type);
                attr = (ITreeAttribute)stack.Attributes["transitionstate"];
            }

            TransitionableProperties[] propsm = GetTransitionableProperties(api.World, stack, null);
            for (int i = 0; i < propsm.Length; i++)
            {
                if (propsm[i].Type == type)
                {
                    (attr["transitionedHours"] as FloatArrayAttribute).value[i] = transitionedHours;
                    return;
                }
            }
        }


        public virtual float GetTransitionRateMul(IWorldAccessor world, ItemSlot inSlot, EnumTransitionType transType)
        {
            float rate = inSlot.Inventory == null ? 1 : inSlot.Inventory.GetTransitionSpeedMul(transType, inSlot.Itemstack);

            if (transType == EnumTransitionType.Perish)
            {
                float temp = inSlot.Itemstack.Collectible.GetTemperature(world, inSlot.Itemstack);
                if (temp > 75)
                {
                    rate = 0;
                }

                rate *= GlobalConstants.PerishSpeedModifier;
            }

            return rate;
        }

        /// <summary>
        /// Returns a list of the current transition states of this item, redirects to UpdateAndGetTransitionStatesNative
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inslot"></param>
        /// <returns></returns>
        public virtual TransitionState[] UpdateAndGetTransitionStates(IWorldAccessor world, ItemSlot inslot)
        {
            return UpdateAndGetTransitionStatesNative(world, inslot);
        }

        /// <summary>
        /// Returns a list of the current transition states of this item. Seperate from UpdateAndGetTransitionStates() so that you can call still call this methods several inheritances down, i.e. there is no base.base.Method() syntax in C#
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inslot"></param>
        /// <returns></returns>
        protected virtual TransitionState[] UpdateAndGetTransitionStatesNative(IWorldAccessor world, ItemSlot inslot)
        {
            if (inslot is ItemSlotCreative) return null;

            ItemStack itemstack = inslot.Itemstack;

            TransitionableProperties[] propsm = GetTransitionableProperties(world, inslot.Itemstack, null);

            if (itemstack == null || propsm == null || propsm.Length == 0)
            {
                return null;
            }

            if (itemstack.Attributes == null)
            {
                itemstack.Attributes = new TreeAttribute();
            }

            if (itemstack.Attributes.GetBool("timeFrozen")) return null;


            if (!(itemstack.Attributes["transitionstate"] is ITreeAttribute))
            {
                itemstack.Attributes["transitionstate"] = new TreeAttribute();
            }

            ITreeAttribute attr = (ITreeAttribute)itemstack.Attributes["transitionstate"];


            float[] transitionedHours;
            float[] freshHours;
            float[] transitionHours;
            TransitionState[] states = new TransitionState[propsm.Length];

            if (!attr.HasAttribute("createdTotalHours"))
            {
                attr.SetDouble("createdTotalHours", world.Calendar.TotalHours);
                attr.SetDouble("lastUpdatedTotalHours", world.Calendar.TotalHours);

                freshHours = new float[propsm.Length];
                transitionHours = new float[propsm.Length];
                transitionedHours = new float[propsm.Length];

                for (int i = 0; i < propsm.Length; i++)
                {
                    transitionedHours[i] = 0;
                    freshHours[i] = propsm[i].FreshHours.nextFloat(1, world.Rand);
                    transitionHours[i] = propsm[i].TransitionHours.nextFloat(1, world.Rand);
                }

                attr["freshHours"] = new FloatArrayAttribute(freshHours);
                attr["transitionHours"] = new FloatArrayAttribute(transitionHours);
                attr["transitionedHours"] = new FloatArrayAttribute(transitionedHours);
            } else
            {
                freshHours = (attr["freshHours"] as FloatArrayAttribute).value;
                transitionHours = (attr["transitionHours"] as FloatArrayAttribute).value;
                transitionedHours = (attr["transitionedHours"] as FloatArrayAttribute).value;

                // A modder/dev might have added a new transition property since last time
                int gw = propsm.Length - freshHours.Length;
                if (gw > 0)
                {
                    int i = freshHours.Length;
                    while (i < propsm.Length)
                    {
                        freshHours = freshHours.Append(propsm[i].FreshHours.nextFloat(1, world.Rand));
                        transitionHours = transitionHours.Append(propsm[i].TransitionHours.nextFloat(1, world.Rand));
                        transitionedHours = transitionedHours.Append(0);
                        i++;
                    }
                    (attr["freshHours"] as FloatArrayAttribute).value = freshHours;
                    (attr["transitionHours"] as FloatArrayAttribute).value = transitionHours;
                    (attr["transitionedHours"] as FloatArrayAttribute).value = transitionedHours;
                }
            }

            double lastUpdatedTotalHours = attr.GetDouble("lastUpdatedTotalHours");
            double nowTotalHours = world.Calendar.TotalHours;


            bool nowSpoiling = false;

            float hoursPassed = (float)(nowTotalHours - lastUpdatedTotalHours);

            for (int i = 0; i < propsm.Length; i++)
            {
                TransitionableProperties prop = propsm[i];
                if (prop == null) continue;

                float transitionRateMul = GetTransitionRateMul(world, inslot, prop.Type);

                if (hoursPassed > 0.05f) // Maybe prevents us from running into accumulating rounding errors?
                {
                    float hoursPassedAdjusted = hoursPassed * transitionRateMul;
                    transitionedHours[i] += hoursPassedAdjusted;

                    /*if (api.World.Side == EnumAppSide.Server && inslot.Inventory.ClassName == "chest")
                    {
                        Console.WriteLine(hoursPassed + " hours passed. " + inslot.Itemstack.Collectible.Code + " spoil by " + transitionRateMul + "x. Is inside " + inslot.Inventory.ClassName + " {0}/{1}", transitionedHours[i], freshHours[i]);
                    }*/
                }



                float freshHoursLeft = Math.Max(0, freshHours[i] - transitionedHours[i]);
                float transitionLevel = Math.Max(0, transitionedHours[i] - freshHours[i]) / transitionHours[i];

                // Don't continue transitioning spoiled foods
                if (transitionLevel > 0)
                {
                    if (prop.Type == EnumTransitionType.Perish)
                    {
                        nowSpoiling = true;
                    } else
                    {
                        if (nowSpoiling) continue;
                    }
                }

                if (transitionLevel >= 1 && world.Side == EnumAppSide.Server)
                {
                    ItemStack newstack = OnTransitionNow(inslot, itemstack.Collectible.TransitionableProps[i]);

                    if (newstack.StackSize <= 0)
                    {
                        inslot.Itemstack = null;
                    } else {
                        itemstack.SetFrom(newstack);
                    }

                    inslot.MarkDirty();

                    // Only do one transformation, then do the next one next update
                    // This does fully not respect time-fast-forward, so that should be fixed some day
                    break;
                }

                states[i] = new TransitionState()
                {
                    FreshHoursLeft = freshHoursLeft,
                    TransitionLevel = Math.Min(1, transitionLevel),
                    TransitionedHours = transitionedHours[i],
                    TransitionHours = transitionHours[i],
                    FreshHours = freshHours[i],
                    Props = prop
                };

                //if (transitionRateMul > 0) break; // Only do one transformation at the time (i.e. food can not cure and perish at the same time) - Tyron 9/oct 2020, but why not at the same time? We need it for cheese ripening
            }

            if (hoursPassed > 0.05f)
            {
                attr.SetDouble("lastUpdatedTotalHours", nowTotalHours);
            }

            return states.Where(s => s != null).OrderBy(s => (int)s.Props.Type).ToArray();
        }


        /// <summary>
        /// Called when any of its TransitionableProperties causes the stack to transition to another stack. Default behavior is to return props.TransitionedStack.ResolvedItemstack and set the stack size according to the transition rtio
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="props"></param>
        /// <returns>The stack it should transition into</returns>
        public virtual ItemStack OnTransitionNow(ItemSlot slot, TransitionableProperties props)
        {
            bool preventDefault = false;
            ItemStack newStack = props.TransitionedStack.ResolvedItemstack.Clone();

            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                ItemStack bhStack = behavior.OnTransitionNow(slot, props, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                    newStack = bhStack;
                }

                if (handled == EnumHandling.PreventSubsequent) return newStack;
            }

            if (preventDefault) return newStack;

            newStack.StackSize = GameMath.RoundRandom(api.World.Rand, slot.Itemstack.StackSize * props.TransitionRatio);
            return newStack;
        }

        public static void CarryOverFreshness(ICoreAPI api, ItemSlot inputSlot, ItemStack outputStack, TransitionableProperties perishProps)
        {
            CarryOverFreshness(api, new ItemSlot[] { inputSlot }, new ItemStack[] { outputStack }, perishProps);
        }

        public static void CarryOverFreshness(ICoreAPI api, ItemSlot[] inputSlots, ItemStack[] outStacks, TransitionableProperties perishProps)
        {
            float transitionedHoursRelative = 0;

            float spoilageRelMax = 0;
            float spoilageRel = 0;
            int quantity = 0;

            for (int i = 0; i < inputSlots.Length; i++)
            {
                ItemSlot slot = inputSlots[i];
                if (slot.Empty) continue;
                TransitionState state = slot.Itemstack?.Collectible?.UpdateAndGetTransitionState(api.World, slot, EnumTransitionType.Perish);
                if (state == null) continue;

                quantity++;
                float val = state.TransitionedHours / (state.TransitionHours + state.FreshHours);

                float spoilageRelOne = Math.Max(0, (state.TransitionedHours - state.FreshHours) / state.TransitionHours);
                spoilageRelMax = Math.Max(spoilageRelOne, spoilageRelMax);

                transitionedHoursRelative += val;
                spoilageRel += spoilageRelOne;
            }

            transitionedHoursRelative /= Math.Max(1, quantity);
            spoilageRel /= Math.Max(1, quantity);

            for (int i = 0; i < outStacks.Length; i++)
            {
                if (outStacks[i] == null) continue;

                if (!(outStacks[i].Attributes["transitionstate"] is ITreeAttribute))
                {
                    outStacks[i].Attributes["transitionstate"] = new TreeAttribute();
                }

                float transitionHours = perishProps.TransitionHours.nextFloat(1, api.World.Rand);
                float freshHours = perishProps.FreshHours.nextFloat(1, api.World.Rand);

                ITreeAttribute attr = (ITreeAttribute)outStacks[i].Attributes["transitionstate"];
                attr.SetDouble("createdTotalHours", api.World.Calendar.TotalHours);
                attr.SetDouble("lastUpdatedTotalHours", api.World.Calendar.TotalHours);

                attr["freshHours"] = new FloatArrayAttribute(new float[] { freshHours });
                attr["transitionHours"] = new FloatArrayAttribute(new float[] { transitionHours });

                if (spoilageRel > 0)
                {
                    // If already spoiled: Take away 40% spoilage and 2 hours
                    spoilageRel *= 0.6f;
                    attr["transitionedHours"] = new FloatArrayAttribute(new float[] { freshHours + Math.Max(0, transitionHours * spoilageRel - 2)  });

                } else
                {
                    // If not yet spoiled: Weird formula :D
                    attr["transitionedHours"] = new FloatArrayAttribute(new float[] { Math.Max(0, transitionedHoursRelative * (0.8f + (2 + quantity) * spoilageRelMax) * (transitionHours + freshHours)) });
                }


            }
        }

        /// <summary>
        /// Test is failed for Perish-able items which have less than 50% of their fresh state remaining (or are already starting to spoil)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool IsReasonablyFresh(IWorldAccessor world, ItemStack itemstack)
        {
            if (GetMaxDurability(itemstack) > 1)
            {
                int leftDurability = GetRemainingDurability(itemstack);
                float p = (float)leftDurability / GetMaxDurability(itemstack);
                if (p < 0.95f) return false;
            }

            if (itemstack == null) return true;
            TransitionableProperties[] propsm = GetTransitionableProperties(world, itemstack, null);
            if (propsm == null) return true;
            ITreeAttribute attr = (ITreeAttribute)itemstack.Attributes["transitionstate"];
            if (attr == null) return true;

            float[] freshHours = (attr["freshHours"] as FloatArrayAttribute).value;
            float[] transitionedHours = (attr["transitionedHours"] as FloatArrayAttribute).value;
            for (int i = 0; i < propsm.Length; i++)
            {
                TransitionableProperties prop = propsm[i];
                if (prop?.Type == EnumTransitionType.Perish)
                {
                    if (transitionedHours[i] > freshHours[i] / 2f) return false;
                }
            }
            return true;
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
        /// <param name="didReceiveHeat">The amount of time it did receive heat since last update/call to this methode</param>
        /// <returns></returns>
        public virtual float GetTemperature(IWorldAccessor world, ItemStack itemstack, double didReceiveHeat)
        {
            if (itemstack?.Attributes?["temperature"] is not ITreeAttribute)
            {
                return 20;
            }

            var attr = (ITreeAttribute)itemstack.Attributes["temperature"];

            var nowHours = world.Calendar.TotalHours;
            var lastUpdateHours = attr.GetDouble("temperatureLastUpdate");

            var hourDiff = nowHours - (lastUpdateHours + didReceiveHeat);

            var temp = attr.GetFloat("temperature", 20);
            // 1.5 deg per irl second
            // 1 game hour = irl 60 seconds
            if (hourDiff > 1 / 85f && temp > 0f)
            {
                temp = Math.Max(0, temp - Math.Max(0, (float)(nowHours - lastUpdateHours) * attr.GetFloat("cooldownSpeed", 90)));
                attr.SetFloat("temperature", temp);
            }
            attr.SetDouble("temperatureLastUpdate", nowHours);

            return temp;
        }

        /// <summary>
        /// Returns the stacks item temperature in degree celsius
        /// </summary>
        /// <param name="world"></param>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual float GetTemperature(IWorldAccessor world, ItemStack itemstack)
        {
            if (itemstack?.Attributes?["temperature"] is not ITreeAttribute)
            {
                return 20;
            }

            ITreeAttribute attr = (ITreeAttribute)itemstack.Attributes["temperature"];

            double nowHours = world.Calendar.TotalHours;
            double lastUpdateHours = attr.GetDecimal("temperatureLastUpdate");

            double hourDiff = nowHours - lastUpdateHours;
            float temp = (float)attr.GetDecimal("temperature", 20);

            if (itemstack.Attributes.GetBool("timeFrozen")) return temp;

            // 1.5 deg per irl second
            // 1 game hour = irl 60 seconds
            if (hourDiff > 1/85f && temp > 0f)
            {
                temp = Math.Max(0, temp - Math.Max(0, (float)(nowHours - lastUpdateHours) * attr.GetFloat("cooldownSpeed", 90)));
                attr.SetFloat("temperature", temp);
                attr.SetDouble("temperatureLastUpdate", nowHours);
            }

            return temp;
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

            ITreeAttribute attr = (ITreeAttribute)itemstack.Attributes["temperature"];

            if (attr == null)
            {
                itemstack.Attributes["temperature"] = attr = new TreeAttribute();
            }

            double nowHours = world.Calendar.TotalHours;
            // If the colletible gets heated, retain the heat for 1 ingame hour
            if (delayCooldown && attr.GetDecimal("temperature") < temperature) nowHours += 0.5f;

            attr.SetDouble("temperatureLastUpdate", nowHours);
            attr.SetFloat("temperature", temperature);
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
            if (thisStack.Class == otherStack.Class && thisStack.Id == otherStack.Id)
            {
                return thisStack.Attributes.IsSubSetOf(api.World, otherStack.Attributes);
            }
            return false;
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

            // on export of schematic save the temperature to the TreeAttribute since in import we need the temperatureLastUpdate to be up to date
            if ((inSlot.Itemstack.Attributes["temperature"] as ITreeAttribute)?.HasAttribute("temperatureLastUpdate") == true)
            {
                GetTemperature(world, inSlot.Itemstack);
            }
        }
        /// <summary>
        /// This method is called after a block/item like this has been imported as part of a block schematic. Has to restore fix the block/item id mappings as they are probably different compared to the world from where they were exported. By default iterates over all the itemstacks attributes and searches for attribute sof type ItenStackAttribute and calls .FixMapping() on them.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="inSlot"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        [Obsolete("Use the variant with resolveImports parameter")]
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, ItemSlot inSlot, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {
            OnLoadCollectibleMappings(worldForResolve, inSlot, oldBlockIdMapping, oldItemIdMapping, true);
        }

        /// <summary>
        /// This method is called after a block/item like this has been imported as part of a block schematic. Has to restore fix the block/item id mappings as they are probably different compared to the world from where they were exported. By default iterates over all the itemstacks attributes and searches for attribute sof type ItenStackAttribute and calls .FixMapping() on them.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="inSlot"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="resolveImports">Turn it off to spawn structures as they are. For example, in this mode, instead of traders, their meta spawners will spawn</param>
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, ItemSlot inSlot, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, bool resolveImports)
        {
            OnLoadCollectibleMappings(worldForResolve, inSlot.Itemstack.Attributes, oldBlockIdMapping, oldItemIdMapping);
        }

        private void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, ITreeAttribute tree, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {
            foreach (var val in tree)
            {
                if (val.Value is ITreeAttribute treeAttribute)
                {
                    OnLoadCollectibleMappings(worldForResolve, treeAttribute, oldBlockIdMapping, oldItemIdMapping);
                    continue;
                }

                if (val.Value is ItemstackAttribute itemAttribute)
                {
                    ItemStack stack = itemAttribute.value;

                    // if the Collectible is null the item maybe from a missing mod so we need to remove it
                    if (stack?.FixMapping(oldBlockIdMapping, oldItemIdMapping, worldForResolve) == false)
                    {
                        itemAttribute.value = null;
                        continue;
                    }
                    stack?.Collectible.OnLoadCollectibleMappings(worldForResolve, stack.Attributes, oldBlockIdMapping, oldItemIdMapping);
                }
            }
            // update the time for the temperature to the current ingame time if imported from another game
            if (tree.HasAttribute("temperatureLastUpdate"))
            {
                tree.SetDouble("temperatureLastUpdate", worldForResolve.Calendar.TotalHours);
            }

            // update food transition time
            if (tree.HasAttribute("createdTotalHours"))
            {
                var created = tree.GetDouble("createdTotalHours");
                var lasUpdated = tree.GetDouble("lastUpdatedTotalHours");
                var diff = lasUpdated - created;
                tree.SetDouble("lastUpdatedTotalHours", worldForResolve.Calendar.TotalHours);
                tree.SetDouble("createdTotalHours", worldForResolve.Calendar.TotalHours - diff);
            }

        }

        void OnStoreCollectibleMappings(IWorldAccessor world, ITreeAttribute tree, Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            foreach (var val in tree)
            {
                if (val.Value is ITreeAttribute treeAttribute)
                {
                    OnStoreCollectibleMappings(world, treeAttribute, blockIdMapping, itemIdMapping);
                    continue;
                }

                if (val.Value is ItemstackAttribute attribute)
                {
                    ItemStack stack = attribute.value;
                    if (stack == null) continue;

                    if (stack.Collectible == null) stack.ResolveBlockOrItem(world);

                    if (stack.Class == EnumItemClass.Item)
                    {
                        itemIdMapping[stack.Id] = stack.Collectible?.Code;
                    } else
                    {
                        blockIdMapping[stack.Id] = stack.Collectible?.Code;
                    }
                }
            }
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
        /// Returns true if this blocks matterstate is liquid.  (Liquid blocks should also implement IBlockFlowing)
        /// <br/>
        /// IMPORTANT: Calling code should have looked up the block using IBlockAccessor.GetBlock(pos, BlockLayersAccess.Fluid)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLiquid()
        {
            return MatterState == EnumMatterState.Liquid;
        }


        void WalkBehaviors(CollectibleBehaviorDelegate onBehavior, Action defaultAction)
        {
            bool executeDefault = true;
            foreach (CollectibleBehavior behavior in CollectibleBehaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                onBehavior(behavior, ref handling);

                if (handling == EnumHandling.PreventSubsequent) return;
                if (handling == EnumHandling.PreventDefault) executeDefault = false;
            }

            if (executeDefault) defaultAction();
        }







        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="withInheritance"></param>
        /// <returns></returns>
        public CollectibleBehavior GetCollectibleBehavior(Type type, bool withInheritance)
        {
            return GetBehavior(CollectibleBehaviors, type, withInheritance);
        }

        public T GetCollectibleBehavior<T>(bool withInheritance) where T : CollectibleBehavior
        {
            return GetBehavior(CollectibleBehaviors, typeof(T), withInheritance) as T;
        }

        protected virtual CollectibleBehavior GetBehavior(CollectibleBehavior[] fromList, Type type, bool withInheritance)
        {
            if (withInheritance)
            {
                for (int i = 0; i < fromList.Length; i++)
                {
                    Type testType = fromList[i].GetType();
                    if (testType == type || type.IsAssignableFrom(testType))
                    {
                        return fromList[i];
                    }
                }
                return null;
            }

            // simpler loop if withInheritance is false
            for (int i = 0; i < fromList.Length; i++)
            {
                if (fromList[i].GetType() == type)
                {
                    return fromList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns instance of class that implements this interface in the following order<br/>
        /// 1. Collectible (returns itself)<br/>
        /// 2. CollectibleBlockBehavior (returns on of our own behavior)<br/>
        /// </summary>
        /// <returns></returns>
        public virtual T GetCollectibleInterface<T>() where T : class
        {
            if (this is T blockt) return blockt;
            var bh = GetCollectibleBehavior(typeof(T), true);
            if (bh != null) return bh as T;

            return null;
        }


        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="withInheritance"></param>
        /// <returns></returns>
        public virtual bool HasBehavior<T>(bool withInheritance = false) where T : CollectibleBehavior
        {
            return (T)GetCollectibleBehavior(typeof(T), withInheritance) != null;
        }


        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="withInheritance"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(Type type, bool withInheritance = false)
        {
            return GetCollectibleBehavior(type, withInheritance) != null;
        }



        /// <summary>
        /// Returns true if the block has given behavior
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classRegistry"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(string type, IClassRegistryAPI classRegistry)
        {
            return GetBehavior(classRegistry.GetBlockBehaviorClass(type)) != null;
        }


        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CollectibleBehavior GetBehavior(Type type)
        {
            return GetCollectibleBehavior(type, false);
        }

        /// <summary>
        /// Returns the blocks behavior of given type, if it has such behavior
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBehavior<T>() where T : CollectibleBehavior
        {
            return (T)GetCollectibleBehavior(typeof(T), false);
        }

        /// <summary>
        /// Called immediately prior to a firepit or similar testing whether this Collectible can be smelted
        /// <br/>Returns true if the caller should be marked dirty
        /// </summary>
        /// <param name="inventorySmelting"></param>
        public virtual bool OnSmeltAttempt(InventoryBase inventorySmelting)
        {
            return false;
        }




        [Obsolete]
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


        [Obsolete]
        public static bool IsBackPack(IItemStack itemstack)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return false;
            return itemstack.Collectible.Attributes["backpack"]["quantitySlots"].AsInt() > 0;
        }

        [Obsolete]
        public static int QuantityBackPackSlots(IItemStack itemstack)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return 0;
            return itemstack.Collectible.Attributes["backpack"]["quantitySlots"].AsInt();
        }

    }
}
