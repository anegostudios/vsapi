using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Displays the slots of an inventory in the form of a slot grid
    /// </summary>
    public class GuiElementItemSlotGrid : GuiElementItemSlotGridBase
    {

        public GuiElementItemSlotGrid(ICoreClientAPI capi, IInventory inventory, API.Common.Action<object> SendPacketHandler, int cols, int[] visibleSlots, ElementBounds bounds) : base(capi, inventory, SendPacketHandler, cols, bounds)
        {
            DetermineAvailableSlots(visibleSlots);

            this.SendPacketHandler = SendPacketHandler;
        }


        public void DetermineAvailableSlots(int[] visibleSlots = null)
        {
            availableSlots.Clear();
            renderedSlots.Clear();

            if (visibleSlots != null)
            {
                for (int i = 0; i < visibleSlots.Length; i++)
                {
                    availableSlots.Add(visibleSlots[i], inventory.GetSlot(visibleSlots[i]));
                    renderedSlots.Add(visibleSlots[i], inventory.GetSlot(visibleSlots[i]));
                }
            }
            else
            {
                for (int i = 0; i < inventory.QuantitySlots; i++)
                {
                    availableSlots.Add(i, inventory.GetSlot(i));
                    renderedSlots.Add(i, inventory.GetSlot(i));
                }
            }
        }
    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddItemSlotGrid(this GuiComposer composer, IInventory inventory, API.Common.Action<object> SendPacket, int cols, ElementBounds bounds, string key=null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGrid(composer.Api, inventory, SendPacket, cols, null, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }
            return composer;
        }

        public static GuiComposer AddItemSlotGrid(this GuiComposer composer, IInventory inventory, API.Common.Action<object> SendPacket, int cols, int[] selectiveSlots, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGrid(composer.Api, inventory, SendPacket, cols, selectiveSlots, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }

            return composer;
        }

        public static GuiElementItemSlotGrid GetSlotGrid(this GuiComposer composer, string key)
        {
            return (GuiElementItemSlotGrid)composer.GetElement(key);
        }
    }

}
