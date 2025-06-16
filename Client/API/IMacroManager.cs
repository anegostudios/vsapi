using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Client
{
    public interface IMacroManager
    {
        void DeleteMacro(int macroIndex);
        void LoadMacros();
        bool RunMacro(int macroIndex, IClientWorldAccessor world);
        bool SaveMacro(int macroIndex);
        void SetMacro(int macroIndex, IMacroBase macro);
        SortedDictionary<int, IMacroBase> MacrosByIndex { get; set; }
    }

    public interface IMacroBase
    {
        int Index { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        string[] Commands { get; set; }
        KeyCombination KeyCombination { get; set; }
        LoadedTexture iconTexture { get; set; }
        void GenTexture(ICoreClientAPI capi, int size);
    }
}