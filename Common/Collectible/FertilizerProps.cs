using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class FertilizerProps
    {
        public float N;
        public float P;
        public float K;

        public PermaFertilityBoost PermaBoost;
    }

    public class PermaFertilityBoost
    {
        public string Code;
        public int N;
        public int P;
        public int K;
    }


}
