using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public delegate bool CanClickSlotDelegate(int slotID);

    public abstract class GuiElementItemSlotGridBase : GuiElement
    {
        public static double unscaledSlotPadding = 3;

        protected IInventory inventory;

        internal OrderedDictionary<int, ItemSlot> availableSlots = new OrderedDictionary<int, ItemSlot>();
        internal OrderedDictionary<int, ItemSlot> renderedSlots = new OrderedDictionary<int, ItemSlot>();

        protected int cols;
        protected int rows;
        protected int prevSlotQuantity;
        

        Dictionary<string, int> slotTextureIdsByBackgroundIcon = new Dictionary<string, int>();
        Dictionary<int, float> slotNotifiedZoomEffect = new Dictionary<int, float>();

        protected ElementBounds[] slotBounds;

        protected LoadedTexture slotTexture, highlightSlotTexture;
        protected LoadedTexture[] slotQuantityTextures;

        protected GuiElementStaticText textComposer;

        protected int highlightSlotId = -1;
        protected int hoverSlotId = -1;

        protected string searchText;

        protected API.Common.Action<object> SendPacketHandler;

        bool isLastSlotGridInComposite;

        bool isRightMouseDownStartedInsideElem;
        bool isLeftMouseDownStartedInsideElem;
        
        HashSet<int> wasMouseDownOnSlotIndex = new HashSet<int>();
        OrderedDictionary<int, int> leftMouseDownDistributeSlotsBySlotid = new OrderedDictionary<int, int>();
        ItemStack referenceDistributStack;

        public CanClickSlotDelegate CanClickSlot;


        public override bool Focusable
        {
            get { return true; }
        }

        public GuiElementItemSlotGridBase(ICoreClientAPI capi, IInventory inventory, API.Common.Action<object> SendPacket, int cols, ElementBounds bounds) : base(capi, bounds)
        {
            slotTexture = new LoadedTexture(capi);
            highlightSlotTexture = new LoadedTexture(capi);
            prevSlotQuantity = inventory.QuantitySlots;
            this.inventory = inventory;
            this.cols = cols;
            this.SendPacketHandler = SendPacket;

            inventory.SlotNotified += OnSlotNotified;
        }

        private void OnSlotNotified(int slotid)
        {
            slotNotifiedZoomEffect[slotid] = 0.4f;
        }


        #region Drawing
        public override void ComposeElements(Context unusedCtx, ImageSurface unusedSurface)
        {
            ComposeInteractiveElements();
        }

        void ComposeInteractiveElements()
        { 
            slotBounds = new ElementBounds[availableSlots.Count];
            slotQuantityTextures = new LoadedTexture[availableSlots.Count];
            for (int k = 0; k < slotQuantityTextures.Length; k++) slotQuantityTextures[k] = new LoadedTexture(this.api);

            this.rows = (int)Math.Ceiling(1f * availableSlots.Count / cols);

            Bounds.CalcWorldBounds();

            double unscaledSlotWidth = GuiElementPassiveItemSlot.unscaledSlotSize;
            double unscaledSlotHeight = GuiElementPassiveItemSlot.unscaledSlotSize;

            // Slot sizes
            double absSlotPadding = scaled(unscaledSlotPadding);
            double absSlotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double absSlotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            ElementBounds textBounds = ElementBounds
                .Fixed(0, GuiElementPassiveItemSlot.unscaledSlotSize - ElementGeometrics.SmallishFontSize - 2, GuiElementPassiveItemSlot.unscaledSlotSize - 5, GuiElementPassiveItemSlot.unscaledSlotSize - 5)
                .WithEmptyParent();

            CairoFont font = CairoFont.WhiteSmallText().WithFontSize((float)ElementGeometrics.SmallishFontSize);
            font.FontWeight = FontWeight.Bold;
            font.Color = new double[] { 1, 1, 1, 1 };
            font.StrokeColor = new double[] { 0, 0, 0, 1 };
            font.StrokeWidth = RuntimeEnv.GUIScale;

            textComposer = new GuiElementStaticText(api, "", EnumTextOrientation.Right, textBounds, font);


            // 1. draw generic slot
            ImageSurface slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth, (int)absSlotWidth);
            Context slotCtx = genContext(slotSurface);

            slotCtx.SetSourceRGBA(ElementGeometrics.DialogAlternateBgColor);
            RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, ElementGeometrics.ElementBGRadius);
            slotCtx.Fill();
            EmbossRoundRectangleElement(slotCtx, 0, 0, absSlotWidth, absSlotHeight, true);

            generateTexture(slotSurface, ref slotTexture, true);

            slotCtx.Dispose();
            slotSurface.Dispose();

            // 2. draw slots with backgrounds
            foreach (ItemSlot slot in availableSlots.Values)
            {
                if (slot.BackgroundIcon == null || slotTextureIdsByBackgroundIcon.ContainsKey(slot.BackgroundIcon)) continue;

                slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth, (int)absSlotWidth);
                slotCtx = genContext(slotSurface);

                slotCtx.SetSourceRGBA(ElementGeometrics.DialogAlternateBgColor);
                RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, ElementGeometrics.ElementBGRadius);
                slotCtx.Fill();
                EmbossRoundRectangleElement(slotCtx, 0, 0, absSlotWidth, absSlotHeight, true);

                api.Gui.Icons.DrawIconInt(slotCtx, slot.BackgroundIcon, 2 * (int)absSlotPadding, 2 * (int)absSlotPadding, (int)(absSlotWidth - 4 * absSlotPadding), (int)(absSlotHeight - 4 * absSlotPadding), new double[] { 0, 0, 0, 0.2 });

                int texId = 0;
                generateTexture(slotSurface, ref texId, true);

                slotCtx.Dispose();
                slotSurface.Dispose();

                slotTextureIdsByBackgroundIcon[slot.BackgroundIcon] = texId;
            }

            // 3. Slot highlight
            slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth, (int)absSlotWidth);
            slotCtx = genContext(slotSurface);

            slotCtx.SetSourceRGBA(ElementGeometrics.DialogBlueBgColor);
            RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, ElementGeometrics.ElementBGRadius);
            slotCtx.Fill();

            generateTexture(slotSurface, ref highlightSlotTexture);

            slotCtx.Dispose();
            slotSurface.Dispose();

            int i = 0;
            foreach (var val in availableSlots)
            {
                int col = i % cols;
                int row = i / cols;

                double x = col * (unscaledSlotWidth + unscaledSlotPadding);
                double y = row * (unscaledSlotHeight + unscaledSlotPadding);

                IItemSlot slot = inventory.GetSlot(val.Key);

                slotBounds[i] = ElementBounds.Fixed(x, y, unscaledSlotWidth, unscaledSlotHeight).WithParent(Bounds);
                slotBounds[i].CalcWorldBounds();

                ComposeSlotOverlays(slot, val.Key);

                i++;
            }
        }


        bool ComposeSlotOverlays(IItemSlot slot, int slotId)
        {
            if (!availableSlots.ContainsKey(slotId)) return false;
            if (slot.Itemstack == null) return true;

            int slotIndex = availableSlots.IndexOfKey(slotId);

            //bool drawStackSize = slot.StackSize != 1;
            bool drawItemDamage = slot.Itemstack.Collectible.ShouldDisplayItemDamage(slot.Itemstack);

            if (!drawItemDamage) // && !drawStackSize)
            {
                slotQuantityTextures[slotIndex].Dispose();
                slotQuantityTextures[slotIndex] = new LoadedTexture(api);
                return true;
            }

            // This is pretty slow to do this for every slot. Should be made with an atlas
            ImageSurface textSurface = new ImageSurface(Format.Argb32, (int)slotBounds[slotIndex].InnerWidth, (int)slotBounds[slotIndex].InnerHeight);
            Context textCtx = genContext(textSurface);
            textCtx.SetSourceRGBA(0, 0, 0, 0);
            textCtx.Paint();

//            if (drawStackSize)
            {
                //textComposer.SetValue(slot.StackSize + "");
                //textComposer.ComposeElements(textCtx, textSurface);
            }

            if (drawItemDamage)
            {
                double x = scaled(4);
                double y = (int)slotBounds[slotIndex].InnerHeight - scaled(3) - scaled(4);
                textCtx.SetSourceRGBA(ElementGeometrics.DialogStrongBgColor);
                double width = (slotBounds[slotIndex].InnerWidth - scaled(8));
                double height = scaled(4);
                RoundRectangle(textCtx, x, y, width, height, 1);
                textCtx.FillPreserve();
                ShadePath(textCtx, 1);


                float[] color = ColorUtil.ToRGBAFloats(slot.Itemstack.Collectible.GetItemDamageColor(slot.Itemstack));
                textCtx.SetSourceRGB(color[0], color[1], color[2]);
                float health = (float)slot.Itemstack.Attributes.GetInt("durability", slot.Itemstack.Collectible.Durability) / slot.Itemstack.Collectible.Durability;

                width = health * (slotBounds[slotIndex].InnerWidth - scaled(8));

                RoundRectangle(textCtx, x, y, width, height, 1);
                textCtx.FillPreserve();
                ShadePath(textCtx, 1);
            }

            generateTexture(textSurface, ref slotQuantityTextures[slotIndex]);
            textCtx.Dispose();
            textSurface.Dispose();

            return true;
        }

        public override void PostRenderInteractiveElements(float deltaTime)
        {
            if (slotNotifiedZoomEffect.Count > 0)
            {
                List<int> slotIds = new List<int>(slotNotifiedZoomEffect.Keys);

                foreach (int slotId in slotIds)
                {
                    slotNotifiedZoomEffect[slotId] -= deltaTime;

                    if (slotNotifiedZoomEffect[slotId] <= 0)
                    {
                        slotNotifiedZoomEffect.Remove(slotId);
                    }
                }
            }



            // Just redo the whole thing if slot quantity has changed
            if (prevSlotQuantity != inventory.QuantitySlots)
            {
                prevSlotQuantity = inventory.QuantitySlots;
                inventory.DirtySlots.Clear();
                ComposeElements(null, null);
                return;
            }

            if (inventory.DirtySlots.Count == 0) return;

            List<int> handled = new List<int>();

            foreach (int slotId in inventory.DirtySlots)
            {
                IItemSlot slot = inventory.GetSlot(slotId);

                if (ComposeSlotOverlays(slot, slotId))
                {
                    handled.Add(slotId);
                }
            }

            if (isLastSlotGridInComposite)
            {
                foreach (int slotId in handled)
                {
                    inventory.DirtySlots.Remove(slotId);
                }
            }

          
            
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            // Slot sizes
            double absSlotPadding = scaled(unscaledSlotPadding);
            double absSlotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double absItemstackSize = scaled(GuiElementPassiveItemSlot.unscaledItemSize);
            double absItemStackSizeAnim = scaled(80);
            double absSlotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            double offset = absSlotWidth / 2;

            int i = 0;
            foreach (var val in renderedSlots)
            {
                // Don't need to render stuff completely outside, saves us many render calls (~down to 100 draw calls instead of 600 for creative inventory)
                if (slotBounds[i].PartiallyInside(Bounds.ParentBounds))
                {
                    ItemSlot slot = val.Value;
                    int slotId = val.Key;

                    if (slot.Itemstack == null && slot.BackgroundIcon != null)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(slotTextureIdsByBackgroundIcon[slot.BackgroundIcon], slotBounds[i]);
                    } else
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(slotTexture.TextureId, slotBounds[i]);
                    }

                    

                    if (highlightSlotId == slotId || hoverSlotId == slotId || leftMouseDownDistributeSlotsBySlotid.ContainsKey(slotId))
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(highlightSlotTexture.TextureId, slotBounds[i]);
                    }

                    if (slot.Itemstack == null) { i++; continue; }

                    
                    float dx = 0;
                    float dy = 0;
                    if (slotNotifiedZoomEffect.ContainsKey(slotId))
                    {
                        dx = 4 * (float)api.World.Rand.NextDouble() - 2f;
                        dy = 4 * (float)api.World.Rand.NextDouble() - 2f;
                    }

                    api.Render.RenderItemstackToGui(
                        slot.Itemstack,
                        slotBounds[i].renderX + offset + dy,
                        slotBounds[i].renderY + offset + dx,
                        90,
                        (float)(absItemstackSize),
                        ColorUtil.WhiteArgb
                    );

                    if (slotQuantityTextures[i].TextureId != 0)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(slotQuantityTextures[i].TextureId, slotBounds[i]);
                    }

                }

                i++;
            }





        }


        public void OnGuiClosed(ICoreClientAPI api)
        {
            if (hoverSlotId != -1) api.Input.TriggerOnMouseLeaveSlot(inventory.GetSlot(hoverSlotId));
            hoverSlotId = -1;
        }

        public override int OutlineColor()
        {
            return (255 << 8) + (255 << 24);
        }

        public void FilterItemsBySearchText(string text)
        {
            this.searchText = text.ToLowerInvariant();

            renderedSlots.Clear();

            foreach (var val in availableSlots)
            {
                ItemSlot slot = inventory.GetSlot(val.Key);

                if (slot.Itemstack == null) continue;

                if (searchText == null || searchText.Length == 0 || slot.Itemstack.MatchesSearchText(searchText))
                {
                    renderedSlots.Add(val.Key, slot);
                }
            }

            this.rows = (int)Math.Ceiling(1f * renderedSlots.Count / cols);
            ComposeInteractiveElements();

        }

        #endregion

        #region Keyboard, Mouse

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            base.OnKeyDown(api, args);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;
            base.OnMouseDownOnElement(api, args);
            wasMouseDownOnSlotIndex.Clear();
            leftMouseDownDistributeSlotsBySlotid.Clear();

            for (int i = 0; i < slotBounds.Length; i++)
            {
                if (i >= renderedSlots.Count) break;

                if (slotBounds[i].PointInside(args.X, args.Y))
                {
                    if (CanClickSlot?.Invoke(i) == false) break;

                    isRightMouseDownStartedInsideElem = args.Button == EnumMouseButton.Right && api.World.Player.InventoryManager.MouseItemSlot.Itemstack != null;
                    isLeftMouseDownStartedInsideElem = args.Button == EnumMouseButton.Left && api.World.Player.InventoryManager.MouseItemSlot.Itemstack != null;
                    
                    wasMouseDownOnSlotIndex.Add(i);
                    if (isLeftMouseDownStartedInsideElem)
                    {
                        referenceDistributStack = api.World.Player.InventoryManager.MouseItemSlot.Itemstack.Clone();
                        int slotid = renderedSlots.GetKeyAtIndex(i);
                        leftMouseDownDistributeSlotsBySlotid.Add(slotid, inventory.GetSlot(slotid).StackSize);
                    }

                    SlotClick(
                        api, 
                        renderedSlots.GetKeyAtIndex(i), 
                        args.Button, 
                        api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft],
                        api.Input.KeyboardKeyState[(int)GlKeys.LControl],
                        api.Input.KeyboardKeyState[(int)GlKeys.LAlt]
                    );

                    break;
                    
                }
            }
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            isRightMouseDownStartedInsideElem = false;
            isLeftMouseDownStartedInsideElem = false;
            wasMouseDownOnSlotIndex.Clear();
            leftMouseDownDistributeSlotsBySlotid.Clear();
            base.OnMouseUp(api, args);
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y))
            {
                if (hoverSlotId != -1)
                {
                    api.Input.TriggerOnMouseLeaveSlot(inventory.GetSlot(hoverSlotId));
                }
                hoverSlotId = -1;
                return;
            }

            for (int i = 0; i < slotBounds.Length; i++)
            {
                if (i >= renderedSlots.Count) break;

                if (slotBounds[i].PointInside(args.X, args.Y))
                {
                    int newHoverSlotid = renderedSlots.GetKeyAtIndex(i);
                    ItemStack stack = inventory.GetSlot(newHoverSlotid).Itemstack;

                    // Cheap hax for hover-right-mouse-down slot filling
                    if (isRightMouseDownStartedInsideElem && !wasMouseDownOnSlotIndex.Contains(i))
                    {
                        wasMouseDownOnSlotIndex.Add(i);
                        
                        if (stack == null || stack.Equals(api.World, api.World.Player.InventoryManager.MouseItemSlot.Itemstack, GlobalConstants.IgnoredStackAttributes))
                        {
                            SlotClick(
                                api,
                                newHoverSlotid,
                                EnumMouseButton.Right,
                                api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft],
                                api.Input.KeyboardKeyState[(int)GlKeys.LControl],
                                api.Input.KeyboardKeyState[(int)GlKeys.LAlt]
                            );
                        }
                    }

                    // Cheap hax for hover-left-mouse-down slot filling
                    if (isLeftMouseDownStartedInsideElem && !wasMouseDownOnSlotIndex.Contains(i))
                    {
                        if (stack == null || stack.Equals(api.World, referenceDistributStack, GlobalConstants.IgnoredStackAttributes))
                        {
                            wasMouseDownOnSlotIndex.Add(i);
                            leftMouseDownDistributeSlotsBySlotid.Add(newHoverSlotid, stack == null ? 0 : stack.StackSize);

                            if (api.World.Player.InventoryManager.MouseItemSlot.StackSize > 0)
                            {
                                SlotClick(
                                    api,
                                    newHoverSlotid,
                                    EnumMouseButton.Left,
                                    api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft],
                                    api.Input.KeyboardKeyState[(int)GlKeys.LControl],
                                    api.Input.KeyboardKeyState[(int)GlKeys.LAlt]
                                );
                            }

                            if (api.World.Player.InventoryManager.MouseItemSlot.StackSize <= 0)
                            {
                                RedistributeStacks(i);
                            }
                        }
                    }



                    if (newHoverSlotid != hoverSlotId && inventory.GetSlot(newHoverSlotid) != null)
                    {
                        api.Input.TriggerOnMouseEnterSlot(inventory.GetSlot(newHoverSlotid));
                    }

                    hoverSlotId = newHoverSlotid;
                    return;
                }
            }

            if (hoverSlotId != -1) api.Input.TriggerOnMouseLeaveSlot(inventory.GetSlot(hoverSlotId));
            hoverSlotId = -1;
        }

        private void RedistributeStacks(int newIndex)
        {
            int stacksPerSlot = referenceDistributStack.StackSize / leftMouseDownDistributeSlotsBySlotid.Count;

            for (int i = 0; i < leftMouseDownDistributeSlotsBySlotid.Count - 1; i++)
            {
                int sourceSlotid = leftMouseDownDistributeSlotsBySlotid.GetKeyAtIndex(i);
                ItemSlot sourceSlot = inventory.GetSlot(sourceSlotid);

                int beforesize = leftMouseDownDistributeSlotsBySlotid[sourceSlotid];
                int nowsize = sourceSlot.StackSize;

                if (nowsize - beforesize > stacksPerSlot)
                {
                    //Console.WriteLine("{0} - {1} > {2}", nowsize, beforesize, stacksPerSlot);

                    for (int j = i+1; j < leftMouseDownDistributeSlotsBySlotid.Count; j++)
                    {
                        int targetslotid = leftMouseDownDistributeSlotsBySlotid.GetKeyAtIndex(j);
                        ItemSlot targetSlot = inventory.GetSlot(targetslotid);
                        ItemStackMoveOperation op = new ItemStackMoveOperation(api.World, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge);                        
                        op.ActingPlayer = api.World.Player;
                        op.RequestedQuantity = nowsize - beforesize - stacksPerSlot;

                        object packet = api.World.Player.InventoryManager.TryTransferTo(sourceSlot, targetSlot, ref op);

                        if (packet != null)
                        {
                            SendPacketHandler(packet);
                        }

                        if (op.NotMovedQuantity == 0) break;
                    }
                }
            }
        }

        internal virtual void SlotClick(ICoreClientAPI api, int slotId, EnumMouseButton mouseButton, bool shiftPressed, bool ctrlPressed, bool altPressed)
        {
            List<IInventory> inventories = api.World.Player.InventoryManager.OpenedInventories;
            IInventory mouseCursorSlot = api.World.Player.InventoryManager.GetOwnInventory(GlobalConstants.mousecursorInvClassName);
            object packet = null;

            EnumModifierKey modifiers =
                (shiftPressed ? EnumModifierKey.SHIFT : 0) |
                (ctrlPressed ? EnumModifierKey.CTRL : 0) |
                (altPressed ? EnumModifierKey.ALT : 0)
            ;
            ItemStackMoveOperation op = new ItemStackMoveOperation(api.World, mouseButton, modifiers, EnumMergePriority.AutoMerge);
            op.ActingPlayer = api.World.Player;

            if (shiftPressed)
            {
                ItemSlot sourceSlot = inventory.GetSlot(slotId);
                op.RequestedQuantity = sourceSlot.StackSize;

                object[] packets = api.World.Player.InventoryManager.TryTransferAway(sourceSlot, ref op, false);

                if (packets != null)
                {
                    for (int i = 0; i < packets.Length; i++)
                    {
                        SendPacketHandler(packets[i]);
                    }
                }

                api.Input.TriggerOnMouseClickSlot(inventory.GetSlot(slotId));

                return;
            }

            op.CurrentPriority = EnumMergePriority.DirectMerge;
            packet = inventory.ActivateSlot(slotId,  mouseCursorSlot.GetSlot(0), ref op);

            if (packet != null)
            {
                SendPacketHandler?.Invoke(packet);
            }

            api.Input.TriggerOnMouseClickSlot(inventory.GetSlot(slotId));
        }

        #endregion

        public void HighlightSlot(int slotId)
        {
            highlightSlotId = slotId;
        }

        public void RemoveSlotHighlight()
        {
            highlightSlotId = -1;
        }


        internal static void UpdateLastSlotGridFlag(GuiComposer composer)
        {
            Dictionary<IInventory, GuiElementItemSlotGridBase> lastelembyInventory = new Dictionary<IInventory, GuiElementItemSlotGridBase>();

            foreach (GuiElement elem in composer.interactiveElements.Values)
            {
                if (elem is GuiElementItemSlotGridBase)
                {
                    GuiElementItemSlotGridBase slotgridelem = elem as GuiElementItemSlotGridBase;
                    slotgridelem.isLastSlotGridInComposite = false;

                    lastelembyInventory[slotgridelem.inventory] = slotgridelem;
                }
            }

            foreach (GuiElementItemSlotGridBase lastelem in lastelembyInventory.Values)
            {
                lastelem.isLastSlotGridInComposite = true;
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; slotQuantityTextures != null && i < slotQuantityTextures.Length; i++) slotQuantityTextures[i]?.Dispose();
            slotTexture.Dispose();
            highlightSlotTexture.Dispose();
        }
    }
}
