using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// An interface that can supply texture atlas positions 
    /// </summary>
    public interface ITexPositionSource
    {
        TextureAtlasPosition this[string textureCode] { get; }

        /// <summary>
        /// This returns the size of the atlas this texture resides in.
        /// </summary>
        int AtlasSize { get; }
    }
}
