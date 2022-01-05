using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class AnimationManager : IAnimationManager
    {
        protected ICoreAPI api;
        ICoreClientAPI capi;

        /// <summary>
        /// Are the animations dirty in this AnimationManager?
        /// </summary>
        public bool AnimationsDirty { get; set; }

        /// <summary>
        /// The animator for the animation manager.
        /// </summary>
        public IAnimator Animator { get; set; }

        /// <summary>
        /// The entity head controller for this animator.
        /// </summary>
        public EntityHeadController HeadController { get; set; }

        /// <summary>
        /// The list of currently active animations that should be playing
        /// </summary>
        public Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode = new Dictionary<string, AnimationMetaData>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, AnimationMetaData> IAnimationManager.ActiveAnimationsByAnimCode => ActiveAnimationsByAnimCode;

        
        /// <summary>
        /// The entity attached to this Animation Manager.
        /// </summary>
        protected Entity entity;

        public AnimationManager()
        {
            
        }

        /// <summary>
        /// Initializes the Animation Manager.
        /// </summary>
        /// <param name="api">The Core API.</param>
        /// <param name="entity">The entity this manager is attached to.</param>
        public virtual void Init(ICoreAPI api, Entity entity)
        {
            this.api = api;
            this.entity = entity;

            if (api.Side == EnumAppSide.Server)
            {
                //listenerId = (api as ICoreServerAPI).Event.RegisterGameTickListener(OnServerTick, 25);
                //api.World.Logger.Notification("AnimationManager: Register tick listener {0} for entity id {1}", listenerId, entity.EntityId);
            }
            else
            {
                // Called by RenderEntities.cs
                //capi.Event.RegisterRenderer(renderer = new DummyRenderer() { action = OnClientFrame, RenderRange = 999 }, EnumRenderStage.Before, "anim");

                capi = (api as ICoreClientAPI);
            }
        }



        public virtual bool IsAnimationActive(params string[] anims)
        {
            foreach (var val in anims)
            {
                if (ActiveAnimationsByAnimCode.ContainsKey(val)) return true;
            }

            return false;
        }

        /// <summary>
        /// Client: Starts given animation
        /// Server: Sends all active anims to all connected clients then purges the ActiveAnimationsByAnimCode list
        /// </summary>
        /// <param name="animdata"></param>
        public virtual bool StartAnimation(AnimationMetaData animdata)
        {
            AnimationMetaData activeAnimdata;

            // Already active, won't do anything
            if (ActiveAnimationsByAnimCode.TryGetValue(animdata.Animation, out activeAnimdata) && activeAnimdata == animdata) return false;

            if (animdata.Code == null)
            {
                throw new Exception("anim meta data code cannot be null!");
            }

            AnimationsDirty = true;
            ActiveAnimationsByAnimCode[animdata.Animation] = animdata;
            entity.UpdateDebugAttributes();

            return true;
        }


        /// <summary>
        /// Start a new animation defined in the entity config file. If it's not defined, it won't play.
        /// Use StartAnimation(AnimationMetaData animdata) to circumvent the entity config anim data.
        /// </summary>
        /// <param name="configCode">Anim config code, not the animation code!</param>
        public virtual bool StartAnimation(string configCode)
        {
            if (configCode == null) return false;

            AnimationMetaData animdata;

            if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(configCode, out animdata))
            {
                StartAnimation(animdata);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Stops given animation
        /// </summary>
        /// <param name="code"></param>
        public virtual void StopAnimation(string code)
        {
            if (code == null) return;

            if (entity.World.Side == EnumAppSide.Server)
            {
                AnimationsDirty = true;
            }

            if (!ActiveAnimationsByAnimCode.Remove(code) && ActiveAnimationsByAnimCode.Count > 0)
            {
                foreach (var val in ActiveAnimationsByAnimCode)
                {
                    if (val.Value.Code == code)
                    {
                        ActiveAnimationsByAnimCode.Remove(val.Key);
                        break;
                    }
                }
            }

            if (entity.World.EntityDebugMode)
            {
                entity.UpdateDebugAttributes();
            }
        }


        /// <summary>
        /// The event fired when the manager recieves the server animations.
        /// </summary>
        /// <param name="activeAnimations"></param>
        /// <param name="activeAnimationsCount"></param>
        /// <param name="activeAnimationSpeeds"></param>
        public virtual void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            HashSet<string> toKeep = new HashSet<string>();
            
            string active = "";
            int mask = ~(1 << 31); // Because I fail to get the sign bit transmitted correctly over the network T_T

            for (int i = 0; i < activeAnimationsCount; i++)
            {
                uint crc32 = (uint)(activeAnimations[i] & mask);

                AnimationMetaData animmetadata;
                if (entity.Properties.Client.AnimationsByCrc32.TryGetValue(crc32, out animmetadata))
                {
                    toKeep.Add(animmetadata.Animation);

                    if (ActiveAnimationsByAnimCode.ContainsKey(animmetadata.Code)) continue;
                    animmetadata.AnimationSpeed = activeAnimationSpeeds[i];

                    ActiveAnimationsByAnimCode[animmetadata.Animation] = animmetadata;
                    continue;
                }

                Animation anim;
                if (entity.Properties.Client.LoadedShapeForEntity.AnimationsByCrc32.TryGetValue(crc32, out anim)) {

                    toKeep.Add(anim.Code);

                    if (ActiveAnimationsByAnimCode.ContainsKey(anim.Code)) continue;

                    string code = anim.Code == null ? anim.Name.ToLowerInvariant() : anim.Code;
                    active += ", " + code;
                    AnimationMetaData animmeta;
                    entity.Properties.Client.AnimationsByMetaCode.TryGetValue(code, out animmeta);

                    if (animmeta == null)
                    {
                        animmeta = new AnimationMetaData()
                        {
                            Code = code,
                            Animation = code,
                            CodeCrc32 = anim.CodeCrc32
                        };
                    }

                    animmeta.AnimationSpeed = activeAnimationSpeeds[i];

                    ActiveAnimationsByAnimCode[anim.Code] = animmeta;
                }
            }


            if (entity.EntityId != (entity.World as IClientWorldAccessor).Player.Entity.EntityId)
            {
                string[] keys = ActiveAnimationsByAnimCode.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!toKeep.Contains(keys[i]))
                    {
                        AnimationMetaData animmetadata;
                        if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(keys[i], out animmetadata))
                        {
                            if (animmetadata.TriggeredBy != null && animmetadata.WasStartedFromTrigger) continue;
                        }
                        ActiveAnimationsByAnimCode.Remove(keys[i]);
                    }
                }
            }

        }

        /// <summary>
        /// Serializes the slots contents to be stored in the SaveGame
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="forClient"></param>
        public virtual void ToAttributes(ITreeAttribute tree, bool forClient)
        {
            foreach (var val in ActiveAnimationsByAnimCode)
            {
                if (val.Value.Code == null) val.Value.Code = val.Key; // ah wtf.

                if (!forClient && val.Value.Code != "die") continue;

                RunningAnimation anim = Animator.GetAnimationState(val.Value.Animation);
                if (anim != null)
                {
                    val.Value.StartFrameOnce = anim.CurrentFrame;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        val.Value.ToBytes(writer);
                    }

                    tree[val.Key] = new ByteArrayAttribute(ms.ToArray());
                }

                val.Value.StartFrameOnce = 0;
            }
        }



        /// <summary>
        /// Loads the entity from a stored byte array from the SaveGame
        /// </summary>
        /// <param name="tree"></param>
        public virtual void FromAttributes(ITreeAttribute tree, string version)
        {
            foreach (var val in tree)
            {
                byte[] data = (val.Value as ByteArrayAttribute).value;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        ActiveAnimationsByAnimCode[val.Key] = AnimationMetaData.FromBytes(reader, version);
                    }
                }
            }
        }

        /// <summary>
        /// The event fired at each server tick.
        /// </summary>
        /// <param name="dt"></param>
        public void OnServerTick(float dt)
        {
            Animator?.OnFrame(ActiveAnimationsByAnimCode, dt);
            Animator.CalculateMatrices = !entity.Alive;
        }
        
        /// <summary>
        /// The event fired each time the client ticks.
        /// </summary>
        /// <param name="dt"></param>
        public void OnClientFrame(float dt)
        {
            if (capi.IsGamePaused || (!entity.IsRendered && entity.Alive)) return; // Too cpu intensive to run all loaded entities
            
            Animator.OnFrame(ActiveAnimationsByAnimCode, dt);

            if (HeadController != null)
            {
                HeadController.OnFrame(dt);
            }
        }

        /// <summary>
        /// Disposes of the animation manager.
        /// </summary>
        public void Dispose()
        {
            if (api.Side == EnumAppSide.Server)
            {
                //(api as ICoreServerAPI).Event.UnregisterGameTickListener(listenerId);
                //api.World.Logger.Notification("AnimationManager: Delete tick listener {0} for entity id {1}", listenerId, entity.EntityId);
            }
            else
            {
                //(api as ICoreClientAPI).Event.UnregisterRenderer(renderer, EnumRenderStage.Before);
                //renderer.Dispose();
            }
        }

        public virtual void OnAnimationStopped(string code)
        {
            
        }
    }
}




