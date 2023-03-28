using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public abstract class GuiElementEditableTextBase : GuiElementTextBase
    {
        public delegate bool OnTryTextChangeDelegate(List<string> lines);

        internal float[] caretColor = new float[] { 1, 1, 1, 1 };

        internal bool hideCharacters;
        internal bool multilineMode;
        internal int maxlines = 99999;


        internal double caretX, caretY;
        internal double topPadding;
        internal double leftPadding = 3;
        internal double rightSpacing;
        internal double bottomSpacing;

       // internal int selectedTextStart;
        //internal int selectedTextEnd;

        internal LoadedTexture caretTexture;
        internal LoadedTexture textTexture;
        //internal int selectionTextureId;

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
            get { return true; }
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

            lines = new List<string> { "" };
            linesStaging = new List<string> { "" };
        }

        public override void OnFocusGained()
        {
            base.OnFocusGained();
            SetCaretPos(TextLengthWithoutLineBreaks);
            OnFocused?.Invoke();
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
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
        /// Sets given text, sets the cursor to the end of the text
        /// </summary>
        /// <param name="text"></param>
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
        /// Sets given texts, leaves cursor position unchanged
        /// </summary>
        /// <param name="text"></param>
        public void LoadValue(List<string> newLines)
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

            RecomposeText();
            TextChanged();
        }

        public List<string> Lineize(string text)
        {
            if (text == null) text = "";

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


        #region Mouse, Keyboard


        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            SetCaretPos(args.X - Bounds.absX, args.Y - Bounds.absY);
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            
            bool handled = multilineMode || args.KeyCode != (int)GlKeys.Tab;

            if (args.KeyCode == (int)GlKeys.BackSpace)
            {
                if (CaretPosWithoutLineBreaks > 0) OnKeyBackSpace();
            }

            if (args.KeyCode == (int)GlKeys.Delete)
            {
                if (CaretPosWithoutLineBreaks < TextLengthWithoutLineBreaks) OnKeyDelete();
            }

            if (args.KeyCode == (int)GlKeys.End)
            {
                if (args.CtrlPressed)
                {
                    SetCaretPos(lines[lines.Count - 1].TrimEnd('\r', '\n').Length, lines.Count - 1);
                } else
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
                } else
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

            if (args.KeyCode == (int)GlKeys.V && (args.CtrlPressed || args.CommandPressed))
            {
                string insert = api.Forms.GetClipboardText();
                insert = insert.Replace("\uFEFF", ""); // UTF-8 bom, we don't need that one, like ever

                string fulltext = string.Join("\n", lines);

                int caretPos = CaretPosInLine;
                for (int i = 0; i < CaretPosLine; i++)
                {
                    caretPos += lines[i].Length + 1;
                }

                SetValue(fulltext.Substring(0, caretPos) + insert + fulltext.Substring(caretPos, fulltext.Length - caretPos));
                api.Gui.PlaySound("tick");
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

            if (args.KeyCode == (int)GlKeys.Enter || args.KeyCode == (int)GlKeys.KeypadEnter)
            {
                if (multilineMode)
                {
                    OnKeyEnter();
                } else
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

        private void OnKeyEnter()
        {
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
            string newline = lines[CaretPosLine].Substring(0, CaretPosInLine) + args.KeyChar + lines[CaretPosLine].Substring(CaretPosInLine, lines[CaretPosLine].Length - CaretPosInLine);
            double width = Bounds.InnerWidth - 2 * Bounds.absPaddingX - rightSpacing;

            if (multilineMode)
            {
                var textExts = Font.GetTextExtents(newline.TrimEnd('\r', '\n'));
                bool lineOverFlow = textExts.Width >= width;
                if (lineOverFlow)
                {
                    StringBuilder newLines = new StringBuilder();
                    for (int i = 0; i < lines.Count; i++) newLines.Append(i == CaretPosLine ? newline : lines[i]);

                    if (lines.Count >= maxlines && Lineize(newLines.ToString()).Count >= maxlines) return;
                }
            }

            linesStaging[CaretPosLine] = newline;

            var cpos = CaretPosWithoutLineBreaks;
            LoadValue(linesStaging); // Ensures word wrapping
            CaretPosWithoutLineBreaks = cpos + 1;

            args.Handled = true;
            api.Gui.PlaySound("tick");

            OnKeyPressed?.Invoke();
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

        public override void Dispose()
        {
            base.Dispose();

            caretTexture.Dispose();
            textTexture.Dispose();
        }


        /// <summary>
        /// Moves the cursor forward and backward by an amount.
        /// </summary>
        /// <param name="dir">The direction to move the cursor.</param>
        /// <param name="wholeWord">Whether or not we skip entire words moving it.</param>
        public void MoveCursor(int dir, bool wholeWord = false)
        {
            bool done = false;
            bool moved = 
                ((CaretPosInLine > 0 || CaretPosLine > 0) && dir < 0) ||
                ((CaretPosInLine < lines[CaretPosLine].Length || CaretPosLine < lines.Count-1) && dir > 0)
            ;

            int newPos = CaretPosInLine;
            int newLine = CaretPosLine;

            while (!done) {
                newPos += dir;

                if (newPos < 0)
                {
                    if (newLine <= 0) break;
                    newLine--;
                    newPos = lines[newLine].TrimEnd('\r', '\n').Length;
                } 

                if (newPos > lines[newLine].TrimEnd('\r', '\n').Length)
                {
                    if (newLine >= lines.Count - 1) break;
                    newPos = 0;
                    newLine++;
                }

                done = !wholeWord || (newPos > 0 && lines[newLine][newPos - 1] == ' ');
            }

            if (moved)
            {
                SetCaretPos(newPos, newLine);
                api.Gui.PlaySound("tick");
            }
        }



        /// <summary>
        /// Sets the number of lines in the Text Area.
        /// </summary>
        /// <param name="maxlines">The maximum number of lines.</param>
        public void SetMaxLines(int maxlines)
        {
            this.maxlines = maxlines;
        }


        public void SetMaxHeight(int maxheight)
        {
            var fontExt = Font.GetFontExtents();
            this.maxlines = (int)Math.Floor(maxheight / fontExt.Height);
        }
    }

}
