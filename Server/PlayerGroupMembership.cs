using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Server
{
    public class PlayerGroupMembership
    {
        /// <summary>
        /// The member ship level in this group
        /// </summary>
        public EnumPlayerGroupMemberShip Level;

        /// <summary>
        /// The last known group name 
        /// </summary>
        public string GroupName;

        /// <summary>
        /// The group id
        /// </summary>
        public int GroupUid;
    }
}
