using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{

    public delegate ItemSlot NewSlotDelegate(int slotId, InventoryGeneric self);

    public delegate float GetSuitabilityDelegate(ItemSlot sourceSlot, ItemSlot targetSlow, bool isMerge);

    public delegate ItemSlot GetAutoPushIntoSlotDelegate(BlockFacing atBlockFace, ItemSlot fromSlot);

    public delegate ItemSlot GetAutoPullFromSlotDelegate(BlockFacing atBlockFace);

    /// <summary>
    /// A general purpose inventory
    /// </summary>
    public class InventoryGeneric : InventoryBase
    {
        ItemSlot[] slots;
        NewSlotDelegate onNewSlot = null;


        public Dictionary<EnumTransitionType, float> TransitionableSpeedMulByType { get; set; } = null;
        public Dictionary<EnumFoodCategory, float> PerishableFactorByFoodCategory { get; set; } = null;

        public GetSuitabilityDelegate OnGetSuitability;
        public GetAutoPushIntoSlotDelegate OnGetAutoPushIntoSlot;
        public GetAutoPullFromSlotDelegate OnGetAutoPullFromSlot;

        public float BaseWeight
        {
            get { return baseWeight; }
            set { baseWeight = value; }
        }

        /// <summary>
        /// Create a new general purpose inventory
        /// </summary>
        /// <param name="quantitySlots"></param>
        /// <param name="className"></param>
        /// <param name="instanceId"></param>
        /// <param name="api"></param>
        /// <param name="onNewSlot"></param>
        public InventoryGeneric(int quantitySlots, string className, string instanceId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base(className, instanceId, api)
        {
            OnGetSuitability = (s, t, isMerge) => isMerge ? (baseWeight + 3) : (baseWeight + 1);

            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        /// <summary>
        /// Create a new general purpose inventory
        /// </summary>
        /// <param name="quantitySlots"></param>
        /// <param name="invId"></param>
        /// <param name="api"></param>
        /// <param name="onNewSlot"></param>
        public InventoryGeneric(int quantitySlots, string invId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base (invId, api)
        {
            OnGetSuitability = (s, t, isMerge) => isMerge ? (baseWeight + 3) : (baseWeight + 1);

            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        /// <summary>
        /// Amount of available slots
        /// </summary>
        public override int Count
        {
            get { return slots.Length; }
        }

        /// <summary>
        /// Get slot for given slot index
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                return slots[slotId];
            }
            set
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                if (value == null) throw new ArgumentNullException(nameof(value));
                slots[slotId] = value;
            }
        }

        /// <summary>
        /// True if all slots are empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Itemstack != null) return false;
                }

                return true;
            }
        }


        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            return OnGetSuitability(sourceSlot, targetSlot, isMerge);
        }

        /// <summary>
        /// Loads the slot contents from given treeAttribute
        /// </summary>
        /// <param name="treeAttribute"></param>
        public override void FromTreeAttributes(ITreeAttribute treeAttribute)
        {
            slots = SlotsFromTreeAttributes(treeAttribute, slots);
        }

        /// <summary>
        /// Stores the slot contents to invtree
        /// </summary>
        /// <param name="invtree"></param>
        public override void ToTreeAttributes(ITreeAttribute invtree)
        {
            SlotsToTreeAttributes(slots, invtree);
        }

        /// <summary>
        /// Called when initializing the inventory or when loading the contents
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        protected override ItemSlot NewSlot(int slotId)
        {
            if (onNewSlot != null) return onNewSlot(slotId, this);
            return new ItemSlotSurvival(this);
        }


        public override float GetTransitionSpeedMul(EnumTransitionType transType, ItemStack stack)
        {
            float baseMul = GetDefaultTransitionSpeedMul(transType);

            if (transType == EnumTransitionType.Perish && PerishableFactorByFoodCategory != null && stack.Collectible.NutritionProps != null)
            {
                if (!PerishableFactorByFoodCategory.TryGetValue(stack.Collectible.NutritionProps.FoodCategory, out baseMul))
                {
                    baseMul = GetDefaultTransitionSpeedMul(transType);
                }
            }
            else
            {
                if (TransitionableSpeedMulByType != null)
                {
                    if (!TransitionableSpeedMulByType.TryGetValue(transType, out baseMul)) baseMul = GetDefaultTransitionSpeedMul(transType);
                }
            }

            float outMul = baseMul;
            if (OnAcquireTransitionSpeed != null)
            {
                outMul = OnAcquireTransitionSpeed(transType, stack, baseMul);
            }

            return outMul;
        }


        public override ItemSlot GetAutoPullFromSlot(BlockFacing atBlockFace)
        {
            if (OnGetAutoPullFromSlot != null)
            {
                return OnGetAutoPullFromSlot(atBlockFace);
            }

            return base.GetAutoPullFromSlot(atBlockFace);
        }

        public override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
        {
            if (OnGetAutoPushIntoSlot != null)
            {
                return OnGetAutoPushIntoSlot(atBlockFace, fromSlot);
            }

            return base.GetAutoPushIntoSlot(atBlockFace, fromSlot);
        }
    }
}
