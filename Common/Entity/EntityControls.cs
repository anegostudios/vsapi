using System;
using System.IO;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public delegate void OnEntityAction(EnumEntityAction action, bool on, ref EnumHandling handled);

    /// <summary>
    /// A players in-world action
    /// </summary>
    public enum EnumEntityAction
    {
        /// <summary>
        /// No action - used when setting preCondition
        /// </summary>
        None = -1,
        /// <summary>
        /// Walk forwards
        /// </summary>
        Forward = 0,
        /// <summary>
        /// Walk backwards
        /// </summary>
        Backward = 1,
        /// <summary>
        /// Walk sideways left
        /// </summary>
        Left = 2,
        /// <summary>
        /// Walk sideways right
        /// </summary>
        Right = 3,
        /// <summary>
        /// Jump
        /// </summary>
        Jump = 4, 
        /// <summary>
        /// Sneak
        /// </summary>
        Sneak = 5, 
        /// <summary>
        /// Sprint mode
        /// </summary>
        Sprint = 6,
        /// <summary>
        /// Glide
        /// </summary>
        Glide = 7,
        /// <summary>
        /// Sit on the ground
        /// </summary>
        FloorSit = 8,
        /// <summary>
        /// Left mouse down
        /// </summary>
        LeftMouseDown = 9,
        /// <summary>
        /// Right mouse down
        /// </summary>
        RightMouseDown = 10,
        /// <summary>
        /// Fly or swim up
        /// </summary>
        Up = 11,
        /// <summary>
        /// Fly or swim down
        /// </summary>
        Down = 12,
        /// <summary>
        /// Holding down the Ctrl key (which might have been remapped)
        /// </summary>
        CtrlKey = 13,
        /// <summary>
        /// Holding down the Shift key (which might have been remapped)
        /// </summary>
        ShiftKey = 14,
        /// <summary>
        /// Left mouse down
        /// </summary>
        InWorldLeftMouseDown = 15,
        /// <summary>
        /// Right mouse down
        /// </summary>
        InWorldRightMouseDown = 16,
    }


    /// <summary>
    /// The available controls to move around a character in a game world
    /// </summary>
    public class EntityControls
    {
        /// <summary>
        /// To execute a call handler registered by the engine. Don't use this one, use api.Input.InWorldAction instead.
        /// </summary>
        public OnEntityAction OnAction = (EnumEntityAction action, bool on, ref EnumHandling handled) => { };

        bool[] flags = new bool[15];

        public bool[] Flags => flags;

        /// <summary>
        /// If true, the entity is either flying, gliding or swimming.
        /// </summary>
        public bool DetachedMode;
        
        /// <summary>
        /// If true, the entity has NoClip active.
        /// </summary>
        public bool NoClip;

        /// <summary>
        /// the axis lock for the fly plane.
        /// </summary>
        public EnumFreeMovAxisLock FlyPlaneLock;

        /// <summary>
        /// Current walking direction.
        /// </summary>
        public Vec3d WalkVector = new Vec3d();

        /// <summary>
        /// Current flying direction
        /// </summary>
        public Vec3d FlyVector = new Vec3d();

        /// <summary>
        /// Checks to see if the entity is attempting to move in any direction (excluding jumping)
        /// </summary>
        public bool TriesToMove { get { return Forward || Backward || Left || Right; } }

        /// <summary>
        /// Whether or not the entity is flying.
        /// </summary>
        public bool IsFlying;

        /// <summary>
        /// Whether or not the entity is climbing
        /// </summary>
        public bool IsClimbing;

        /// <summary>
        /// Whether or not the entity is aiming
        /// </summary>
        public bool IsAiming;

        /// <summary>
        /// Whether or not the entity is currently stepping up a block
        /// </summary>
        public bool IsStepping;

        /// <summary>
        /// If the player is currently using the currently held item in a special way (e.g. attacking with smithing hammer or eating an edible item)
        /// </summary>
        public EnumHandInteract HandUse;

        /// <summary>
        /// The block pos the player started using
        /// </summary>
        public BlockSelection HandUsingBlockSel;


        public int UsingCount;
        public long UsingBeginMS;
        public ModelTransform LeftUsingHeldItemTransformBefore;

        [Obsolete("Setting this value has no effect anymore. Add an animation to the seraph instead")]
        public ModelTransform UsingHeldItemTransformBefore;
        [Obsolete("Setting this value has no effect anymore. Add an animation to the seraph instead")]
        public ModelTransform UsingHeldItemTransformAfter;

        /// <summary>
        /// The movement speed multiplier.
        /// </summary>
        public float MovespeedMultiplier = 1f;

        /// <summary>
        /// Whether or not this entity is dirty.
        /// </summary>
        public bool Dirty;

        public double GlideSpeed = 0;


        /// <summary>
        /// A check for if the entity is moving in the direction it's facing.
        /// </summary>
        public virtual bool Forward {
            get { return flags[(int)EnumEntityAction.Forward]; }
            set {
                AttemptToggleAction(EnumEntityAction.Forward, value);
            }
        }



        /// <summary>
        /// A check for if the entity is moving the opposite direction it's facing.
        /// </summary>
        public virtual bool Backward
        {
            get { return flags[(int)EnumEntityAction.Backward]; }
            set { AttemptToggleAction(EnumEntityAction.Backward, value); }
        }

        /// <summary>
        /// A check to see if the entity is moving left the direction it's facing.
        /// </summary>
        public virtual bool Left
        {
            get { return flags[(int)EnumEntityAction.Left]; }
            set { AttemptToggleAction(EnumEntityAction.Left, value); }
        }

        /// <summary>
        /// A check to see if the entity is moving right the direction it's facing.
        /// </summary>
        public virtual bool Right
        {
            get { return flags[(int)EnumEntityAction.Right]; }
            set { AttemptToggleAction(EnumEntityAction.Right, value); }
        }

        /// <summary>
        /// A check whether to see if the entity is jumping.
        /// </summary>
        public virtual bool Jump
        {
            get { return flags[(int)EnumEntityAction.Jump]; }
            set { AttemptToggleAction(EnumEntityAction.Jump, value); }
        }

        /// <summary>
        /// A check whether to see if the entity is sneaking. Use Controls.ShiftKey instead for mouse interaction modifiers, as it is a separable control.
        /// <br/>A test for Sneak should be used only when we want to know whether the entity is crouching or using Sneak motion, which affects things like whether it is detectable by other entities, seen on the map, or how the shield is used
        /// </summary>
        public virtual bool Sneak
        {
            get { return flags[(int)EnumEntityAction.Sneak]; }
            set { AttemptToggleAction(EnumEntityAction.Sneak, value); }
        }

        /// <summary>
        /// A check to see whether the entity is gliding
        /// </summary>
        public virtual bool Gliding
        {
            get { return flags[(int)EnumEntityAction.Glide]; }
            set { AttemptToggleAction(EnumEntityAction.Glide, value); }
        }

        /// <summary>
        /// A check to see whether the entity is sitting on the floor.
        /// </summary>
        public virtual bool FloorSitting
        {
            get { return flags[(int)EnumEntityAction.FloorSit]; }
            set { AttemptToggleAction(EnumEntityAction.FloorSit, value); }
        }

        /// <summary>
        /// A check to see whether the entity is sprinting. Use Controls.CtrlKey instead for mouse interaction modifiers, as it is a separable control.
        /// <br/>A test for Sprint should be used only when we want to know whether the entity is sprinting.
        /// </summary>
        public virtual bool Sprint
        {
            get { return flags[(int)EnumEntityAction.Sprint]; }
            set { AttemptToggleAction(EnumEntityAction.Sprint, value); }
        }

        /// <summary>
        /// A check to see whether the entity is moving up.
        /// </summary>
        public virtual bool Up
        {
            get { return flags[(int)EnumEntityAction.Up]; }
            set { AttemptToggleAction(EnumEntityAction.Up, value); }
        }

        /// <summary>
        /// A check to see whether the entity is moving down.
        /// </summary>
        public virtual bool Down
        {
            get { return flags[(int)EnumEntityAction.Down]; }
            set { AttemptToggleAction(EnumEntityAction.Down, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding the in-world rleft mouse button down.
        /// </summary>
        public virtual bool LeftMouseDown
        {
            get { return flags[(int)EnumEntityAction.LeftMouseDown]; }
            set { AttemptToggleAction(EnumEntityAction.LeftMouseDown, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding the in-world right mouse button down.
        /// </summary>
        public virtual bool RightMouseDown
        {
            get { return flags[(int)EnumEntityAction.RightMouseDown]; }
            set { AttemptToggleAction(EnumEntityAction.RightMouseDown, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding down the Ctrl key (which may be the same as the Sprint key or one or other may have been remapped).
        /// <br/>Should normally be used in conjunction with a mouse button, including OnHeldInteractStart() methods etc
        /// </summary>
        public virtual bool CtrlKey
        {
            get { return flags[(int)EnumEntityAction.CtrlKey]; }
            set { AttemptToggleAction(EnumEntityAction.CtrlKey, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding down the Shift key (which may be the same as the Sneak key or one or other may have been remapped).
        /// <br/>Should normally be used in conjunction with a mouse button, including OnHeldInteractStart() methods etc
        /// </summary>
        public virtual bool ShiftKey
        {
            get { return flags[(int)EnumEntityAction.ShiftKey]; }
            set { AttemptToggleAction(EnumEntityAction.ShiftKey, value); }
        }

        public virtual bool this[EnumEntityAction action]
        {
            get
            {
                return flags[(int)action];
            }
            set
            {
                flags[(int)action] = value;
            }
        }



        protected virtual void AttemptToggleAction(EnumEntityAction action, bool on)
        {
            if (flags[(int)action] != on)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                OnAction(action, on, ref handling);
                if (handling != EnumHandling.PassThrough) return;
                flags[(int)action] = on;
                Dirty = true;
            }
        }


        /// <summary>
        /// Calculates the movement vectors for the player.
        /// </summary>
        /// <param name="pos">The position of the player.</param>
        /// <param name="dt">The change in time.</param>
        public virtual void CalcMovementVectors(EntityPos pos, float dt)
        {
            double moveSpeed = dt * GlobalConstants.BaseMoveSpeed * MovespeedMultiplier * GlobalConstants.OverallSpeedMultiplier;
            
            double dz = (Forward ? moveSpeed : 0) + (Backward ? -moveSpeed : 0);
            double dx = (Right ? -moveSpeed : 0) + (Left ? moveSpeed : 0);
            
            double cosPitch = Math.Cos(pos.Pitch);
            double sinPitch = Math.Sin(pos.Pitch);

            double cosYaw = Math.Cos(-pos.Yaw);
            double sinYaw = Math.Sin(-pos.Yaw);

            WalkVector.Set(
                dx * cosYaw - dz * sinYaw,
                0,
                dx * sinYaw + dz * cosYaw
            );

            if (FlyPlaneLock == EnumFreeMovAxisLock.Y) { cosPitch=-1 ; }

            FlyVector.Set(
                dx * cosYaw + dz * cosPitch * sinYaw,
                dz * sinPitch,
                dx * sinYaw - dz * cosPitch * cosYaw
            );

            double normalization = (Forward || Backward) && (Right || Left) ? 1 / Math.Sqrt(2) : 1;
            WalkVector.Mul(normalization);

            if (FlyPlaneLock == EnumFreeMovAxisLock.X) { FlyVector.X = 0; }
            if (FlyPlaneLock == EnumFreeMovAxisLock.Y) { FlyVector.Y = 0; }
            if (FlyPlaneLock == EnumFreeMovAxisLock.Z) { FlyVector.Z = 0; }
        }

        /// <summary>
        /// Copies the controls from the provided controls to this set of controls.
        /// </summary>
        /// <param name="controls">The controls to copy over.</param>
        public virtual void SetFrom(EntityControls controls)
        {
            for (int i = 0; i < controls.flags.Length; i++)
            {
                flags[i] = controls.flags[i];
            }
            
            DetachedMode = controls.DetachedMode;
            FlyPlaneLock = controls.FlyPlaneLock;
            IsFlying = controls.IsFlying;
            NoClip = controls.NoClip;
        }


        /// <summary>
        /// Updates the data from the packet.
        /// </summary>
        /// <param name="pressed">Whether or not the key was pressed.</param>
        /// <param name="action">the id of the key that was pressed.</param>
        public virtual void UpdateFromPacket(bool pressed, int action)
        {
            if (flags[action] != pressed)
            {
                AttemptToggleAction((EnumEntityAction)action, pressed);
            }
            
        }
        
        /// <summary>
        /// Forces the entity to stop all movements, resets all flags to false
        /// </summary>
        public virtual void StopAllMovement()
        {
            for (int i = 0; i < flags.Length; i++) flags[i] = false;
        }

        /// <summary>
        /// Converts the values to a single int flag.
        /// </summary>
        /// <returns>the compressed integer.</returns>
        public virtual int ToInt()
        {
            return
                (Forward ? 1 : 0) |
                (Backward ? 2 : 0) |
                (Left ? 4 : 0) |
                (Right ? 8 : 0) |
                (Jump ? 16 : 0) |
                (Sneak ? 32 : 0) |
                (Sprint ? 64 : 0) |
                (Up ? 128 : 0) |
                (Down ? 256 : 0) |
                (flags[(int)EnumEntityAction.Glide] ? 512 : 0) |
                (flags[(int)EnumEntityAction.FloorSit] ? 1024 : 0) |
                (flags[(int)EnumEntityAction.LeftMouseDown] ? 2048 : 0) |
                (flags[(int)EnumEntityAction.RightMouseDown] ? 4096 : 0) |
                (IsClimbing ? 8192 : 0) |
                (flags[(int)EnumEntityAction.CtrlKey] ? 16384 : 0) |
                (flags[(int)EnumEntityAction.ShiftKey] ? 32768 : 0)
            ;
        }

        /// <summary>
        /// Converts the int flags to movement controls.
        /// </summary>
        /// <param name="flagsInt">The compressed integer.</param>
        public virtual void FromInt(int flagsInt)
        {
            Forward = (flagsInt & 1) > 0;
            Backward = (flagsInt & 2) > 0;
            Left = (flagsInt & 4) > 0;
            Right = (flagsInt & 8) > 0;
            Jump = (flagsInt & 16) > 0;
            Sneak = (flagsInt & 32) > 0;
            Sprint = (flagsInt & 64) > 0;
            Up = (flagsInt & 128) > 0;
            Down = (flagsInt & 256) > 0;

            flags[(int)EnumEntityAction.Glide] = (flagsInt & 512) > 0;
            flags[(int)EnumEntityAction.FloorSit] = (flagsInt & 1024) > 0;
            flags[(int)EnumEntityAction.LeftMouseDown] = (flagsInt & 2048) > 0;
            flags[(int)EnumEntityAction.RightMouseDown] = (flagsInt & 4096) > 0;
            
            IsClimbing = (flagsInt & 8192) > 0;

            flags[(int)EnumEntityAction.CtrlKey] = (flagsInt & 16384) > 0;
            flags[(int)EnumEntityAction.ShiftKey] = (flagsInt & 32768) > 0;
        }


        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write(ToInt());
        }

        public virtual void FromBytes(BinaryReader reader, bool ignoreData)
        {
            int flags = reader.ReadInt32();
            if (!ignoreData) FromInt(flags);
        }


    }

}
