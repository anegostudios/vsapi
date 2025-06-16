using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A client side player
    /// </summary>
    public interface IClientPlayer : IPlayer
    {

        /// <summary>
        /// The cameras current pitch
        /// </summary>
        float CameraPitch { get; set; }

        float CameraRoll { get; set; }

        float CameraYaw { get; set; }



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
