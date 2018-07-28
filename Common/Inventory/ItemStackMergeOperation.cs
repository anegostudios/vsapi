using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumMergePriority
    {
        /// <summary>
        /// Automatic merge operation
        /// </summary>
        AutoMerge = 0,
        /// <summary>
        /// When using mouse to manually merge item stacks
        /// </summary>
        DirectMerge = 1,
        /// <summary>
        /// Confirmed merge via dialog 
        /// </summary>
        ConfirmedMerge = 2
    }

    public class ItemStackMoveOperation
    {
        public IWorldAccessor World;
        public IPlayer ActingPlayer;

        public EnumMouseButton MouseButton;
        public EnumModifierKey Modifiers;

        public EnumMergePriority CurrentPriority;
        public EnumMergePriority? RequiredPriority;

        public string ConfirmationMessageCode;

        public int RequestedQuantity;
        public int MovableQuantity;
        public int MovedQuantity;

        public int NotMovedQuantity
        {
            get { return Math.Max(0, RequestedQuantity - MovedQuantity); }
        }

        public bool ShiftDown { get { return (Modifiers & EnumModifierKey.SHIFT) > 0; }  }
        public bool CtrlDown { get { return (Modifiers & EnumModifierKey.CTRL) > 0; } }
        public bool AltDown { get { return (Modifiers & EnumModifierKey.ALT) > 0; } }


        public ItemStackMoveOperation(IWorldAccessor world, EnumMouseButton mouseButton, EnumModifierKey modifiers, EnumMergePriority currentPriority, int requestedQuantity = 0)
        {
            World = world;
            MouseButton = mouseButton;
            Modifiers = modifiers;
            CurrentPriority = currentPriority;
            RequestedQuantity = requestedQuantity;
        }

        public ItemStackMergeOperation ToMergeOperation(IItemSlot SinkSlot, IItemSlot SourceSlot)
        {
            return new ItemStackMergeOperation(World, MouseButton, Modifiers, CurrentPriority, RequestedQuantity)
            {
                SinkSlot = SinkSlot,
                SourceSlot = SourceSlot
            };
        }
    }


    public class ItemStackMergeOperation : ItemStackMoveOperation
    {
        public IItemSlot SinkSlot;
        public IItemSlot SourceSlot;

        public ItemStackMergeOperation(IWorldAccessor world, EnumMouseButton mouseButton, EnumModifierKey modifiers, EnumMergePriority currentPriority, int requestedQuantity) : base(world, mouseButton, modifiers, currentPriority, requestedQuantity)
        {
        }

    }
}
