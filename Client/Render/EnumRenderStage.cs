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
        /// To render the held item over water. If done in the opaque pass it would not render water behind it.
        /// </summary>
        AfterOIT = 3,
        /// <summary>
        /// Shadow map
        /// </summary>
        ShadowFar = 4,
        /// <summary>
        /// Shadow map done
        /// </summary>
        ShadowFarDone = 5,

        /// <summary>
        /// Shadow map
        /// </summary>
        ShadowNear = 6,
        /// <summary>
        /// Shadow map done
        /// </summary>
        ShadowNearDone = 7,

        /// <summary>
        /// After all 3d geometry has rendered and post processing of the frame is complete
        /// </summary>
        AfterPostProcessing = 8,
        /// <summary>
        /// Ortho mode for rendering GUIs and everything 2D
        /// </summary>
        Ortho = 9,
        /// <summary>
        /// The post processing passes are merged with all 3d geometry and the scene is color graded
        /// </summary>
        AfterFinalComposition = 10,
        /// <summary>
        /// Scene is blitted onto the default frame buffer, buffers not yet swapped though so can still render to default FB
        /// </summary>
        Done = 11
        
    }
}
