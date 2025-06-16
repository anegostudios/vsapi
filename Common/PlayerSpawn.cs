using ProtoBuf;

#nullable disable

namespace Vintagestory.API.Common
{

    [ProtoContract]
    public class PlayerSpawnPos
    {
        [ProtoMember(1)]
        public int x;

        [ProtoMember(2)]
        public int? y;

        [ProtoMember(3)]
        public int z;

        [ProtoMember(4)]
        public float? yaw;

        [ProtoMember(5)]
        public float? pitch;

        [ProtoMember(6)]
        public float? roll;

        [ProtoMember(7)]
        public int RemainingUses = -1;


        public PlayerSpawnPos()
        {
            this.x = 0;
            this.y = null;
            this.z = 0;
        }

        public PlayerSpawnPos(int x, int? y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + z;
        }
    }
}
