using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A client side player
    /// </summary>
    public interface IClientPlayer : IPlayer
    {
        /// <summary>
        /// The cameras current yaw
        /// </summary>
        float CameraYaw { get; }

        /// <summary>
        /// The cameras current pitch
        /// </summary>
        float CameraPitch { get; }

        /// <summary>
        /// The players current camera mode
        /// </summary>
        EnumCameraMode CameraMode { get; }

        /// <summary>
        /// Writes given message to the players current chat group but doesn't send it to the server
        /// </summary>
        /// <param name="message"></param>
        void ShowChatNotification(string message);

        /// <summary>
        /// Tells the engine to run a first person animtion
        /// </summary>
        /// <param name="anim"></param>
        void TriggerFpAnimation(EnumHandInteract anim);
    }
}
