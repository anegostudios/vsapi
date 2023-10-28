﻿namespace Vintagestory.API.Server
{
    public interface IServerPhysicsTicker
    {
        ref int FlagDoneTick { get; }

        void onPhysicsTickServer(long elapsedMS);
        void AfterPhysicsTick();
    }
}