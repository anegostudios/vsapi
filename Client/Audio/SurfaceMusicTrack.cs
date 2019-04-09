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
    [JsonObject(MemberSerialization.OptIn)]
    public class SurfaceMusicTrack : IMusicTrack
    {
        /// <summary>
        /// The location of the track.
        /// </summary>
        [JsonProperty("File")]
        public AssetLocation Location;

        /// <summary>
        /// The current play style of the track
        /// </summary>
        [JsonProperty]
        public string Playstyle = "*";

        /// <summary>
        /// Minimum sunlight to play the track.
        /// </summary>
        [JsonProperty]
        public int MinSunlight = 5;

        /// <summary>
        /// Earliest to play the track.
        /// </summary>
        [JsonProperty]
        public float? MinHour = 0;

        /// <summary>
        /// Latest to play the track.
        /// </summary>
        [JsonProperty]
        public float? MaxHour = 1;

        /// <summary>
        /// Is it loading?
        /// </summary>
        bool loading = false;

        /// <summary>
        /// Get the current sound file.
        /// </summary>
        public ILoadedSound Sound;
        
        /// <summary>
        /// Is the current song actively playing or is it loading? (False if neither action.
        /// </summary>
        public bool IsActive {
            get {
                return loading || (Sound != null && Sound.IsPlaying);
            }
        }

        /// <summary>
        /// The current song's priority.
        /// </summary>
        public float Priority { get { return 1f; } }

        /// <summary>
        /// The name of the track.
        /// </summary>
        public string Name { get { return Location.ToShortString(); } }

        /// <summary>
        /// The music seed for random values.
        /// </summary>
        static Random rand = new Random();

        /// <summary>
        /// Cooldowns between songs. First value is the minimum delay, second value is added randomness. (in seconds)
        /// </summary>
        static readonly float[][] AnySongCoolDowns = new float[][] {
            // Rare
            new float[] { 16*60, 8*60 },
            // Sometimes
            new float[] { 8*60, 4*60 },
            // Often
            new float[] { 4*60, 2*60 }
        };

        /// <summary>
        /// Time before we play the same song again. First value is the minimum delay, second value is added randomness. (in seconds)
        /// </summary>
        static readonly float[] SameSongCoolDown = new float[] { 25 * 60, 20 * 60 };

        /// <summary>
        /// Is this track initialized?
        /// </summary>
        static bool initialized = false;

        /// <summary>
        /// Global cooldown until next track
        /// </summary>
        static long globalCooldownUntilMs;

        /// <summary>
        /// Cooldown for each track by name.
        /// </summary>
        static Dictionary<string, long> tracksCooldownUntilMs = new Dictionary<string, long>();

        /// <summary>
        /// Core client API.
        /// </summary>
        ICoreClientAPI capi;
        IMusicEngine musicEngine;

        /// <summary>
        /// Static initializer.
        /// </summary>
        static SurfaceMusicTrack() {
            
        }

        /// <summary>
        /// Gets the previous frequency setting.
        /// </summary>
        static int prevFrequency;

        /// <summary>
        /// Gets the current Music Frequency setting.
        /// </summary>
        public int MusicFrequency
        {
            get { return capi.Settings.Int["musicFrequency"]; }
        }

        /// <summary>
        /// Initialize the track.
        /// </summary>
        /// <param name="assetManager">the global Asset Manager</param>
        /// <param name="capi">The Core Client API</param>
        public void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine)
        {
            this.capi = capi;
            this.musicEngine = musicEngine;

            Location.Path = "music/" + Location.Path.ToLowerInvariant() + ".ogg";

            if (!initialized)
            {
                globalCooldownUntilMs = (long)(1000 * (AnySongCoolDowns[MusicFrequency][0] / 4 + rand.NextDouble() * AnySongCoolDowns[MusicFrequency][1] / 2));

                capi.Settings.Int.AddWatcher("musicFrequency", (newval) => { FrequencyChanged(newval, capi); });


                initialized = true;

                prevFrequency = MusicFrequency;
            }   
        }

        /// <summary>
        /// The Frequency change in the static system.
        /// </summary>
        /// <param name="newFreq">The new frequency</param>
        /// <param name="capi">the core client API</param>
        private static void FrequencyChanged(int newFreq, ICoreClientAPI capi)
        {
            if (newFreq > prevFrequency)
            {
                globalCooldownUntilMs = 0;
            }
            if (newFreq < prevFrequency)
            {
                globalCooldownUntilMs = (long)(capi.World.ElapsedMilliseconds + 1000 * (AnySongCoolDowns[newFreq][0] / 4 + rand.NextDouble() * AnySongCoolDowns[newFreq][1] / 2));
            }

            prevFrequency = newFreq;
        }

        /// <summary>
        /// Should this current track play?
        /// </summary>
        /// <param name="props">Player Properties</param>
        /// <returns>Should we play the current track?</returns>
        public bool ShouldPlay(TrackedPlayerProperties props)
        {
            if (IsActive) return false;
            if (capi.World.ElapsedMilliseconds < globalCooldownUntilMs) return false;
            if (Playstyle != "*" && props.Playstyle != Playstyle) return false;
            if (props.sunSlight < MinSunlight) return false;
            if (musicEngine.LastPlayedTrack == this) return false;

            long trackCoolDownMs = 0;
            tracksCooldownUntilMs.TryGetValue(Name, out trackCoolDownMs);
            if (capi.World.ElapsedMilliseconds < trackCoolDownMs)
            {
                //world.Logger.Debug("{0}: On track cooldown ({1}s)", Name, (trackCoolDownMs - world.ElapsedMilliseconds) / 1000);

                return false;
            }

            float hour = capi.World.Calendar.HourOfDay / 24f * capi.World.Calendar.HoursPerDay;
            if (hour < MinHour || hour > MaxHour)
            {
                //world.Logger.Debug("{0}: {1} not inside [{2},{3}]", Name, hour, MinHour, MaxHour);
                return false;
            }

            //world.Logger.Debug("{0}: {1} is inside [{2},{3}]!", Name, hour, MinHour, MaxHour);

            return true;
        }

        /// <summary>
        /// Begins playing the Music track.
        /// </summary>
        /// <param name="props">Player Properties</param>
        public void BeginPlay(TrackedPlayerProperties props)
        {
            loading = true;
            musicEngine.LoadTrack(Location, (sound) => {
                sound.Start();

                if (!loading) { sound.Stop(); sound.Dispose(); }
                else Sound = sound;
                loading = false;
            });
        }

        /// <summary>
        /// Is it cool for the current track to continue playing?
        /// </summary>
        /// <param name="dt">Delta Time/Change in time.</param>
        /// <param name="props">Track properties.</param>
        /// <returns>Cool or not cool?</returns>
        public bool ContinuePlay(float dt, TrackedPlayerProperties props)
        {
            if (!IsActive)
            {
                Sound?.Dispose();
                Sound = null;
                SetCooldown(1f);
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Fades out the current track.  
        /// </summary>
        /// <param name="seconds">The duration of the fade out in seconds.</param>
        /// <param name="onFadedOut">What to have happen after the track has faded out.</param>
        public void FadeOut(float seconds, Common.Action onFadedOut = null)
        {
            loading = false;

            if (Sound != null && IsActive)
            {
                Sound.FadeOut(seconds, (sound) => {
                    sound.Dispose();
                    Sound = null;
                    onFadedOut?.Invoke();
                });

                // Half cool down when interupted
                SetCooldown(0.5f);
                return;
            }

            // Full cooldown when stopped normally
            SetCooldown(1f);
        }

        /// <summary>
        /// Sets the cooldown of the current track.
        /// </summary>
        /// <param name="multiplier">The multiplier for the cooldown.</param>
        public void SetCooldown(float multiplier)
        {
            globalCooldownUntilMs = (long)(capi.World.ElapsedMilliseconds + (long)(1000 * (AnySongCoolDowns[MusicFrequency][0] + rand.NextDouble() * AnySongCoolDowns[MusicFrequency][1])) * multiplier);

            tracksCooldownUntilMs[Name] = (long)(capi.World.ElapsedMilliseconds + (long)(1000 * (SameSongCoolDown[0] + rand.NextDouble() * SameSongCoolDown[1])) * multiplier);
        }

        /// <summary>
        /// Updates the volume of the current track provided Sound is not null. (effectively calls Sound.SetVolume)
        /// </summary>
        public void UpdateVolume()
        {
            if (Sound != null)
            {
                Sound.SetVolume();
            }
        }
    }
}
