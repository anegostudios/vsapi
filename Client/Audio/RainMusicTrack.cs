using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a track for rain related music.  [Not yet implemented]
    /// </summary>
    public class RainMusicTrack : IMusicTrack
    {
        public bool IsActive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Priority
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PositionString => throw new NotImplementedException();

        public float StartPriority => throw new NotImplementedException();

        public void FadeOut(float seconds, Action<ILoadedSound> onFadedOut)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IAssetManager assetManager, ICoreClientAPI capi, IMusicEngine musicEngine)
        {
            throw new NotImplementedException();
        }


        public bool ShouldPlay(TrackedPlayerProperties props, ClimateCondition conds, BlockPos pos)
        {
            
            return false;
        }

        public void BeginPlay(TrackedPlayerProperties props)
        {
            
        }

        public bool ContinuePlay(float dt, TrackedPlayerProperties props)
        {
            // Track should be started
            /*if (shouldStart)
            {
                string filename = part.Files[rand.Next(part.Files.Length)];
                part.Sound = musicEngine.StartTrack(
                    filename, 
                    part.CurrentVolume(world, props) + VolumeRandomization - 2 * rand.Next() * VolumeRandomization,
                    PitchRandomization - 2 * rand.Next() * PitchRandomization
                );
                part.startedMs = world.ElapsedMilliseconds;
                quantityPlaying++;
            }

            if (isPlaying && shouldPlay && RuntimeRandomizationInterval != null && world.ElapsedMilliseconds - part.startedMs > RuntimeRandomizationInterval * 1000)
            {
                part.Sound.SetPitch(part.Sound.Params.Pitch + PitchRandomization - 2 * rand.Next() * PitchRandomization);
                part.Sound.SetVolume(part.Sound.Params.Volume + VolumeRandomization - 2 * rand.Next() * VolumeRandomization);
            }*/
            return false;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void UpdateVolume()
        {
            throw new NotImplementedException();
        }

        public void FadeOut(float seconds, Action onFadedOut = null)
        {
            throw new NotImplementedException();
        }

        public void FastForward(float seconds)
        {
            throw new NotImplementedException();
        }

        public void BeginSort()
        {
            throw new NotImplementedException();
        }
    }
}