/*if (player == api.World.Player && api.Render.CameraType == EnumCameraMode.FirstPerson)
{
    AttachmentPointAndPose apap = null;
    curAnimator.AttachmentPointByCode.TryGetValue("Eyes", out apap);
    float[] tmpMat = Mat4f.Create();

    for (int i = 0; i < 16; i++) tmpMat[i] = ModelMat[i];
    AttachmentPoint ap = apap.AttachPoint;

    float[] mat = apap.Pose.AnimModelMatrix;
    Mat4f.Mul(tmpMat, tmpMat, mat);

    Mat4f.Translate(tmpMat, tmpMat, (float)ap.PosX / 16f, (float)ap.PosY / 16f, (float)ap.PosZ / 16f);
    Mat4f.RotateX(tmpMat, tmpMat, (float)(ap.RotationX) * GameMath.DEG2RAD);
    Mat4f.RotateY(tmpMat, tmpMat, (float)(ap.RotationY) * GameMath.DEG2RAD);
    Mat4f.RotateZ(tmpMat, tmpMat, (float)(ap.RotationZ) * GameMath.DEG2RAD);
    float[] vec = new float[] { 0,0,0, 0 };
    float[] outvec = Mat4f.MulWithVec4(tmpMat, vec);

    api.Render.CameraOffset.Translation.Set(outvec[0], outvec[1] + 1, outvec[2]);
}*/

