﻿using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Client;
using Vintagestory.API.Util;
using System.Linq;

namespace Vintagestory.API.Client
{
    public delegate bool CanClickSlotDelegate(int slotID);
    public delegate void DrawIconDelegate(Context cr, string type, int x, int y, float width, float height, double[] rgba);

    /// <summary>
    /// A base class for the slot grid.  For all your slot grid needs.
    /// </summary>
    public abstract class GuiElementItemSlotGridBase : GuiElement
    {
        public static double unscaledSlotPadding = 3;

        protected IInventory inventory;

        internal OrderedDictionary<int, ItemSlot> availableSlots = new OrderedDictionary<int, ItemSlot>();
        internal OrderedDictionary<int, ItemSlot> renderedSlots = new OrderedDictionary<int, ItemSlot>();

        protected int cols;
        protected int rows;
        protected int prevSlotQuantity;
        

        Dictionary<string, int> slotTextureIdsByBgIconAndColor = new Dictionary<string, int>();
        Dictionary<int, float> slotNotifiedZoomEffect = new Dictionary<int, float>();

        protected ElementBounds[] slotBounds;
        protected ElementBounds[] scissorBounds;

        protected LoadedTexture slotTexture, highlightSlotTexture;
        protected LoadedTexture crossedOutTexture;
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
        
        IInventory hoverInv;

        public DrawIconDelegate DrawIconHandler;
        public bool AlwaysRenderIcon { get; set; } = false;

        public override bool Focusable
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="inventory">The attached inventory</param>
        /// <param name="SendPacket">A handler that should send supplied network packet to the server, if the inventory modifications should be synced</param>
        /// <param name="columns">The number of columns in the GUI.</param>
        /// <param name="bounds">The bounds of the slot grid.</param>
        public GuiElementItemSlotGridBase(ICoreClientAPI capi, IInventory inventory, API.Common.Action<object> SendPacket, int columns, ElementBounds bounds) : base(capi, bounds)
        {
            slotTexture = new LoadedTexture(capi);
            highlightSlotTexture = new LoadedTexture(capi);
            crossedOutTexture = new LoadedTexture(capi);

            prevSlotQuantity = inventory.Count;
            this.inventory = inventory;
            cols = columns;
            SendPacketHandler = SendPacket;

            inventory.SlotNotified += OnSlotNotified;

            DrawIconHandler = (cr, type, x, y, w, h, rgba) => api.Gui.Icons.DrawIconInt(cr, type, x, y, w, h, rgba);
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
            scissorBounds = new ElementBounds[availableSlots.Count];
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
                .Fixed(0, GuiElementPassiveItemSlot.unscaledSlotSize - GuiStyle.SmallishFontSize - 2, GuiElementPassiveItemSlot.unscaledSlotSize - 5, GuiElementPassiveItemSlot.unscaledSlotSize - 5)
                .WithEmptyParent();

            CairoFont font = CairoFont.WhiteSmallText().WithFontSize((float)GuiStyle.SmallishFontSize);
            font.FontWeight = FontWeight.Bold;
            font.Color = new double[] { 1, 1, 1, 1 };
            font.StrokeColor = new double[] { 0, 0, 0, 1 };
            font.StrokeWidth = RuntimeEnv.GUIScale;

            textComposer = new GuiElementStaticText(api, "", EnumTextOrientation.Right, textBounds, font);


            // 1. draw generic slot
            ImageSurface slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth, (int)absSlotWidth);
            Context slotCtx = genContext(slotSurface);

            slotCtx.SetSourceRGBA(GuiStyle.DialogSlotBackColor);
            RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
            slotCtx.Fill();

            slotCtx.SetSourceRGBA(GuiStyle.DialogSlotFrontColor);
            RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
            slotCtx.LineWidth = scaled(4.5);
            slotCtx.Stroke();
            slotSurface.Blur(scaled(4), true);
            slotSurface.Blur(scaled(4), true);

            RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, 1);
            slotCtx.LineWidth = scaled(4.5);
            slotCtx.SetSourceRGBA(0,0,0,0.8);
            slotCtx.Stroke();



