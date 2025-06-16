
#nullable disable
namespace Vintagestory.API.Client
{
    public class AvailableCodec
    {
        public string Name;
        public string Code;
    }

    public interface IAviWriter
    {
        void Open(string filename, int width, int height);
        void AddFrame();
        void Close();
    }
}
