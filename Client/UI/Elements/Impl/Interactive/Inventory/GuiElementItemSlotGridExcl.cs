using System.Linq;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class GuiElementItemSlotGridExcl : GuiElementItemSlotGridBase
    {
        int[] excludingSlots;

        public GuiElementItemSlotGridExcl(ICoreClientAPI capi, IInventory inventory, API.Common.Action<object> SendPacketHandler, int cols, int[] excludingSlots, ElementBounds bounds) : base(capi, inventory, SendPacketHandler, cols, bounds)
        {
            this.excludingSlots = excludingSlots;
            InitDicts();
            this.SendPacketHandler = SendPacketHandler;
        }


        internal void InitDicts()
        {
            availableSlots.Clear();
            renderedSlots.Clear();

            if (excludingSlots != null)
            {
                for (int i = 0; i < inventory.QuantitySlots; i++)
                {
                    if (excludingSlots.Contains(i)) continue;
                    ItemSlot slot = inventory.GetSlot(i);

                    availableSlots.Add(i, slot);
                    renderedSlots.Add(i, slot);
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

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            InitDicts();
            base.ComposeElements(ctx, surface);
        }

        public override void PostRenderInteractiveElements(float deltaTime)
        {
            if (inventory.DirtySlots.Count > 0)
            {
                InitDicts();
            }

            base.PostRenderInteractiveElements(deltaTime);
        }


    }


    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddItemSlotGridExcl(this GuiComposer composer, IInventory inventory, API.Common.Action<object> SendPacket, int cols, int[] excludingSlots, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGridExcl(composer.Api, inventory, SendPacket, cols, excludingSlots, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }
            return composer;
        }

        public static GuiElementItemSlotGridExcl GetSlotGridExcl(this GuiComposer composer, string key)
        {
            return (GuiElementItemSlotGridExcl)composer.GetElement(key);
        }
    }
}
