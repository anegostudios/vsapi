using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementItemSlotGridJounral : GuiElementItemSlotGrid
    {
        public API.Common.Action<int> OnSlotClickLeftMouse;

        public GuiElementItemSlotGridJounral(ICoreClientAPI capi, IInventory inventory, API.Common.Action<object> SendPacketHandler, int cols, int[] visibleSlots, ElementBounds bounds) : base(capi, inventory, SendPacketHandler, cols, visibleSlots, bounds)
        {
        }

        internal override void SlotClick(ICoreClientAPI api, int slotId, EnumMouseButton mouseButton, bool shiftPressed, bool ctrlPressed, bool altPressed)
        {
            if (mouseButton == EnumMouseButton.Left)
            {
                OnSlotClickLeftMouse(slotId);
                return;
            }

            base.SlotClick(api, slotId, mouseButton, shiftPressed, ctrlPressed, altPressed);
        }
    }
}
