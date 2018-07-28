using Action = Vintagestory.API.Common.Action;

namespace Vintagestory.API.Client
{
    public class TableCell
    {
        public string Title;
        public string DetailText;
        public string RightTopText;
        public float RightTopOffY;

        public Action OnClick;

        public CairoFont TitleFont;
        public CairoFont DetailTextFont;

        public double HighlightCell = 1;

        public bool IsOn;

        public object Data;
        
    }
}