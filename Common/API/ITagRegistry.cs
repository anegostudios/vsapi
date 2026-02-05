using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common;

public interface ITagsManager : ITagTypeRegistry
{
    void RegisterEntityTags(params string[] tags);
    void RegisterGeneralTags(params string[] tags);

    EntityTagSet GetEntityTagSet(params string[] tags);
    TagSet GetGeneralTagSet(params string[] tags);

    IEnumerable<string> GetEntityTags(EntityTagSet array);
    IEnumerable<string> GetGeneralTags(TagSet array);

    TTagSet GetTagSetUnsafe<TTagSet>(params string[] tags)
        where TTagSet : ITagSet;
    IEnumerable<string> GetTagsUnsafe<TTagSet>(TTagSet array)
        where TTagSet : ITagSet;
}

public interface ITagTypeRegistry
{
    void RegisterTagType<TTagSet>()
        where TTagSet : ITagSet;

    void RegisterTags<TTagSet>(params string[] tags)
        where TTagSet : ITagSet;

    TTagSet GetTagSet<TTagSet>(params string[] tags)
        where TTagSet : ITagSet;

    IEnumerable<string> GetTags<TTagSet>(TTagSet array)
        where TTagSet : ITagSet;
}

public interface ITagRegistry
{
    void RegisterTags(params string[] tags);

    string[] GetAllTags();
}

public interface ITagRegistry<TTagSet> : ITagRegistry
{
    TTagSet GetTagSet(params string[] tags);

    IEnumerable<string> GetTags(TTagSet array);
}

public interface ITagSet
{
    abstract static ITagSet Empty { get; }

    abstract static ITagSet GetArray(IEnumerable<ushort> indexes);

    abstract static string TypeCode { get; }

    IEnumerable<ushort> GetIndexes();
}

public interface IHasSetOperations<TSelf>
{
    static abstract TSelf GetEmpty();

    TSelf Union(TSelf array);
    TSelf Intersect(TSelf array);
    TSelf Except(TSelf array);
    TSelf SymmetricExcept(TSelf array);

    bool IsSubsetOf(TSelf array);
    bool IsSupersetOf(TSelf array);
    bool Overlaps(TSelf array);
    bool SetEquals(TSelf array);

    bool OverlapsWithEach(IEnumerable<TSelf> arrays);
    bool SupersetOfAtLeastOne(IEnumerable<TSelf> arrays);

    bool IsEmpty();
}

public interface ITagCondition<TSelf, TTagSet>
{
    static abstract TSelf GetEmpty();
    static abstract bool OverlapsWithEach(TTagSet array, IEnumerable<TSelf> conditions);
    static abstract bool SupersetOfAtLeastOne(TTagSet array, IEnumerable<TSelf> conditions);

    bool IsSubsetOf(TTagSet array);
    bool IsSupersetOf(TTagSet array);
    bool Overlaps(TTagSet array);
    bool SetEquals(TTagSet array);

    TSelf Union(TSelf array);
    TSelf Intersect(TSelf array);
    TSelf Except(TSelf array);
    TSelf SymmetricExcept(TSelf array);

    bool IsEmpty();
}

public interface ITagLoader
{
    void LoadTagsFromAssets(ICoreServerAPI api);

    void LoadTagsFromPacket(Dictionary<string, string[]> tags);

    Dictionary<string, string[]> GetTagsForPacket();

    void LockRegistry();
}
