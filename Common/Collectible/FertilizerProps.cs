
#nullable disable
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
