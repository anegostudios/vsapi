
#nullable disable
namespace Vintagestory.API.Common
{
    public class MetalProperty : WorldProperty<MetalPropertyVariant>
    {
        public float MeltPoint;
        public float BoilPoint;
        public float Density;
        public float SpecificHeatCapacity;
        public bool Elemental;
        public int Tier;
    }
}
