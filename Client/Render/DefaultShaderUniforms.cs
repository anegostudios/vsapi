using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
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

        public float FlagFogDensity;
        public float FlatFogStartYPos;
        public int Dusk;


        public float WaterStillCounter = 0f;
        public float WaterFlowCounter = 0f;
        public float WaterWaveCounter = 0f;
        public float WindWaveCounter = 0f;
        public float WindWaveIntensity = 1f;
        public float FogWaveCounter = 0f;

        public float GlobalWorldWarp = 0f;

        public float SunsetMod = 0f;

        public Vec3f PlayerPos = new Vec3f();
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
            WaterWaveCounter += dt * 0.75f;

            WindWaveCounter += 1.5f * dt;
            FogWaveCounter += 0.1f * dt;

            if (WindWaveCounter > 8 * GameMath.TWOPI)
            {
                WindWaveCounter -= 8 * GameMath.TWOPI;
            }

            // This used to be Entity.CameraPos but that seems to lag behind?
            // iirc the "-capi.World.BlockAccessor.MapSizeX / 2" is there so that the greatest accuracy is in the map middle
            PlayerPos.Set((float)(capi.World.Player.Entity.Pos.X - capi.World.BlockAccessor.MapSizeX / 2), (float)capi.World.Player.Entity.Pos.Y, (float)(capi.World.Player.Entity.Pos.Z - capi.World.BlockAccessor.MapSizeZ / 2));

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
