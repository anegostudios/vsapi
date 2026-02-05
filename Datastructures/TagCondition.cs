using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;

public readonly struct TagCondition<TTagSet> : ITagCondition<TagCondition<TTagSet>, TTagSet>
    where TTagSet : IHasSetOperations<TTagSet>, ITagSet
{
    public readonly TTagSet Tags;
    public readonly TTagSet InvertedTags;

    public readonly static TagCondition<TTagSet> Empty = new(TTagSet.GetEmpty(), TTagSet.GetEmpty());

    public const string InvertPrefix = "not-";



    public TagCondition(TTagSet tags, TTagSet inverted)
    {
        TTagSet intersection = tags.Intersect(inverted);

        Tags = tags.Except(intersection);
        InvertedTags = inverted.Except(intersection);
    }



    public static TagCondition<TTagSet> Get(ICoreAPI api, params string[] tags)
    {
        IEnumerable<string> straightTags = tags.Where(tag => !tag.StartsWith(InvertPrefix));
        IEnumerable<string> invertedTags = tags.Where(tag => tag.StartsWith(InvertPrefix));

        TTagSet straightTagsArray = api.TagsManager.GetTagSet<TTagSet>(straightTags.ToArray());
        TTagSet invertedTagsArray = api.TagsManager.GetTagSet<TTagSet>(invertedTags.ToArray());

        return new(straightTagsArray, invertedTagsArray);
    }

    public IEnumerable<string> ToTags(ICoreAPI api)
    {
        return api.TagsManager
            .GetTags(Tags)
            .Concat(api.TagsManager
                .GetTags(InvertedTags)
                .Select(tag => InvertPrefix + tag));
    }

    public bool IsEmpty() => Tags.IsEmpty() && InvertedTags.IsEmpty();

    public bool IsSupersetOf(TTagSet array)
    {
        if (!Tags.IsEmpty() && !Tags.IsSupersetOf(array)) return false;

        return true;
    }
    public bool IsSubsetOf(TTagSet array)
    {
        if (!Tags.IsEmpty() && !Tags.IsSubsetOf(array)) return false;
        if (!InvertedTags.IsEmpty() && InvertedTags.Overlaps(array)) return false;

        return true;
    }
    public bool Overlaps(TTagSet array)
    {
        if (!Tags.IsEmpty() && !array.Overlaps(Tags)) return false;
        if (!InvertedTags.IsEmpty() && array.IsSubsetOf(InvertedTags)) return false;

        return true;
    }
    public bool SetEquals(TTagSet array)
    {
        if (!InvertedTags.IsEmpty()) return false;
        return Tags.SetEquals(array);
    }

    public static bool OverlapsWithEach(TTagSet array, IEnumerable<TagCondition<TTagSet>> conditions)
    {
        foreach (TagCondition<TTagSet> condition in conditions)
        {
            if (!condition.Overlaps(array))
            {
                return false;
            }
        }

        return true;
    }
    public static bool SupersetOfAtLeastOne(TTagSet array, IEnumerable<TagCondition<TTagSet>> conditions)
    {
        foreach (TagCondition<TTagSet> condition in conditions)
        {
            if (condition.IsSubsetOf(array))
            {
                return true;
            }
        }

        return false;
    }

    public TagCondition<TTagSet> Union(TagCondition<TTagSet> array)
    {
        return new(Tags.Union(array.Tags), InvertedTags.Union(array.InvertedTags));
    }
    public TagCondition<TTagSet> Intersect(TagCondition<TTagSet> array)
    {
        return new(Tags.Intersect(array.Tags), InvertedTags.Intersect(array.InvertedTags));
    }
    public TagCondition<TTagSet> Except(TagCondition<TTagSet> array)
    {
        return new(Tags.Except(array.Tags), InvertedTags.Except(array.InvertedTags));
    }
    public TagCondition<TTagSet> SymmetricExcept(TagCondition<TTagSet> array)
    {
        return new(Tags.SymmetricExcept(array.Tags), InvertedTags.SymmetricExcept(array.InvertedTags));
    }

    public static TagCondition<TTagSet> GetEmpty() => Empty;
}
