using System.IO;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void OnValueSetDelegate(string elementCode, string newValue);
    public delegate string OnValueGetDelegate(string elementCode);

    public enum EnumDialogElementType
    {
        Select,
        Input,
        Slider,
        Switch,
        NumberInput,
        Button,
        Text,
        DynamicSelect
    }

    public enum EnumDialogElementMode
    {
        DropDown,
        Buttons
    }

    public class JsonDialogSettings
    {
        public string Code;
        public EnumDialogArea Alignment;
        public float PosX;
        public float PosY;
        public DialogRow[] Rows;
        public double SizeMultiplier = 1;
        public double Padding = 10;
        public bool DisableWorldInteract = true;

        /// <summary>
        /// Called whenever the value of a input field has changed
        /// </summary>
        public OnValueSetDelegate OnSet;
        /// <summary>
        /// Called when the dialog is opened the first time or when dialog.ReloadValues() is called. Should return the values the input fields should be set to
        /// </summary>
        public OnValueGetDelegate OnGet;

        /// <summary>
        /// Writes the content to the writer.
        /// </summary>
        /// <param name="writer">The writer to fill with data.</param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write((int)Alignment);
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write(Rows.Length);
            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].ToBytes(writer);
            }
            writer.Write(SizeMultiplier);
        }

        /// <summary>
        /// Reads the content to the dialog.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public void FromBytes(BinaryReader reader)
        {
            Code = reader.ReadString();
            Alignment = (EnumDialogArea)reader.ReadInt32();
            PosX = reader.ReadSingle();
            PosY = reader.ReadSingle();
            Rows = new DialogRow[reader.ReadInt32()];
            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new DialogRow();
                Rows[i].FromBytes(reader);
            }
            SizeMultiplier = reader.ReadSingle();
        }
    }

    public class DialogRow
    {
        public DialogElement[] Elements;
        public float BottomPadding = 10;
        public float TopPadding = 0;

        public DialogRow()
        {

        }

        public DialogRow(params DialogElement[] elements)
        {
            this.Elements = elements;
        }

        internal void FromBytes(BinaryReader reader)
        {
            Elements = new DialogElement[reader.ReadInt32()];
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i] = new DialogElement();
                Elements[i].FromBytes(reader);
            }

            TopPadding = reader.ReadInt16();
            BottomPadding = reader.ReadInt16();
        }

        internal void ToBytes(BinaryWriter writer)
        {
            writer.Write(Elements.Length);
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i].ToBytes(writer);
            }

            writer.Write((short)TopPadding);
            writer.Write((short)BottomPadding);
        }
    }

    public class DialogElement
    {
        public string Code;
        public int Width = 150;
        public int Height = 30;
        public int PaddingLeft = 0;
        public EnumDialogElementType Type;
        public EnumDialogElementMode Mode;
        public string Label;
        public string Icon;
        public string Text;
        public string Tooltip;
        public float FontSize = 14;

        public string[] Icons;
        public string[] Values;
        public string[] Names;
        public string[] Tooltips;
        public int MinValue;
        public int MaxValue;
        public int Step;

        /// <summary>
        /// To hold generic data
        /// </summary>
        public string Param;
        

        internal void FromBytes(BinaryReader reader)
        {
            Code = reader.ReadString();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Type = (EnumDialogElementType)reader.ReadInt32();
            Mode = (EnumDialogElementMode)reader.ReadInt32();
            Label = reader.ReadString();
            Icons = reader.ReadStringArray();
            Values = reader.ReadStringArray();
            Names = reader.ReadStringArray();
            Tooltips = reader.ReadStringArray();
            Param = reader.ReadString();
            MinValue = reader.ReadInt32(); 
            MaxValue = reader.ReadInt32();
            Step = reader.ReadInt32();
            Icon = reader.ReadString();
            Text = reader.ReadString();
            FontSize = reader.ReadSingle();
        }

        internal void ToBytes(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write((int)Type);
            writer.Write((int)Mode);
            writer.Write(Label);
            writer.WriteArray(Icons);
            writer.WriteArray(Values);
            writer.WriteArray(Names);
            writer.WriteArray(Tooltips);
            writer.Write(Param);
            writer.Write(MinValue);
            writer.Write(MaxValue);
            writer.Write(Step);
            writer.Write(Icon);
            writer.Write(Text);
            writer.Write(FontSize);
        }
    }
}
