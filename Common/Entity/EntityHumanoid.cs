
#nullable disable
namespace Vintagestory.API.Common
{
    public class EntityHumanoid : EntityAgent
    {
        public override bool CanStepPitch => false;   // Villagers and traders and players etc. should not stepPitch, looks weird

        //public override double EyeHeight => base.Properties.EyeHeight - (controls.Sneak ? 0.1 : 0.0);

    }
}
