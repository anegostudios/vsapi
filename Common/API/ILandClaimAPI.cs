using System.Collections.Generic;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface ILandClaimAPI
    {
        /// <summary>
        /// List of all claims on the server. Same as WorldManager.SaveGame.Claims.
        /// </summary>
        List<LandClaim> All { get; }

        /// <summary>
        /// Checks with the permission system if given player has use or place/break permissions on supplied position. Returns always true when called on the client!
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <param name="accessFlag"></param>
        /// <returns></returns>
        EnumWorldAccessResponse TestAccess(IPlayer player, BlockPos pos, EnumBlockAccessFlags accessFlag);


        /// <summary>
        /// Same as <see cref="TestAccess(IPlayer, BlockPos, EnumBlockAccessFlags)"/> but also sends an error message to the player and executes a MarkDirty() event the block. Returns always true when called on the client!
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pos"></param>
        /// <param name="accessFlag"></param>
        /// <returns></returns>
        bool TryAccess(IPlayer player, BlockPos pos, EnumBlockAccessFlags accessFlag);

        /// <summary>
        /// Get all claims registered at this position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        LandClaim[] Get(BlockPos pos);

        /// <summary>
        /// Add a new claim. 
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        void Add(LandClaim claim);


        /// <summary>
        /// Remove a claim. Returns false if no such claim was registered
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        bool Remove(LandClaim claim);

    }
}
