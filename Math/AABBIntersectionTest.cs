using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class Line3D
    {
        public double[] Start;
        public double[] End;
    }


    public delegate bool BlockFilter(BlockPos pos, Block block);
    public delegate bool EntityFilter(Entity entity);


    // Based on https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
    // Our picking ray will always be a block at any position along the ray, because empty areas are considered air blocks. 
    // So what we are really interested in is on what block face is the ray exiting and continue search in that direction
    // => This code is a ray-intersects-plane algo. It wanders along the exiting faces of full blocks and checks at each position for collisions with the blocks selection box
    // TODO: Replace with Slab method? -> https://tavianator.com/fast-branchless-raybounding-box-intersections/
    public class AABBIntersectionTest
    {
        BlockFacing hitOnBlockFaceTmp = BlockFacing.DOWN;
        Vec3d hitPositionTmp = new Vec3d();

        Vec3d lastExitedBlockFacePos = new Vec3d();
        public IWorldIntersectionSupplier bsTester;
        Cuboidd tmpCuboidd = new Cuboidd();

        public Vec3d hitPosition = new Vec3d();
        public Ray ray = new Ray();
        public BlockPos pos = new BlockPos();
        public BlockFacing hitOnBlockFace = BlockFacing.DOWN;
        public int hitOnSelectionBox = 0;


        

        public AABBIntersectionTest(IWorldIntersectionSupplier blockSelectionTester)
        {
            this.bsTester = blockSelectionTester;
        }

        public void LoadRayAndPos(Line3D line3d)
        {
            ray.origin.Set(line3d.Start[0], line3d.Start[1], line3d.Start[2]);
            ray.dir.Set(line3d.End[0] - line3d.Start[0],
                 line3d.End[1] - line3d.Start[1],
                 line3d.End[2] - line3d.Start[2]
            );
            pos.SetAndCorrectDimension((int)line3d.Start[0], (int)line3d.Start[1], (int)line3d.Start[2]);
        }

        public void LoadRayAndPos(Ray ray)
        {
            this.ray = ray;
            pos.SetAndCorrectDimension(ray.origin);
        }

        public BlockSelection GetSelectedBlock(Vec3d from, Vec3d to, BlockFilter filter = null)
        {
            LoadRayAndPos(new Line3D()
            {
                Start = new double[] { from.X, from.Y, from.Z },
                End = new double[] { to.X, to.Y, to.Z }
            });

            float maxDistance = from.DistanceTo(to);

            return GetSelectedBlock(maxDistance, filter);
        }

        public BlockSelection GetSelectedBlock(float maxDistance, BlockFilter filter = null, bool testCollide = false)
        {
            float distanceSq = 0;

            // Get the face where our ray will exit
            BlockFacing lastExitedBlockFace = GetExitingFullBlockFace(pos, ref lastExitedBlockFacePos);
            if (lastExitedBlockFace == null) return null;

            float maxDistanceSq = (maxDistance + 1) * (maxDistance + 1);

            // Wander along the block exiting faces until we collide with a block selection box
            while (!RayIntersectsBlockSelectionBox(pos, filter, testCollide))
            {
                if (distanceSq >= maxDistanceSq) return null;

                pos.Offset(lastExitedBlockFace);

                lastExitedBlockFace = GetExitingFullBlockFace(pos, ref lastExitedBlockFacePos);
                if (lastExitedBlockFace == null) return null;

                distanceSq = pos.DistanceSqTo(ray.origin.X - 0.5f, ray.origin.Y - 0.5f, ray.origin.Z - 0.5f);
            }


            if (hitPosition.SquareDistanceTo(ray.origin) > maxDistance * maxDistance) return null;

            return new BlockSelection()
            {
                Face = hitOnBlockFace,
                Position = pos.CopyAndCorrectDimension(),
                HitPosition = hitPosition.SubCopy(pos.X, pos.InternalY, pos.Z),
                SelectionBoxIndex = hitOnSelectionBox,
                Block = blockIntersected
            };
        }


        Block blockIntersected;
        public bool RayIntersectsBlockSelectionBox(BlockPos pos, BlockFilter filter, bool testCollide = false)
        {
            Cuboidf[] hitboxes;
            Block block = bsTester.blockAccessor.GetBlock(pos, BlockLayersAccess.Fluid);
            if (block.SideSolid.Any)   // It's ice!
            {
                hitboxes = testCollide ? block.GetCollisionBoxes(bsTester.blockAccessor, pos) : block.GetSelectionBoxes(bsTester.blockAccessor, pos);
            }
            else
            {
                block = bsTester.GetBlock(pos);
                hitboxes = testCollide ? block.GetCollisionBoxes(bsTester.blockAccessor, pos) : bsTester.GetBlockIntersectionBoxes(pos);
            }
            if (hitboxes == null) return false;
            if (filter?.Invoke(pos, block) == false) return false;

            bool intersects = false;
            bool wasDecor = false;

            for (int i = 0; i < hitboxes.Length; i++)
            {
                tmpCuboidd.Set(hitboxes[i]).Translate(pos.X, pos.InternalY, pos.Z);
                if (RayIntersectsWithCuboid(tmpCuboidd, ref hitOnBlockFaceTmp, ref hitPositionTmp))
                {
                    bool isDecor = hitboxes[i] is DecorSelectionBox;
                    if (intersects && (!wasDecor || isDecor) && hitPosition.SquareDistanceTo(ray.origin) <= hitPositionTmp.SquareDistanceTo(ray.origin))
                    {
                        continue;
                    }

                    hitOnSelectionBox = i;
                    intersects = true;
                    wasDecor = isDecor;
                    hitOnBlockFace = hitOnBlockFaceTmp;
                    hitPosition.Set(hitPositionTmp);
                }
            }

            if (intersects && hitboxes[hitOnSelectionBox] is DecorSelectionBox dsb)
            {
                Vec3i posAdjust = dsb.PosAdjust;
                if (posAdjust != null)
                {
                    pos.Add(posAdjust);
                    block = bsTester.GetBlock(pos);
                }
            }

            if (intersects) blockIntersected = block;
            return intersects;
        }

        public bool RayIntersectsWithCuboid(Cuboidd selectionBox)
        {
            if (selectionBox == null) return false;
            return RayIntersectsWithCuboid(tmpCuboidd, ref hitOnBlockFace, ref hitPosition);
        }

        public bool RayIntersectsWithCuboid(Cuboidf selectionBox, double posX, double posY, double posZ)
        {
            if (selectionBox == null) return false;

            tmpCuboidd.Set(selectionBox).Translate(posX, posY, posZ);
            return RayIntersectsWithCuboid(tmpCuboidd, ref hitOnBlockFace, ref hitPosition);
        }


        public bool RayIntersectsWithCuboid(Cuboidd selectionBox, ref BlockFacing hitOnBlockFace, ref Vec3d hitPosition)
        {
            if (selectionBox == null) return false;

            double w = selectionBox.X2 - selectionBox.X1;
            double h = selectionBox.Y2 - selectionBox.Y1;
            double l = selectionBox.Z2 - selectionBox.Z1;

            for (int i = 0; i < BlockFacing.NumberOfFaces; i++)
            {
                BlockFacing blockSideFacing = BlockFacing.ALLFACES[i];
                Vec3i planeNormal = blockSideFacing.Normali;

                // Dot product of 2 vectors
                // If they are parallel the dot product is 1
                // At 90 degrees the dot product is 0
                double demon = planeNormal.X * ray.dir.X + planeNormal.Y * ray.dir.Y + planeNormal.Z * ray.dir.Z;

                // Does intersect this plane somewhere (only negative because we are not interested in the ray leaving a face, negative because the ray points into the cube, the plane normal points away from the cube)
                if (demon < -0.00001)
                {
                    Vec3d planeCenterPosition = blockSideFacing.PlaneCenter
                        .ToVec3d()
                        .Mul(w, h, l)
                        .Add(selectionBox.X1, selectionBox.Y1, selectionBox.Z1)
                    ;

                    Vec3d pt = Vec3d.Sub(planeCenterPosition, ray.origin);
                    double t = (pt.X * planeNormal.X + pt.Y * planeNormal.Y + pt.Z * planeNormal.Z) / demon;

                    if (t >= 0)
                    {
                        hitPosition = new Vec3d(ray.origin.X + ray.dir.X * t, ray.origin.Y + ray.dir.Y * t, ray.origin.Z + ray.dir.Z * t);
                        lastExitedBlockFacePos = Vec3d.Sub(hitPosition, planeCenterPosition);

                        // Does intersect this plane within the block
                        if (Math.Abs(lastExitedBlockFacePos.X) <= w / 2 && Math.Abs(lastExitedBlockFacePos.Y) <= h / 2 && Math.Abs(lastExitedBlockFacePos.Z) <= l / 2)
                        {
                            hitOnBlockFace = blockSideFacing;
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        // This doesnt work at all FYI. Completely broken, no idea why
        public static bool RayInteresectWithCuboidSlabMethod(Cuboidd b, Ray r)
        {
            double tx1 = (b.X1 - r.dir.X) / r.dir.X;
            double tx2 = (b.X2 - r.dir.X) / r.dir.X;

            double tmin = Math.Min(tx1, tx2);
            double tmax = Math.Max(tx1, tx2);

            double ty1 = (b.Y1 - r.dir.Y) / r.dir.Y;
            double ty2 = (b.Y2 - r.dir.Y) / r.dir.Y;

            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            double tz1 = (b.Z1 - r.dir.Z) / r.dir.Z;
            double tz2 = (b.Z2 - r.dir.Z) / r.dir.Z;

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            return tmax >= tmin;
        }



        private BlockFacing GetExitingFullBlockFace(BlockPos pos, ref Vec3d exitPos)
        {
            for (int i = 0; i < BlockFacing.NumberOfFaces; i++)
            {
                BlockFacing blockSideFacing = BlockFacing.ALLFACES[i];
                Vec3i planeNormal = blockSideFacing.Normali;

                double demon = planeNormal.X * ray.dir.X + planeNormal.Y * ray.dir.Y + planeNormal.Z * ray.dir.Z;

                if (demon > 0.00001)
                {
                    Vec3d planePosition = pos.ToVec3d().Add(blockSideFacing.PlaneCenter);

                    Vec3d pt = Vec3d.Sub(planePosition, ray.origin);
                    double t = (pt.X * planeNormal.X + pt.Y * planeNormal.Y + pt.Z * planeNormal.Z) / demon;

                    if (t >= 0)
                    {
                        Vec3d pHit = new Vec3d(ray.origin.X + ray.dir.X * t, ray.origin.Y + ray.dir.Y * t, ray.origin.Z + ray.dir.Z * t);
                        exitPos = Vec3d.Sub(pHit, planePosition);

                        if (Math.Abs(exitPos.X) <= 0.5 && Math.Abs(exitPos.Y) <= 0.5 && Math.Abs(exitPos.Z) <= 0.5)
                        {
                            return blockSideFacing;
                        }
                    }
                }
            }

            return null;
        }

    }





    public interface IWorldIntersectionSupplier
    {
        Block GetBlock(BlockPos pos);

        Cuboidf[] GetBlockIntersectionBoxes(BlockPos pos);

        Entity[] GetEntitiesAround(Vec3d position, float horRange, float vertRange, ActionConsumable<Entity> matches = null);

        bool IsValidPos(BlockPos pos);

        Vec3i MapSize { get; }

        IBlockAccessor blockAccessor { get; }
    }

}
