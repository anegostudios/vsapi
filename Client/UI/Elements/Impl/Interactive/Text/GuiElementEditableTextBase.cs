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
        internal float[] caretColor = new float[] { 1, 1, 1, 1 };

        internal bool hideCharacters;
        internal bool multilineMode;
        internal int maxlines = 99999;

        internal int caretPosLine;
        internal int caretPosInLine;

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

        public List<string> Lines;

        public bool WordWrap = true;


        public int TextLengthWithoutLineBreaks {
            get {
                int length = 0;
                for (int i = 0; i < Lines.Count; i++) length += Lines[i].Length;
                return length;
            }
        }

        public int CaretPosWithoutLineBreaks
        {
            get
            {
                int pos = 0;
                for (int i = 0; i < caretPosLine; i++) pos += Lines[i].Length;
                return pos + caretPosInLine;
            }
            set
            {
                int sum = 0;
                for (int i = 0; i < Lines.Count; i++) {
                    int len = Lines[i].Length;

                    if (sum + len >= value)
                    {
                        SetCaretPos(value - sum, i);
                        return;
                    }

                    sum += len;
                }

                if (!multilineMode) SetCaretPos(sum, 0);
                else SetCaretPos(value - sum, Lines.Count);
            }
        }

        public int CaretPosLine {
            get
            {
                return caretPosLine;
            }
            set
            {
                caretPosLine = value;
            }

        }
        public int CaretPosInLine => caretPosInLine;

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

            Lines = new List<string>
            {
                ""
            };
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
            caretPosLine = 0;

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = genContext(surface);
            Font.SetupContext(ctx);

            if (multilineMode)
            {
                double lineY = y / ctx.FontExtents.Height;
                if (lineY > Lines.Count)
                {
                    caretPosLine = Lines.Count - 1;
                    caretPosInLine = Lines[caretPosLine].Length;

                    ctx.Dispose();
                    surface.Dispose();
                    return;
                }

                caretPosLine = Math.Max(0, (int)lineY);
            }

            string line = Lines[caretPosLine].TrimEnd('\r', '\n');
            caretPosInLine = line.Length;

            for (int i = 0; i < line.Length; i++)
            {
                double posx = ctx.TextExtents(line.Substring(0, i+1)).XAdvance;

                if (x - posx <= 0)
                {
                    caretPosInLine = i;
                    break;
                }
            }

            ctx.Dispose();
            surface.Dispose();

            SetCaretPos(caretPosInLine, caretPosLine);
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

            caretPosLine = GameMath.Clamp(posLine, 0, Lines.Count - 1);
            caretPosInLine = GameMath.Clamp(posInLine, 0, Lines[caretPosLine].TrimEnd('\r', '\n').Length);


            if (multilineMode)
            {
                caretX = Font.GetTextExtents(Lines[caretPosLine].Substring(0, caretPosInLine)).XAdvance;
                caretY = Font.GetFontExtents().Height * caretPosLine;
            }
            else
            {
                string displayedText = Lines[0];

                if (hideCharacters)
                {
                    displayedText = new StringBuilder(Lines[0]).Insert(0, "•", displayedText.Length).ToString();
                }

                caretX = Font.GetTextExtents(displayedText.Substring(0, caretPosInLine)).XAdvance;
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
            LoadValue(text);
            if (setCaretPosToEnd) SetCaretPos(Lines[Lines.Count - 1].Length, Lines.Count - 1);
        }

        /// <summary>
        /// Sets given texts, leaves cursor position unchanged
        /// </summary>
        /// <param name="text"></param>
        public void LoadValue(string text)
        {
            Lines = Lineize(text);

            while (Lines.Count > maxlines) {
                Lines.RemoveAt(Lines.Count - 1);
            }

            RecomposeText();
            TextChanged();
        }

        protected List<string> Lineize(string text)
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
            OnTextChanged?.Invoke(string.Join("", Lines));
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
                displayedText = Lines[0];

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

                TextLine[] textlines = new TextLine[Lines.Count];
                for (int i = 0; i < textlines.Length; i++)
                {
                    textlines[i] = new TextLine()
                    {
                        Text = Lines[i].Replace("\r\n", "").Replace("\n", ""),
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
                    SetCaretPos(Lines[Lines.Count - 1].TrimEnd('\r', '\n').Length, Lines.Count - 1);
                } else
                {
                    SetCaretPos(Lines[caretPosLine].TrimEnd('\r', '\n').Length, caretPosLine);
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
                    SetCaretPos(0, caretPosLine);
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

                string fulltext = string.Join("\n", Lines);

                int caretPos = caretPosInLine;
                for (int i = 0; i < caretPosLine; i++)
                {
                    caretPos += Lines[i].Length + 1;
                }

                SetValue(fulltext.Substring(0, caretPos) + insert + fulltext.Substring(caretPos, fulltext.Length - caretPos));
                api.Gui.PlaySound("tick");
            }

            if (args.KeyCode == (int)GlKeys.Down && caretPosLine < Lines.Count - 1)
            {
                SetCaretPos(caretPosInLine, caretPosLine + 1);
                api.Gui.PlaySound("tick");
            }

            if (args.KeyCode == (int)GlKeys.Up && caretPosLine > 0)
            {
                SetCaretPos(caretPosInLine, caretPosLine - 1);
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
            return string.Join("", Lines);
        }

        private void OnKeyEnter()
        {
            if (Lines.Count >= maxlines) return;

            string leftText = Lines[caretPosLine].Substring(0, caretPosInLine);
            string rightText = Lines[caretPosLine].Substring(caretPosInLine);

            Lines[caretPosLine] = leftText + "\n";
            Lines.Insert(caretPosLine + 1, rightText);

            TextChanged();
            SetCaretPos(0, caretPosLine + 1);
            api.Gui.PlaySound("tick");
        }

        private void OnKeyDelete()
        {
            if (caretPosInLine < Lines[caretPosLine].Length)
            {
                Lines[caretPosLine] = Lines[caretPosLine].Substring(0, caretPosInLine) + Lines[caretPosLine].Substring(caretPosInLine + 1, Lines[caretPosLine].Length - (caretPosInLine + 1));
            }
            else
            {
                if (caretPosLine < Lines.Count - 1)
                {
                    Lines[caretPosLine] += Lines[caretPosLine + 1];
                    Lines.RemoveAt(caretPosLine + 1);
                }
            }


            LoadValue(GetText());
            api.Gui.PlaySound("tick");
        }

        private void OnKeyBackSpace()
        {
            if (caretPosLine == 0 && caretPosInLine == 0) return;

            if (caretPosInLine > 0)
            {
                if (caretPosLine < Lines.Count)
                {
                    Lines[caretPosLine] = Lines[caretPosLine].Substring(0, Math.Max(0, caretPosInLine - 1)) + Lines[caretPosLine].Substring(caretPosInLine, Lines[caretPosLine].Length - caretPosInLine);
                }
                SetCaretPos(caretPosInLine - 1, caretPosLine);
            } else if (caretPosLine > 0)
            {
                SetCaretPos(Lines[caretPosLine - 1].Length - 1, caretPosLine - 1);
                Lines[caretPosLine] = Lines[caretPosLine].Substring(0, Lines[caretPosLine].Length - 1);
            }

            LoadValue(GetText());
            api.Gui.PlaySound("tick");
        }

        public override void OnKeyPress(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            string newline = Lines[caretPosLine].Substring(0, caretPosInLine) + args.KeyChar + Lines[caretPosLine].Substring(caretPosInLine, Lines[caretPosLine].Length - caretPosInLine);
            double width = Bounds.InnerWidth - 2 * Bounds.absPaddingX - rightSpacing;

            var textExts = Font.GetTextExtents(newline.TrimEnd('\r', '\n'));
            bool lineOverFlow = textExts.Width >= width;
            if (lineOverFlow)
            {
                if (!multilineMode) return;
                StringBuilder newLines = new StringBuilder();
                for (int i = 0; i < Lines.Count; i++) newLines.Append(i == caretPosLine ? newline : Lines[i]);

                if (Lines.Count >= maxlines && Lineize(newLines.ToString()).Count >= maxlines) return;
            }
            
            Lines[caretPosLine] = newline;

            var cpos = CaretPosWithoutLineBreaks;
            LoadValue(GetText()); // Ensures word wrapping
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
                ((caretPosInLine > 0 || caretPosLine > 0) && dir < 0) ||
                ((caretPosInLine < Lines[caretPosLine].Length || caretPosLine < Lines.Count-1) && dir > 0)
            ;

            int newPos = caretPosInLine;
            int newLine = caretPosLine;

            while (!done) {
                newPos += dir;

                if (newPos < 0)
                {
                    if (newLine <= 0) break;
                    newLine--;
                    newPos = Lines[newLine].TrimEnd('\r', '\n').Length;
                } 

                if (newPos > Lines[newLine].TrimEnd('\r', '\n').Length)
                {
                    if (newLine >= Lines.Count - 1) break;
                    newPos = 0;
                    newLine++;
                }

                done = !wholeWord || (newPos > 0 && Lines[newLine][newPos - 1] == ' ');
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
