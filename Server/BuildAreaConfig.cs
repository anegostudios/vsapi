using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Server
{
    public class BuildAreaConfig
    {
        public int Id;

        private int x1;
        private int x2;
        private int y1;
        private int y2;
        private int z1;
        private int z2;
        private string coords;

        public List<string> PermittedGroupCodes;
        public List<string> PermittedUsers;
        public int? Level;

        public BuildAreaConfig()
        {
            this.Id = -1;
            this.Coords = "0,0,0,0,0,0";
            this.PermittedGroupCodes = new List<string>();
            this.PermittedUsers = new List<string>();
        }

        public string Coords
        {
            get
            {
                return this.coords;
            }

            set
            {
                this.coords = value;
                string[] myCoords = this.Coords.Split(new char[] { ',' });
                x1 = Convert.ToInt32(myCoords[0]);
                x2 = Convert.ToInt32(myCoords[3]);
                y1 = Convert.ToInt32(myCoords[1]);
                y2 = Convert.ToInt32(myCoords[4]);
                z1 = Convert.ToInt32(myCoords[2]);
                z2 = Convert.ToInt32(myCoords[5]);
            }
        }

        public bool IsInCoords(int x, int y, int z)
        {
            if (x >= x1 && x <= x2 && y >= y1 && y <= y2 && z >= z1 && z <= z2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanUserBuild(string playerUid, IPlayerRole clientRole)
        {
            if (this.Level != null)
            {
                if (clientRole.Level >= this.Level)
                {
                    return true;
                }
            }

            foreach (string allowedGroup in this.PermittedGroupCodes)
            {
                if (allowedGroup.Equals(clientRole.Code))
                {
                    return true;
                }
            }

            foreach (string allowedUser in this.PermittedUsers)
            {
                if (allowedUser.Equals(playerUid))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string permittedGroupsString = "";
            if (this.PermittedGroupCodes.Count > 0)
            {
                permittedGroupsString = this.PermittedGroupCodes[0].ToString();
                for (int i = 1; i < this.PermittedGroupCodes.Count; i++)
                {
                    permittedGroupsString += "," + this.PermittedGroupCodes[i].ToString();
                }
            }

            string permittedUsersString = "";
            if (this.PermittedUsers.Count > 0)
            {
                permittedUsersString = this.PermittedUsers[0].ToString();
                for (int i = 1; i < this.PermittedUsers.Count; i++)
                {
                    permittedUsersString += "," + this.PermittedUsers[i].ToString();
                }
            }

            string levelString = "";
            if (Level != null)
            {
                levelString = this.Level.ToString();
            }

            return Id + ":" + Coords + ":" + permittedGroupsString + ":" + permittedUsersString + ":" + levelString;
        }
    }
}
