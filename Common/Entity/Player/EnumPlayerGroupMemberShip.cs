using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common.Entities
{
    public enum EnumPlayerGroupMemberShip
    {
        None,

        /// <summary>
        /// Member, but chat window is usually not opened 
        /// </summary>
        TransientMember,

        /// <summary>
        /// Member, chat window always opened
        /// </summary>
        PersistentMember,

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
