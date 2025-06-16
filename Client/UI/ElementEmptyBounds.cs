
#nullable disable
namespace Vintagestory.API.Client
{
    class ElementEmptyBounds : ElementBounds
    {

        public override double relX { get { return 0; } }
        public override double relY { get { return 0; } }
        public override double absX { get { return 0; } }
        public override double absY { get { return 0; } }
        public override double renderX { get { return 0; } }
        public override double renderY { get { return 0; } }
        public override double drawX { get { return 0; } }
        public override double drawY { get { return 0; } }

        public override double OuterWidth { get { return 1; } }
        public override double OuterHeight { get { return 1; } }

        public override int OuterWidthInt { get { return 1; } }
        public override int OuterHeightInt { get { return 1; } }

        public override double InnerHeight { get { return 1; } }
        public override double InnerWidth { get { return 1; } }


        public ElementEmptyBounds()
        {
            BothSizing = ElementSizing.FitToChildren;
        }

        public override void CalcWorldBounds()
        {
       
        }



    }
}
