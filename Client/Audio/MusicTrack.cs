using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Adds a basic music track.
    /// </summary>
    public class MusicTrack : IMusicTrack
    {
        public AssetLocation Location;

        /// <summary>
        /// Is it loading?
        /// </summary>
        public bool loading = false;

        public bool ManualDispose = false;

        /// <summary>
        /// Get the current sound file.
        /// </summary>
        public ILoadedSound Sound;

        /// <summary>
        /// Is the current song actively playing or is it loading? (False if neither action.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return ForceActive || loading || (Sound != null && Sound.IsPlaying);
            }
        }

        /// <summary>
        /// The current song's priority.
        /// </summary>
        public float Priority { get; set; } = 1f;

        /// <summary>
        /// The name of the track.
        /// </summary>
        public string Name { get { return Location.ToShortString(); } }


        /// <summary>
        /// Core client API.
        /// </summary>
        IMusicEngine musicEngine;

        bool docontinue = true;

        /// <summary>
        /// If true, the music is considered active/playing until set to false and so no other track will play
        /// </summary>
        public bool ForceActive;

        /// <summary>
        /// Stops the track immediately
        /// </summary>
        public void Stop()
        {
            docontinue = false;
            Sound?.Stop();
            musicEngine.StopTrack(this);
        }


        /// <summary>
        /// Initialize the track.
        /// </summary>
        /// <param name="assetManager">the global Asset Manager</param>
        /// <param name="capi">The Core Client API</param>
        /// <param name="musicEngine"></param>
        public virtual void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine)
        {
            this.musicEngine = musicEngine;
            Location.Path = Location.Path.ToLowerInvariant();
            if (!Location.PathStartsWith("sounds")) Location.WithPathPrefixOnce("music/");
            Location.WithPathAppendixOnce(".ogg");
        }

        /// <summary>
        /// Should this current track play?
        /// </summary>
        /// <param name="props">Player Properties</param>
        /// <param name="conds"></param>
        /// <param name="pos"></param>
        /// <returns>Should we play the current track?</returns>
        public virtual bool ShouldPlay(TrackedPlayerProperties props, ClimateCondition conds, BlockPos pos)
        {
            if (IsActive) return false;
            return true;
        }

        /// <summary>
        /// Begins playing the Music track.
        /// </summary>
        /// <param name="props">Player Properties</param>
        public virtual void BeginPlay(TrackedPlayerProperties props)
        {
            loading = true;
            musicEngine.LoadTrack(Location, (sound) => {
                if (sound != null)
                {
                    sound.Start();
                    if (!loading) { sound.Stop(); sound.Dispose(); }
                    else Sound = sound;
                }
                loading = false;
            });
        }

        /// <summary>
        /// Is it cool for the current track to continue playing?
        /// </summary>
        /// <param name="dt">Delta Time/Change in time.</param>
        /// <param name="props">Track properties.</param>
        /// <returns>Cool or not cool?</returns>
        public virtual bool ContinuePlay(float dt, TrackedPlayerProperties props)
        {
            if (ForceActive) return true;

            if (!IsActive && !ManualDispose)
            {
                Sound?.Dispose();
                Sound = null;
                return false;
            }

            return docontinue;
        }

        /// <summary>
        /// Fades out the current track.  
        /// </summary>
        /// <param name="seconds">The duration of the fade out in seconds.</param>
        /// <param name="onFadedOut">What to have happen after the track has faded out.</param>
        public virtual void FadeOut(float seconds, Action onFadedOut = null)
        {
            loading = false;

            if (Sound != null && IsActive)
            {
                Sound.FadeOut(seconds, (sound) => {
                    sound.Dispose();
                    Sound = null;
                    onFadedOut?.Invoke();
                });
                
                return;
            }
        }

        /// <summary>
        /// Updates the volume of the current track provided Sound is not null. (effectively calls Sound.SetVolume)
        /// </summary>
        public virtual void UpdateVolume()
        {
            if (Sound != null)
            {
                Sound.SetVolume();
            }
        }

        public void FastForward(float seconds)
        {
            Sound.PlaybackPosition += seconds;
        }

        public virtual void BeginSort()
        {
            
        }

        public string PositionString
        {
            get
            {
                return string.Format("{0}/{1}", Sound.PlaybackPosition, Sound.SoundLengthSeconds);
            }
        }

        public virtual float StartPriority => Priority;
    }
}
