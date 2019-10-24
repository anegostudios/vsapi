using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumPlayerGroupMemberShip
    {
        None,

        /// <summary>
        /// Member
        /// </summary>
        Member,

        /// <summary>
        /// Operator of this channel
        /// </summary>
        Op,

        /// <summary>
        /// Owner of this channel
        /// </summary>
        Owner
    }
}
