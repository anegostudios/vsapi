using System;

#nullable disable


namespace Vintagestory.API.Client
{

    public class SavegameCellEntry
    {
        /// <summary>
        /// The title of the Table Cell.
        /// </summary>
        public string Title;

        /// <summary>
        /// The details of the table cell.
        /// </summary>
        public string DetailText;

        /// <summary>
        /// The text displayed in the top right corner of the cell. 
        /// </summary>
        public string RightTopText;

        public string HoverText;

        /// <summary>
        /// The y offset of the right top text
        /// </summary>
        public float RightTopOffY;

        /// <summary>
        /// The y offset of the left title and detail text
        /// </summary>
        public float LeftOffY;


        public double DetailTextOffY;

        /// <summary>
        /// The event fired when the tablecell is clicked.
        /// </summary>
        public Action OnClick;

        /// <summary>
        /// The font of the cell title.
        /// </summary>
        public CairoFont TitleFont;

        /// <summary>
        /// The font of the detail text.
        /// </summary>
        public CairoFont DetailTextFont;

        /// <summary>
        /// 
        /// </summary>
        public bool DrawAsButton = true;

        /// <summary>
        /// Whether or not the cell is on.
        /// </summary>
        public bool Enabled;

        public bool Selected;

    }
}