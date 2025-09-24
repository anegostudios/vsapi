using System;
using System.Collections.Generic;
using System.Text;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public abstract class GuiElementEditableTextBase : GuiElementTextBase
    {
        public delegate bool OnTryTextChangeDelegate(List<string> lines);
        private const int DoubleClickMilliseconds = 400;

        internal float[] caretColor = new float[] { 1, 1, 1, 1 };

        internal bool hideCharacters;
        internal bool multilineMode;
        internal int maxlines = 99999;
        internal int maxlength = -1;

        internal double caretX, caretY;
        internal double topPadding;
        internal double leftPadding = 3;
        internal double rightSpacing;
        internal double bottomSpacing;

        private int? selectedTextStart;
        private long lastClickTime;
        private int lastClickCursor;
        private bool handlingOnKeyEvent;
        private bool mouseDown;

        internal LoadedTexture caretTexture;
        internal LoadedTexture textTexture;
        private int selectionTextureId;

        public Action<int, int> OnCaretPositionChanged;
        public Action<string> OnTextChanged;
        public OnTryTextChangeDelegate OnTryTextChangeText;
        public Action<double, double> OnCursorMoved;

        internal Action OnFocused = null;
        internal Action OnLostFocus = null;

        /// <summary>
        /// Called when a keyboard key was pressed, received and handled
        /// </summary>
        public Action OnKeyPressed;


        internal long caretBlinkMilliseconds;
        internal bool caretDisplayed;
        internal double caretHeight;

        internal double renderLeftOffset;
        internal Vec2i textSize = new Vec2i();

        protected List<string> lines;
        /// <summary>
        /// Contains the same as Lines, but may momentarily have different values when an edit is being made
        /// </summary>
        protected List<string> linesStaging;

        public bool WordWrap = true;


        public List<string> GetLines() => new List<string>(lines);

        public int TextLengthWithoutLineBreaks {
            get {
                int length = 0;
                for (int i = 0; i < lines.Count; i++) length += lines[i].Length;
                return length;
            }
        }

        public int CaretPosWithoutLineBreaks
        {
            get
            {
                int pos = 0;
                for (int i = 0; i < CaretPosLine; i++) pos += lines[i].Length;
                return pos + CaretPosInLine;
            }
            set
            {
                int sum = 0;
                for (int i = 0; i < lines.Count; i++) {
                    int len = lines[i].Length;

                    if (sum + len > value)
                    {
                        SetCaretPos(value - sum, i);
                        return;
                    }

                    sum += len;
                }

                if (!multilineMode) SetCaretPos(sum, 0);
                //else SetCaretPos(value - sum, Lines.Count); - why value-sum? that makes no sense
                else SetCaretPos(sum, lines.Count);
            }
        }

        protected int pcaretPosLine;
        protected int pcaretPosInLine;
        public int CaretPosLine {
            get
            {
                return pcaretPosLine;
            }
            set
            {
                pcaretPosLine = value;
            }

        }
        public int CaretPosInLine
        {
            get { return pcaretPosInLine; }
            set {
                if (value > lines[CaretPosLine].Length) throw new IndexOutOfRangeException("Caret @"+value+", cannot beyond current line length of " + pcaretPosInLine);
                pcaretPosInLine = value; 
            }
        }

        public override bool Focusable
        {
            get { return enabled; }
        }

        /// <summary>
        /// Initializes the text component.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the component.</param>
        public GuiElementEditableTextBase(ICoreClientAPI capi, CairoFont font, ElementBounds bounds) : base(capi, "", font, bounds)
        {
            caretTexture = new LoadedTexture(capi);
            textTexture = new LoadedTexture(capi);
            selectionTextureId = GenerateSelectionTexture(capi);

            lines = new List<string> { "" };
            linesStaging = new List<string> { "" };
        }

        public override void OnFocusGained()
        {
            base.OnFocusGained();
            OnFocused?.Invoke();
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
            selectedTextStart = null;
            OnLostFocus?.Invoke();
        }

        /// <summary>
        /// Sets the position of the cursor at a given point.
        /// </summary>
        /// <param name="x">X position of the cursor.</param>
        /// <param name="y">Y position of the cursor.</param>
        public void SetCaretPos(double x, double y)
        {
            CaretPosLine = 0;

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = genContext(surface);
            Font.SetupContext(ctx);

            if (multilineMode)
            {
                double lineY = y / ctx.FontExtents.Height;
                if (lineY > lines.Count)
                {
                    CaretPosLine = lines.Count - 1;
                    CaretPosInLine = lines[CaretPosLine].Length;

                    ctx.Dispose();
                    surface.Dispose();
                    return;
                }

                CaretPosLine = Math.Max(0, (int)lineY);
            }

            CaretPosLine = Math.Clamp(CaretPosLine, 0, lines.Count - 1);
            string line = lines[CaretPosLine].TrimEnd('\r', '\n');
            CaretPosInLine = line.Length;

            for (int i = 0; i < line.Length; i++)
            {
                double posx = ctx.TextExtents(line.Substring(0, i+1)).XAdvance;

                if (x - posx <= 0)
                {
                    CaretPosInLine = i;
                    break;
                }
            }

            ctx.Dispose();
            surface.Dispose();

            SetCaretPos(CaretPosInLine, CaretPosLine);
        }


       
        /// <summary>
        /// Sets the position of the cursor to a specific character.
        /// </summary>
        /// <param name="posInLine">The position in the line.</param>
        /// <param name="posLine">The line of the text.</param>
        public void SetCaretPos(int posInLine, int posLine = 0)
        {
            caretBlinkMilliseconds = api.ElapsedMilliseconds;
            caretDisplayed = true;

            CaretPosLine = GameMath.Clamp(posLine, 0, lines.Count - 1);
            CaretPosInLine = GameMath.Clamp(posInLine, 0, lines[CaretPosLine].TrimEnd('\r', '\n').Length);


            if (multilineMode)
            {
                caretX = Font.GetTextExtents(lines[CaretPosLine].Substring(0, CaretPosInLine)).XAdvance;
                caretY = Font.GetFontExtents().Height * CaretPosLine;
            }
            else
            {
                string displayedText = lines[0];

                if (hideCharacters)
                {
                    displayedText = new StringBuilder(lines[0]).Insert(0, "•", displayedText.Length).ToString();
                }

                caretX = Font.GetTextExtents(displayedText.Substring(0, CaretPosInLine)).XAdvance;
                caretY = 0;
            }

            OnCursorMoved?.Invoke(caretX, caretY);

            renderLeftOffset = Math.Max(0, caretX - Bounds.InnerWidth + rightSpacing);

            OnCaretPositionChanged?.Invoke(posLine, posInLine);
        }

        /// <summary>
        /// Sets a numerical value to the text, appending it to the end of the text.
        /// </summary>
        /// <param name="value">The value to add to the text.</param>
        public void SetValue(float value)
        {
            SetValue(value.ToString(GlobalConstants.DefaultCultureInfo));
        }

        /// <summary>
        /// Sets a numerical value to the text, appending it to the end of the text.
        /// </summary>
        /// <param name="value">The value to add to the text.</param>
        public void SetValue(double value)
        {
            SetValue(value.ToString(GlobalConstants.DefaultCultureInfo));
        }

        /// <summary>
        /// Sets given text, sets the cursor to the end of the text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="setCaretPosToEnd"></param>
        public void SetValue(string text, bool setCaretPosToEnd = true)
        {
            LoadValue(Lineize(text));

            if (setCaretPosToEnd)
            {
                var endLine = lines[lines.Count - 1];
                var endPos = endLine.Length;
                SetCaretPos(endPos, lines.Count - 1);
            }
        }

        /// <summary>
        /// Sets given texts, leaves cursor position unchanged unless it's now invalid
        /// </summary>
        /// <param name="newLines"></param>
        public virtual void LoadValue(List<string> newLines)
        {
            // Disallow edit if prevent by event or if it adds another line beyond max lines
            if (OnTryTextChangeText?.Invoke(newLines) == false || (newLines.Count > maxlines && newLines.Count >= lines.Count))
            {
                // Revert edits
                linesStaging = new List<string>(lines);
                return;
            }

            lines = new List<string>(newLines);
            linesStaging = new List<string>(lines);

            CaretPosLine = Math.Clamp(CaretPosLine, 0, lines.Count - 1);
            CaretPosInLine = Math.Clamp(CaretPosInLine, 0, lines[CaretPosLine].Length);

            //RecomposeText(); - wtf is this here for, its alread called in TextChanged()
            TextChanged();
        }

        public List<string> Lineize(string text)
        {
            if (text == null || maxlength == 0) text = "";
            if (maxlength > 0 && text.Length > maxlength) text = text.Substring(0, maxlength);

            List<string> lines = new List<string>();

            // We only allow Linux style newlines (only \n)
            text = text.Replace("\r\n", "\n").Replace('\r', '\n');

            if (multilineMode)
            {
                double boxWidth = Bounds.InnerWidth - 2 * Bounds.absPaddingX;
                if (!WordWrap) boxWidth = 999999;

                TextLine[] textlines = textUtil.Lineize(Font, text, boxWidth, EnumLinebreakBehavior.Default, true);
                foreach (var val in textlines) lines.Add(val.Text);

                if (lines.Count == 0)
                {
                    lines.Add("");
                }
            }
            else
            {
                lines.Add(text);
            }

            return lines;
        }


        internal virtual void TextChanged()
        {
            OnTextChanged?.Invoke(string.Join("", lines));
            if (!handlingOnKeyEvent) selectedTextStart = null;
            RecomposeText();
        }

        internal virtual void RecomposeText()
        {
            Bounds.CalcWorldBounds();

            string displayedText = null;
            ImageSurface surface;

            if (multilineMode) {
                textSize.X = (int)(Bounds.OuterWidth - rightSpacing);
                textSize.Y = (int)(Bounds.OuterHeight - bottomSpacing);
                
            } else {
                displayedText = lines[0];

                if (hideCharacters)
                {
                    displayedText = new StringBuilder(displayedText.Length).Insert(0, "•", displayedText.Length).ToString();
                }

                textSize.X = (int)Math.Max(Bounds.InnerWidth - rightSpacing, Font.GetTextExtents(displayedText).Width);
                textSize.Y = (int)(Bounds.InnerHeight - bottomSpacing);
            }

            surface = new ImageSurface(Format.Argb32, textSize.X, textSize.Y);

            Context ctx = genContext(surface);
            Font.SetupContext(ctx);

            double fontHeight = ctx.FontExtents.Height;
            
            if (multilineMode)
            {
                double width = Bounds.InnerWidth - 2 * Bounds.absPaddingX - rightSpacing;

                TextLine[] textlines = new TextLine[lines.Count];
                for (int i = 0; i < textlines.Length; i++)
                {
                    textlines[i] = new TextLine()
                    {
                        Text = lines[i].Replace("\r\n", "").Replace("\n", ""),
                        Bounds = new LineRectangled(0, i*fontHeight, Bounds.InnerWidth, fontHeight)
                    };
                }

                textUtil.DrawMultilineTextAt(ctx, Font, textlines, Bounds.absPaddingX + leftPadding, Bounds.absPaddingY, width, EnumTextOrientation.Left);
            } else
            {
                this.topPadding = Math.Max(0, Bounds.OuterHeight - bottomSpacing - ctx.FontExtents.Height) / 2;
                textUtil.DrawTextLine(ctx, Font, displayedText, Bounds.absPaddingX + leftPadding, Bounds.absPaddingY + this.topPadding);
            }


            generateTexture(surface, ref textTexture);
            ctx.Dispose();
            surface.Dispose();

            if (caretTexture.TextureId == 0)
            {
                caretHeight = fontHeight;
                surface = new ImageSurface(Format.Argb32, (int)3.0, (int)fontHeight);
                ctx = genContext(surface);
                Font.SetupContext(ctx);

                ctx.SetSourceRGBA(caretColor[0], caretColor[1], caretColor[2], caretColor[3]);
                ctx.LineWidth = 1;
                ctx.NewPath();
                ctx.MoveTo(2, 0);
                ctx.LineTo(2, fontHeight);
                ctx.ClosePath();
                ctx.Stroke();

                generateTexture(surface, ref caretTexture.TextureId);

                ctx.Dispose();
                surface.Dispose();
            }
        }

        private void DeleteSelectedText(int caretPos, int caretPosOffset)
        {
            var textSelection = GetSelection(caretPos);
            SetValue(GetText().Remove(textSelection.Start, textSelection.End - textSelection.Start), false);
            selectedTextStart = null;
            if (caretPos == textSelection.End) CaretPosWithoutLineBreaks = textSelection.Start + caretPosOffset;
        }

        static int GetCharRank(char c)
        {
            if (IsWordChar(c)) return 3;
            if (!char.IsWhiteSpace(c)) return 2;
            return 1;
        }

        private void SelectWordAtCursor()
        {
            string line = lines[CaretPosLine];
            int caretPosInLine = CaretPosInLine;
            int targetRank = caretPosInLine > 0 ? GetCharRank(line[caretPosInLine - 1]) : 0;
            if (caretPosInLine < line.Length) targetRank = Math.Max(targetRank, GetCharRank(line[caretPosInLine]));

            int start = caretPosInLine;
            while(start > 0 && GetCharRank(line[start - 1]) == targetRank) --start;

            int end = caretPosInLine;
            while(end < line.Length && GetCharRank(line[end]) == targetRank) ++end;

            if (start == caretPosInLine && end == caretPosInLine) return;
            selectedTextStart = CaretPosWithoutLineBreaks - (caretPosInLine - start);
            SetCaretPos(end, CaretPosLine);
        }

        struct TextSelection
        {
            internal int Start;
            internal int End;

            internal TextSelection(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        private TextSelection GetSelection(int caretPos)
        {
            return new(Math.Min(selectedTextStart.Value, caretPos), Math.Max(selectedTextStart.Value, caretPos));
        }


        #region Mouse, Keyboard


        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);
            if (args.Button != EnumMouseButton.Left) return;

            bool shiftDown = new Func<bool>(() => api.Input.KeyboardKeyStateRaw[(int)GlKeys.ShiftLeft] || api.Input.KeyboardKeyStateRaw[(int)GlKeys.ShiftRight])();
            if (shiftDown && selectedTextStart == null) selectedTextStart = CaretPosWithoutLineBreaks;
            SetCaretPos(args.X - Bounds.absX, args.Y - Bounds.absY);

            if (!shiftDown)
            {
                long now = api.ElapsedMilliseconds;
                int caretPos = CaretPosWithoutLineBreaks;
                bool isDoubleClick = lastClickCursor == caretPos && now - lastClickTime < DoubleClickMilliseconds;
                lastClickTime = now;
                lastClickCursor = caretPos;

                if(isDoubleClick)
                {
                    SelectWordAtCursor();
                    return;
                }
                else selectedTextStart = caretPos;
            }
            mouseDown = true;
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseMove(api, args);
            if (mouseDown) SetCaretPos((double)args.X - Bounds.absX, (double)args.Y - Bounds.absY);
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseUp(api, args);
            if (args.Button != EnumMouseButton.Left) return;

            mouseDown = false;
            if (selectedTextStart == CaretPosWithoutLineBreaks) selectedTextStart = null;
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;

            handlingOnKeyEvent = true;
            OnKeyDownInternal(api, args);
            handlingOnKeyEvent = false;
        }

        private void OnKeyDownInternal(ICoreClientAPI api, KeyEvent args)
        {
            if (args.AltPressed) // Ignore all inputs if alt is held down as we don't use alt for any of our shortcuts
            {
                args.Handled = true;
                return;
            }

            if ((args.CtrlPressed || args.CommandPressed) && OnControlAction(args))
            {
                api.Gui.PlaySound("tick");
                args.Handled = true;
                return;
            }
            
            bool handled = multilineMode || args.KeyCode != (int)GlKeys.Tab;

            if (args.KeyCode is (int)GlKeys.BackSpace or (int)GlKeys.Delete)
            {
                if (args.CtrlPressed && OnDeleteWord(args.KeyCode == (int)GlKeys.BackSpace ? -1 : 1));
                else if (selectedTextStart == null)
                {
                    if (args.KeyCode == (int)GlKeys.BackSpace) OnKeyBackSpace();
                    else OnKeyDelete();
                }
                else
                {
                    DeleteSelectedText(CaretPosWithoutLineBreaks, 0);
                    api.Gui.PlaySound("tick");
                }
                args.Handled = true;
                return;
            }

            if (args.ShiftPressed != selectedTextStart.HasValue && (args.KeyCode is (int)GlKeys.Up or (int)GlKeys.Down or (int)GlKeys.Left or (int)GlKeys.Right or (int)GlKeys.Home or (int)GlKeys.End))
            {
                if (!args.CtrlPressed && selectedTextStart.HasValue)
                {
                    int caretPos = CaretPosWithoutLineBreaks;
                    if (selectedTextStart < caretPos && args.KeyCode is (int)GlKeys.Up or (int)GlKeys.Left or (int)GlKeys.Home ||
                        selectedTextStart > caretPos && args.KeyCode is (int)GlKeys.Down or (int)GlKeys.Right or (int)GlKeys.End)
                    {
                        CaretPosWithoutLineBreaks = selectedTextStart.Value;
                    }
                    if (args.KeyCode is (int)GlKeys.Left or (int)GlKeys.Right)
                    {
                        selectedTextStart = null;
                        args.Handled = true;
                        api.Gui.PlaySound("tick");
                        return;
                    }
                }
                selectedTextStart = args.ShiftPressed ? CaretPosWithoutLineBreaks : null;
            }

            if (args.KeyCode == (int)GlKeys.End)
            {
                if (args.CtrlPressed)
                {
                    SetCaretPos(lines[lines.Count - 1].TrimEnd('\r', '\n').Length, lines.Count - 1);
                }
                else
                {
                    SetCaretPos(lines[CaretPosLine].TrimEnd('\r', '\n').Length, CaretPosLine);
                }

                api.Gui.PlaySound("tick");
            }

            if (args.KeyCode == (int)GlKeys.Home)
            {
                if (args.CtrlPressed)
                {
                    SetCaretPos(0);
                }
                else
                {
                    SetCaretPos(0, CaretPosLine);
                }
                    
                api.Gui.PlaySound("tick");
            }

            if (args.KeyCode == (int)GlKeys.Left)
            {
                MoveCursor(-1, args.CtrlPressed);
            }

            if (args.KeyCode == (int)GlKeys.Right)
            {
                MoveCursor(1, args.CtrlPressed);
            }

            if (args.KeyCode == (int)GlKeys.Down && CaretPosLine < lines.Count - 1)
            {
                SetCaretPos(CaretPosInLine, CaretPosLine + 1);
                api.Gui.PlaySound("tick");
            }

            if (args.KeyCode == (int)GlKeys.Up && CaretPosLine > 0)
            {
                SetCaretPos(CaretPosInLine, CaretPosLine - 1);
                api.Gui.PlaySound("tick");
            }

            if (!mouseDown && selectedTextStart == CaretPosWithoutLineBreaks) selectedTextStart = null;

            if (args.KeyCode is (int)GlKeys.Enter or (int)GlKeys.KeypadEnter)
            {
                if (multilineMode)
                {
                    OnKeyEnter();
                }
                else
                {
                    handled = false;
                }
            }

            if (args.KeyCode == (int)GlKeys.Escape) handled = false;

            args.Handled = handled;
        }


        public override string GetText()
        {
            return string.Join("", lines);
        }

        private bool OnControlAction(KeyEvent args)
        {
            string keyString = GlKeyNames.GetPrintableChar(args.KeyCode); // we want layout-independent keys
            if (keyString == "a")
            {
                selectedTextStart = 0;
                SetCaretPos(lines[^1].Length, lines.Count - 1);
                return true;
            }
            if (keyString == "c") return OnCopyCut(CopyCutMode.Copy);
            if (keyString == "x") return OnCopyCut(CopyCutMode.Cut);
            if (keyString == "v") return OnPaste();
            return false;
        }

        private enum CopyCutMode
        {
            Copy,
            Cut
        }
        private bool OnCopyCut(CopyCutMode mode) {
            if (selectedTextStart == null) return false;

            string text = GetText();
            int caretPos = CaretPosWithoutLineBreaks;
            var textSelection = GetSelection(caretPos);
            string subtext = text[textSelection.Start..textSelection.End];
            if (subtext.Length != 0) api.Forms.SetClipboardText(subtext);
            if (mode == CopyCutMode.Cut) DeleteSelectedText(caretPos, 0);
            return true;
        }

        private bool OnPaste()
        {
            if (selectedTextStart != null) DeleteSelectedText(CaretPosWithoutLineBreaks, 0);

            string insert = api.Forms.GetClipboardText();
            insert = insert.Replace("\uFEFF", ""); // UTF-8 bom, we don't need that one, like ever

            string fulltext = string.Join("", lines);
            int caretPos = CaretPosWithoutLineBreaks;
            SetValue(fulltext[..caretPos] + insert + fulltext[caretPos..], false);
            CaretPosWithoutLineBreaks = caretPos + insert.Length;
            return true;
        }

        private bool OnDeleteWord(int direction)
        {
            if (selectedTextStart != null) return false;

            selectedTextStart = CaretPosWithoutLineBreaks;
            MoveCursor(direction, true, true);
            DeleteSelectedText(CaretPosWithoutLineBreaks, 0);
            return true;
        }

        private void OnKeyEnter()
        {
            if (selectedTextStart != null) DeleteSelectedText(CaretPosWithoutLineBreaks, 0);
            if (lines.Count >= maxlines) return;

            string leftText = linesStaging[CaretPosLine].Substring(0, CaretPosInLine);
            string rightText = linesStaging[CaretPosLine].Substring(CaretPosInLine);

            linesStaging[CaretPosLine] = leftText + "\n";
            linesStaging.Insert(CaretPosLine + 1, rightText);

            if (OnTryTextChangeText?.Invoke(linesStaging) == false) return;

            lines = new List<string>(linesStaging);

            TextChanged();
            SetCaretPos(0, CaretPosLine + 1);
            api.Gui.PlaySound("tick");
        }

        private void OnKeyDelete()
        {
            string alltext = GetText();
            var caret = CaretPosWithoutLineBreaks;
            if (alltext.Length == caret) return;

            alltext = alltext.Substring(0, caret) + alltext.Substring(caret + 1, alltext.Length - caret - 1);
            LoadValue(Lineize(alltext));
            api.Gui.PlaySound("tick");
        }

        private void OnKeyBackSpace()
        {
            var caret = CaretPosWithoutLineBreaks;
            if (caret == 0) return;

            string alltext = GetText();
            alltext = alltext.Substring(0, caret - 1) + alltext.Substring(caret, alltext.Length - caret);
            var cpos = CaretPosWithoutLineBreaks;
            LoadValue(Lineize(alltext));
            if (cpos > 0)
            {
                CaretPosWithoutLineBreaks = cpos-1;
            }            

            api.Gui.PlaySound("tick");
        }

        public override void OnKeyPress(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            if (isLengthCapped()) return;

            if (selectedTextStart != null) DeleteSelectedText(CaretPosWithoutLineBreaks, 0);

            string newline = lines[CaretPosLine].Substring(0, CaretPosInLine) + args.KeyChar + lines[CaretPosLine].Substring(CaretPosInLine, lines[CaretPosLine].Length - CaretPosInLine);
            double width = Bounds.InnerWidth - 2 * Bounds.absPaddingX - rightSpacing;
            linesStaging[CaretPosLine] = newline;

            if (multilineMode)
            {
                var textExts = Font.GetTextExtents(newline.TrimEnd('\r', '\n'));
                bool lineOverFlow = textExts.Width >= width;
                if (lineOverFlow)
                {
                    StringBuilder newLines = new StringBuilder();
                    for (int i = 0; i < lines.Count; i++) newLines.Append(i == CaretPosLine ? newline : lines[i]);

                    linesStaging = Lineize(newLines.ToString());

                    if (lines.Count >= maxlines && linesStaging.Count >= maxlines) return;
                }
            }

            handlingOnKeyEvent = true;
            LoadValue(linesStaging); // Ensures word wrapping
            CaretPosWithoutLineBreaks++;
            handlingOnKeyEvent = false;

            args.Handled = true;
            api.Gui.PlaySound("tick");
            OnKeyPressed?.Invoke();
        }

        private bool isLengthCapped()
        {
            if (maxlength < 0) return false;
            int len = 0;
            foreach (var line in lines) len += line.Length;
            return len >= maxlength;
        }

        #endregion


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (!HasFocus) return;
            
            if (api.ElapsedMilliseconds - caretBlinkMilliseconds > 900)
            {
                caretBlinkMilliseconds = api.ElapsedMilliseconds;
                caretDisplayed = !caretDisplayed;
            }

            if (caretDisplayed && caretX - renderLeftOffset < Bounds.InnerWidth)
            {
                api.Render.Render2DTexturePremultipliedAlpha(caretTexture.TextureId, Bounds.renderX + caretX + scaled(1.5) - renderLeftOffset, Bounds.renderY + caretY + topPadding, 2, caretHeight);
            }
        }

        // Children are responsible for calling it because it should be behind text.
        protected void RenderTextSelection()
        {
            if (selectedTextStart == null) return;

            var textSelection = GetSelection(CaretPosWithoutLineBreaks);
            var start = GetPosition(textSelection.Start);
            var end = GetPosition(textSelection.End);

            if (start.Y == end.Y) RenderSelectionLine(start.X, end.X, start.Y);
            else
            {
                RenderSelectionLine(start.X, -1, start.Y);
                for (int lineIndex = start.Y + 1; lineIndex < end.Y; ++lineIndex) RenderSelectionLine(0, -1, lineIndex);
                RenderSelectionLine(0, end.X, end.Y);
            }
        }

        void RenderSelectionLine(int fromX, int toX, int lineIndex)
        {
            double renderX = Bounds.renderX + leftPadding;
            double renderY = Bounds.renderY + topPadding;
            double height = Font.GetFontExtents().Height;

            double x = renderX + (fromX == 0 ? 0 : Font.GetTextExtents(lines[lineIndex][..fromX]).XAdvance);
            double y = renderY + height * lineIndex;
            double width = Font.GetTextExtents(lines[lineIndex].Substring(fromX, (toX == -1 ? lines[lineIndex].Length : toX) - fromX)).XAdvance;
            api.Render.Render2DTexturePremultipliedAlpha(selectionTextureId, x - renderLeftOffset, y, width, height);
        }

        struct SelectedTextPos
        {
            internal int X;
            internal int Y;

            internal SelectedTextPos (int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        SelectedTextPos GetPosition(int positionWithoutLineBreaks)
        {
            int linePos = 0;
            foreach (string line in lines)
            {
                if (positionWithoutLineBreaks > line.Length)
                {
                    ++linePos;
                    positionWithoutLineBreaks -= line.Length;
                }
                else break;
            }
            return new(positionWithoutLineBreaks, linePos);
        }

        public override void Dispose()
        {
            base.Dispose();

            caretTexture.Dispose();
            textTexture.Dispose();
            if (selectionTextureId != 0)
            {
                api.Gui.DeleteTexture(selectionTextureId);
                selectionTextureId = 0;
            }
        }


        /// <summary>
        /// Moves the cursor forward and backward by an amount.
        /// </summary>
        /// <param name="dir">The direction to move the cursor.</param>
        /// <param name="wholeWord">Whether or not we skip entire words moving it.</param>
        /// <param name="wholeWordWithWhitespace">Force the cursor to skip whitespace after a word.</param>
        public void MoveCursor(int dir, bool wholeWord = false, bool wholeWordWithWhitespace = false)
        {
            bool moved = 
                ((CaretPosInLine > 0 || CaretPosLine > 0) && dir < 0) ||
                ((CaretPosInLine < lines[CaretPosLine].Length || CaretPosLine < lines.Count-1) && dir > 0)
            ;

            if (wholeWord)
            {
                dir = dir < 0 ? -1 : 1;
                string text = GetText();
                int stop = dir < 0 ? -1 : text.Length;
                int offset = dir < 0 ? -1 : 0;
                int caretPos = CaretPosWithoutLineBreaks + offset;
                bool startsWithWhitespace = caretPos != stop && char.IsWhiteSpace(text[caretPos]);
                while (caretPos != stop && char.IsWhiteSpace(text[caretPos])) caretPos += dir;

                if (caretPos != stop && !IsWordChar(text[caretPos]))
                {
                    while (caretPos != stop && !IsWordChar(text[caretPos]) && !char.IsWhiteSpace(text[caretPos])) caretPos += dir;
                }
                else
                {
                    while (caretPos != stop && IsWordChar(text[caretPos])) caretPos += dir;
                }

                if (!startsWithWhitespace && wholeWordWithWhitespace)
                {
                    while (caretPos != stop && char.IsWhiteSpace(text[caretPos])) caretPos += dir;
                }

                CaretPosWithoutLineBreaks = caretPos - offset;
            }
            else CaretPosWithoutLineBreaks += dir;

            if (moved) api.Gui.PlaySound("tick");
        }



        /// <summary>
        /// Sets the number of lines in the Text Area.
        /// </summary>
        /// <param name="maxlines">The maximum number of lines.</param>
        public void SetMaxLines(int maxlines)
        {
            this.maxlines = maxlines;
        }

        /// <summary>
        /// Limits the number of characters a user may enter (default: no limit). Use -1 to set no limit
        /// </summary>
        /// <param name="maxlength"></param>
        public void SetMaxLength(int maxlength)
        {
            this.maxlength = maxlength;
        }


        public void SetMaxHeight(int maxheight)
        {
            var fontExt = Font.GetFontExtents();
            this.maxlines = (int)Math.Floor(maxheight / fontExt.Height);
        }

        private static int GenerateSelectionTexture(ICoreClientAPI api) {
            using ImageSurface surface = new(Format.Argb32, 32, 32);
            using Context context = new(surface);
            context.SetSourceRGBA(0, 0.75, 1, 0.5);
            context.Paint();
            return api.Gui.LoadCairoTexture(surface, true);
        }

        private static bool IsWordChar(char c)
        {
            return c == '_' || char.IsLetterOrDigit(c);
        }
    }

}
