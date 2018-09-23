using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
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
        [JsonProperty]
        public float EaseInSpeed = 10f;
        [JsonProperty]
        public float EaseOutSpeed = 10f;
        [JsonProperty]
        public AnimationTrigger TriggeredBy;
        [JsonProperty]
        public EnumAnimationBlendMode BlendMode = EnumAnimationBlendMode.Add;
        [JsonProperty]
        public Dictionary<string, EnumAnimationBlendMode> ElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>();

        int withActivitiesMerged;
        public uint CodeCrc32;
        public bool WasStartedFromTrigger;

        public float GetCurrentAnimationSpeed(float walkspeed)
        {
            return AnimationSpeed * (MulWithWalkSpeed ? walkspeed : 1);
        }

        public AnimationMetaData Init()
        {
            withActivitiesMerged = 0;
            for (int i = 0; TriggeredBy?.OnControls != null && i < TriggeredBy.OnControls.Length; i++)
            {
                withActivitiesMerged |= (int)TriggeredBy.OnControls[i];
            }

            Animation = Animation.ToLowerInvariant();
            CodeCrc32 = GameMath.Crc32(Animation);
            if (Code == null) Code = Animation;

            return this;
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
                WasStartedFromTrigger = this.WasStartedFromTrigger
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
                writer.Write(TriggeredBy.OnControls.Length);
                for (int i = 0; i < TriggeredBy.OnControls.Length; i++)
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
        }

        public static AnimationMetaData FromBytes(BinaryReader reader)
        {
            AnimationMetaData animdata = new AnimationMetaData();

            animdata.Code = reader.ReadString();
            animdata.Animation = reader.ReadString();
            animdata.Weight = reader.ReadSingle();

            int c = reader.ReadInt32();
            for (int i = 0; i < c; i++)
            {
                animdata.ElementWeight[reader.ReadString()] = reader.ReadSingle();
            }

            animdata.AnimationSpeed = reader.ReadSingle();
            animdata.EaseInSpeed = reader.ReadSingle();
            animdata.EaseOutSpeed = reader.ReadSingle();

            bool b = reader.ReadBoolean();
            if (b)
            {
                animdata.TriggeredBy = new AnimationTrigger();
                animdata.TriggeredBy.MatchExact = reader.ReadBoolean();
                c = reader.ReadInt32();
                animdata.TriggeredBy.OnControls = new EnumEntityActivity[c];
                for (int i = 0; i < c; i++)
                {
                    animdata.TriggeredBy.OnControls[i] = (EnumEntityActivity)reader.ReadInt32();
                }
                animdata.TriggeredBy.DefaultAnim = reader.ReadBoolean();
            }

            animdata.BlendMode = (EnumAnimationBlendMode)reader.ReadInt32();

            c = reader.ReadInt32();
            for (int i = 0; i < c; i++)
            {
                animdata.ElementBlendMode[reader.ReadString()] = (EnumAnimationBlendMode)reader.ReadInt32();
            }

            animdata.MulWithWalkSpeed = reader.ReadBoolean();

            return animdata;
        }
    }


}
