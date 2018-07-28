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

        public bool FlyMode;  // true when flying or swimming
        
        public bool NoClip;
        public EnumFreeMovAxisLock FlyPlaneLock;

        public Vec3d WalkVector = new Vec3d();
        public Vec3d FlyVector = new Vec3d();

        public bool TriesToMove { get { return Forward || Backward || Left || Right; } }


        public bool IsFlying;
        public bool IsClimbing;
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
        public ModelTransform UsingHeldItemTransform;

        public float MovespeedMultiplier = 1f;
        public bool Dirty;
        

        public bool Forward {
            get { return forward; }
            set { forward = value; Dirty = true; }
        }

        public bool Backward
        {
            get { return backward; }
            set { backward = value; Dirty = true; }
        }

        public bool Left
        {
            get { return left; }
            set { left = value; Dirty = true; }
        }

        public bool Right
        {
            get { return right; }
            set { right = value; Dirty = true; }
        }

        public bool Jump
        {
            get { return jump; }
            set { jump = value; Dirty = true; }
        }

        public bool Sneak
        {
            get { return sneak; }
            set { sneak = value; Dirty = true; }
        }

        public bool Sitting
        {
            get { return sitting; }
            set { sitting = value; Dirty = true; }
        }

        public bool FloorSitting
        {
            get { return floorSitting; }
            set { floorSitting = value; Dirty = true; }
        }

        public bool Sprint
        {
            get { return sprint; }
            set { sprint = value; Dirty = true; }
        }

        public bool Up
        {
            get { return up; }
            set { up = value; Dirty = true; }
        }

        public bool Down
        {
            get { return down; }
            set { down = value; Dirty = true; }
        }

        public bool LeftMouseDown
        {
            get { return leftMouseDown; }
            set { leftMouseDown = value; Dirty = true; }
        }

        public bool RightMouseDown
        {
            get { return rightMouseDown; }
            set { rightMouseDown = value; Dirty = true; }
        }


        public void CalcMovementVectors(EntityPos pos, float dt)
        {
            double moveSpeed = dt * GlobalConstants.BaseMoveSpeed * MovespeedMultiplier;
            
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
