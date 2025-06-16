using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public interface IMusicTrack
    {
        /// <summary>
        /// The name of the track
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Is the track active?
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// The priority of the track. A higher priority will cancel active tracks.
        /// </summary>
        float Priority { get; }

        /// <summary>
        /// The start priority of the track. A higher priority will leave active tracks playing.
        /// </summary>
        float StartPriority { get; }

        /// <summary>
        /// Called before sorting by start priority happens
        /// </summary>
        void BeginSort();

        /// <summary>
        /// Initialization of the Music Track.
        /// </summary>
        /// <param name="assetManager">the global Asset Manager</param>
        /// <param name="capi">The Core Client API</param>
        /// <param name="musicEngine"></param>
        void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine);

        /// <summary>
        /// Should this current track play?
        /// </summary>
        /// <param name="props">Player Properties</param>
        /// <param name="conds"></param>
        /// <param name="pos"></param>
        /// <returns>Should we play the current track?</returns>
        bool ShouldPlay(TrackedPlayerProperties props, ClimateCondition conds, BlockPos pos);

        /// <summary>
        /// Begin playing the current track.
        /// </summary>
        /// <param name="props">Player Properties</param>
        void BeginPlay(TrackedPlayerProperties props);

        /// <summary>
        /// Is it cool for the current track to continue playing?
        /// </summary>
        /// <param name="dt">Delta Time/Change in time.</param>
        /// <param name="props">Track properties.</param>
        /// <returns>Cool or not cool?</returns>
        bool ContinuePlay(float dt, TrackedPlayerProperties props);

        /// <summary>
        /// Updates the volume on the current track.
        /// </summary>
        void UpdateVolume();

        /// <summary>
        /// Called when the track to interupted or when Update() returned false. 
        /// So called every time the tracked ended or has to end
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFadedOut"></param>
        void FadeOut(float seconds, Action onFadedOut = null);
        void FastForward(float seconds);
        string PositionString { get; }
    }
}
