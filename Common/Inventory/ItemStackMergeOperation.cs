using System;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumMergePriority
    {
        /// <summary>
        /// Automatic merge operation, when a player did not specifically request a merge, e.g. with shift + left click, or when collected from the ground
        /// </summary>
        AutoMerge = 0,
        /// <summary>
        /// When using mouse to manually merge item stacks
        /// </summary>
        DirectMerge = 1,
        /// <summary>
        /// Confirmed merge via dialog. Not implemented as of v1.14
        /// </summary>
        ConfirmedMerge = 2
    }

    public class ItemStackMoveOperation
    {
        /// <summary>
        /// The world that the move operation is being performed.
        /// </summary>
        public IWorldAccessor World;

        /// <summary>
        /// The acting player within the world.
        /// </summary>
        public IPlayer ActingPlayer;

        /// <summary>
        /// The mouse button the ActingPlayer has pressed.
        /// </summary>
        public EnumMouseButton MouseButton;

        /// <summary>
        /// Any modifiers that the ActingPlayer is using for the operation (Ctrl, shift, alt)
        /// </summary>
        public EnumModifierKey Modifiers;

        /// <summary>
        /// The current Priority for merging slots.
        /// </summary>
        public EnumMergePriority CurrentPriority;

        /// <summary>
        /// The required Priority (can be null)
        /// </summary>
        public EnumMergePriority? RequiredPriority;

        /// <summary>
        /// The confirmation message code for this operation.
        /// </summary>
        public string ConfirmationMessageCode;

        /// <summary>
        /// The amount requested.
        /// </summary>
        public int RequestedQuantity;

        /// <summary>
        /// The amount moveable.
        /// </summary>
        public int MovableQuantity;

        /// <summary>
        /// The amount moved.
        /// </summary>
        public int MovedQuantity;

        /// <summary>
        /// The amount not moved.
        /// </summary>
        public int NotMovedQuantity
        {
            get { return Math.Max(0, RequestedQuantity - MovedQuantity); }
        }

        /// <summary>
        /// Checks if the Shift Key is held down.
        /// </summary>
        public bool ShiftDown { get { return (Modifiers & EnumModifierKey.SHIFT) > 0; }  }

        /// <summary>
        /// Checks if the Ctrl key is held down.
        /// </summary>
        public bool CtrlDown { get { return (Modifiers & EnumModifierKey.CTRL) > 0; } }

        /// <summary>
        /// Checks if the Alt key is held down.
        /// </summary>
        public bool AltDown { get { return (Modifiers & EnumModifierKey.ALT) > 0; } }

        public int WheelDir = 0;


        public ItemStackMoveOperation(IWorldAccessor world, EnumMouseButton mouseButton, EnumModifierKey modifiers, EnumMergePriority currentPriority, int requestedQuantity = 0)
        {
            World = world;
            MouseButton = mouseButton;
            Modifiers = modifiers;
            CurrentPriority = currentPriority;
            RequestedQuantity = requestedQuantity;
        }

        /// <summary>
        /// Converts this MoveOperation to a Merge Operation.
        /// </summary>
        /// <param name="SinkSlot">The slot to put items.</param>
        /// <param name="SourceSlot">The slot to take items.</param>
        public ItemStackMergeOperation ToMergeOperation(ItemSlot SinkSlot, ItemSlot SourceSlot)
        {
            return new ItemStackMergeOperation(World, MouseButton, Modifiers, CurrentPriority, RequestedQuantity)
            {
                SinkSlot = SinkSlot,
                SourceSlot = SourceSlot,
                ActingPlayer = ActingPlayer
            };
        }
    }


    public class ItemStackMergeOperation : ItemStackMoveOperation
    {
        /// <summary>
        /// The slot that the item is attempting transfer to.
        /// </summary>
        public ItemSlot SinkSlot;

        /// <summary>
        /// The slot that the item is being transferred from
        /// </summary>
        public ItemSlot SourceSlot;

        public ItemStackMergeOperation(IWorldAccessor world, EnumMouseButton mouseButton, EnumModifierKey modifiers, EnumMergePriority currentPriority, int requestedQuantity) : base(world, mouseButton, modifiers, currentPriority, requestedQuantity)
        {
        }

    }
}
