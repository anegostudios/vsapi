using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Represents a loaded game sound 
    /// </summary>
    public interface ILoadedSound
    {
        bool IsDisposed { get; }

        /// <summary>
        /// Is sound currently playing
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// True if the track is fading to a higher volume
        /// </summary>
        bool IsFadingIn { get; }

        /// <summary>
        /// True if the track is fading to a lower volume
        /// </summary>
        bool IsFadingOut { get; }

        /// <summary>
        /// Is the sound finished with playing? (false when only paused)
        /// </summary>
        bool HasStopped { get; }

        /// <summary>
        /// The params the sound was created with.
        /// </summary>
        SoundParams Params { get; }

        /// <summary>
        /// Starts the sound
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the sound
        /// </summary>
        void Stop();

        /// <summary>
        /// Pauses the sound
        /// </summary>
        void Pause();

        /// <summary>
        /// True for Starting, False for Stopping the sound
        /// </summary>
        /// <param name="on"></param>
        void Toggle(bool on);


        /// <summary>
        /// Dispose the object sound. May no longer be used after disposing. 
        /// </summary>
        void Dispose();

        /// <summary>
        /// Allows you to modify the pitch of the sound. May also be called while the sound is currently playing.
        /// </summary>
        /// <param name="val"></param>
        void SetPitch(float val);

        /// <summary>
        /// Allows you to modify the volumne of the sound. May also be called while the sound is currently playing.
        /// </summary>
        /// <param name="val"></param>
        void SetVolume(float val);

        /// <summary>
        /// Sets the current volumne again. Use this to update the sounds volumne after the global sound level has changed
        /// </summary>
        void SetVolume();

        /// <summary>
        /// Sets the position from where the sound is originating from
        /// </summary>
        /// <param name="position"></param>
        void SetPosition(Vec3f position);

        /// <summary>
        /// Fades the sounds volumne to given value
        /// </summary>
        /// <param name="newVolume"></param>
        /// <param name="duration"></param>
        /// <param name="onFaded"></param>
        void FadeTo(float newVolume, float duration, Action<ILoadedSound> onFaded);

        /// <summary>
        /// Causes the sound to fade out 
        /// </summary>
        /// <param name="seconds"></param>
        void FadeOut(float seconds, Action<ILoadedSound> onFadedOut);

        /// <summary>
        /// Causes the sound to fade in
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFadedIn"></param>
        void FadeIn(float seconds, Action<ILoadedSound> onFadedIn);


        /// <summary>
        /// Causes the sound to fade out and stop the track
        /// </summary>
        /// <param name="seconds"></param>
        void FadeOutAndStop(float seconds);
    }
}
