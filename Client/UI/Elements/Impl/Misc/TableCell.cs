using Action = Vintagestory.API.Common.Action;

namespace Vintagestory.API.Client
{
    public class TableCell
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

        /// <summary>
        /// The offset from the top right.
        /// </summary>
        public float RightTopOffY;

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
        public double HighlightCell = 1;

        /// <summary>
        /// Whether or not the cell is on.
        /// </summary>
        public bool IsOn;

        /// <summary>
        /// The data stored inside the cell.
        /// </summary>
        public object Data;
        
    }
}