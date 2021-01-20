using Cairo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public delegate RichTextComponentBase Tag2RichTextDelegate(ICoreClientAPI capi, VtmlTagToken token, Stack<CairoFont> fontStack, Action<LinkTextComponent> didClickLink);

    public class VtmlUtil {

        /// <summary>
        /// You can register your own tag converters here
        /// </summary>
        public static Dictionary<string, Tag2RichTextDelegate> TagConverters = new Dictionary<string, Tag2RichTextDelegate>();


        public static RichTextComponentBase[] Richtextify(ICoreClientAPI capi, string vtmlCode, CairoFont baseFont, Action<LinkTextComponent> didClickLink = null)
        {
            List<RichTextComponentBase> elems = new List<RichTextComponentBase>();

            Stack<CairoFont> fontStack = new Stack<CairoFont>();
            fontStack.Push(baseFont);

            VtmlToken[] tokens = VtmlParser.Tokenize(capi.Logger, vtmlCode);
            Richtextify(capi, tokens, ref elems, fontStack, didClickLink);

            return elems.ToArray();
        }

        static void Richtextify(ICoreClientAPI capi, VtmlToken[] tokens, ref List<RichTextComponentBase> elems, Stack<CairoFont> fontStack, Action<LinkTextComponent> didClickLink)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                Richtextify(capi, tokens[i], ref elems, fontStack, didClickLink);
            }
        }
        

        static void Richtextify(ICoreClientAPI capi, VtmlToken token, ref List<RichTextComponentBase> elems, Stack<CairoFont> fontStack, Common.Action<LinkTextComponent> didClickLink)
        {
            if (token is VtmlTagToken)
            {
                VtmlTagToken tagToken = token as VtmlTagToken;

                switch (tagToken.Name)
                {
                    case "br":
                        elems.Add(new RichTextComponent(capi, "\r\n", fontStack.Peek()));
                        break;

                    case "i":
                        CairoFont font = fontStack.Peek().Clone();
                        font.Slant = FontSlant.Italic;
                        fontStack.Push(font);
                        foreach (var val in tagToken.ChildElements)
                        {
                            Richtextify(capi, val, ref elems, fontStack, didClickLink);
                        }
                        fontStack.Pop();
                        break;

                    case "a":
                        LinkTextComponent cmp = new LinkTextComponent(capi, tagToken.ContentText, fontStack.Peek(), didClickLink);
                        tagToken.Attributes.TryGetValue("href", out cmp.Href);

                        elems.Add(cmp);
                        break;

                    case "icon":
                        string iconName;
                        tagToken.Attributes.TryGetValue("name", out iconName);
                        IconComponent iconcmp = new IconComponent(capi, iconName, fontStack.Peek());
                        elems.Add(iconcmp);
                        break;

                    case "itemstack":
                        string code;
                        string type;
                        float size = (float)fontStack.Peek().GetFontExtents().Height;
                        EnumFloat floatType = EnumFloat.Inline;
                        string floattypestr;
                        if (tagToken.Attributes.TryGetValue("floattype", out floattypestr))
                        {
                            if (!Enum.TryParse(floattypestr, out floatType))
                            {
                                floatType = EnumFloat.Inline;
                            }
                        }

                        tagToken.Attributes.TryGetValue("code", out code);
                        if (!tagToken.Attributes.TryGetValue("type", out type))
                        {
                            type = "block";
                        }

                        ItemStack stack;
                        if (type == "item")
                        {
                            stack = new ItemStack(capi.World.GetItem(new AssetLocation(code)));
                        } else
                        {
                            stack = new ItemStack(capi.World.GetBlock(new AssetLocation(code)));
                        }

                        ItemstackComponentBase stckcmp = new SlideshowItemstackTextComponent(capi, new ItemStack[] { stack }, size / RuntimeEnv.GUIScale, floatType);
                        elems.Add(stckcmp);
                        break;

                    case "font":
                        fontStack.Push(getFont(tagToken, fontStack));
                        foreach (var val in tagToken.ChildElements)
                        {
                            Richtextify(capi, val, ref elems, fontStack, didClickLink);
                        }
                        fontStack.Pop();
                        break;
                    
                    case "clear":
                        elems.Add(new ClearFloatTextComponent(capi));
                        break;

                    case "strong":
                        fontStack.Push(fontStack.Peek().Clone().WithWeight(Cairo.FontWeight.Bold));
                        foreach (var val in tagToken.ChildElements)
                        {
                            Richtextify(capi, val, ref elems, fontStack, didClickLink);
                        }
                        fontStack.Pop();
                        break;
                }


                if (tagToken.Name != null && TagConverters.ContainsKey(tagToken.Name))
                {
                    RichTextComponentBase elem = TagConverters[tagToken.Name](capi, tagToken, fontStack, didClickLink);
                    if (elem != null) elems.Add(elem);
                }

            } else
            {
                VtmlTextToken textToken = token as VtmlTextToken;
                elems.Add(new RichTextComponent(capi, textToken.Text, fontStack.Peek()));
            }
        }



        static CairoFont getFont(VtmlTagToken tag, Stack<CairoFont> fontStack)
        {
            double size = 0;
            double lineHeight = 1;
            string fontName = "";
            EnumTextOrientation orient = EnumTextOrientation.Left;
            double[] color = ColorUtil.WhiteArgbDouble;
            FontWeight weight = FontWeight.Normal;

            CairoFont prevFont = fontStack.Pop();

            if (!tag.Attributes.ContainsKey("size") || !double.TryParse(tag.Attributes["size"], NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out size))
            {
                size = prevFont.UnscaledFontsize;
            }

            if (!tag.Attributes.ContainsKey("family"))
            {
                fontName = prevFont.Fontname;
            } else
            {
                fontName = tag.Attributes["family"];
            }

            if (!tag.Attributes.ContainsKey("color") || !parseHexColor(tag.Attributes["color"], out color))
            {
                color = prevFont.Color;
            }

            double opacity;
            if (tag.Attributes.ContainsKey("opacity") && double.TryParse(tag.Attributes["opacity"], NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out opacity))
            {
                color = (double[])color.Clone();
                color[3] *= opacity;
            }

            if (tag.Attributes.ContainsKey("weight"))
            {
                weight = tag.Attributes["weight"] == "bold" ? FontWeight.Bold : FontWeight.Normal;
            }
            else
            {
                weight = prevFont.FontWeight;
            }

            if (!tag.Attributes.ContainsKey("lineheight") || !double.TryParse(tag.Attributes["lineheight"], NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out lineHeight))
            {
                lineHeight = prevFont.LineHeightMultiplier;
            }

            if (tag.Attributes.ContainsKey("align"))
            {
                switch (tag.Attributes["align"])
                {
                    case "left":
                        orient = EnumTextOrientation.Left;
                        break;
                    case "right":
                        orient = EnumTextOrientation.Right;
                        break;
                    case "center":
                        orient = EnumTextOrientation.Center;
                        break;
                    case "justify":
                        orient = EnumTextOrientation.Justify;
                        break;
                }
                
            } else
            {
                orient = prevFont.Orientation;
            }

            fontStack.Push(prevFont);

            return new CairoFont(size, fontName, color).WithWeight(weight).WithLineHeightMultiplier(lineHeight).WithOrientation(orient);
        }



        public static bool parseHexColor(string colorText, out double[] color)
        {
            System.Drawing.Color cl;
            try
            {
                cl = ColorTranslator.FromHtml(colorText);
            } catch (Exception)
            {
                color = new double[] { 0, 0, 0, 1 };
                return false;
            }

            if (cl == null)
            {
                color = null;
                return false;
            }

            color = new double[] { cl.R / 255.0, cl.G / 255.0, cl.B / 255.0, cl.A / 255.0 };
            return true;
        }

        public static string toHexColor(double[] color)
        {
            return "#" + ((int)(color[0] * 255)).ToString("X2") + ((int)(color[1] * 255)).ToString("X2") + ((int)(color[2] * 255)).ToString("X2");
        }
    }


    public class VtmlParser
    {
        public static VtmlToken[] Tokenize(ILogger errorLogger, string vtml)
        {
            List<VtmlToken> tokenized = new List<VtmlToken>();

            Stack<VtmlTagToken> tokenStack = new Stack<VtmlTagToken>();
            string text = "";
            string tag = "";
            bool insideTag = false;

            for (int pos = 0; pos < vtml.Length; pos++)
            {
                if (vtml[pos] == '<')
                {
                    insideTag = true;

                    if (text.Length > 0)
                    {
                        text = text
                            .Replace("&gt;", ">")
                            .Replace("&lt;", "<")
                            .Replace("&nbsp;", " ")
                        ;

                        if (tokenStack.Count > 0)
                        {
                            tokenStack.Peek().ChildElements.Add(new VtmlTextToken() { Text = text });
                        }
                        else
                        {
                            tokenized.Add(new VtmlTextToken() { Text = text });
                        }
                    }

                    text = "";

                    continue;
                }

                if (vtml[pos] == '>')
                {
                    if (!insideTag)
                    {
                        errorLogger.Error("Found closing tag char > but no tag was opened at " + pos + ". Use &gt;/&lt; if you want to display them as plain characters. See debug log for full text.");
                        errorLogger.VerboseDebug(vtml);
                    }

                    insideTag = false;

                    // </div>
                    if (tag.Length > 0 && tag[0] == '/')
                    {
                        if (tokenStack.Count == 0 || tokenStack.Peek().Name != tag.Substring(1))
                        {
                            if (tokenStack.Count == 0)
                                errorLogger.Error("Found closing tag <" + tag.Substring(1) + "> at position " + pos + " but it was never opened. See debug log for full text.");
                            else
                                errorLogger.Error("Found closing tag <" + tag.Substring(1) + "> at position " + pos + " but <"+ tokenStack.Peek().Name + "> should be closed first. See debug log for full text.");

                            errorLogger.VerboseDebug(vtml);
                        }

                        if (tokenStack.Count > 0)
                        {
                            tokenStack.Pop();
                        }
                        tag = "";
                        continue;
                    }

                    VtmlTagToken tagToken;

                    // <br>
                    if (tag == "br")
                    {
                        tagToken = new VtmlTagToken() { Name = "br" };

                        if (tokenStack.Count > 0)
                        {
                            tokenStack.Peek().ChildElements.Add(tagToken);
                        }
                        else
                        {
                            tokenized.Add(tagToken);
                        }
                        tag = "";
                        continue;
                    }

                    // <div a=b />
                    if (vtml[pos - 1] == '/')
                    {
                        tagToken = parseTagAttributes(tag.Substring(0, tag.Length - 1));

                        if (tokenStack.Count > 0)
                        {
                            tokenStack.Peek().ChildElements.Add(tagToken);
                        }
                        else
                        {
                            tokenized.Add(tagToken);
                        }
                        tag = "";
                        continue;
                    }

                    // <div a=b>
                    {
                        tagToken = parseTagAttributes(tag);

                        if (tokenStack.Count > 0)
                        {
                            tokenStack.Peek().ChildElements.Add(tagToken);
                        }
                        else
                        {
                            tokenized.Add(tagToken);
                        }

                        tokenStack.Push(tagToken);
                        tag = "";
                    }

                    continue;
                }

                if (insideTag)
                {
                    tag += vtml[pos];
                } else
                {
                    text += vtml[pos];
                }
            }

            if (text.Length > 0)
            {
                text = text
                    .Replace("&gt;", ">")
                    .Replace("&lt;", "<")
                    //.Replace("  ", "")
                    //.Replace("  ", "")
                    .Replace("&nbsp;", " ")
                ;

                tokenized.Add(new VtmlTextToken() { Text = text });
            }

            return tokenized.ToArray();
        }


        public enum ParseState { SeekKey, ParseTagName, ParseKey, SeekValue, ParseQuotedValue, ParseValue }

        private static VtmlTagToken parseTagAttributes(string tag)
        {
            Dictionary<string, string>  attributes = new Dictionary<string, string>();
            string tagName = null;

            ParseState state = ParseState.ParseTagName;

            int i;
            char insideQuotedValueChar=(char)0;
            string key = "";
            string value = "";

            for (i = 0; i < tag.Length; i++)
            {
                bool isWhiteSpace = tag[i] == ' ' || tag[i] == '\t' || tag[i] == '\r' || tag[i] == '\n';
                bool isQuote = tag[i] == '\'' || tag[i] == '"';

                switch(state)
                {
                    case ParseState.ParseTagName:
                        if (isWhiteSpace)
                        {
                            state = ParseState.SeekKey;
                        } else
                        {
                            tagName += tag[i];
                        }
                        break;

                    case ParseState.SeekKey:
                        if (isWhiteSpace) break;

                        key = ""+tag[i];
                        state = ParseState.ParseKey;
                        break;

                    case ParseState.ParseKey:
                        if (tag[i] == '=')
                        {
                            state = ParseState.SeekValue;
                            value = "";
                            break;
                        }

                        if (isWhiteSpace)
                        {
                            attributes[key] = null;
                            state = ParseState.SeekKey;
                            break;
                        }
                        
                        key += tag[i];
                       
                        
                        break;

                    case ParseState.SeekValue:
                        if (isWhiteSpace) break;

                        if (isQuote)
                        {
                            state = ParseState.ParseQuotedValue;
                            insideQuotedValueChar = tag[i];
                        } else
                        {
                            state = ParseState.ParseValue;
                            value = "" + tag[i];
                        }

                        break;

                    case ParseState.ParseValue:
                        if (isWhiteSpace)
                        {
                            attributes[key.ToLowerInvariant()] = value;
                            state = ParseState.SeekKey;
                            break;
                        }

                        value += tag[i];
                        break;

                    case ParseState.ParseQuotedValue:
                        if (tag[i] == insideQuotedValueChar && tag[i-1] != '\\')
                        {
                            attributes[key.ToLowerInvariant()] = value;
                            state = ParseState.SeekKey;
                            break;
                        }

                        value += tag[i];

                        break;
                }
            }

            if (state == ParseState.ParseValue || state == ParseState.SeekValue)
            {
                attributes[key] = value;
            }

            return new VtmlTagToken()
            {
                Name = tagName,
                Attributes = attributes
            };
        }
    }


    public class VtmlTagToken : VtmlToken
    {
        public List<VtmlToken> ChildElements { get; set; } = new List<VtmlToken>();

        /// <summary>
        /// Name of this tag
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of attribute names and values for this tag
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }


        public string ContentText
        {
            get
            {
                string text = "";

                foreach (var val in ChildElements)
                {
                    if (val is VtmlTextToken) text += (val as VtmlTextToken).Text;
                    else
                    {
                        text += (val as VtmlTagToken).ContentText;
                    }
                }

                return text;
            }
        }
    }

    public class VtmlTextToken : VtmlToken
    {
        public string Text;
    }

    public class VtmlToken
    {
        public int StartPosition { get; set; }
    }

    public enum EnumTokenType
    {
        Text,
        Tag
    }

}