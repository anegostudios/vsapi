using System;
using System.Linq;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Builds slot grid with exclusions to the grid.
    /// </summary>
    public class GuiElementItemSlotGridExcl : GuiElementItemSlotGridBase
    {
        int[] excludingSlots;

        /// <summary>
        /// Creates a new slot grid with exclusions.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="inventory">The attached inventory.</param>
        /// <param name="SendPacketHandler">A handler that should send supplied network packet to the server, if the inventory modifications should be synced</param>
        /// <param name="columns">The number of columns in the slot grid.</param>
        /// <param name="excludingSlots">The slots that have been excluded.</param>
        /// <param name="bounds">The bounds of the slot grid.</param>
        public GuiElementItemSlotGridExcl(ICoreClientAPI capi, IInventory inventory, Action<object> SendPacketHandler, int columns, int[] excludingSlots, ElementBounds bounds) : base(capi, inventory, SendPacketHandler, columns, bounds)
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
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (excludingSlots.Contains(i)) continue;
                    ItemSlot slot = inventory[i];

                    availableSlots.Add(i, slot);
                    renderedSlots.Add(i, slot);
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

        /// <summary>
        /// Adds an ItemSlotGrid with Exclusions.
        /// </summary>
        /// <param name="inventory">The attached inventory.</param>
        /// <param name="SendPacket">A handler that should send supplied network packet to the server, if the inventory modifications should be synced</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="excludingSlots">The slots that have been excluded from the slot grid.</param>
        /// <param name="bounds">The bounds of the slot grid.</param>
        /// <param name="key">The name of the slot grid.</param>
        public static GuiComposer AddItemSlotGridExcl(this GuiComposer composer, IInventory inventory, Action<object> SendPacket, int columns, int[] excludingSlots, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementItemSlotGridExcl(composer.Api, inventory, SendPacket, columns, excludingSlots, bounds), key);
                GuiElementItemSlotGridBase.UpdateLastSlotGridFlag(composer);
            }
            return composer;
        }

        /// <summary>
        /// Gets the ItemSlotGridExcl by name.
        /// </summary>
        /// <param name="key">The name of the ItemSlotGridExcl</param>
        public static GuiElementItemSlotGridExcl GetSlotGridExcl(this GuiComposer composer, string key)
        {
            return (GuiElementItemSlotGridExcl)composer.GetElement(key);
        }
    }
}
