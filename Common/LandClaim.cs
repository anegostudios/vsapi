using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumOwnerType
    {
        Entity,
        Player,
        Group
    }

    public enum EnumClaimError
    {
        NoError,
        NotAdjacent,
        Overlapping
    }

    public enum EnumPlayerAccessResult
    {
        Denied,
        OkOwner,
        OkGroup,
        OkPrivilege,
        OkGrantedPlayer,
        OkGrantedGroup
    }


    [ProtoContract]
    public class LandClaim
    {
        [ProtoMember(1)]
        public List<Cuboidi> Areas = new List<Cuboidi>();
        [ProtoMember(2)]
        public int ProtectionLevel;
        [ProtoMember(3)]
        public long OwnedByEntityId;
        [ProtoMember(4)]
        public string OwnedByPlayerUid;
        [ProtoMember(5)]
        public uint OwnedByPlayerGroupUid;
        [ProtoMember(6)]
        public string LastKnownOwnerName;
        [ProtoMember(7)]
        public string Description;
        /// <summary>
        /// Other groups allowed to use this land
        /// </summary>
        [ProtoMember(8)]
        public Dictionary<int, EnumBlockAccessFlags> PermittedPlayerGroupIds = new Dictionary<int, EnumBlockAccessFlags>();
        /// <summary>
        /// Other players allowed to use this land
        /// </summary>
        [ProtoMember(9)]
        public Dictionary<string, EnumBlockAccessFlags> PermittedPlayerUids = new Dictionary<string, EnumBlockAccessFlags>();
        /// <summary>
        /// Other players allowed to use this land, name of the player at the time the privilege was granted
        /// </summary>
        [ProtoMember(10)]
        public Dictionary<string, string> PermittedPlayerLastKnownPlayerName = new Dictionary<string, string>();
        [ProtoMember(11)]
        public bool AllowUseEveryone;
        [ProtoMember(12)]
        public bool AllowTraverseEveryone;

        public BlockPos Center
        {
            get
            {
                if (Areas.Count == 0)
                {
                    return new BlockPos(0, 0, 0);
                }

                Vec3d centerSum = new Vec3d();
                int sizeSum = 0;

                foreach (Cuboidi area in Areas)
                {
                    Vec3i center = area.Center;
                    sizeSum += area.SizeXYZ;
                }

                foreach (Cuboidi area in Areas)
                {
                    Vec3i centeri = area.Center;
                    Vec3d center = new Vec3d(centeri.X, centeri.Y, centeri.Z);
                    centerSum += center * ((double)area.SizeXYZ / sizeSum);
                }

                return new BlockPos(
                    (int)(centerSum.X / Areas.Count),
                    (int)(centerSum.Y / Areas.Count),
                    (int)(centerSum.Z / Areas.Count)
                );
            }
        }

        public int SizeXZ
        {
            get
            {
                int sizeSum = 0;
                foreach (Cuboidi area in Areas)
                {
                    Vec3i center = area.Center;
                    sizeSum += area.SizeXZ;
                }

                return sizeSum;
            }
        }

        public int SizeXYZ
        {
            get
            {
                int sizeSum = 0;
                foreach (Cuboidi area in Areas)
                {
                    Vec3i center = area.Center;
                    sizeSum += area.SizeXYZ;
                }

                return sizeSum;
            }
        }

        public static LandClaim CreateClaim(IPlayer player, int protectionLevel = 1)
        {
            return new LandClaim()
            {
                OwnedByPlayerUid = player.PlayerUID,
                ProtectionLevel = protectionLevel,
                LastKnownOwnerName = Lang.Get("Player " + player.PlayerName)
            };
        }

        public static LandClaim CreateClaim(EntityAgent entity, int protectionLevel = 1)
        {
            string entityName = entity.GetName();

            return new LandClaim()
            {
                OwnedByEntityId = entity.EntityId,
                ProtectionLevel = protectionLevel,
                LastKnownOwnerName = Lang.Get("item-creature-" + entity.Code) + (entityName == null ? "" : " " + entityName)
            };
        }


        public static LandClaim CreateClaim(string ownerName, int protectionLevel = 1)
        {
            return new LandClaim()
            {
                ProtectionLevel = protectionLevel,
                LastKnownOwnerName = ownerName
            };
        }

        public EnumPlayerAccessResult TestPlayerAccess(IPlayer player, EnumBlockAccessFlags claimFlag)
        {
            // Owner
            if (player.PlayerUID.Equals(OwnedByPlayerUid))
            {
                return EnumPlayerAccessResult.OkOwner;
            }

            if (OwnedByPlayerGroupUid > 0 && player.Groups.Any((ms) => ms.GroupUid == OwnedByPlayerGroupUid))
            {
                return EnumPlayerAccessResult.OkGroup;
            }

            // Has higher priv level
            if (player.Role.PrivilegeLevel > ProtectionLevel && player.WorldData.CurrentGameMode == EnumGameMode.Creative)
            {
                return EnumPlayerAccessResult.OkPrivilege;
            }

            if (PermittedPlayerUids.TryGetValue(player.PlayerUID, out EnumBlockAccessFlags flags) && (flags & claimFlag) > 0)
            {
                return EnumPlayerAccessResult.OkGrantedPlayer;
            }

            foreach (PlayerGroupMembership group in player.Groups)
            {
                if (PermittedPlayerGroupIds.TryGetValue(group.GroupUid, out flags) && (flags & claimFlag) > 0)
                {
                    return EnumPlayerAccessResult.OkGrantedGroup;
                }
            }

            return EnumPlayerAccessResult.Denied;
        }


        public bool PositionInside(Vec3d position)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].Contains(position)) return true;
            }

            return false;
        }

        public bool PositionInside(BlockPos position)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].Contains(position)) return true;
            }

            return false;
        }

        public EnumClaimError AddArea(Cuboidi cuboidi)
        {
            // Require to be adjacent other claims
            if (Areas.Count == 0)
            {
                Areas.Add(cuboidi);
                return EnumClaimError.NoError;
            }

            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].Intersects(cuboidi))
                {
                    return EnumClaimError.Overlapping;
                }

            }

            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].IsAdjacent(cuboidi))
                {
                    Areas.Add(cuboidi);
                    return EnumClaimError.NoError;
                }
            }

            return EnumClaimError.NotAdjacent;
        }


        public bool Intersects(Cuboidi cuboidi)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].Intersects(cuboidi))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ignores y-values
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public bool Intersects2d(HorRectanglei rec)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].Intersects(rec))
                {
                    return true;
                }
            }
            return false;
        }


        public LandClaim Clone()
        {
            List<Cuboidi> areas = new List<Cuboidi>();
            for (int i = 0; i < Areas.Count; i++)
            {
                areas.Add(Areas[i].Clone());
            }

            return new LandClaim()
            {
                Areas = areas,
                Description = Description,
                LastKnownOwnerName = LastKnownOwnerName,
                OwnedByEntityId = OwnedByEntityId,
                OwnedByPlayerGroupUid = OwnedByPlayerGroupUid,
                OwnedByPlayerUid = OwnedByPlayerUid,
                PermittedPlayerGroupIds = new Dictionary<int, EnumBlockAccessFlags>(PermittedPlayerGroupIds),
                PermittedPlayerUids = new Dictionary<string, EnumBlockAccessFlags>(PermittedPlayerUids),
                ProtectionLevel = ProtectionLevel
            };
        }
    }
}
