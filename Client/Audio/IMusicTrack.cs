using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Client
{
    public interface IMusicTrack
    {
        string Name { get; }
        bool IsActive { get; }
        float Priority { get; }

        void Initialize(IAssetManager assetManager, ICoreClientAPI capi);

        bool ShouldPlay(TrackedPlayerProperties props, IMusicEngine musicEngine);
        void BeginPlay(TrackedPlayerProperties props, IMusicEngine musicEngine);
        bool ContinuePlay(float dt, TrackedPlayerProperties props, IMusicEngine musicEngine);


        void UpdateVolume();

        /// <summary>
        /// Called when the track to interupted or when Update() returned false. 
        /// So called every time the tracked ended or has to end
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFadedOut"></param>
        void FadeOut(float seconds, Common.Action onFadedOut = null);
    }
}
