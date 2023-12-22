using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

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

            if (haveHandUse && handUseStopped)
            {
                startHeldReadyAnimIfMouseUp();
            }

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

            if (useFpAnimSet)
            {
                plrEntity.TpAnimManager.StartAnimation(configCode);
            }

            AnimationMetaData animdata;

            if (useFpAnimSet && entity.Properties.Client.AnimationsByMetaCode.TryGetValue(configCode + fpEnding, out animdata))
            {
                StartAnimation(animdata);
                return true;
            }

            return base.StartAnimation(configCode);
        }

        public override bool StartAnimation(AnimationMetaData animdata)
        {
            if (useFpAnimSet && !animdata.Code.EndsWith(fpEnding))
            {
                plrEntity.TpAnimManager.StartAnimation(animdata);

                if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(animdata.Code + fpEnding, out var animdatafp))
                {
                    if (ActiveAnimationsByAnimCode.TryGetValue(animdatafp.Animation, out var activeAnimdata) && activeAnimdata == animdatafp) return false;
                    return base.StartAnimation(animdatafp);
                }
            }

            return base.StartAnimation(animdata);
        }

        public override void RegisterFrameCallback(AnimFrameCallback trigger)
        {
            if (useFpAnimSet && !trigger.Animation.EndsWith(fpEnding) && entity.Properties.Client.AnimationsByMetaCode.ContainsKey(trigger.Animation + fpEnding))
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

        protected override void onReceivedServerAnimation(AnimationMetaData animmetadata)
        {
            StartAnimation(animmetadata);
        }

        public override void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            base.OnReceivedServerAnimations(activeAnimations, activeAnimationsCount, activeAnimationSpeeds);
        }


        string prevHeldReadyAnim;
        protected string lastRunningHeldUseAnimation;
        protected string lastRunningRightHeldIdleAnimation;
        protected string lastRunningLeftHeldIdleAnimation;
        protected string lastRunningHeldHitAnimation;

        bool haveHandUse;
        bool handUseStopped;

        public void OnActiveSlotChanged(ItemSlot slot)
        {
            string beginholdAnim = slot.Itemstack?.Collectible?.GetHeldReadyAnimation(slot, entity, EnumHand.Right);
            if (beginholdAnim != null) StartHeldReadyAnim(beginholdAnim);
        }
        public void OnHandUseStopped()
        {
            handUseStopped = true;
            startHeldReadyAnimIfMouseUp();
        }

        private void startHeldReadyAnimIfMouseUp()
        {
            bool mouseDown = capi.Input.MouseButton.Left || capi.Input.MouseButton.Right;
            if (!mouseDown)
            {
                var slot = capi.World.Player.InventoryManager?.ActiveHotbarSlot;
                var animCode = slot.Itemstack?.Collectible?.GetHeldReadyAnimation(slot, entity, EnumHand.Right);
                if (animCode != null) StartHeldReadyAnim(animCode, true);
                handUseStopped = true;
                haveHandUse = false;
            }
        }

        public void StartHeldReadyAnim(string heldReadyAnim, bool force = false)
        {
            if (!force && (IsHeldHitActive() || IsHeldUseActive())) return;
            
            if (prevHeldReadyAnim != null) StopAnimation(prevHeldReadyAnim);

            ResetAnimation(heldReadyAnim);
            StartAnimation(heldReadyAnim);

            prevHeldReadyAnim = heldReadyAnim;
        }
        

        public void StartHeldUseAnim(string animCode)
        {
            StopHeldReadyAnim();
            StopAnimation(lastRunningRightHeldIdleAnimation);
            StopAnimation(lastRunningHeldHitAnimation);
            StartAnimation(animCode);
            lastRunningHeldUseAnimation = animCode;

            haveHandUse = true;
            handUseStopped = false;
        }

        public void StartHeldHitAnim(string animCode)
        {
            StopHeldReadyAnim();
            StopAnimation(lastRunningRightHeldIdleAnimation);
            StopAnimation(lastRunningHeldUseAnimation);
            StartAnimation(animCode);
            lastRunningHeldHitAnimation = animCode;

            haveHandUse = true;
            handUseStopped = false;
        }

        public void StartRightHeldIdleAnim(string animCode)
        {
            StopAnimation(lastRunningRightHeldIdleAnimation);
            StopAnimation(lastRunningHeldUseAnimation);
            StartAnimation(animCode);
            lastRunningRightHeldIdleAnimation = animCode;
        }


        public void StartLeftHeldIdleAnim(string animCode)
        {
            StopAnimation(lastRunningLeftHeldIdleAnimation);
            StartAnimation(animCode);
            lastRunningLeftHeldIdleAnimation = animCode;
        }



        public void StopHeldReadyAnim()
        {
            StopAnimation(prevHeldReadyAnim);
            prevHeldReadyAnim = null;
        }

        public void StopHeldUseAnim()
        {
            StopAnimation(lastRunningHeldUseAnimation);
            lastRunningHeldUseAnimation = null;
        }

        public void StopHeldAttackAnim()
        {
            if (lastRunningHeldHitAnimation != null && entity.Properties.Client.AnimationsByMetaCode.TryGetValue(lastRunningHeldHitAnimation, out var animData))
            {
                if (animData.Attributes?.IsTrue("authorative") == true)
                {
                    if (IsHeldHitActive()) return;
                }
            }

            StopAnimation(lastRunningHeldHitAnimation);
            lastRunningHeldHitAnimation = null;
        }

        public void StopRightHeldIdleAnim()
        {
            StopAnimation(lastRunningRightHeldIdleAnimation);
            lastRunningRightHeldIdleAnimation = null;
        }
        public void StopLeftHeldIdleAnim()
        {
            StopAnimation(lastRunningLeftHeldIdleAnimation);
            lastRunningLeftHeldIdleAnimation = null;
        }


        public bool IsHeldHitAuthorative()
        {
            return IsAuthorative(lastRunningHeldHitAnimation);
        }

        private bool IsAuthorative(string anim)
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
            return lastRunningHeldUseAnimation != null && IsAnimationActive(lastRunningHeldUseAnimation);
        }

        public bool IsHeldHitActive()
        {
            return lastRunningHeldHitAnimation != null && IsAnimationActive(lastRunningHeldHitAnimation);
        }

        public bool IsLeftHeldActive()
        {
            return lastRunningLeftHeldIdleAnimation != null && IsAnimationActive(lastRunningLeftHeldIdleAnimation);
        }

        public bool IsRightHeldActive()
        {
            return lastRunningRightHeldIdleAnimation != null && IsAnimationActive(lastRunningRightHeldIdleAnimation);
        }

        public bool HeldUseAnimChanged(string nowHeldRightUseAnim)
        {
            return lastRunningHeldUseAnimation != null && nowHeldRightUseAnim != lastRunningHeldUseAnimation;
        }

        public bool HeldHitAnimChanged(string nowHeldRightHitAnim)
        {
            return lastRunningHeldHitAnimation != null && nowHeldRightHitAnim != lastRunningHeldHitAnimation;
        }

        public bool RightHeldIdleChanged(string nowHeldRightIdleAnim)
        {
            return lastRunningRightHeldIdleAnimation != null && nowHeldRightIdleAnim != lastRunningRightHeldIdleAnimation;
        }

        public bool LeftHeldIdleChanged(string nowHeldLeftIdleAnim)
        {
            return lastRunningLeftHeldIdleAnimation != null && nowHeldLeftIdleAnim != lastRunningLeftHeldIdleAnimation;
        }


        public override void FromAttributes(ITreeAttribute tree, string version)
        {
            base.FromAttributes(tree, version);

            lastRunningHeldUseAnimation = tree.GetString("lrHeldUseAnim");
            lastRunningHeldHitAnimation = tree.GetString("lrHeldHitAnim");
            // Can we not have this line? It breaks fp hands when loading up with a world with a block in hands - the shoulds of the hands become visible when walking and looking down
            //lastRunningRightHeldIdleAnimation = tree.GetString("lrRightHeldIdleAnim");
        }

        public override void ToAttributes(ITreeAttribute tree, bool forClient)
        {
            base.ToAttributes(tree, forClient);

            if (lastRunningHeldUseAnimation != null)
            {
                tree.SetString("lrHeldUseAnim", lastRunningHeldUseAnimation);
            }
            if (lastRunningHeldHitAnimation != null)
            {
                tree.SetString("lrHeldHitAnim", lastRunningHeldHitAnimation);
            }
            if (lastRunningRightHeldIdleAnimation != null)
            {
                tree.SetString("lrRightHeldIdleAnim", lastRunningRightHeldIdleAnimation);
            }
        }

    }
}
