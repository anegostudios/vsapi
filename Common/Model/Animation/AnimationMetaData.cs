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
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
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
        /// An array of controls that should begin the animation.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Recommended")]
        public EnumEntityActivity[] OnControls;

        /// <summary>
        /// If set to true, all OnControls elements need to be happening simultaneously to trigger the animation.
        /// If set to false, at least one OnControls element needs to be happening to trigger the animation.
        /// Defaults to false.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool MatchExact = false;

        /// <summary>
        /// Is this animation the default animation for the entity?
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
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
        /// Unique identifier to be able to reference this AnimationMetaData instance
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Required")]
        public string Code;

        /// <summary>
        /// Custom attributes that can be used for the animation.
        /// Valid vanilla attributes are:<br/>
        /// - damageAtFrame (float)<br/>
        /// - soundAtFrame (float)<br/>
        /// - authorative (bool)<br/>
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        [DocumentAsJson("Optional", "None")]
        public JsonObject Attributes;

        /// <summary>
        /// The animations code identifier that we want to play
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Required")]
        public string Animation;

        /// <summary>
        /// The weight of this animation. When using multiple animations at a time, this controls the significance of each animation.
        /// The method for determining final animation values depends on this and <see cref="BlendMode"/>.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public float Weight = 1f;

        /// <summary>
        /// A way of specifying <see cref="Weight"/> for each element.
        /// Also see <see cref="ElementBlendMode"/> to control blend modes per element..
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public Dictionary<string, float> ElementWeight = new Dictionary<string, float>();

        /// <summary>
        /// The speed this animation should play at.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public float AnimationSpeed = 1f;

        /// <summary>
        /// Should this animation speed be multiplied by the movement speed of the entity?
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool MulWithWalkSpeed = false;

        /// <summary>
        /// This property can be used in cases where a animation with high weight is played alongside another animation with low element weight.
        /// In these cases, the easeIn become unaturally fast. Setting a value of 0.8f or similar here addresses this issue.<br/>
        /// - 0f = uncapped weight<br/>
        /// - 0.5f = weight cannot exceed 2<br/>
        /// - 1f = weight cannot exceed 1
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public float WeightCapFactor = 0f;

        /// <summary>
        /// A multiplier applied to the weight value to "ease in" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "10")]
        public float EaseInSpeed = 10f;

        /// <summary>
        /// A multiplier applied to the weight value to "ease out" the animation. Choose a high value for looping animations or it will be glitchy
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "10")]
        public float EaseOutSpeed = 10f;

        /// <summary>
        /// Controls when this animation should be played.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public AnimationTrigger TriggeredBy;

        /// <summary>
        /// The animation blend mode. Controls how this animation will react with other concurrent animations.
        /// Also see <see cref="ElementBlendMode"/> to control blend mode per element.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "Add")]
        public EnumAnimationBlendMode BlendMode = EnumAnimationBlendMode.Add;

        /// <summary>
        /// A way of specifying <see cref="BlendMode"/> per element.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public Dictionary<string, EnumAnimationBlendMode> ElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Should this animation stop default animations from playing?
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool SupressDefaultAnimation = false;

        /// <summary>
        /// A value that determines whether to change the first-person eye position for the camera.
        /// Higher values will keep eye position static.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "99")]
        public float HoldEyePosAfterEasein = 99f;

        /// <summary>
        /// If true, the server does not sync this animation.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool ClientSide;

        [JsonProperty]
        public bool WithFpVariant;

        [JsonProperty]
        public AnimationSound[] AnimationSounds;

        public AnimationMetaData FpVariant;
        public float StartFrameOnce;
        int withActivitiesMerged;
        public uint CodeCrc32;
        public bool WasStartedFromTrigger;

        [JsonProperty]
        public bool AdjustCollisionBox { get; set; }

        public float GetCurrentAnimationSpeed(float walkspeed)
        {
            return AnimationSpeed * (MulWithWalkSpeed ? walkspeed : 1) * GlobalConstants.OverallSpeedMultiplier;
        }

        public AnimationMetaData Init()
        {
            withActivitiesMerged = 0;
            if (TriggeredBy?.OnControls is EnumEntityActivity[] OnControls)
            for (int i = 0; i < OnControls.Length; i++)
            {
                withActivitiesMerged |= (int)OnControls[i];
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

            for (int i = 0; AnimationSounds != null && i < AnimationSounds.Length; ++i)
            {
                if (AnimationSounds[i]?.Attributes.Location == null)
                {
                    ShapeElement.Logger?.Warning("Animation with code '" + Code + "' has null sound at index " + i);
                }
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
            bool result;
            if (TriggeredBy?.MatchExact == true)
            {
                result = (currentActivities == withActivitiesMerged);
            }
            else
            {   result = ((currentActivities & withActivitiesMerged) > 0);
                if (result && (withActivitiesMerged & ((int)EnumEntityActivity.Fly | (int)EnumEntityActivity.Fall)) != 0)
                {
                    // Doesn't make sense to match Fly or Fall ("land") animations when we are mounted
                    if ((currentActivities & (int)EnumEntityActivity.Mounted) != 0) result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Not a deep clone of all fields. Scalar fields, for example AnimationSpeed, are fresh copies and can be changed dynamically or per-entity, after cloning the AnimationMetaData.
        /// <br/> For performance reasons, the non-scalar fields such as the Attributes, ElementWeight, ElementBlendMode and TriggeredBy are not deep-cloned as these fields are usually unchanging for all entities using the animation.
        /// <br/> Note: If any implementing entity needs to change the non-scalar fields dynamically, or on a per-entity basis, that entity's code can clone and replace the object in its own individual copy of the AnimationMetaData.  e.g. EntityDrifter replaces .TriggeredBy in some of its animations
        /// </summary>
        public AnimationMetaData Clone()
        {
            // radfast 1.3.25: things like the Attributes, ElementWeight, ElementBlendMode and TriggeredBy do not need to be .Clone() as these are only read, not written to
            // If any entity needs to change these dynamically or on a per-entity basis, that entity's code can replace the object read from properties with its own version or a clone before modifying it.  As does vanilla EntityDrifter already, for TriggeredBy, for example
            // But a *few* fields can be changed dynamically or per-entity, for example AnimationSpeed.  That's the reason why we need to clone the AnimationMetaData.

            return new AnimationMetaData()
            {
                Code = this.Code,
                Animation = this.Animation,
                AnimationSounds = this.AnimationSounds,
                Weight = this.Weight,
                Attributes = this.Attributes,
                ClientSide = this.ClientSide,
                ElementWeight = this.ElementWeight,
                AnimationSpeed = this.AnimationSpeed,
                MulWithWalkSpeed = this.MulWithWalkSpeed,
                EaseInSpeed = this.EaseInSpeed,
                EaseOutSpeed = this.EaseOutSpeed,
                TriggeredBy = this.TriggeredBy,
                AdjustCollisionBox = this.AdjustCollisionBox,
                BlendMode = this.BlendMode,
                ElementBlendMode = this.ElementBlendMode,
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

                if (TriggeredBy.OnControls is EnumEntityActivity[] OnControls)
                {
                    writer.Write(OnControls.Length);
                    for (int i = 0; i < OnControls.Length; i++)
                    {
                        writer.Write((int)OnControls[i]);
                    }
                }
                else writer.Write(0);
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

            writer.Write(AnimationSounds?.Length ?? 0);

            if (AnimationSounds != null)
            {
                foreach (var animsound in AnimationSounds)
                {
                    animsound.ToBytes(writer);
                }
            }

            writer.Write(AdjustCollisionBox);
        }

        public static AnimationMetaData FromBytes(BinaryReader reader, string version)
        {
            AnimationMetaData animdata = new AnimationMetaData();

            animdata.Code = reader.ReadString().DeDuplicate();
            animdata.Animation = reader.ReadString();
            animdata.Weight = reader.ReadSingle();

            int weightCount = reader.ReadInt32();
            for (int i = 0; i < weightCount; i++)
            {
                animdata.ElementWeight[reader.ReadString().DeDuplicate()] = reader.ReadSingle();
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
                animdata.ElementBlendMode[reader.ReadString().DeDuplicate()] = (EnumAnimationBlendMode)reader.ReadInt32();
            }

            animdata.MulWithWalkSpeed = reader.ReadBoolean();

            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.12.5-dev.1"))
            {
                animdata.StartFrameOnce = reader.ReadSingle();
            }
            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.13.0-dev.3"))
            {
               animdata.HoldEyePosAfterEasein = reader.ReadSingle();
            }
            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.17.0-dev.18"))
            {
                animdata.ClientSide = reader.ReadBoolean();
            }
            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.19.0-dev.20"))
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
            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.19.0-rc.6"))
            {
                animdata.WeightCapFactor = reader.ReadSingle();
            }

            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.22.0-dev.2"))
            {
                int animsoundslen = reader.ReadInt32();
                if (animsoundslen > 0)
                {
                    animdata.AnimationSounds = new AnimationSound[animsoundslen];
                    for (int i = 0; i < animsoundslen; i++)
                    {
                        animdata.AnimationSounds[i] = AnimationSound.CreateFromBytes(reader, version);
                    }
                }
            }
            else
            {
                if (version != "" && GameVersion.IsAtLeastVersion(version, "1.20.0-dev.13"))
                {
                    if (reader.ReadBoolean())
                    {
                        animdata.AnimationSounds = new AnimationSound[1];
                        animdata.AnimationSounds[0] = AnimationSound.CreateFromBytes(reader, version);
                    }
                }
            }

            if (version != "" && GameVersion.IsAtLeastVersion(version, "1.21.0-dev.1"))
            {
                animdata.AdjustCollisionBox = reader.ReadBoolean();
            }

            animdata.Init();

            return animdata;
        }

        internal void DeDuplicate()
        {
            Code = Code.DeDuplicate();

            var newElementWeight = new Dictionary<string, float>(ElementWeight.Count);
            foreach (var entry in ElementWeight) newElementWeight[entry.Key.DeDuplicate()] = entry.Value;
            ElementWeight = newElementWeight;

            var newElementBlendMode = new Dictionary<string, EnumAnimationBlendMode>(ElementBlendMode.Count);
            foreach (var entry in ElementBlendMode) newElementBlendMode[entry.Key.DeDuplicate()] = entry.Value;
            ElementBlendMode = newElementBlendMode;
        }
    }


}
