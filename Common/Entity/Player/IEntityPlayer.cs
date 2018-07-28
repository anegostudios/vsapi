using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    public interface IEntityPlayer : IEntityAgent
    {
        string PlayerUID { get; }

        /// <summary>
        /// Render position for the own player on the client
        /// </summary>
        Vec3d CameraPos { get; }
    }
}
