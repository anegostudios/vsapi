
#nullable disable
namespace Vintagestory.API.Common
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
