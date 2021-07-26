using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Util
{
    public enum EnumTalkType
    {
        Meet,
        Idle,
        Hurt,
        Hurt2,
        Death,
        Purchase,
        Complain,
        Goodbye,
        IdleShort,
        Laugh
    }

    public class SlidingPitchSound
    {
        public ILoadedSound sound;
        public float startPitch;
        public float endPitch;
        public long startMs;
    }

    public class EntityTalkUtil
    {
        protected int lettersLeftToTalk = 0;
        protected int totalLettersToTalk = 0;

        protected int currentLetterInWord = 0;
        protected int totalLettersTalked = 0;

        protected float chordDelay = 0f;

        protected Dictionary<EnumTalkType, float> TalkSpeed;


        protected EnumTalkType talkType;

        protected ICoreClientAPI capi;
        protected Entity entity;

        public AssetLocation soundName = new AssetLocation("sounds/voice/saxophone");

        protected List<SlidingPitchSound> slidingPitchSounds = new List<SlidingPitchSound>();
        protected List<SlidingPitchSound> stoppedSlidingSounds = new List<SlidingPitchSound>();

        public float talkSpeedModifier = 1;
        public float pitchModifier = 1;
        public float volumneModifier = 1;
        public float idleTalkChance = 0.0005f;

        public EntityTalkUtil(ICoreClientAPI capi, Entity atEntity)
        {
            this.capi = capi;
            this.entity = atEntity;
            TalkSpeed = defaultTalkSpeeds();
        }

        public virtual void SetModifiers(float talkSpeedModifier = 1, float pitchModifier = 1, float volumneModifier = 1)
        {
            this.talkSpeedModifier = talkSpeedModifier;
            this.pitchModifier = pitchModifier;
            this.volumneModifier = volumneModifier;
            TalkSpeed = defaultTalkSpeeds();
            foreach (var key in TalkSpeed.Keys.ToArray())
            {
                TalkSpeed[key] = Math.Max(0.06f, TalkSpeed[key] * talkSpeedModifier);
            }
        }

        protected virtual Random Rand { get { return capi.World.Rand; } }

        public virtual void OnGameTick(float dt)
        {
            for (int i = 0; i < slidingPitchSounds.Count; i++)
            {
                SlidingPitchSound sps = slidingPitchSounds[i];
                if (sps.sound.HasStopped)
                {
                    stoppedSlidingSounds.Add(sps);
                    continue;
                }

                float secondspassed = (capi.World.ElapsedMilliseconds - sps.startMs) / 1000f;
                float progress = GameMath.Min(1, 1 - secondspassed / sps.sound.SoundLengthSeconds);
                float pitch = sps.endPitch + (sps.startPitch - sps.endPitch) * progress;
                sps.sound.SetPitch(pitch);
            }

            foreach (var val in stoppedSlidingSounds) slidingPitchSounds.Remove(val);


            if (lettersLeftToTalk > 0)
            {
                chordDelay -= dt;

                if (chordDelay < 0)
                {
                    chordDelay = TalkSpeed[talkType] * talkSpeedModifier;

                    switch (talkType)
                    {
                        case EnumTalkType.Purchase:
                            {
                                float startpitch = 1.5f;
                                float endpitch = totalLettersTalked > 0 ? 0.9f : 1.5f;
                                PlaySound(startpitch, endpitch, 0.5f);
                                chordDelay = 0.3f * talkSpeedModifier;
                            }
                            break;

                        case EnumTalkType.Goodbye:
                            {
                                float pitch = 1.25f - 0.6f * (float)totalLettersTalked / totalLettersToTalk;
                                PlaySound(pitch, pitch * 0.9f, 0.25f);
                                chordDelay = 0.25f * talkSpeedModifier;
                            }
                            break;

                        case EnumTalkType.Death:
                            {
                                float startpitch = 1.25f - 0.6f * (float)totalLettersTalked / totalLettersToTalk;
                                PlaySound(startpitch, startpitch * 0.4f, 0.4f);
                            }

                            break;

                        case EnumTalkType.Meet:
                            {
                                float pitch = 0.75f + 0.5f * (float)Rand.NextDouble() + (float)totalLettersTalked / totalLettersToTalk / 3;
                                PlaySound(pitch, pitch * 1.5f, 0.25f);

                                if (currentLetterInWord > 1 && capi.World.Rand.NextDouble() < 0.35)
                                {
                                    chordDelay = 0.45f * talkSpeedModifier;
                                    currentLetterInWord = 0;
                                }
                                break;
                            }

                        case EnumTalkType.Complain:
                            {
                                float startPitch = 0.75f + 0.5f * (float)Rand.NextDouble();
                                float endPitch = 0.75f + 0.5f * (float)Rand.NextDouble();
                                PlaySound(startPitch, endPitch, 0.25f);

                                if (currentLetterInWord > 1 && capi.World.Rand.NextDouble() < 0.35)
                                {
                                    chordDelay = 0.45f * talkSpeedModifier;
                                    currentLetterInWord = 0;
                                }

                                break;
                            }

                        case EnumTalkType.Idle:
                        case EnumTalkType.IdleShort:
                            {
                                float startPitch = 0.75f + 0.25f * (float)Rand.NextDouble();
                                float endPitch = 0.75f + 0.25f * (float)Rand.NextDouble();
                                PlaySound(startPitch, endPitch, 0.7f);



                                if (currentLetterInWord > 1 && capi.World.Rand.NextDouble() < 0.35)
                                {
                                    chordDelay = 0.55f * talkSpeedModifier;
                                    currentLetterInWord = 0;
                                }
                                break;
                            }

                        case EnumTalkType.Laugh:
                            {
                                float rnd = (float)Rand.NextDouble() * 0.1f;
                                float pfac = (float)Math.Pow(Math.Min(1, 1 / pitchModifier), 2);

                                float startPitch = rnd + 1.3f - currentLetterInWord / (15f / pfac);
                                float endPitch = startPitch - 0.2f;
                                PlaySound(startPitch, endPitch, 0.8f);

                                chordDelay = 0.23f * talkSpeedModifier * pfac;
                                
                                break;
                            }

                        case EnumTalkType.Hurt:
                            {
                                float pitch = 0.75f + 0.5f * (float)Rand.NextDouble() + (1 - (float)totalLettersTalked / totalLettersToTalk);

                                PlaySound(pitch, 0.25f + (1 - (float)totalLettersTalked / totalLettersToTalk) / 2);

                                if (currentLetterInWord > 1 && capi.World.Rand.NextDouble() < 0.35)
                                {
                                    chordDelay = 0.25f * talkSpeedModifier;
                                    currentLetterInWord = 0;
                                }

                                break;
                            }

                        case EnumTalkType.Hurt2:
                            {
                                float pitch = 0.75f + 0.4f * (float)Rand.NextDouble() + (1 - (float)totalLettersTalked / totalLettersToTalk);

                                PlaySound(pitch, 0.25f + (1 - (float)totalLettersTalked / totalLettersToTalk) / 2.5f);

                                if (currentLetterInWord > 1 && capi.World.Rand.NextDouble() < 0.35)
                                {
                                    chordDelay = 0.25f * talkSpeedModifier;
                                    currentLetterInWord = 0;
                                }
                                break;
                            }
                    }


                    lettersLeftToTalk--;
                    currentLetterInWord++;
                    totalLettersTalked++;
                }

                return;
            }



            if (lettersLeftToTalk == 0 && capi.World.Rand.NextDouble() < idleTalkChance && entity.Alive)
            {
                Talk(EnumTalkType.Idle);
            }
        }


        protected virtual void PlaySound(float startpitch, float volume)
        {
            PlaySound(startpitch, startpitch, volume);
        }

        protected virtual void PlaySound(float startPitch, float endPitch, float volume)
        {
            startPitch *= pitchModifier;
            endPitch *= pitchModifier;
            volume *= volumneModifier;

            SoundParams param = new SoundParams()
            {
                Location = soundName,
                DisposeOnFinish = true,
                Pitch = startPitch,
                Volume = volume,
                Position = entity.Pos.XYZ.ToVec3f().Add(0, (float)entity.LocalEyePos.Y, 0),
                ShouldLoop = false,
                Range = 8,
            };

            ILoadedSound sound = capi.World.LoadSound(param);

            if (startPitch != endPitch)
            {
                slidingPitchSounds.Add(new SlidingPitchSound()
                {
                    startPitch = startPitch,
                    endPitch = endPitch,
                    sound = sound,
                    startMs = capi.World.ElapsedMilliseconds
                });
            }


            sound.Start();
        }


        public virtual void Talk(EnumTalkType talkType)
        {
            IClientWorldAccessor world = capi.World as IClientWorldAccessor;

            this.talkType = talkType;
            totalLettersTalked = 0;
            currentLetterInWord = 0;

            chordDelay = TalkSpeed[talkType];

            if (talkType == EnumTalkType.Meet)
            {
                lettersLeftToTalk = 2 + world.Rand.Next(10);
            }

            if (talkType == EnumTalkType.Hurt || talkType == EnumTalkType.Hurt2)
            {
                lettersLeftToTalk = 3 + world.Rand.Next(6);
            }

            if (talkType == EnumTalkType.Idle)
            {
                lettersLeftToTalk = 3 + world.Rand.Next(12);
            }

            if (talkType == EnumTalkType.IdleShort)
            {
                lettersLeftToTalk = 3 + world.Rand.Next(4);
            }

            if (talkType == EnumTalkType.Laugh)
            {
                lettersLeftToTalk = (int)((3 + world.Rand.Next(3)) * Math.Max(1, pitchModifier));
            }

            if (talkType == EnumTalkType.Purchase)
            {
                lettersLeftToTalk = 2 + world.Rand.Next(2);
            }

            if (talkType == EnumTalkType.Complain)
            {
                lettersLeftToTalk = 3 + world.Rand.Next(5);
            }

            if (talkType == EnumTalkType.Goodbye)
            {
                lettersLeftToTalk = 2 + world.Rand.Next(2);
            }

            if (talkType == EnumTalkType.Death)
            {
                lettersLeftToTalk = 2 + world.Rand.Next(2);
            }

            totalLettersToTalk = lettersLeftToTalk;
        }


        Dictionary<EnumTalkType, float> defaultTalkSpeeds()
        {
            return new Dictionary<EnumTalkType, float>()
            {
                { EnumTalkType.Meet, 0.13f },
                { EnumTalkType.Death, 0.3f },
                { EnumTalkType.Idle, 0.2f },
                { EnumTalkType.IdleShort, 0.2f },
                { EnumTalkType.Laugh, 0.2f },
                { EnumTalkType.Hurt, 0.07f },
                { EnumTalkType.Hurt2, 0.07f },
                { EnumTalkType.Goodbye, 0.07f },
                { EnumTalkType.Complain, 0.09f },
                { EnumTalkType.Purchase, 0.15f },
            };
        }
    }
}
