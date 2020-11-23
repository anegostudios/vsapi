using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class DefaultShaderUniforms
    {
        /// <summary>
        /// The currently configured z-near plane
        /// </summary>
        public float ZNear;

        /// <summary>
        /// The currently configured z-far plane
        /// </summary>
        public float ZFar;

        public float DropShadowIntensity;
        public float ShadowRangeNear;
        public float ShadowRangeFar;

        public float ShadowZExtendNear;
        public float ShadowZExtendFar;

        public float[] ToShadowMapSpaceMatrixFar = new float[16];
        public float[] ToShadowMapSpaceMatrixNear = new float[16];

        public int PointLightsCount;
        public float[] PointLights3;
        public float[] PointLightColors3;

        public Vec3f SunPositionScreen;
        public Vec3f SunPosition3D;
        public Vec3f LightPosition3D;

        public Vec3f PlayerViewVector;
        public float DamageVignetting;
        public float FrostVignetting;
        public float ExtraSepia;
        public float ExtraBloom;

        public float FlagFogDensity;
        public float FlatFogStartYPos;
        public int Dusk;


        public float WaterStillCounter = 0f;
        public float WaterFlowCounter = 0f;
        public float WaterWaveCounter = 0f;
        public float WindWaveCounter = 0f;
        public float WindWaveCounterHighFreq = 0f;

        public float GlitchStrength = 0f;
        public float WindSpeed = 0f;
        public float WindWaveIntensity = 1f;
        public float FogWaveCounter = 0f;

        public float GlobalWorldWarp = 0f;
        public float SeaLevel = 0f;

        public float SunsetMod = 0f;
        public int SunLightTextureId;
        public int GlowTextureId;
        public int SkyTextureId;
        
        public int DitherSeed;
        public int FrameWidth;
        public float PlayerToSealevelOffset;

        public float[] ColorMapRects4;
        public float SeasonRel;
        public float BlockAtlasHeight;
        public float SeasonTemperature;
        public float SunSpecularIntensity = 1;

        public Vec3f PlayerPos = new Vec3f();
        public Vec3d playerReferencePos;
        BlockPos plrPos = new BlockPos();

        internal float SkyDaylight;


        public DefaultShaderUniforms()
        {
            SunPositionScreen = new Vec3f();
            SunPosition3D = new Vec3f();
            LightPosition3D = new Vec3f();
            PlayerViewVector = new Vec3f();
            DamageVignetting = 0;
            PointLightsCount = 0;
            PointLights3 = new float[3 * 100];
            PointLightColors3 = new float[3 * 100];
        }


        public static int DescaleTemperature(float temperature)
        {
            return (int)((temperature + 20) * 4.25f);
        }

        public void Update(float dt, ICoreClientAPI capi)
        {
            IGameCalendar calendar = capi.World.Calendar;
            dt *= calendar.SpeedOfTime / 60f;
            if (capi.IsGamePaused) dt = 0;

            WaterStillCounter = (WaterStillCounter + dt / 1.5f) % 2f;
            WaterFlowCounter = (WaterFlowCounter + dt / 1.5f) % 6000f;
            WaterWaveCounter = (WaterWaveCounter + dt * 0.75f) % 6000f;
            
            WindWaveCounter = (WindWaveCounter + (0.5f + 5 * GlobalConstants.CurrentWindSpeedClient.X * (1 - GlitchStrength)) * dt) % 6000f;
            WindSpeed = GlobalConstants.CurrentWindSpeedClient.X;

            float freq = (0.4f + WindSpeed / 10);
            WindWaveCounterHighFreq = (WindWaveCounterHighFreq + freq * (0.5f + 5 * GlobalConstants.CurrentWindSpeedClient.X * (1 - GlitchStrength)) * dt) % 6000f;

            FogWaveCounter += 0.1f * dt;

            
            plrPos.Set(capi.World.Player.Entity.Pos.XInt, capi.World.Player.Entity.Pos.YInt, capi.World.Player.Entity.Pos.ZInt);

            // For godrays shader
            PlayerViewVector = EntityPos.GetViewVector(capi.Input.MousePitch, capi.Input.MouseYaw);

            Dusk = capi.World.Calendar.Dusk ? 1 : 0;

            
            PlayerToSealevelOffset = (float)capi.World.Player.Entity.Pos.Y - capi.World.SeaLevel;
            SeaLevel = capi.World.SeaLevel;
            FrameWidth = capi.Render.FrameWidth;
            BlockAtlasHeight = capi.BlockTextureAtlas.Size.Height;

            int y = plrPos.Y;
            plrPos.Y = capi.World.SeaLevel;
            ClimateCondition nowConds = capi.World.BlockAccessor.GetClimateAt(plrPos, EnumGetClimateMode.NowValues);
            plrPos.Y = y;
            SeasonTemperature = (DescaleTemperature(nowConds.Temperature) - DescaleTemperature(nowConds.WorldGenTemperature)) / 255f;

            // We might need to do the hemisphere thing as a single bit for every vertex
            SeasonRel = capi.World.Calendar.GetSeasonRel(plrPos);


            // updated by ClientWorldMap.cs
            // ColorMapRects

            // updated by RenderSkyColor.cs
            // DitherSeed, SkyTextureId, GlowTextureId, SkyDaylight

            // updated by RenderSunMoon.cs
            // SunPositionScreen, SunPosition3D, sunSpecularIntensity

            // updated by RenderShadowMap.cs: 
            // ShadowRangeNear, ShadowZExtendNear, DropShadowIntensity
            // ToShadowMapSpaceMatrixNear, ShadowRangeFar, ShadowZExtendFar, ToShadowMapSpaceMatrixFar

            // updated by RenderPlayerEffects.cs:
            // DamageVignetting, PointLights*

        }
    }
}
