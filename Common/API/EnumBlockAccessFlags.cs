using System;

namespace Vintagestory.API.Common
{
    [Flags]
    public enum EnumBlockAccessFlags
    {
        None = 0,
        BuildOrBreak = 1,
        Use = 2,
        Traverse = 4
    }
}
