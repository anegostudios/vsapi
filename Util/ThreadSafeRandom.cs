using System;

#nullable disable

namespace Vintagestory.API.Util
{
    public class ThreadSafeRandom : Random
    {
        object randLock = new object();

        public ThreadSafeRandom(int seed) : base(seed)
        {

        }

        public ThreadSafeRandom() : base()
        {
        }

        public override int Next(int maxValue)
        {
            lock (randLock)
            {
                return base.Next(maxValue);
            }
        }

        public override double NextDouble()
        {
            lock (randLock)
            {
                return base.NextDouble();
            }
        }

        public override int Next()
        {
            lock (randLock)
            {
                return base.Next();
            }
        }

        public override int Next(int minValue, int maxValue)
        {
            lock (randLock)
            {
                return base.Next(minValue, maxValue);
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            lock (randLock)
            {
                base.NextBytes(buffer);
            }
        }
        
    }
}
