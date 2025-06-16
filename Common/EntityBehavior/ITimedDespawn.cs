
#nullable disable
namespace Vintagestory.API.Common
{
    public interface ITimedDespawn
    {
        float DespawnSeconds { get; set; }
        void SetDespawnByCalendarDate(double value);
    }
}
