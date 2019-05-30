using System;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public delegate void OnEntityAction(EnumEntityAction action, ref EnumHandling handled);

    /// <summary>
    /// A players in-world action
    /// </summary>
    public enum EnumEntityAction
    {
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
        /// Sit (unused)
        /// </summary>
        Sit = 7,
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
        Down = 12
    }

    /// <summary>
    /// The available controls to move around a character in a game world
    /// </summary>
    public class EntityControls
    {
        /// <summary>
        /// To execute a call handler registered by the engine. Don't use this one, use api.Input.InWorldAction instead.
        /// </summary>
        public OnEntityAction OnAction = (EnumEntityAction action, ref EnumHandling handled) => { };

        bool[] flags = new bool[13];

        public bool[] Flags => flags;

        /// <summary>
        /// If true, the entity is either flying or swimming.
        /// </summary>
        public bool FlyMode;  // true when flying or swimming
        
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
        public ModelTransform UsingHeldItemTransformBefore;
        public ModelTransform UsingHeldItemTransformAfter;

        /// <summary>
        /// The movement speed multiplier.
        /// </summary>
        public float MovespeedMultiplier = 1f;

        /// <summary>
        /// Whether or not this entity is dirty.
        /// </summary>
        public bool Dirty;

        

        /// <summary>
        /// A check for if the entity is moving in the direction it's facing.
        /// </summary>
        public bool Forward {
            get { return flags[(int)EnumEntityAction.Forward]; }
            set {
                AttemptToggleAction(EnumEntityAction.Forward, value);
            }
        }



        /// <summary>
        /// A check for if the entity is moving the opposite direction it's facing.
        /// </summary>
        public bool Backward
        {
            get { return flags[(int)EnumEntityAction.Backward]; }
            set { AttemptToggleAction(EnumEntityAction.Backward, value); }
        }

        /// <summary>
        /// A check to see if the entity is moving left the direction it's facing.
        /// </summary>
        public bool Left
        {
            get { return flags[(int)EnumEntityAction.Left]; }
            set { AttemptToggleAction(EnumEntityAction.Left, value); }
        }

        /// <summary>
        /// A check to see if the entity is moving right the direction it's facing.
        /// </summary>
        public bool Right
        {
            get { return flags[(int)EnumEntityAction.Right]; }
            set { AttemptToggleAction(EnumEntityAction.Right, value); }
        }

        /// <summary>
        /// A check whether to see if the entity is jumping.
        /// </summary>
        public bool Jump
        {
            get { return flags[(int)EnumEntityAction.Jump]; }
            set { AttemptToggleAction(EnumEntityAction.Jump, value); }
        }

        /// <summary>
        /// A check whether to see if the entity is sneaking.
        /// </summary>
        public bool Sneak
        {
            get { return flags[(int)EnumEntityAction.Sneak]; }
            set { AttemptToggleAction(EnumEntityAction.Sneak, value); }
        }

        /// <summary>
        /// A check to see whether the entity is sitting.
        /// </summary>
        public bool Sitting
        {
            get { return flags[(int)EnumEntityAction.Sit]; }
            set { AttemptToggleAction(EnumEntityAction.Sit, value); }
        }

        /// <summary>
        /// A check to see whether the entity is sitting on the floor.
        /// </summary>
        public bool FloorSitting
        {
            get { return flags[(int)EnumEntityAction.FloorSit]; }
            set { AttemptToggleAction(EnumEntityAction.FloorSit, value); }
        }

        /// <summary>
        /// A check to see whether the entity is sprinting.
        /// </summary>
        public bool Sprint
        {
            get { return flags[(int)EnumEntityAction.Sprint]; }
            set { AttemptToggleAction(EnumEntityAction.Sprint, value); }
        }

        /// <summary>
        /// A check to see whether the entity is moving up.
        /// </summary>
        public bool Up
        {
            get { return flags[(int)EnumEntityAction.Up]; }
            set { AttemptToggleAction(EnumEntityAction.Up, value); }
        }

        /// <summary>
        /// A check to see whether the entity is moving down.
        /// </summary>
        public bool Down
        {
            get { return flags[(int)EnumEntityAction.Down]; }
            set { AttemptToggleAction(EnumEntityAction.Down, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding the left mouse button down.
        /// </summary>
        public bool LeftMouseDown
        {
            get { return flags[(int)EnumEntityAction.LeftMouseDown]; }
            set { AttemptToggleAction(EnumEntityAction.LeftMouseDown, value); }
        }

        /// <summary>
        /// A check to see if the entity is holding the right mouse button down.
        /// </summary>
        public bool RightMouseDown
        {
            get { return flags[(int)EnumEntityAction.RightMouseDown]; }
            set { AttemptToggleAction(EnumEntityAction.RightMouseDown, value); }
        }

        public bool this[EnumEntityAction action]
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



        void AttemptToggleAction(EnumEntityAction action, bool on)
        {
            if (on)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                OnAction(action, ref handling);
                if (handling != EnumHandling.PassThrough) return;
                flags[(int)action] = true;
            } else
            {
                flags[(int)action] = false;
            }

            Dirty = true;
        }


        /// <summary>
        /// Calculates the movement vectors for the player.
        /// </summary>
        /// <param name="pos">The position of the player.</param>
        /// <param name="dt">The change in time.</param>
        public void CalcMovementVectors(EntityPos pos, float dt)
        {
            double moveSpeed = dt * GlobalConstants.BaseMoveSpeed * MovespeedMultiplier * GlobalConstants.OverallSpeedMultiplier;
            
            double dz = (Forward ? -moveSpeed : 0) + (Backward ? moveSpeed : 0);
            double dx = (Right ? moveSpeed : 0) + (Left ? -moveSpeed : 0);
            
            if (this.Sitting) { dz = 0; dx = 0; }

            double cosPitch = Math.Cos(pos.Pitch);
            double sinPitch = Math.Sin(pos.Pitch);

            double cosYaw = Math.Cos(Math.PI / 2 - pos.Yaw);
            double sinYaw = Math.Sin(Math.PI / 2 - pos.Yaw);

            WalkVector.Set(
                dx * cosYaw - dz * sinYaw,
                0,
                dx * sinYaw + dz * cosYaw
            );

            if (FlyPlaneLock == EnumFreeMovAxisLock.Y) { cosPitch=-1 ; }

            FlyVector.Set(
                dx * cosYaw + dz * cosPitch * sinYaw,
                - dz * sinPitch,
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
        public void SetFrom(EntityControls controls)
        {
            for (int i = 0; i < controls.flags.Length; i++)
            {
                flags[i] = controls.flags[i];
            }
            
            FlyMode = controls.FlyMode;
            FlyPlaneLock = controls.FlyPlaneLock;
            IsFlying = controls.IsFlying;
            NoClip = controls.NoClip;
        }


        /// <summary>
        /// Updates the data from the packet.
        /// </summary>
        /// <param name="pressed">Whether or not the key was pressed.</param>
        /// <param name="action">the id of the key that was pressed.</param>
        public void UpdateFromPacket(bool pressed, int action)
        {
            flags[action] = pressed;
        }
        
        /// <summary>
        /// Forces the entity to stop all movements.
        /// </summary>
        public void StopAllMovement()
        {
            for (int i = 0; i < flags.Length; i++) flags[i] = false;
        }

        /// <summary>
        /// Converts the values to a single int flag.
        /// </summary>
        /// <returns>the compressed integer.</returns>
        public int ToInt()
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
                (flags[(int)EnumEntityAction.Sit] ? 512 : 0) |
                (flags[(int)EnumEntityAction.FloorSit] ? 1024 : 0) |
                (flags[(int)EnumEntityAction.LeftMouseDown] ? 2048 : 0) |
                (flags[(int)EnumEntityAction.RightMouseDown] ? 4096 : 0) |
                (IsClimbing ? 8192 : 0)
            ;
        }

        /// <summary>
        /// Converts the int flags to movement controls.
        /// </summary>
        /// <param name="flagsInt">The compressed integer.</param>
        public void FromInt(int flagsInt)
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

            flags[(int)EnumEntityAction.Sit] = (flagsInt & 512) > 0;
            flags[(int)EnumEntityAction.FloorSit] = (flagsInt & 1024) > 0;
            flags[(int)EnumEntityAction.LeftMouseDown] = (flagsInt & 2048) > 0;
            flags[(int)EnumEntityAction.RightMouseDown] = (flagsInt & 4096) > 0;
            
            IsClimbing = (flagsInt & 8192) > 0;
        }


        internal void ToBytes(BinaryWriter writer)
        {
            writer.Write(ToInt());
        }

        internal void FromBytes(BinaryReader reader, bool ignoreData)
        {
            int flags = reader.ReadInt32();
            if (!ignoreData) FromInt(flags);
        }


    }

}
