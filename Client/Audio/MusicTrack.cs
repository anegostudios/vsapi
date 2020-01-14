using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

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
                return loading || (Sound != null && Sound.IsPlaying);
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
        /// The music seed for random values.
        /// </summary>
        static Random rand = new Random();
        
        /// <summary>
        /// Core client API.
        /// </summary>
        ICoreClientAPI capi;
        IMusicEngine musicEngine;

        bool docontinue = true;

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

            this.capi = capi;
            this.musicEngine = musicEngine;
            Location.Path = Location.Path.ToLowerInvariant();
            if (!Location.Path.StartsWith("sounds")) Location.WithPathPrefixOnce("music/");
            Location.WithPathAppendixOnce(".ogg");
        }

        /// <summary>
        /// Should this current track play?
        /// </summary>
        /// <param name="props">Player Properties</param>
        /// <returns>Should we play the current track?</returns>
        public virtual bool ShouldPlay(TrackedPlayerProperties props)
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
            if (!IsActive)
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
        public virtual void FadeOut(float seconds, Common.Action onFadedOut = null)
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

        public string PositionString
        {
            get
            {
                return string.Format("{0}/{1}", Sound.PlaybackPosition, Sound.SoundLengthSeconds);
            }
        }
    }
}
