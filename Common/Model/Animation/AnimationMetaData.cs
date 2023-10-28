using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public enum EnumAnimationBlendMode
    {
        /// <summary>
        /// Add the animation without taking other animations into considerations
        /// </summary>
        Add,
        /// <summary>
        /// Add the pose and average it together with all other running animations with blendmode Average or AddAverage
        /// </summary>
        Average,
        /// <summary>
        /// Add the animation without taking other animations into consideration, but add it's weight for averaging 
        /// </summary>
        AddAverage
    }

    public class AnimationTrigger
    {
        [JsonProperty]
        public EnumEntityActivity[] OnControls;
        [JsonProperty]
        public bool MatchExact = false;
        [JsonProperty]
        public bool DefaultAnim = false;

        public AnimationTrigger Clone()
        {
            return new AnimationTrigger()
            {
                OnControls = (EnumEntityActivity[])this.OnControls?.Clone(),
                MatchExact = this.MatchExact,
                DefaultAnim = this.DefaultAnim
            };
        }
    }

    public class AnimationMetaData
    {
        /// <summary>
        /// Unique identifier to be able to reference this AnimationMetaData instance
        /// </summary>
        [JsonProperty]
        public string Code;
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;
        /// <summary>
        /// The animations code identifier that we want to play
        /// </summary>
        [JsonProperty]
        public string Animation;
        [JsonProperty]
        public float Weight = 1f;
        [JsonProperty]
        public Dictionary<string, float> ElementWeight = new Dictionary<string, float>();
        [JsonProperty]
        public float AnimationSpeed = 1f;
        [JsonProperty]
        public bool MulWithWalkSpeed = false;
        /// <summary>
        /// A multiplier applied to the weight value to "ease in" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        public float EaseInSpeed = 10f;
        /// <summary>
        /// A multiplier applied to the weight value to "ease out" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        public float EaseOutSpeed = 10f;
        [JsonProperty]
        public AnimationTrigger TriggeredBy;
        [JsonProperty]
        public EnumAnimationBlendMode BlendMode = EnumAnimationBlendMode.Add;
        [JsonProperty]
        public Dictionary<string, EnumAnimationBlendMode> ElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>(StringComparer.OrdinalIgnoreCase);
        [JsonProperty]
        public bool SupressDefaultAnimation = false;
        [JsonProperty]
        public float HoldEyePosAfterEasein = 99f;
        /// <summary>
        /// If true, the server does not sync this animation
        /// </summary>
        [JsonProperty]
        public bool ClientSide;

        public float StartFrameOnce;

        int withActivitiesMerged;
        public uint CodeCrc32;
        public bool WasStartedFromTrigger;
        

        public float GetCurrentAnimationSpeed(float walkspeed)
        {
            return AnimationSpeed * (MulWithWalkSpeed ? walkspeed : 1) * GlobalConstants.OverallSpeedMultiplier;
        }

        public AnimationMetaData Init()
        {
            withActivitiesMerged = 0;
            for (int i = 0; TriggeredBy?.OnControls != null && i < TriggeredBy.OnControls.Length; i++)
            {
                withActivitiesMerged |= (int)TriggeredBy.OnControls[i];
            }

            CodeCrc32 = GetCrc32(Code);

            return this;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Animation = Animation.ToLowerInvariant();
            if (Code == null) Code = Animation;

            CodeCrc32 = GetCrc32(Code); 
        }


        public static uint GetCrc32(string animcode)
        {
            int mask = ~(1 << 31); // Because I fail to get the sign bit transmitted correctly over the network T_T
            return (uint)(GameMath.Crc32(animcode.ToLowerInvariant()) & mask);
        }

        public bool Matches(int currentActivities)
        {
            return (TriggeredBy?.MatchExact == true) ? (currentActivities == withActivitiesMerged) : ((currentActivities & withActivitiesMerged) > 0);
        }

        public AnimationMetaData Clone()
        {
            return new AnimationMetaData()
            {
                Code = this.Code,
                Animation = this.Animation,
                Weight = this.Weight,
                Attributes = this.Attributes?.Clone(),
                ClientSide = this.ClientSide,
                ElementWeight = new Dictionary<string, float>(this.ElementWeight),
                AnimationSpeed = this.AnimationSpeed,
                MulWithWalkSpeed = this.MulWithWalkSpeed,
                EaseInSpeed = this.EaseInSpeed,
                EaseOutSpeed = this.EaseOutSpeed,
                TriggeredBy = this.TriggeredBy?.Clone(),
                BlendMode = this.BlendMode,
                ElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>(this.ElementBlendMode),
                withActivitiesMerged = this.withActivitiesMerged,
                CodeCrc32 = this.CodeCrc32,
                WasStartedFromTrigger = this.WasStartedFromTrigger,
                HoldEyePosAfterEasein = HoldEyePosAfterEasein,
                StartFrameOnce = StartFrameOnce,
                SupressDefaultAnimation = SupressDefaultAnimation
            };
        }

        public override bool Equals(object obj)
        {
            AnimationMetaData other = obj as AnimationMetaData;

            return other != null && other.Animation == Animation && other.AnimationSpeed == AnimationSpeed && other.BlendMode == BlendMode;
        }

        public override int GetHashCode()
        {
            return Animation.GetHashCode() ^ AnimationSpeed.GetHashCode() ^ BlendMode.GetHashCode();
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Animation);
            writer.Write(Weight);

            writer.Write(ElementWeight.Count);
            foreach (var val in ElementWeight)
            {
                writer.Write(val.Key);
                writer.Write(val.Value);
            }

            writer.Write(AnimationSpeed);
            writer.Write(EaseInSpeed);
            writer.Write(EaseOutSpeed);


            writer.Write(TriggeredBy != null);
            if (TriggeredBy != null)
            {
                writer.Write(TriggeredBy.MatchExact);

                writer.Write(TriggeredBy.OnControls == null ? 0 : TriggeredBy.OnControls.Length);
                for (int i = 0; TriggeredBy.OnControls != null && i < TriggeredBy.OnControls.Length; i++)
                {
                    writer.Write((int)TriggeredBy.OnControls[i]);
                }
                writer.Write(TriggeredBy.DefaultAnim);
            }


            writer.Write((int)BlendMode);

            writer.Write(ElementBlendMode.Count);
            foreach (var val in ElementBlendMode)
            {
                writer.Write(val.Key);
                writer.Write((int)val.Value);
            }

            writer.Write(MulWithWalkSpeed);
            writer.Write(StartFrameOnce);

            writer.Write(HoldEyePosAfterEasein);
            writer.Write(ClientSide);
            writer.Write(Attributes?.ToString() ?? "");
        }

        public static AnimationMetaData FromBytes(BinaryReader reader, string version)
        {
            AnimationMetaData animdata = new AnimationMetaData();

            animdata.Code = reader.ReadString();
            animdata.Animation = reader.ReadString();
            animdata.Weight = reader.ReadSingle();

            int weightCount = reader.ReadInt32();
            for (int i = 0; i < weightCount; i++)
            {
                animdata.ElementWeight[reader.ReadString()] = reader.ReadSingle();
            }

            animdata.AnimationSpeed = reader.ReadSingle();
            animdata.EaseInSpeed = reader.ReadSingle();
            animdata.EaseOutSpeed = reader.ReadSingle();

            bool haveTrigger = reader.ReadBoolean();
            if (haveTrigger)
            {
                animdata.TriggeredBy = new AnimationTrigger();
                animdata.TriggeredBy.MatchExact = reader.ReadBoolean();
                weightCount = reader.ReadInt32();
                animdata.TriggeredBy.OnControls = new EnumEntityActivity[weightCount];

                for (int i = 0; i < weightCount; i++)
                {
                    animdata.TriggeredBy.OnControls[i] = (EnumEntityActivity)reader.ReadInt32();
                }
                animdata.TriggeredBy.DefaultAnim = reader.ReadBoolean();
            }

            animdata.BlendMode = (EnumAnimationBlendMode)reader.ReadInt32();

            weightCount = reader.ReadInt32();
            for (int i = 0; i < weightCount; i++)
            {
                animdata.ElementBlendMode[reader.ReadString()] = (EnumAnimationBlendMode)reader.ReadInt32();
            }

            animdata.MulWithWalkSpeed = reader.ReadBoolean();

            if (GameVersion.IsAtLeastVersion(version, "1.12.5-dev.1"))
            {
                animdata.StartFrameOnce = reader.ReadSingle();
            }
            if (GameVersion.IsAtLeastVersion(version, "1.13.0-dev.3"))
            {
               animdata.HoldEyePosAfterEasein = reader.ReadSingle();
            }
            if (GameVersion.IsAtLeastVersion(version, "1.17.0-dev.18"))
            {
                animdata.ClientSide = reader.ReadBoolean();
            }
            if (GameVersion.IsAtLeastVersion(version, "1.19.0-dev.20"))
            {
                string attributes = reader.ReadString();
                if (attributes != "")
                {
                    animdata.Attributes = new JsonObject(JToken.Parse(attributes));
                } else
                {
                    animdata.Attributes = new JsonObject(JToken.Parse("{}"));
                }
            }


            animdata.Init();

            return animdata;
        }

        
    }


}
