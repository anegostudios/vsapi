
#nullable disable
namespace Vintagestory.API.Server
{
    /// <summary>
    /// Mods can create server threads to carry out an asynchronous process, by implementing this interface and calling IServerApi.AddServerThread()
    /// </summary>
    public interface IAsyncServerSystem
    {
        int OffThreadInterval();

        void OnSeparateThreadTick();

        void ThreadDispose();
    }
}
