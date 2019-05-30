using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Some extra methods available for server side chunks
    /// </summary>
    public interface IServerChunk : IWorldChunk
    {
        /// <summary>
        /// Allows setting of server side only moddata of this chunk
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetServerModdata(string key, byte[] data);

        /// <summary>
        /// Retrieve server side only mod data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetServerModdata(string key);
    }
}
