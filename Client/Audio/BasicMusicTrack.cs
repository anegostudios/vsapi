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
    [JsonObject(MemberSerialization.OptIn)]
    public class BasicMusicTrack : IMusicTrack
    {
        [JsonProperty("File")]
        public AssetLocation Location;

        [JsonProperty]
        public string Playstyle = "*";
        [JsonProperty]
        public int MinSunlight = 5;
        [JsonProperty]
        public float? MinHour = 0;
        [JsonProperty]
        public float? MaxHour = 24;

        bool loading = false;

        public ILoadedSound Sound;
        
        public bool IsActive {
            get {
                return loading || (Sound != null && Sound.IsPlaying);
            }
        }
        public float Priority { get { return 1f; } }
        public string Name { get { return Location.ToShortString(); } }


        static Random rand = new Random();
        static float[] globalCoolDown = new float[] { 8*60, 4*60 };
        static float[] ownCoolDown = new float[] { 25 * 60, 20 * 60 };

        static long globalCooldownUntilMs;
        static Dictionary<string, long> tracksCooldownUntilMs = new Dictionary<string, long>();


        IWorldAccessor world;

        static BasicMusicTrack() {
            globalCooldownUntilMs = (long)(1000*(globalCoolDown[0]/4 + rand.NextDouble() * globalCoolDown[1]/2));
        }

        public void Initialize(IAssetManager assetManager, IClientWorldAccessor world)
        {
            this.world = world;
            Location.Path = "music/" + Location.Path.ToLowerInvariant() + ".ogg";
        }

        public bool ShouldPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            if (IsActive) return false;
            if (world.ElapsedMilliseconds < globalCooldownUntilMs) return false;
            if (Playstyle != "*" && props.Playstyle + "" != Playstyle) return false;
            if (props.sunSlight < MinSunlight) return false;
            if (musicEngine.LastPlayedTrack == this) return false;

            long trackCoolDownMs = 0;
            tracksCooldownUntilMs.TryGetValue(Name, out trackCoolDownMs);
            if (world.ElapsedMilliseconds < trackCoolDownMs)
            {
                //world.Logger.Debug("{0}: On track cooldown ({1}s)", Name, (trackCoolDownMs - world.ElapsedMilliseconds) / 1000);

                return false;
            }
            float hour = world.Calendar.HourOfDay;
            if (hour < MinHour || hour > MaxHour)
            {
                //world.Logger.Debug("{0}: {1} not inside [{2},{3}]", Name, hour, MinHour, MaxHour);
                return false;
            }

            //world.Logger.Debug("{0}: {1} is inside [{2},{3}]!", Name, hour, MinHour, MaxHour);

            return true;
        }

        public void BeginPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            loading = true;
            musicEngine.StartTrack(Location, (sound) => {
                if (!loading) { sound.Stop(); sound.Dispose(); }
                else Sound = sound;
                loading = false;
            });
        }

        public bool ContinuePlay(float dt, TrackedPlayerProperties props, IMusicEngine musicEngine)
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


        public void SetCooldown(float multiplier)
        {
            globalCooldownUntilMs = (long)(world.ElapsedMilliseconds + (long)(1000 * (globalCoolDown[0] + rand.NextDouble() * globalCoolDown[1])) * multiplier);

            tracksCooldownUntilMs[Name] = (long)(world.ElapsedMilliseconds + (long)(1000 * (ownCoolDown[0] + rand.NextDouble() * ownCoolDown[1])) * multiplier);
        }




        public void UpdateVolume()
        {
            if (Sound != null)
            {
                Sound.SetVolume();
            }
        }
    }
}
