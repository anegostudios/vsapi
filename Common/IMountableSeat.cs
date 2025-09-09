using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumMountAngleMode
    {
        /// <summary>
        /// Don't affect the mounted entity angles
        /// </summary>
        Unaffected,
        /// <summary>
        /// Turn the player but allow him to still change its yaw
        /// </summary>
        PushYaw,
        /// <summary>
        /// Turn the player in all directions but allow him to still change its angles
        /// </summary>
        Push,
        /// <summary>
        /// Fixate the mounted entity yaw to the mount
        /// </summary>
        FixateYaw,
        /// <summary>
        /// Fixate all entity angles to the mount
        /// </summary>
        Fixate,
    }

    public static class MountableUtil
    {
        public static bool IsMountedBy(this IMountable mountable, Entity entity)
        {
            foreach (var seat in mountable.Seats)
            {
                if (seat.Passenger == entity) return true;
            }

            return false;
        }

        public static IMountableSeat GetSeatOfMountedEntity(this IMountable mountable, Entity entity)
        {
            foreach (var seat in mountable.Seats)
            {
                if (seat.Passenger == entity) return seat;
            }

            return null;
        }


        public static bool IsBeingControlled(this IMountable mountable)
        {
            foreach (IMountableSeat seat in mountable.Seats)
            {
                if (seat.CanControl && seat.Passenger != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class SeatConfig
    {
        /// <summary>
        /// Attachment point name
        /// </summary>
        public string APName;
        public string SelectionBox;
        public string SeatId;
        public bool Controllable;
        public Vec3f MountOffset;
        public Vec3f MountRotation = new Vec3f();
        public float? BodyYawLimit;
        public float EyeHeight = 1.5f;
        public float EyeOffsetX = 0f;
        public EnumMountAngleMode AngleMode = EnumMountAngleMode.FixateYaw;

        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;
        public string Animation { get; set; }
    }


    public interface IVariableSeatsMountable : IMountable
    {
        void RegisterSeat(SeatConfig seat);
        void RemoveSeat(string seatId);
    }

    public interface IMountableListener
    {
        void DidUnmount(EntityAgent entityAgent);
        void DidMount(EntityAgent entityAgent);
    }

    /// <summary>
    /// Represents something the player can mount. Usually a block or an entity.
    /// </summary>
    public interface IMountable
    {
        /// <summary>
        /// The seats of this mountable
        /// </summary>
        IMountableSeat[] Seats { get; }

        /// <summary>
        /// Position of this mountable
        /// </summary>
        EntityPos Position { get; }

        /// <summary>
        /// StepPitch (pitching when stepping up or down a block) of this mountable - valid client-side only, taken from EntityShapeRenderer
        /// </summary>
        double StepPitch { get; }


        bool AnyMounted();

        /// <summary>
        /// The entity that controls this mountable - there can only be one
        /// </summary>
        Entity Controller { get; }

        /// <summary>
        /// The entity which this mountable really is (for example raft, boat or elk) - may be null if the IMountable is a bed or other block
        /// </summary>
        Entity OnEntity { get; }

        /// <summary>
        /// The controls of the controlling seat (if any)
        /// </summary>
        EntityControls ControllingControls { get; }
    }


    /// <summary>
    /// Represents a seat of a mountable object.
    /// </summary>
    public interface IMountableSeat
    {
        SeatConfig Config { get; set; }
        string SeatId { get; set; }
        long PassengerEntityIdForInit { get; set; }
        bool DoTeleportOnUnmount { get; set; }

        /// <summary>
        /// The entity behind this mountable supplier, if any
        /// </summary>
        Entity Entity { get; }

        /// <summary>
        /// The entity sitting on this seat
        /// </summary>
        Entity Passenger { get; }

        /// <summary>
        /// The supplier of this mount provider. e.g. the raft entity for the 2 raft seats
        /// </summary>
        IMountable MountSupplier { get; }

        /// <summary>
        /// If this "mountable seat" is the one that controls the mountable entity/block
        /// </summary>
        bool CanControl { get; }

        /// <summary>
        /// How the mounted entity should rotate
        /// </summary>
        EnumMountAngleMode AngleMode { get; }

        /// <summary>
        /// What animation the mounted entity should play
        /// </summary>
        AnimationMetaData SuggestedAnimation { get; }

        /// <summary>
        /// Whether or not the mount should play the idle anim
        /// </summary>
        bool SkipIdleAnimation { get; }

        float FpHandPitchFollow { get; }

        /// <summary>
        /// Where to place the first person camera
        /// </summary>
        Vec3f LocalEyePos { get; }

        /// <summary>
        /// Exact position of this seat
        /// </summary>
        EntityPos SeatPosition { get; }

        /// <summary>
        /// Transformation matrix that can be used to render the mounted entity at the right position. The transform is relative to the SeatPosition. May be null.
        /// </summary>
        Matrixf RenderTransform { get; }

        /// <summary>
        /// The control scheme of this seat
        /// </summary>
        EntityControls Controls { get; }

        /// <summary>
        /// When the entity unloads you should write whatever you need in here to reconstruct the IMountable after it's loaded again
        /// Reconstruct it by registering a mountable instancer through api.RegisterMountable(string className, GetMountableDelegate mountableInstancer)
        /// You must also set a string with key className, that is the same string that you used for RegisterMountable()
        /// </summary>
        /// <param name="tree"></param>
        void MountableToTreeAttributes(TreeAttribute tree);

        /// <summary>
        /// Called when the entity unmounted himself
        /// </summary>
        /// <param name="entityAgent"></param>
        void DidUnmount(EntityAgent entityAgent);

        /// <summary>
        /// Called when the entity mounted himself
        /// </summary>
        /// <param name="entityAgent"></param>
        void DidMount(EntityAgent entityAgent);

        /// <summary>
        /// Return true if the currently mounted entity can unmount (or if not mounted in the first place)
        /// </summary>
        /// <param name="entityAgent"></param>
        /// <returns></returns>
        bool CanUnmount(EntityAgent entityAgent);

        bool CanMount(EntityAgent entityAgent);
    }
}
