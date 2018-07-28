using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public enum EnumRenderStage
    {
        /// <summary>
        /// Before any rendering has begun, use for setting up stuff during render
        /// </summary>
        Before = 0,
        /// <summary>
        /// Opaque/Alpha tested rendering
        /// </summary>
        Opaque = 1,
        /// <summary>
        /// Order independent transparency 
        /// </summary>
        OIT = 2,
        /// <summary>
        /// Shadow map
        /// </summary>
        ShadowFar = 3,
        /// <summary>
        /// Shadow map done
        /// </summary>
        ShadowFarDone = 4,

        /// <summary>
        /// Shadow map
        /// </summary>
        ShadowNear = 5,
        /// <summary>
        /// Shadow map done
        /// </summary>
        ShadowNearDone = 6,

        /// <summary>
        /// After all 3d geometry has rendered and post processing of the frame is complete
        /// </summary>
        AfterPostProcessing = 7,
        /// <summary>
        /// Ortho mode for rendering GUIs and everything 2D
        /// </summary>
        Ortho = 8,
        /// <summary>
        /// The post processing passes are merged with all 3d geometry and the scene is color graded
        /// </summary>
        AfterFinalComposition = 9,
        /// <summary>
        /// Scene is blitted onto the default frame buffer, buffers not yet swapped though so can still render to default FB
        /// </summary>
        Done = 10
        
    }
}
