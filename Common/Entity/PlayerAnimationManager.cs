using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    public class PlayerAnimationManager : AnimationManager
    {
        public bool UseFpAnmations=true;

        bool useFpAnimSet => UseFpAnmations && api.Side == EnumAppSide.Client && capi.World.Player.Entity.EntityId == entity.EntityId && capi.World.Player.CameraMode == EnumCameraMode.FirstPerson;

        string fpEnding => UseFpAnmations && capi?.World.Player.CameraMode == EnumCameraMode.FirstPerson ? ((api as ICoreClientAPI)?.Settings.Bool["immersiveFpMode"] == true ? "-ifp" : "-fp") : "";

        EntityPlayer plrEntity;

        public override void Init(ICoreAPI api, Entity entity)
        {
            base.Init(api, entity);

            plrEntity = entity as EntityPlayer;
        }

        public override void OnClientFrame(float dt)
        {
            base.OnClientFrame(dt);

            if (useFpAnimSet)
            {
                plrEntity.TpAnimManager.OnClientFrame(dt);
            }
        }


        public override void ResetAnimation(string animCode)
        {
            base.ResetAnimation(animCode);
            base.ResetAnimation(animCode + "-ifp");
            base.ResetAnimation(animCode + "-fp");
        }

        public override bool StartAnimation(string configCode)
        {
            if (configCode == null) return false;

            if (useFpAnimSet) // We are in the first person anim manager - play anim for tp animator, and anim-fp for ourselves
            {
                plrEntity.TpAnimManager.StartAnimationBase(configCode);

                if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(configCode + fpEnding, out var animdata))
                {
                    StartAnimation(animdata);
                    return true;
                }
            } else
            {
                if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(configCode + fpEnding, out var animdata))
                {
                    plrEntity.SelfFpAnimManager.StartAnimationBase(animdata);
                }
            }

            return base.StartAnimation(configCode);
        }

        public override bool StartAnimation(AnimationMetaData animdata)
        {
            if (useFpAnimSet && !animdata.Code.EndsWithOrdinal(fpEnding))
            {
                plrEntity.TpAnimManager.StartAnimation(animdata);

                if (animdata.WithFpVariant)
                {
                    if (ActiveAnimationsByAnimCode.TryGetValue(animdata.FpVariant.Animation, out var activeAnimdata) && activeAnimdata == animdata.FpVariant) return false;
                    return base.StartAnimation(animdata.FpVariant);
                }

                if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(animdata.Code + fpEnding, out var animdatafp))
                {
                    if (ActiveAnimationsByAnimCode.TryGetValue(animdatafp.Animation, out var activeAnimdata) && activeAnimdata == animdatafp) return false;
                    return base.StartAnimation(animdatafp);
                }
            }

            return base.StartAnimation(animdata);
        }

        public bool StartAnimationBase(string configCode)
        {
            if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(configCode + fpEnding, out var animdata))
            {
                StartAnimation(animdata);
                return true;
            }
            return base.StartAnimation(configCode);
        }
        public bool StartAnimationBase(AnimationMetaData animdata)
        {
            return base.StartAnimation(animdata);
        }

        public override void RegisterFrameCallback(AnimFrameCallback trigger)
        {
            if (useFpAnimSet && !trigger.Animation.EndsWithOrdinal(fpEnding) && entity.Properties.Client.AnimationsByMetaCode.ContainsKey(trigger.Animation + fpEnding))
            {
                trigger.Animation += fpEnding;
            }
            base.RegisterFrameCallback(trigger);
        }

        public override void StopAnimation(string code)
        {
            if (code == null) return;

            if (api.Side == EnumAppSide.Client) (plrEntity.OtherAnimManager as PlayerAnimationManager).StopSelfAnimation(code);
            StopSelfAnimation(code);
        }

        public void StopSelfAnimation(string code)
        {
            string[] anims = new string[] { code, code + "-ifp", code + "-fp" };
            foreach (var anim in anims)
            {
                base.StopAnimation(anim);
            }
        }

        public override bool IsAnimationActive(params string[] anims)
        {
            if (useFpAnimSet)
            {
                foreach (var val in anims)
                {
                    if (ActiveAnimationsByAnimCode.ContainsKey(val + fpEnding)) return true;
                }
            }

            return base.IsAnimationActive(anims);
        }


        public override RunningAnimation GetAnimationState(string anim)
        {
            if (useFpAnimSet && !anim.EndsWithOrdinal(fpEnding) && entity.Properties.Client.AnimationsByMetaCode.ContainsKey(anim + fpEnding))
            {
                return base.GetAnimationState(anim + fpEnding);
            }

            return base.GetAnimationState(anim);
        }


        public bool IsAnimationActiveOrRunning(string anim, float untilProgress = 0.95f)
        {
            if (anim == null || Animator == null) return false;
            return IsAnimationMostlyRunning(anim, untilProgress) || IsAnimationMostlyRunning(anim + fpEnding, untilProgress);
        }

        protected bool IsAnimationMostlyRunning(string anim, float untilProgress = 0.95f)
        {
            var ranim = Animator.GetAnimationState(anim);
            return ranim != null && ranim.Running && ranim.AnimProgress < untilProgress && ranim.Active /* ranim.Active check makes a short left mouse click with axe in hands look much better */;
        }

        protected override void onReceivedServerAnimation(AnimationMetaData animmetadata)
        {
            StartAnimation(animmetadata);
            if (plrEntity.MountedOn != null)
            {
                if (plrEntity.curMountedAnim != null && plrEntity.curMountedAnim.Code != animmetadata.Code) StopAnimation(plrEntity.curMountedAnim.Code);
                plrEntity.curMountedAnim = animmetadata;
            }
        }

        public override void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            base.OnReceivedServerAnimations(activeAnimations, activeAnimationsCount, activeAnimationSpeeds);
        }


        protected string lastActiveHeldReadyAnimation;
        protected string lastActiveRightHeldIdleAnimation;
        protected string lastActiveLeftHeldIdleAnimation;

        protected string lastActiveHeldHitAnimation;
        protected string lastActiveHeldUseAnimation;

        public string lastRunningHeldHitAnimation;
        public string lastRunningHeldUseAnimation;

        public void OnActiveSlotChanged(ItemSlot slot)
        {
            string beginholdAnim = slot.Itemstack?.Collectible?.GetHeldReadyAnimation(slot, entity, EnumHand.Right);
            
            if (beginholdAnim != lastActiveHeldReadyAnimation) StopHeldReadyAnim();

            if (beginholdAnim != null) StartHeldReadyAnim(beginholdAnim);

            lastActiveHeldHitAnimation = null;
        }
        

        public void StartHeldReadyAnim(string heldReadyAnim, bool force = false)
        {
            if (!force && (IsHeldHitActive() || IsHeldUseActive())) return;
            
            if (lastActiveHeldReadyAnimation != null) StopAnimation(lastActiveHeldReadyAnimation);

            ResetAnimation(heldReadyAnim);
            StartAnimation(heldReadyAnim);

            lastActiveHeldReadyAnimation = heldReadyAnim;
        }
        

        public void StartHeldUseAnim(string animCode)
        {
            StopHeldReadyAnim();
            StopAnimation(lastActiveRightHeldIdleAnimation);
            StopAnimation(lastActiveHeldHitAnimation);
            StartAnimation(animCode);
            lastActiveHeldUseAnimation = animCode;
            lastRunningHeldUseAnimation = animCode;
        }

        public void StartHeldHitAnim(string animCode)
        {
            StopHeldReadyAnim();
            StopAnimation(lastActiveRightHeldIdleAnimation);
            StopAnimation(lastActiveHeldUseAnimation);
            StartAnimation(animCode);
            lastActiveHeldHitAnimation = animCode;
            lastRunningHeldHitAnimation= animCode;
        }

        public void StartRightHeldIdleAnim(string animCode)
        {
            StopAnimation(lastActiveRightHeldIdleAnimation);
            StopAnimation(lastActiveHeldUseAnimation);
            StartAnimation(animCode);
            lastActiveRightHeldIdleAnimation = animCode;
        }


        public void StartLeftHeldIdleAnim(string animCode)
        {
            StopAnimation(lastActiveLeftHeldIdleAnimation);
            StartAnimation(animCode);
            lastActiveLeftHeldIdleAnimation = animCode;
        }



        public void StopHeldReadyAnim()
        {
            if (!plrEntity.RightHandItemSlot.Empty)
            {
                if (plrEntity.RightHandItemSlot.Itemstack.ItemAttributes?.IsTrue("alwaysPlayHeldReady") == true) return;
            }
            StopAnimation(lastActiveHeldReadyAnimation);
            lastActiveHeldReadyAnimation = null;
        }

        public void StopHeldUseAnim()
        {
            StopAnimation(lastActiveHeldUseAnimation);
            lastActiveHeldUseAnimation = null;
        }

        public void StopHeldAttackAnim()
        {
            if (lastActiveHeldHitAnimation != null && entity.Properties.Client.AnimationsByMetaCode.TryGetValue(lastActiveHeldHitAnimation, out var animData))
            {
                if (animData.Attributes?.IsTrue("authorative") == true)
                {
                    if (IsHeldHitActive()) return;
                }
            }

            StopAnimation(lastActiveHeldHitAnimation);
            lastActiveHeldHitAnimation = null;
        }

        public void StopRightHeldIdleAnim()
        {
            StopAnimation(lastActiveRightHeldIdleAnimation);
            lastActiveRightHeldIdleAnimation = null;
        }
        public void StopLeftHeldIdleAnim()
        {
            StopAnimation(lastActiveLeftHeldIdleAnimation);
            lastActiveLeftHeldIdleAnimation = null;
        }


        public bool IsHeldHitAuthoritative()
        {
            return IsAuthoritative(lastActiveHeldHitAnimation);
        }

        public bool IsAuthoritative(string anim)
        {
            if (anim == null) return false;
            if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(anim, out var animData))
            {
                return animData.Attributes?.IsTrue("authorative") == true;
            }

            return false;
        }

        public bool IsHeldUseActive()
        {
            return lastActiveHeldUseAnimation != null && IsAnimationActiveOrRunning(lastActiveHeldUseAnimation);
        }

        public bool IsHeldHitActive(float untilProgress = 0.95f)
        {
            return lastActiveHeldHitAnimation != null && IsAnimationActiveOrRunning(lastActiveHeldHitAnimation, untilProgress);
        }

        public bool IsLeftHeldActive()
        {
            return lastActiveLeftHeldIdleAnimation != null && IsAnimationActiveOrRunning(lastActiveLeftHeldIdleAnimation);
        }

        public bool IsRightHeldActive()
        {
            return lastActiveRightHeldIdleAnimation != null && IsAnimationActiveOrRunning(lastActiveRightHeldIdleAnimation);
        }

        public bool IsRightHeldReadyActive()
        {
            return lastActiveHeldReadyAnimation != null && IsAnimationActiveOrRunning(lastActiveHeldReadyAnimation);
        }

        public bool HeldRightReadyAnimChanged(string nowHeldRightReadyAnim)
        {
            return lastActiveHeldReadyAnimation != null && nowHeldRightReadyAnim != lastActiveHeldReadyAnimation;
        }

        public bool HeldUseAnimChanged(string nowHeldRightUseAnim)
        {
            return lastActiveHeldUseAnimation != null && nowHeldRightUseAnim != lastActiveHeldUseAnimation;
        }

        public bool HeldHitAnimChanged(string nowHeldRightHitAnim)
        {
            return lastActiveHeldHitAnimation != null && nowHeldRightHitAnim != lastActiveHeldHitAnimation;
        }

        public bool RightHeldIdleChanged(string nowHeldRightIdleAnim)
        {
            return lastActiveRightHeldIdleAnimation != null && nowHeldRightIdleAnim != lastActiveRightHeldIdleAnimation;
        }

        public bool LeftHeldIdleChanged(string nowHeldLeftIdleAnim)
        {
            return lastActiveLeftHeldIdleAnimation != null && nowHeldLeftIdleAnim != lastActiveLeftHeldIdleAnimation;
        }


        public override void FromAttributes(ITreeAttribute tree, string version)
        {
            if (entity == null || capi?.World.Player.Entity.EntityId != entity.EntityId) // Don't update animations for ourselves, it breaks stuff and is not necessary - client has authority over own player animations. 
            {
                base.FromAttributes(tree, version);
            }

            lastActiveHeldUseAnimation = tree.GetString("lrHeldUseAnim");
            lastActiveHeldHitAnimation = tree.GetString("lrHeldHitAnim");
            // Can we not have this line? It breaks fp hands when loading up with a world with a block in hands - the shoulds of the hands become visible when walking and looking down
            //lastRunningRightHeldIdleAnimation = tree.GetString("lrRightHeldIdleAnim");
        }

        public override void ToAttributes(ITreeAttribute tree, bool forClient)
        {
            base.ToAttributes(tree, forClient);
            WriteAdditionalAttributes(forClient, tree.SetString);
        }

        public override void ToAttributeBytes(BinaryWriter writer, bool forClient)
        {
            base.ToAttributeBytes(writer, forClient);
            WriteAdditionalAttributes(forClient, (key, value) => StringAttribute.DirectWrite(writer, key, value));
        }

        private void WriteAdditionalAttributes(bool forClient, Action<string, string> output)
        {
            if (lastActiveHeldUseAnimation != null)
            {
                output("lrHeldUseAnim", lastActiveHeldUseAnimation);
            }
            if (lastActiveHeldHitAnimation != null)
            {
                output("lrHeldHitAnim", lastActiveHeldHitAnimation);
            }
            if (lastActiveRightHeldIdleAnimation != null)
            {
                output("lrRightHeldIdleAnim", lastActiveRightHeldIdleAnimation);
            }
        }

        public void OnIfpModeChanged(bool prev, bool now)
        {
            if (prev == now) return;

            var animcodes = ActiveAnimationsByAnimCode.Keys.ToArray();

            string stopVariant = now ? "-fp" : "-ifp";

            foreach (var animcode in animcodes)
            {
                if (animcode.EndsWith(stopVariant))
                {
                    StopAnimation(animcode);
                }
            }
        }
    }
}
