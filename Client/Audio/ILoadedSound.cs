using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Represents a loaded game sound
    /// </summary>
    public interface ILoadedSound : IDisposable
    {
        /// <summary>
        /// Length of the sound in seconds
        /// </summary>
        float SoundLengthSeconds { get; }

        /// <summary>
        /// Get the current playback position or set it (in seconds)
        /// </summary>
        float PlaybackPosition { get; set; }

        /// <summary>
        /// Is the sound disposed of?
        /// </summary>
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
        /// Amount of audio channels this sound has
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// The params the sound was created with.
        /// </summary>
        SoundParams Params { get; }
        bool IsPaused { get; }

        bool IsReady { get; }

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
        /// Allows you to modify the pitch of the sound. May also be called while the sound is currently playing.
        /// </summary>
        /// <param name="val"></param>
        void SetPitch(float val);

        /// <summary>
        /// Allows you to modify the pitch of the sound. May also be called while the sound is currently playing. This value is added together with the normal pitch level. This method is currently used to distort sound during low temporal stability
        /// </summary>
        /// <param name="val"></param>
        void SetPitchOffset(float val);

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
        void SetPosition(float x, float y, float z);

        void SetLooping(bool on);

        /// <summary>
        /// Fades the sounds volumne to given value
        /// </summary>
        /// <param name="newVolume"></param>
        /// <param name="duration"></param>
        /// <param name="onFaded">Called when the fade has completed. If in the meantime another FadeXXX call has been made, the method is not called</param>
        void FadeTo(double newVolume, float duration, Action<ILoadedSound> onFaded);

        /// <summary>
        /// Causes the sound to fade out
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFadedOut">Called when the fade out has completed. If in the meantime another FadeXXX call has been made, the method is not called</param>
        void FadeOut(float seconds, Action<ILoadedSound> onFadedOut);

        /// <summary>
        /// Causes the sound to fade in
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFadedIn">Called when the fade in has completed. If in the meantime another FadeXXX call has been made, the method is not called</param>
        void FadeIn(float seconds, Action<ILoadedSound> onFadedIn);


        /// <summary>
        /// Causes the sound to fade out and stop the track
        /// </summary>
        /// <param name="seconds"></param>
        void FadeOutAndStop(float seconds);
        void SetLowPassfiltering(float value);
        void SetReverb(float reverbDecayTime);
        bool HasReverbStopped(long elapsedMilliseconds);
    }
}
