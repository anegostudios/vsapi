using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common;

public class ChatCommandComparer : IEqualityComparer<IChatCommand>
{
    private static ChatCommandComparer _instance;
    public static ChatCommandComparer Comparer  {
        get {
            _instance ??= new ChatCommandComparer();
            return _instance;
                    
        }
    }
    public bool Equals(IChatCommand x, IChatCommand y)
    {
        return y != null && x != null && x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode(IChatCommand obj)
    {
        return obj.GetHashCode();
    }
}