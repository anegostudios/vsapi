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
        public float ExtraSepia;
        public float ExtraBloom;

        public float FlagFogDensity;
        public float FlatFogStartYPos;
        public int Dusk;


        public float WaterStillCounter = 0f;
        public float WaterFlowCounter = 0f;
        public float WaterWaveCounter = 0f;
        public float WindWaveCounter = 0f;
        public float GlitchStrength = 0f;
        public float WindSpeed = 0f;
        public float WindWaveIntensity = 1f;
        public float FogWaveCounter = 0f;

        public float GlobalWorldWarp = 0f;

        public float SunsetMod = 0f;

        public Vec3f PlayerPos = new Vec3f();
        Vec3d playerReferencePos;

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


        public void Update(float dt, ICoreClientAPI capi)
        {
            IGameCalendar calendar = capi.World.Calendar;
            dt *= calendar.SpeedOfTime / 60f;
            if (capi.IsGamePaused) dt = 0;

            WaterStillCounter = (WaterStillCounter + dt / 1.5f) % 2f;
            WaterFlowCounter = (WaterFlowCounter + dt / 1.5f) % 141f;
            WaterWaveCounter = (WaterWaveCounter + dt * 0.75f) % 578f;

            WindWaveCounter = (WindWaveCounter + (0.5f + 5*GlobalConstants.CurrentWindSpeedClient.X) * dt) % 578f;
            WindSpeed = GlobalConstants.CurrentWindSpeedClient.X;

            FogWaveCounter += 0.1f * dt;

            // So here's an interesting piece of code.
            // We sometimes need to know a player position for noise functions and such to be precise. However the player position is usually too large to be precise 
            // So we can do a weird hack. If the players position moves by over 30.0000 we use that position as a new reference point
            // This will cause a 1 frame jitter for the player, but hopefully not too noticable
            // A proper fix would be to up the GL requirements to OpenGL 4.0 and use vec3d for double precision math.

            Vec3d pos = capi.World.Player.Entity.CameraPos;

            if (playerReferencePos == null)
            {
                playerReferencePos = new Vec3d(capi.World.BlockAccessor.MapSizeX / 2, 0, capi.World.BlockAccessor.MapSizeZ / 2);
            }
            if (playerReferencePos.HorizontalSquareDistanceTo(pos.X, pos.Z) > 30000.0 * 30000)
            {
                playerReferencePos.Set((float)pos.X, 0, (float)pos.Z);
            }

            
            PlayerPos.Set((float)(pos.X - playerReferencePos.X), (float)(pos.Y - playerReferencePos.Y), (float)(pos.Z - playerReferencePos.Z));

            // For godrays shader
            PlayerViewVector = EntityPos.GetViewVector(capi.Input.MousePitch, capi.Input.MouseYaw);

            Dusk = capi.World.Calendar.Dusk ? 1 : 0;


            // updated by RenderSunMoon.cs
            // SunPositionScreen, SunPosition3D

            // updated by RenderShadowMap.cs: 
            // ShadowRangeNear, ShadowZExtendNear, DropShadowIntensity
            // ToShadowMapSpaceMatrixNear, ShadowRangeFar, ShadowZExtendFar, ToShadowMapSpaceMatrixFar

            // updated by RenderPlayerEffects.cs:
            // DamageVignetting, PointLights*

        }
    }
}
