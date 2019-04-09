using System;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The available controls to move around a character in a game world
    /// </summary>
    public class EntityControls
    {
        bool forward;
        bool backward;
        bool left;
        bool right;
        bool jump;
        bool sneak;
        bool sprint;
        bool sitting;
        bool floorSitting;

        bool leftMouseDown;
        bool rightMouseDown;

        // When flying or swimming
        bool up;
        bool down;

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
            get { return forward; }
            set { forward = value; Dirty = true; }
        }

        /// <summary>
        /// A check for if the entity is moving the opposite direction it's facing.
        /// </summary>
        public bool Backward
        {
            get { return backward; }
            set { backward = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see if the entity is moving left the direction it's facing.
        /// </summary>
        public bool Left
        {
            get { return left; }
            set { left = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see if the entity is moving right the direction it's facing.
        /// </summary>
        public bool Right
        {
            get { return right; }
            set { right = value; Dirty = true; }
        }

        /// <summary>
        /// A check whether to see if the entity is jumping.
        /// </summary>
        public bool Jump
        {
            get { return jump; }
            set { jump = value; Dirty = true; }
        }

        /// <summary>
        /// A check whether to see if the entity is sneaking.
        /// </summary>
        public bool Sneak
        {
            get { return sneak; }
            set { sneak = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see whether the entity is sitting.
        /// </summary>
        public bool Sitting
        {
            get { return sitting; }
            set { sitting = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see whether the entity is sitting on the floor.
        /// </summary>
        public bool FloorSitting
        {
            get { return floorSitting; }
            set { floorSitting = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see whether the entity is sprinting.
        /// </summary>
        public bool Sprint
        {
            get { return sprint; }
            set { sprint = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see whether the entity is moving up.
        /// </summary>
        public bool Up
        {
            get { return up; }
            set { up = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see whether the entity is moving down.
        /// </summary>
        public bool Down
        {
            get { return down; }
            set { down = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see if the entity is holding the left mouse button down.
        /// </summary>
        public bool LeftMouseDown
        {
            get { return leftMouseDown; }
            set { leftMouseDown = value; Dirty = true; }
        }

        /// <summary>
        /// A check to see if the entity is holding the right mouse button down.
        /// </summary>
        public bool RightMouseDown
        {
            get { return rightMouseDown; }
            set { rightMouseDown = value; Dirty = true; }
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
            
            if (sitting) { dz = 0; dx = 0; }

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
            Forward = controls.Forward;
            Backward = controls.Backward;
            Left = controls.Left;
            Right = controls.Right;
            Jump = controls.Jump;
            Sneak = controls.Sneak;
            Sprint = controls.Sprint;
            Up = controls.Up;
            Down = controls.Down;
            FlyMode = controls.FlyMode;
            FlyPlaneLock = controls.FlyPlaneLock;
            IsFlying = controls.IsFlying;
            NoClip = controls.NoClip;
            sitting = controls.sitting;
            floorSitting = controls.floorSitting;
            leftMouseDown = controls.leftMouseDown;
            rightMouseDown = controls.rightMouseDown;
        }


        /// <summary>
        /// Updates the data from the packet.
        /// </summary>
        /// <param name="pressed">Whether or not the key was pressed.</param>
        /// <param name="key">the id of the key that was pressed.</param>
        public void UpdateFromPacket(bool pressed, int key)
        {
            switch (key)
            {
                case 0:
                    Forward = pressed;
                    break;

                case 1:
                    Backward = pressed;
                    break;

                case 2:
                    Left = pressed;
                    break;

                case 3:
                    Right = pressed;
                    break;

                case 4:
                    Jump = pressed;
                    break;

                case 5:
                    Sneak = pressed;
                    break;

                case 6:
                    Down = pressed;
                    break;

                case 7:
                    Sprint = pressed;
                    break;

                case 8:
                    sitting = pressed;
                    break;

                case 9:
                    floorSitting = pressed;
                    break;

                case 10:
                    leftMouseDown = pressed;
                    break;

                case 11:
                    rightMouseDown = pressed;
                    break;
            }
        }
        
        /// <summary>
        /// Forces the entity to stop all movements.
        /// </summary>
        public void StopAllMovement()
        {
            Forward = false;
            Backward = false;
            Left = false;
            Right = false;
            Jump = false;
            Sneak = false;
            Sprint = false;
            Up = false;
            Down = false;
            LeftMouseDown = false;
            RightMouseDown = false;
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
                (sitting ? 512 : 0) |
                (floorSitting ? 1024 : 0) |
                (leftMouseDown ? 2048 : 0) |
                (rightMouseDown ? 4096 : 0) |
                (IsClimbing ? 8192 : 0)
            ;
        }

        /// <summary>
        /// Converts the int flags to movement controls.
        /// </summary>
        /// <param name="flags">The compressed integer.</param>
        public void FromInt(int flags)
        {
            Forward = (flags & 1) > 0;
            Backward = (flags & 2) > 0;
            Left = (flags & 4) > 0;
            Right = (flags & 8) > 0;
            Jump = (flags & 16) > 0;
            Sneak = (flags & 32) > 0;
            Sprint = (flags & 64) > 0;
            Up = (flags & 128) > 0;
            Down = (flags & 256) > 0;
            sitting = (flags & 512) > 0;
            floorSitting = (flags & 1024) > 0;
            leftMouseDown = (flags & 2048) > 0;
            rightMouseDown = (flags & 4096) > 0;
            IsClimbing = (flags & 8192) > 0;
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
