using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    [Flags]
    public enum EnumBlockAccessFlags
    {
        None = 0,
        BuildOrBreak = 1,
        Use = 2
    }

}
