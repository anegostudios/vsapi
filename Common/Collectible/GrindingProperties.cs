using System;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Defines a set of properties that allow an object to be ground in a quern.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"grindingProps": {
	///	"groundStack": {
	///		"type": "item",
	///		"code": "bonemeal"
	///	}
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class GrindingProperties
    {
        public bool usedObsoleteNotation = false;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// If set, the collectible is grindable in a quern and this is the resulting itemstack once the grinding time is over.
        /// </summary>
        [DocumentAsJson] public JsonItemStack GroundStack;

        /// <summary>
        /// <!--<jsonoptional>Obsolete</jsonoptional>-->
        /// Obsolete. Please use <see cref="GroundStack"/> instead.
        /// </summary>
        [DocumentAsJson]
        [Obsolete("Use GroundStack instead")]
        public JsonItemStack GrindedStack { 
            get
            {
                return this.GroundStack;
            }
            set
            {
                GroundStack = value;
                usedObsoleteNotation = true;
            }
        }

        /// <summary>
        /// Makes a deep copy of the properties.
        /// </summary>
        /// <returns></returns>
        public GrindingProperties Clone()
        {
            return new GrindingProperties()
            {
                GroundStack = this.GroundStack.Clone()
            };
        }
    }
}
