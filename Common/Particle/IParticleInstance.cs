using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a particle that has been spawned
    /// </summary>
    public interface IParticleInstance
    {
        /// <summary>
        /// Returns the current position of the particle
        /// </summary>
        /// <returns></returns>
        Vec3d GetPosition();

        /// <summary>
        /// Returns the current velocity of the particle
        /// </summary>
        /// <returns></returns>
        Vec3f GetVelocity();
    }
}