            generateTexture(slotSurface, ref slotTexture, true);

            slotCtx.Dispose();
            slotSurface.Dispose();

            // 2. draw slots with backgrounds
            foreach (var val in availableSlots)
            {
                ItemSlot slot = val.Value;
                string key = slot.BackgroundIcon + "-" + slot.HexBackgroundColor;

                if ((slot.BackgroundIcon == null && slot.HexBackgroundColor == null) || slotTextureIdsByBgIconAndColor.ContainsKey(key)) continue;

                
                slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth, (int)absSlotWidth);
                slotCtx = genContext(slotSurface);

                if (slot.HexBackgroundColor != null)
                {
                    double[] bgcolor = ColorUtil.Hex2Doubles(slot.HexBackgroundColor);

                    slotCtx.SetSourceRGBA(bgcolor);
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
                    slotCtx.Fill();
                    
                    slotCtx.SetSourceRGBA(bgcolor[0] * 0.25, bgcolor[1] * 0.25, bgcolor[2] * 0.25, 1);
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
                    slotCtx.LineWidth = scaled(4.5);
                    slotCtx.Stroke();
                    slotSurface.Blur(scaled(4), true);
                    slotSurface.Blur(scaled(4), true);

                    slotCtx.SetSourceRGBA(0, 0, 0, 0.8);
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, 1);
                    slotCtx.LineWidth = scaled(4.5);
                    slotCtx.Stroke();
                }
                else
                {
                    slotCtx.SetSourceRGBA(GuiStyle.DialogSlotBackColor);
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
                    slotCtx.Fill();

                    slotCtx.SetSourceRGBA(GuiStyle.DialogSlotFrontColor);
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, GuiStyle.ElementBGRadius);
                    slotCtx.LineWidth = scaled(4.5);
                    slotCtx.Stroke();
                    slotSurface.Blur(scaled(4), true);
                    slotSurface.Blur(scaled(4), true);
                    
                    RoundRectangle(slotCtx, 0, 0, absSlotWidth, absSlotHeight, 1);
                    slotCtx.LineWidth = scaled(4.5);
                    slotCtx.SetSourceRGBA(0, 0, 0, 0.8);
                    slotCtx.Stroke();
                }


                if (slot.BackgroundIcon != null)
                {
                    DrawIconHandler?.Invoke(
                        slotCtx, slot.BackgroundIcon, 2 * (int)absSlotPadding, 2 * (int)absSlotPadding,
                        (int)(absSlotWidth - 4 * absSlotPadding), (int)(absSlotHeight - 4 * absSlotPadding),
                        new double[] { 0, 0, 0, 0.2 }
                    );
                }

                int texId = 0;
                generateTexture(slotSurface, ref texId, true);

                slotCtx.Dispose();
                slotSurface.Dispose();

                slotTextureIdsByBgIconAndColor[key] = texId;
            }

            // 3. Crossed out overlay
            int csize = (int)absSlotWidth - 4;
            slotSurface = new ImageSurface(Format.Argb32, (int)csize, (int)csize);
            slotCtx = genContext(slotSurface);

            slotCtx.SetSourceRGBA(0, 0, 0, 0.8);
            api.Gui.Icons.DrawCross(slotCtx, 4, 4, 7, csize - 18, true);
            slotCtx.SetSourceRGBA(1, 0.2, 0.2, 0.8);
            slotCtx.LineWidth = 2;
            slotCtx.Stroke();
            generateTexture(slotSurface, ref crossedOutTexture);
            slotCtx.Dispose();
            slotSurface.Dispose();


            // 4. Slot highlight overlay
            slotSurface = new ImageSurface(Format.Argb32, (int)absSlotWidth + 4, (int)absSlotWidth + 4);
            slotCtx = genContext(slotSurface);
            
            slotCtx.SetSourceRGBA(GuiStyle.ActiveSlotColor);
            RoundRectangle(slotCtx, 0, 0, absSlotWidth + 4, absSlotHeight + 4, GuiStyle.ElementBGRadius);
            slotCtx.LineWidth = scaled(9);
            slotCtx.StrokePreserve();
            slotSurface.Blur(scaled(6), true);
            slotCtx.StrokePreserve();
            slotSurface.Blur(scaled(6), true);

            slotCtx.LineWidth = scaled(3);
            slotCtx.Stroke();

            slotCtx.LineWidth = scaled(1);
            slotCtx.SetSourceRGBA(GuiStyle.ActiveSlotColor);
            slotCtx.Stroke();

            generateTexture(slotSurface, ref highlightSlotTexture);

            

            slotCtx.Dispose();
            slotSurface.Dispose();

            int slotIndex = 0;
            foreach (var val in availableSlots)
            {
                int col = slotIndex % cols;
                int row = slotIndex / cols;

                double x = col * (unscaledSlotWidth + unscaledSlotPadding);
                double y = row * (unscaledSlotHeight + unscaledSlotPadding);

                ItemSlot slot = inventory[val.Key];

                slotBounds[slotIndex] = ElementBounds.Fixed(x, y, unscaledSlotWidth, unscaledSlotHeight).WithParent(Bounds);
                slotBounds[slotIndex].CalcWorldBounds();

                scissorBounds[slotIndex] = ElementBounds.Fixed(x + 2, y + 2, unscaledSlotWidth - 4, unscaledSlotHeight - 4).WithParent(Bounds);
                scissorBounds[slotIndex].CalcWorldBounds();

                ComposeSlotOverlays(slot, val.Key, slotIndex);

                slotIndex++;
            }
        }


        bool ComposeSlotOverlays(ItemSlot slot, int slotId, int slotIndex)
        {
            if (!availableSlots.ContainsKey(slotId)) return false;
            if (slot.Itemstack == null) return true;

            //int slotIndex = availableSlots.IndexOfKey(slotId);

            bool drawItemDamage = slot.Itemstack.Collectible.ShouldDisplayItemDamage(slot.Itemstack);

            if (!drawItemDamage)
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


            if (drawItemDamage)
            {
                double x = scaled(4);
                double y = (int)slotBounds[slotIndex].InnerHeight - scaled(3) - scaled(4);
                textCtx.SetSourceRGBA(GuiStyle.DialogStrongBgColor);
                double width = (slotBounds[slotIndex].InnerWidth - scaled(8));
                double height = scaled(4);
                RoundRectangle(textCtx, x, y, width, height, 1);
                textCtx.FillPreserve();
                ShadePath(textCtx, 2);


                float[] color = ColorUtil.ToRGBAFloats(slot.Itemstack.Collectible.GetItemDamageColor(slot.Itemstack));
                textCtx.SetSourceRGB(color[0], color[1], color[2]);

                int dura = slot.Itemstack.Collectible.GetDurability(slot.Itemstack);
                float health = (float)slot.Itemstack.Attributes.GetInt("durability", dura) / dura;

                width = health * (slotBounds[slotIndex].InnerWidth - scaled(8));

                RoundRectangle(textCtx, x, y, width, height, 1);
                textCtx.FillPreserve();
                ShadePath(textCtx, 2);
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
            if (prevSlotQuantity != inventory.Count)
            {
                prevSlotQuantity = inventory.Count;
                inventory.DirtySlots.Clear();
                ComposeElements(null, null);
                return;
            }

            if (inventory.DirtySlots.Count == 0) return;

            List<int> handled = new List<int>();

            foreach (int slotId in inventory.DirtySlots)
            {
                ItemSlot slot = inventory[slotId];

                if (ComposeSlotOverlays(slot, slotId, availableSlots.IndexOfKey(slotId)))
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


        ElementBounds scBounds = new ElementBounds();

        public override void RenderInteractiveElements(float deltaTime)
        {
            (inventory as InventoryBase).InvNetworkUtil.AcceptServerUpdates = !isLeftMouseDownStartedInsideElem;

            // Slot sizes
            double absSlotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double absItemstackSize = scaled(GuiElementPassiveItemSlot.unscaledItemSize);

            double offset = absSlotWidth / 2;

            int i = 0;
            foreach (var val in renderedSlots)
            {
                ElementBounds bounds = slotBounds[i];

                // Don't need to render stuff completely outside, saves us many render calls (~down to 100 draw calls instead of 600 for creative inventory)
                if (bounds.PartiallyInside(Bounds.ParentBounds))
                {
                    ItemSlot slot = val.Value;
                    int slotId = val.Key;

                    if (((slot.Itemstack == null || AlwaysRenderIcon) && slot.BackgroundIcon != null)  || slot.HexBackgroundColor != null)
                    {
                        string key = slot.BackgroundIcon + "-" + slot.HexBackgroundColor;
                        api.Render.Render2DTexturePremultipliedAlpha(slotTextureIdsByBgIconAndColor[key], bounds);
                    } else
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(slotTexture.TextureId, bounds);
                    }
                    

                    if (highlightSlotId == slotId || hoverSlotId == slotId || leftMouseDownDistributeSlotsBySlotid.ContainsKey(slotId))
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(
                            highlightSlotTexture.TextureId, (int)(bounds.renderX - 2), (int)(bounds.renderY - 2), bounds.OuterWidthInt + 4, bounds.OuterHeightInt + 4
                        );
                    }

                    if (slot.Itemstack == null) { i++; continue; }

                    
                    float dx = 0;
                    float dy = 0;
                    if (slotNotifiedZoomEffect.ContainsKey(slotId))
                    {
                        dx = 4 * (float)api.World.Rand.NextDouble() - 2f;
                        dy = 4 * (float)api.World.Rand.NextDouble() - 2f;
                    }

                    api.Render.PushScissor(scissorBounds[i], true);

                    api.Render.RenderItemstackToGui(
                        slot,
                        slotBounds[i].renderX + offset + dy,
                        slotBounds[i].renderY + offset + dx,
                        90,
                        (float)(absItemstackSize),
                        ColorUtil.WhiteArgb
                    );

                    api.Render.PopScissor();

                    if (slot.DrawUnavailable)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(crossedOutTexture.TextureId, (int)(bounds.renderX), (int)(bounds.renderY), crossedOutTexture.Width, crossedOutTexture.Height, 250);
                    }

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
            if (hoverSlotId != -1 && inventory[hoverSlotId] != null)
            {
                api.Input.TriggerOnMouseLeaveSlot(inventory[hoverSlotId]);
            }
            hoverSlotId = -1;
            tabbedSlotId = -1;

            (inventory as InventoryBase).InvNetworkUtil.AcceptServerUpdates = true;
        }

        public override int OutlineColor()
        {
            return (255 << 8) + (255 << 24);
        }

        

        /// <summary>
        /// Renders only a subset of all available slots filtered by searching given text on the item name/description
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchCache">Can be set to increase search performance, otherwise a slow search is performed</param>
        public void FilterItemsBySearchText(string text, Dictionary<int, string> searchCache = null, Dictionary<int, string> searchCacheNames = null)
        {
            this.searchText = text.ToLowerInvariant();

            renderedSlots.Clear();

            OrderedDictionary<int, WeightedSlot> wSlots = new OrderedDictionary<int, WeightedSlot>();

            foreach (var val in availableSlots)
            {
                ItemSlot slot = inventory[val.Key];

                if (slot.Itemstack == null) continue;
                if (searchText == null || searchText.Length == 0)
                {
                    renderedSlots.Add(val.Key, slot);
                    continue;
                }

                string cachedtext = "";

                if (searchCache != null && searchCache.TryGetValue(val.Key, out cachedtext))
                {
                    if (searchCacheNames[val.Key].Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        wSlots.Add(val.Key, new WeightedSlot() { slot = slot, weight = 0 });
                        continue;
                    }

                    if (searchCacheNames[val.Key].IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        wSlots.Add(val.Key, new WeightedSlot() { slot = slot, weight = 1 });
                        continue;
                    }


                    if (cachedtext.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        wSlots.Add(val.Key, new WeightedSlot() { slot = slot, weight = 2 });
                        continue;
                    }

                    if (cachedtext.CaseInsensitiveContains(searchText))
                    {
                        wSlots.Add(val.Key, new WeightedSlot() { slot = slot, weight = 3 });
                        continue;
                    }

                } else
                {
                    if (slot.Itemstack.MatchesSearchText(api.World, searchText))
                    {
                        renderedSlots.Add(val.Key, slot);
                    }
                }
            }

            foreach (var pair in wSlots.OrderBy(pair => pair.Value.weight))
            {
                renderedSlots.Add(pair.Key, pair.Value.slot);
            }


            /*var elems = renderedSlots.ToArray().Shuffle(api.World.Rand);
            renderedSlots.Clear();
            foreach (var val in elems)
            {
                renderedSlots[val.Key] = val.Value;
            }*/



            this.rows = (int)Math.Ceiling(1f * renderedSlots.Count / cols);
            ComposeInteractiveElements();
        }



        #endregion

        #region Keyboard, Mouse

        int tabbedSlotId=-1;

        public bool KeyboardControlEnabled = true;

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            base.OnKeyDown(api, args);

            if (!HasFocus) return;

            if (KeyboardControlEnabled)
            {
                if (args.KeyCode == (int)GlKeys.Up)
                {
                    tabbedSlotId = Math.Max(-1, tabbedSlotId - cols);
                    highlightSlotId = tabbedSlotId >= 0 ? renderedSlots.GetKeyAtIndex(tabbedSlotId) : -1;
                }

                if (args.KeyCode == (int)GlKeys.Down)
                {
                    tabbedSlotId = Math.Min(renderedSlots.Count - 1, tabbedSlotId + cols);
                    highlightSlotId = renderedSlots.GetKeyAtIndex(tabbedSlotId);
                }

                if (args.KeyCode == (int)GlKeys.Right)
                {
                    tabbedSlotId = Math.Min(renderedSlots.Count - 1, tabbedSlotId + 1);
                    highlightSlotId = renderedSlots.GetKeyAtIndex(tabbedSlotId);
                }
                if (args.KeyCode == (int)GlKeys.Left)
                {
                    tabbedSlotId = Math.Max(-1, tabbedSlotId - 1);
                    highlightSlotId = tabbedSlotId >= 0 ? renderedSlots.GetKeyAtIndex(tabbedSlotId) : -1;
                }
                if (args.KeyCode == (int)GlKeys.Enter)
                {
                    if (highlightSlotId >= 0)
                    {
                        SlotClick(api, highlightSlotId, EnumMouseButton.Left, true, false, false);
                    }
                }
            }
        }

        public override void OnMouseDown(ICoreClientAPI api, MouseEvent mouse)
        {
            base.OnMouseDown(api, mouse);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

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
                        leftMouseDownDistributeSlotsBySlotid.Add(slotid, inventory[slotid].StackSize);
                    }

                    SlotClick(
                        api, 
                        renderedSlots.GetKeyAtIndex(i), 
                        args.Button, 
                        api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft],
                        api.Input.KeyboardKeyState[(int)GlKeys.LControl],
                        api.Input.KeyboardKeyState[(int)GlKeys.LAlt]
                    );
                    args.Handled = true;

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
            (inventory as InventoryBase).InvNetworkUtil.AcceptServerUpdates = true;
            base.OnMouseUp(api, args);
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y))
            {
                if (hoverSlotId != -1)
                {
                    api.Input.TriggerOnMouseLeaveSlot(inventory[hoverSlotId]);
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
                    ItemSlot newHoverSlot = inventory[newHoverSlotid];
                    ItemStack stack = newHoverSlot.Itemstack;

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
                                RedistributeStacks(newHoverSlotid);
                            }
                        }
                    }



                    if (newHoverSlotid != hoverSlotId && newHoverSlot != null)
                    {
                        api.Input.TriggerOnMouseEnterSlot(newHoverSlot);
                        hoverInv = newHoverSlot.Inventory;
                    }

                    if (newHoverSlotid != hoverSlotId)
                    {
                        tabbedSlotId = -1;
                    }

                    hoverSlotId = newHoverSlotid;
                    return;
                }
            }

            if (hoverSlotId != -1) api.Input.TriggerOnMouseLeaveSlot(inventory[hoverSlotId]);
            hoverSlotId = -1;
        }

        
        public override bool OnMouseLeaveSlot(ICoreClientAPI api, ItemSlot slot)
        {
            if (slot.Inventory == hoverInv)
            {
                hoverSlotId = -1;
            }
            return false;
        }


        private void RedistributeStacks(int intoSlotId)
        {
            int stacksPerSlot = referenceDistributStack.StackSize / leftMouseDownDistributeSlotsBySlotid.Count;
            
            for (int i = 0; i < leftMouseDownDistributeSlotsBySlotid.Count - 1; i++)
            {
                int sourceSlotid = leftMouseDownDistributeSlotsBySlotid.GetKeyAtIndex(i);
                if (sourceSlotid == intoSlotId) continue;

                ItemSlot sourceSlot = inventory[sourceSlotid];

                int beforesize = leftMouseDownDistributeSlotsBySlotid[sourceSlotid];
                int nowsize = sourceSlot.StackSize;

                if (nowsize - beforesize > stacksPerSlot)
                {
                    ItemSlot targetSlot = inventory[intoSlotId];
                    ItemStackMoveOperation op = new ItemStackMoveOperation(api.World, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge);
                    op.ActingPlayer = api.World.Player;
                    op.RequestedQuantity = nowsize - beforesize - stacksPerSlot;

                    object packet = api.World.Player.InventoryManager.TryTransferTo(sourceSlot, targetSlot, ref op);

                    if (packet != null)
                    {
                        SendPacketHandler(packet);
                    }
                }
            }
        }

        public virtual void SlotClick(ICoreClientAPI api, int slotId, EnumMouseButton mouseButton, bool shiftPressed, bool ctrlPressed, bool altPressed)
        {
            List<IInventory> inventories = api.World.Player.InventoryManager.OpenedInventories;
            IInventory mouseCursorInv = api.World.Player.InventoryManager.GetOwnInventory(GlobalConstants.mousecursorInvClassName);
            object packet;

            EnumModifierKey modifiers =
                (shiftPressed ? EnumModifierKey.SHIFT : 0) |
                (ctrlPressed ? EnumModifierKey.CTRL : 0) |
                (altPressed ? EnumModifierKey.ALT : 0)
            ;

            ItemStackMoveOperation op = new ItemStackMoveOperation(api.World, mouseButton, modifiers, EnumMergePriority.AutoMerge);
            op.ActingPlayer = api.World.Player;

            if (shiftPressed)
            {
                ItemSlot sourceSlot = inventory[slotId];
                op.RequestedQuantity = sourceSlot.StackSize;
                packet = inventory.ActivateSlot(slotId, sourceSlot, ref op);
            }
            else
            {
                op.CurrentPriority = EnumMergePriority.DirectMerge;
                packet = inventory.ActivateSlot(slotId, mouseCursorInv[0], ref op);
            }

            if (packet != null)
            {
                if (packet is object[] packets)
                {
                    for (int i = 0; i < packets.Length; i++)
                    {
                        SendPacketHandler(packets[i]);
                    }
                } else
                {
                    SendPacketHandler?.Invoke(packet);
                }
                
            }

            api.Input.TriggerOnMouseClickSlot(inventory[slotId]);
        }

        #endregion

        /// <summary>
        /// Highlights a specific slot.
        /// </summary>
        /// <param name="slotId">The slot to highlight.</param>
        public void HighlightSlot(int slotId)
        {
            highlightSlotId = slotId;
        }

        /// <summary>
        /// Removes the active slot highlight.
        /// </summary>
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
            crossedOutTexture?.Dispose();
        }

        
    }
}
