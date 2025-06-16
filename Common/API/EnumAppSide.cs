using System;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A server/client side used by for the Vintage Story app. 
    /// </summary>
    [Flags]
    [DocumentAsJson]
    public enum EnumAppSide
    {
        /// <summary>
        /// For server side things only.
        /// </summary>
        Server    = 1,
        /// <summary>
        /// For client side things only.
        /// </summary>
        Client    = 2,

        /// <summary>
        /// For server and client side things.
        /// </summary>
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
