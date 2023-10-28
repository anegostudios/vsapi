namespace Vintagestory.API.Common
{
    public interface ITimedDespawn
    {
        void SetTimer(int value);
        void SetForcedCalendarDespawn(double value);
    }
}
