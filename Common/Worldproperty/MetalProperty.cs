using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
