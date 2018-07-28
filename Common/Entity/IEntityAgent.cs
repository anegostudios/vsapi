using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Return false to stop walking the inventory
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public delegate bool OnInventorySlot(IItemSlot slot);

    /// <summary>
    /// A non passive, acting entity. Has some sort of motor controls and can hold items.
    /// </summary>
    public interface IEntityAgent : IEntity
    {
        /// <summary>
        /// Returns the current right hand slot of the entity or null if it doesn't have one
        /// </summary>
        ItemSlot RightHandItemSlot { get; }

        /// <summary>
        /// Returns the current left hand slot of the entity or null if it doesn't have one
        /// </summary>
        ItemSlot LeftHandItemSlot { get; }

        /// <summary>
        /// Returns the current gear inventory of the entity or null if it doesn't have any
        /// </summary>
        IInventory GearInventory { get; }
        

        /// <summary>
        /// Iterates over the entities inventory. Return false in your handler to stop walking the inventory
        /// </summary>
        /// <param name="slot"></param>
        void WalkInventory(OnInventorySlot handler);

        /// <summary>
        /// An object that stores where the entity is currently moving or should be moving
        /// </summary>
        EntityControls Controls { get; }

        /// <summary>
        /// Saturates the entity by given value, reducing their hunger
        /// </summary>
        /// <param name="saturation"></param>
        void ReceiveSaturation(float saturation);

        /// <summary>
        /// The height from where the camera is placed when being played by a player
        /// </summary>
        /// <returns></returns>
        double EyeHeight();

        /// <summary>
        /// True if the eyes are under water
        /// </summary>
        /// <returns></returns>
        bool IsEyesSubmerged();

        /// <summary>
        /// Attempt to stop the current using action, if any is running
        /// </summary>
        /// <param name="isCancel"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        bool TryStopHandAction(bool isCancel, EnumItemUseCancelReason cancelReason = EnumItemUseCancelReason.ReleasedMouse);

    }

}
