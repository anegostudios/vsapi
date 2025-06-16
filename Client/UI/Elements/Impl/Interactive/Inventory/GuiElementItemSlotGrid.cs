using System;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Displays the slots of an inventory in the form of a slot grid
    /// </summary>
    public class GuiElementItemSlotGrid : GuiElementItemSlotGridBase
    {

        public GuiElementItemSlotGrid(ICoreClientAPI capi, IInventory inventory, Action<object> SendPacketHandler, int cols, int[] visibleSlots, ElementBounds bounds) : base(capi, inventory, SendPacketHandler, cols, bounds)
        {
            DetermineAvailableSlots(visibleSlots);

            this.SendPacketHandler = SendPacketHandler;
        }

        

        /// <summary>
        /// Determines the available slots for the slot grid.
        /// </summary>
        /// <param name="visibleSlots"></param>
        public void DetermineAvailableSlots(int[] visibleSlots = null)
        {
            availableSlots.Clear();
            renderedSlots.Clear();

            if (visibleSlots != null)
            {
                for (int i = 0; i < visibleSlots.Length; i++)
                {
                    availableSlots.Add(visibleSlots[i], inventory[visibleSlots[i]]);
                    renderedSlots.Add(visibleSlots[i], inventory[visibleSlots[i]]);
                }
            }
            else
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    availableSlots.Add(i, inventory[i]);
                    renderedSlots.Add(i, inventory[i]);
                }
            }
        }

        
    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds an item slot grid to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="inventory">The inventory attached to the slot grid.</param>
        /// <param name="sendPacket">A handler that should send supplied network packet to the server, if the inventory modifications should be synced</param>
        /// <param name="columns">The number of columns in the slot grid.</param>
        /// <param name="bounds">the bounds of the slot grid.</param>
        /// <param name="key">The key for this particular slot grid.</param>
        public static GuiComposer AddItemSlotGrid(this GuiComposer composer, IInventory inventory, Action<object> sendPacket, int columns, ElementBounds bounds, string key=null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGrid(composer.Api, inventory, sendPacket, columns, null, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }
            return composer;
        }

        /// <summary>
        /// Adds an item slot grid to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="inventory">The inventory attached to the slot grid.</param>
        /// <param name="sendPacket">A handler that should send supplied network packet to the server, if the inventory modifications should be synced</param>
        /// <param name="columns">The number of columns in the slot grid.</param>
        /// <param name="selectiveSlots">The slots within the inventory that are currently accessible.</param>
        /// <param name="bounds">the bounds of the slot grid.</param>
        /// <param name="key">The key for this particular slot grid.</param>
        public static GuiComposer AddItemSlotGrid(this GuiComposer composer, IInventory inventory, Action<object> sendPacket, int columns, int[] selectiveSlots, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGrid(composer.Api, inventory, sendPacket, columns, selectiveSlots, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }

            return composer;
        }

        /// <summary>
        /// Gets the slot grid by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the slot grid to get.</param>
        public static GuiElementItemSlotGrid GetSlotGrid(this GuiComposer composer, string key)
        {
            return (GuiElementItemSlotGrid)composer.GetElement(key);
        }
    }

}
