using System;

namespace Vintagestory.API.Common
{
    [Flags]
    public enum EnumAppSide
    {
        Server    = 1,
        Client    = 2,
        Universal = Server | Client
    }
    
    public static class EnumAppSideExtensions
    {
        public static bool IsServer(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Server);
        }
        
        public static bool IsClient(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Client);
        }
        
        public static bool IsUniversal(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Universal);
        }
        
        public static bool Is(this EnumAppSide side, EnumAppSide other)
        {
            return (side & other) == other;
        }
    }
}
