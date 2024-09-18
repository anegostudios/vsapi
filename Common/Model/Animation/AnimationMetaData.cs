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
    [DocumentAsJson]
    public class AnimationSound
    {
        public int Frame;
        public AssetLocation Location;
        public bool RandomizePitch = true;
        public float Range = 32;

        public AnimationSound Clone()
        {
            return new AnimationSound()
            {
                Frame = Frame,
                Location = Location.Clone(),
                RandomizePitch = RandomizePitch,
                Range = Range
            };
        }
    }

    /// <summary>
    /// Defines how multiple animations should be blended together.
    /// </summary>
    [DocumentAsJson]
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

    /// <summary>
    /// Data about when an animation should be triggered.
    /// </summary>
    [DocumentAsJson]
    public class AnimationTrigger
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional>-->
        /// An array of controls that should begin the animation.
        /// </summary>
        [JsonProperty]
        public EnumEntityActivity[] OnControls;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// If set to true, all OnControls elements need to be happening simultaneously to trigger the animation.
        /// If set to false, at least one OnControls element needs to be happening to trigger the animation.
        /// Defaults to false.
        /// </summary>
        [JsonProperty]
        public bool MatchExact = false;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// Is this animation the default animation for the entity?
        /// </summary>
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

    /// <summary>
    /// Animation Meta Data is a json type that controls how an animation should be played.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"animations": [
	///	{
	///		"code": "hurt",
	///		"animation": "hurt",
	///		"animationSpeed": 2.2,
	///		"weight": 10,
	///		"blendMode": "AddAverage"
	///	},
	///	{
	///		"code": "die",
	///		"animation": "death",
	///		"animationSpeed": 1.25,
	///		"weight": 10,
	///		"blendMode": "Average",
	///		"triggeredBy": { "onControls": [ "dead" ] }
	///	},
	///	{
	///		"code": "idle",
	///		"animation": "idle",
	///		"blendMode": "AddAverage",
	///		"easeOutSpeed": 4,
	///		"triggeredBy": { "defaultAnim": true }
	///	},
	///	{
	///		"code": "walk",
	///		"animation": "walk",
	///		"weight": 5
	///	}
	///]
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class AnimationMetaData
    {
        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// Unique identifier to be able to reference this AnimationMetaData instance
        /// </summary>
        [JsonProperty]
        public string Code;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Custom attributes that can be used for the animation.
        /// Valid vanilla attributes are:<br/>
        /// - damageAtFrame (float)<br/>
        /// - soundAtFrame (float)<br/>
        /// - authorative (bool)<br/>
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The animations code identifier that we want to play
        /// </summary>
        [JsonProperty]
        public string Animation;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The weight of this animation. When using multiple animations at a time, this controls the significance of each animation.
        /// The method for determining final animation values depends on this and <see cref="BlendMode"/>.
        /// </summary>
        [JsonProperty]
        public float Weight = 1f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// A way of specifying <see cref="Weight"/> for each element.
        /// Also see <see cref="ElementBlendMode"/> to control blend modes per element..
        /// </summary>
        [JsonProperty]
        public Dictionary<string, float> ElementWeight = new Dictionary<string, float>();

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The speed this animation should play at.
        /// </summary>
        [JsonProperty]
        public float AnimationSpeed = 1f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// Should this animation speed be multiplied by the movement speed of the entity?
        /// </summary>
        [JsonProperty]
        public bool MulWithWalkSpeed = false;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// This property can be used in cases where a animation with high weight is played alongside another animation with low element weight.
        /// In these cases, the easeIn become unaturally fast. Setting a value of 0.8f or similar here addresses this issue.<br/>
        /// - 0f = uncapped weight<br/>
        /// - 0.5f = weight cannot exceed 2<br/>
        /// - 1f = weight cannot exceed 1
        /// </summary>
        [JsonProperty]
        public float WeightCapFactor = 0f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>10</jsondefault>-->
        /// A multiplier applied to the weight value to "ease in" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        public float EaseInSpeed = 10f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>10</jsondefault>-->
        /// A multiplier applied to the weight value to "ease out" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        public float EaseOutSpeed = 10f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Controls when this animation should be played.
        /// </summary>
        [JsonProperty]
        public AnimationTrigger TriggeredBy;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Add</jsondefault>-->
        /// The animation blend mode. Controls how this animation will react with other concurrent animations.
        /// Also see <see cref="ElementBlendMode"/> to control blend mode per element.
        /// </summary>
        [JsonProperty]
        public EnumAnimationBlendMode BlendMode = EnumAnimationBlendMode.Add;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// A way of specifying <see cref="BlendMode"/> per element.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, EnumAnimationBlendMode> ElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// Should this animation stop default animations from playing?
        /// </summary>
        [JsonProperty]
        public bool SupressDefaultAnimation = false;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>99</jsondefault>-->
        /// A value that determines whether to change the first-person eye position for the camera.
        /// Higher values will keep eye position static.
        /// </summary>
        [JsonProperty]
        public float HoldEyePosAfterEasein = 99f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// If true, the server does not sync this animation.
        /// </summary>
        [JsonProperty]
        public bool ClientSide;
        [JsonProperty]
        public bool WithFpVariant;
        [JsonProperty]
        public AnimationSound AnimationSound;

        public AnimationMetaData FpVariant;
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

            if (WithFpVariant)
            {
                FpVariant = this.Clone();
                FpVariant.WithFpVariant = false;
                FpVariant.Animation += "-fp";
                FpVariant.Code += "-fp";
                FpVariant.Init();
            }

            if (AnimationSound != null)
            {
                AnimationSound.Location.WithPathPrefixOnce("sounds/");
            }

            return this;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Animation = Animation?.ToLowerInvariant() ?? "";
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
                AnimationSound = this.AnimationSound?.Clone(),
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
                SupressDefaultAnimation = SupressDefaultAnimation,
                WeightCapFactor = WeightCapFactor
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
            writer.Write(WeightCapFactor);

            writer.Write(AnimationSound != null);
            if (AnimationSound != null)
            {
                writer.Write(AnimationSound.Location.ToShortString());
                writer.Write(AnimationSound.Range);
                writer.Write(AnimationSound.Frame);
                writer.Write(AnimationSound.RandomizePitch);
            }
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
            if (GameVersion.IsAtLeastVersion(version, "1.19.0-rc.6"))
            {
                animdata.WeightCapFactor = reader.ReadSingle();
            }

            if (GameVersion.IsAtLeastVersion(version, "1.20.0-dev.13"))
            {
                if (reader.ReadBoolean())
                {
                    animdata.AnimationSound = new AnimationSound()
                    {
                        Location = AssetLocation.Create(reader.ReadString()),
                        Range = reader.ReadSingle(),
                        Frame = reader.ReadInt32(),
                        RandomizePitch = reader.ReadBoolean()
                    };
                }
            }

            animdata.Init();

            return animdata;
        }

        
    }


}
