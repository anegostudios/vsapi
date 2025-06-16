using Newtonsoft.Json;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Represent a dynamically composed track made out of individual small pieces of music mixed together defined by specific rules
    /// Requirements:
    /// - Start/Stop Multiple Trackpieces
    /// - Set their volumne dynamically
    /// - Decide which Trackpieces to play
    /// - Allow individual rules per Trackpiece
    /// Specific examples:
    /// - Play Thunder ambient only if thunderlevel above 10
    ///   - Thunder ambient volume based on thunderlevel (between 0.3 and 1.1?)
    /// - Play Aquatic Drone only when y below 60
    /// - Play Deep Drone only when y below 50
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CaveMusicTrack : IMusicTrack
    {
        Random rand = new Random();

        [JsonProperty]
        MusicTrackPart[] Parts = null;

        MusicTrackPart[] PartsShuffled;

        int maxSimultaenousTracks = 3;
        float simultaenousTrackChance = 0.01f;
        float priority = 2f;

        long activeUntilMs;
        long cooldownUntilMs;
        ICoreClientAPI capi;
        IMusicEngine musicEngine;


        // Updated by BehaviorTemporalStabilityAffected
        public static bool ShouldPlayCaveMusic = true;

        /// <summary>
        /// The name of the music track.
        /// </summary>
        public string Name { get {
                string active = "";
                for (int i = 0; i < Parts.Length; i++)
                {
                    if (Parts[i].IsPlaying)
                    {
                        if (active.Length > 0)
                        {
                            active += ", ";
                        }
                        active += Parts[i].NowPlayingFile.GetName();
                    }
                }
                return "Cave Mix ("+active+")";
        } }

        /// <summary>
        /// When playing cave sounds, play between 4-10 minutes each time
        /// </summary>
        double SessionPlayTime
        {
            get { return 4 * 60 + 6 * 60 * rand.NextDouble(); }
        }

        /// <summary>
        /// Is the track active?
        /// </summary>
        public bool IsActive
        {
            get
            {
                foreach (MusicTrackPart part in Parts)
                {
                    if (part.IsPlaying || part.Loading) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// The priority of the track.
        /// </summary>
        float IMusicTrack.Priority
        {
            get { return priority; }
        }

        public string PositionString => "?";

        public float StartPriority => priority;

        /// <summary>
        /// Initializes the music track
        /// </summary>
        /// <param name="assetManager">the global Asset Manager</param>
        /// <param name="capi">The Core Client API</param>
        /// <param name="musicEngine"></param>
        public void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine)
        {
            this.capi = capi;
            this.musicEngine = musicEngine;
            PartsShuffled = new MusicTrackPart[Parts.Length];

            for (int i = 0; i < Parts.Length; i++)
            {
                AssetLocation[] filesbefore = (AssetLocation[])Parts[i].Files.Clone();
                Parts[i].ExpandFiles(assetManager);
                if (filesbefore.Length > 0 && Parts[i].Files.Length == 0)
                {
                    capi.Logger.Warning("No files for cave music track part? Will not play anything (first file = {0}).", filesbefore[0]);
                }

                PartsShuffled[i] = Parts[i];
            }
        }

        /// <summary>
        /// Should the game play this track?
        /// </summary>
        /// <param name="props">The properties of the current track.</param>
        /// <param name="conds"></param>
        /// <param name="pos"></param>
        /// <returns>Do we play this track?</returns>
        public bool ShouldPlay(TrackedPlayerProperties props, ClimateCondition conds, BlockPos pos)
        {
            if (props.sunSlight > 3 || !ShouldPlayCaveMusic) return false;
            if (capi.World.ElapsedMilliseconds < cooldownUntilMs) return false;

            return true;
        }

        /// <summary>
        /// Starts playing the track.
        /// </summary>
        /// <param name="props">The properties of the current track.</param>
        public void BeginPlay(TrackedPlayerProperties props)
        {
            activeUntilMs = capi.World.ElapsedMilliseconds + (int)(SessionPlayTime * 1000);
        }

        /// <summary>
        /// Do we continue playing this track?
        /// </summary>
        /// <param name="dt">Delta time or Change in time</param>
        /// <param name="props">The properties of the current track.</param>
        /// <returns>Are we still playing or do we stop?</returns>
        public bool ContinuePlay(float dt, TrackedPlayerProperties props)
        {
            if (props.sunSlight > 3 || !ShouldPlayCaveMusic)
            {
                FadeOut(3);
                return false;
            }

            if (activeUntilMs > 0 && capi.World.ElapsedMilliseconds >= activeUntilMs)
            {
                // Ok, time to stop. We play the current tracks until the end and stop
                bool active = IsActive;
                if (!active)
                {
                    activeUntilMs = 0;
                    foreach (MusicTrackPart part in Parts)
                    {
                        part.Sound?.Dispose();
                    }
                }
                return active;
            }

            int quantityActive = 0;
            for (int i = 0; i < Parts.Length; i++)
            {
                quantityActive += (Parts[i].IsPlaying || Parts[i].Loading) ? 1: 0;
            }

            int beforePlaying = quantityActive;

            GameMath.Shuffle(rand, PartsShuffled);

            for (int i = 0; i < PartsShuffled.Length; i++)
            {
                MusicTrackPart part = PartsShuffled[i];
                if (part.Files.Length == 0) continue;

                bool isPlaying = part.IsPlaying;
                bool shouldPlay = part.Applicable(capi.World, props);

                // Part has recently ended
                if (!isPlaying && part.Sound != null)
                {
                    part.Sound.Dispose();
                    part.Sound = null;
                    continue;
                }

                // Part should be stopped
                if (isPlaying && !shouldPlay)
                {
                    if (!part.Sound.IsFadingOut)
                    {
                        part.Sound.FadeOut(3, (sound) => { part.Sound.Dispose(); part.Sound = null; });
                    }
                    continue;
                }


                bool shouldStart =
                    !isPlaying && 
                    shouldPlay && 
                    !part.Loading &&
                    quantityActive < maxSimultaenousTracks && 
                    (quantityActive == 0 || rand.NextDouble() < simultaenousTrackChance)
                ;

                if (shouldStart)
                {
                    AssetLocation location = part.Files[rand.Next(part.Files.Length)];
                    part.NowPlayingFile = location;
                    part.Loading = true;
                    musicEngine.LoadTrack(location, (sound) => {
                        if (sound != null)
                        {
                            sound.Start();
                            part.Sound = sound;
                        }
                        part.Loading = false;
                    });

                    part.StartedMs = capi.World.ElapsedMilliseconds;
                    quantityActive++;
                }
            }

            return true;
        }

        /// <summary>
        /// Fade out the track to end.
        /// </summary>
        /// <param name="seconds">Seconds to fade out across.</param>
        /// <param name="onFadedOut">Delegate to have happen once the fade-out is done.</param>
        public void FadeOut(float seconds, Action onFadedOut = null)
        {
            bool wasInterupted = false;

            foreach (MusicTrackPart part in Parts)
            {
                if (part.IsPlaying)
                {
                    part.Sound.FadeOut(seconds, (sound) => {
                        sound.Dispose();
                        part.Sound = null;
                        onFadedOut?.Invoke();
                    });

                    wasInterupted = true;
                }
            }

            // When naturally stopped, give the player a break from the cave sounds (2-7 minutes)
            if (!wasInterupted)
            {
                cooldownUntilMs = capi.World.ElapsedMilliseconds + (long)(1000 * (2*60 + rand.NextDouble() * 5*60));
            }
        }
        
        /// <summary>
        /// Updates the volume of the track.
        /// </summary>
        public void UpdateVolume()
        {
            foreach (MusicTrackPart part in Parts)
            {
                if (part.IsPlaying) part.Sound.SetVolume();
            }
                
        }

        public void FastForward(float seconds)
        {
            
        }

        public void BeginSort()
        {
            
        }
    }



}
