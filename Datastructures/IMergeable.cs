using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Datastructures
{
    public interface IMergeable<T>
    {
        bool MergeIfEqual(T target);
    }
}
