using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public enum EnumMountAngleMode
    {
        /// <summary>
        /// Don't affected the mounted entity angles
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

    public interface IMountableSupplier
    {
        IMountable[] MountPoints { get; }

        bool IsMountedBy(Entity entity);

        Vec3f GetMountOffset(Entity entity);
    }

    public interface IMountable
    {
        /// <summary>
        /// If this "mountable seat" is the one that controls the mountable entity/block
        /// </summary>
        bool CanControl { get; }

        Entity MountedBy { get; }

        /// <summary>
        /// Return null if you don't have a mountable supplier implementation
        /// </summary>
        IMountableSupplier MountSupplier { get; }

        EntityPos MountPosition { get; }

        EnumMountAngleMode AngleMode { get; }

        string SuggestedAnimation { get; }

        Vec3f LocalEyePos { get; }

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
    }
}
