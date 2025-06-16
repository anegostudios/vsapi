using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MusicTrackPart
    {
        /// <summary>
        /// The minimum Suitability of the given track
        /// </summary>
        [JsonProperty]
        public float MinSuitability = 0.1f;

        /// <summary>
        /// The maximum Suitability of the given track
        /// </summary>
        [JsonProperty]
        public float MaxSuitability = 1f;

        /// <summary>
        /// The minimum volume of a given track.
        /// </summary>
        [JsonProperty]
        public float MinVolumne = 0.35f;

        /// <summary>
        /// The maximum volume of a given track.
        /// </summary>
        [JsonProperty]
        public float MaxVolumne = 1f;

        /// <summary>
        /// the Y position.
        /// </summary>
        [JsonProperty]
        public float[] PosY;

        [JsonProperty]
        public float[] Sunlight;
        //[JsonProperty]
        //public Dictionary<EnumWeatherPattern, float[]> Weather = new Dictionary<EnumWeatherPattern, float[]>();
        
        /// <summary>
        /// The files for the part.
        /// </summary>
        [JsonProperty]
        public AssetLocation[] Files;

        /// <summary>
        /// The loaded sound
        /// </summary>
        public ILoadedSound Sound;

        /// <summary>
        /// Start time in Miliseconds
        /// </summary>
        public long StartedMs;

        /// <summary>
        /// Am I loading?
        /// </summary>
        public bool Loading;

        internal AssetLocation NowPlayingFile;

        /// <summary>
        /// Am I playing?
        /// </summary>
        public bool IsPlaying
        {
            get { return Sound != null && Sound.IsPlaying; }
        }

        /// <summary>
        /// Am I applicable?
        /// </summary>
        /// <param name="world">world information</param>
        /// <param name="props">the properties of the current track.</param>
        /// <returns></returns>
        public bool Applicable(IWorldAccessor world, TrackedPlayerProperties props)
        {
            return CurrentSuitability(world, props) > MinSuitability;
        }

        /// <summary>
        /// The current volume of the track.
        /// </summary>
        /// <param name="world">world information</param>
        /// <param name="props">the properties of the current track.</param>
        /// <returns></returns>
        public float CurrentVolume(IWorldAccessor world, TrackedPlayerProperties props)
        {
            // y = k * x + d
            // k = (y2 - y1) / (x2 - x1);
            // d = y1 - k * x1 
            float x = CurrentSuitability(world, props);
            if (x == 1) return 1;

            float k = (MaxVolumne - MinVolumne) / (MaxSuitability - MinSuitability);
            float d = MinVolumne - k * MinSuitability;          

            if (x < MinSuitability) return 0;

            return GameMath.Min(k * x + d, MaxVolumne);
        }

        /// <summary>
        /// The current Suitability of the track.
        /// </summary>
        /// <param name="world">world information</param>
        /// <param name="props">the properties of the current track.</param>
        /// <returns></returns>
        public float CurrentSuitability(IWorldAccessor world, TrackedPlayerProperties props)
        {
            int applied = 0;
            float suitability = 0;

            
            if (PosY != null)
            {
                suitability += GameMath.TriangleStep(props.posY, PosY[0], PosY[1]);
                applied++;
            }

            if (Sunlight != null)
            {
                suitability += GameMath.TriangleStep(props.sunSlight, Sunlight[0], Sunlight[1]);
                applied++;
            }

            /*foreach (var val in Weather)
            {
                suitability += GameMath.TriangleStep(props.Weather[(int)val.Key], val.Value[0], val.Value[1]);
                applied++;
            }*/

            if (applied == 0) return 1;

            return suitability / applied;
        }

        /// <summary>
        /// Expands the target files.
        /// </summary>
        /// <param name="assetManager">The current AssetManager instance.</param>
        public virtual void ExpandFiles(IAssetManager assetManager)
        {
            List<AssetLocation> expandedFiles = new List<AssetLocation>();
            for (int i = 0; i < Files.Length; i++)
            {
                AssetLocation fileLocation = Files[i];
                if (fileLocation.Path.EndsWith('*'))
                {
                    List<AssetLocation> locations = assetManager.GetLocations("music/" + fileLocation.Path.Substring(0, fileLocation.Path.Length - 1), fileLocation.Domain);

                    foreach(var location in locations)
                    {
                        expandedFiles.Add(location);
                    }
                }
                else
                {
                    expandedFiles.Add(fileLocation);
                }
            }

            Files = expandedFiles.ToArray();
        }

    }
}
