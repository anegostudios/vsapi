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
        /// <summary>
        /// Am I the server?
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static bool IsServer(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Server);
        }
        
        /// <summary>
        /// Am I the client?
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static bool IsClient(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Client);
        }
        
        /// <summary>
        /// Am I a universal?
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static bool IsUniversal(this EnumAppSide side)
        {
            return side.Is(EnumAppSide.Universal);
        }
        
        /// <summary>
        /// Am I this side?
        /// </summary>
        /// <param name="side"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Is(this EnumAppSide side, EnumAppSide other)
        {
            return (side & other) == other;
        }
    }
}
