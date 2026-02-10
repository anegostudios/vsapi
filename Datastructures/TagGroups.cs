using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;

[JsonConverter(typeof(JsonGeneralTagGroupsConverter))]
public sealed class GeneralTagGroups : TagGroups<TagSet>, IConcreteCloneable<GeneralTagGroups>
{
    private sealed class JsonGeneralTagGroupsConverter : JsonConverter<GeneralTagGroups>
    {
        public override GeneralTagGroups? ReadJson(JsonReader reader, Type objectType, GeneralTagGroups? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= new();

            GeneralTagGroups tagGroups = existingValue;

            JToken token = JToken.ReadFrom(reader);
            if (token is JArray tagArray)
            {
                tagGroups.SetTags(tagArray);
            }
            else if (token is JObject tagObject)
            {
                if (tagObject.ContainsKey("tags") && tagObject["tags"] is JArray tagsArray)
                {
                    tagGroups.SetTags(tagsArray);
                }
                if (tagObject.ContainsKey("reverseCheck") && tagObject["reverseCheck"] is JValue reverseCheck)
                {
                    tagGroups.ReverseCheck = (bool?)reverseCheck.Value ?? false;
                }
            }
            else
            {
                throw new InvalidOperationException($"Error on parsing tag groups, json:\n{token.ToString()}");
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, GeneralTagGroups? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public new GeneralTagGroups Clone()
    {
        return new()
        {
            resolved = resolved,
            conditions = conditions,
            tagNames = (string[][])tagNames.Clone(),
            ReverseCheck = ReverseCheck
        };
    }
}

[JsonConverter(typeof(JsonEntityTagGroupsConverter))]
public sealed class EntityTagGroups : TagGroups<EntityTagSet>, IConcreteCloneable<EntityTagGroups>
{
    private sealed class JsonEntityTagGroupsConverter : JsonConverter<EntityTagGroups>
    {
        public override EntityTagGroups? ReadJson(JsonReader reader, Type objectType, EntityTagGroups? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= new();

            EntityTagGroups tagGroups = existingValue;

            JToken token = JToken.ReadFrom(reader);
            if (token is JArray tagArray)
            {
                tagGroups.SetTags(tagArray);
            }
            else if (token is JObject tagObject)
            {
                if (tagObject.ContainsKey("tags") && tagObject["tags"] is JArray tagsArray)
                {
                    tagGroups.SetTags(tagsArray);
                }
                if (tagObject.ContainsKey("reverseCheck") && tagObject["reverseCheck"] is JValue reverseCheck)
                {
                    tagGroups.ReverseCheck = (bool?)reverseCheck.Value ?? false;
                }
            }
            else
            {
                throw new InvalidOperationException($"Error on parsing tag groups, json:\n{token.ToString()}");
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, EntityTagGroups? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public new EntityTagGroups Clone()
    {
        return new()
        {
            resolved = resolved,
            conditions = conditions,
            tagNames = (string[][])tagNames.Clone(),
            ReverseCheck = ReverseCheck
        };
    }
}

public class TagGroups<TTagSet> : IConcreteCloneable<TagGroups<TTagSet>>, IByteSerializable
    where TTagSet : IHasSetOperations<TTagSet>, ITagSet
{
    public bool ReverseCheck { get; set; } = false;


    protected bool resolved = false;
    protected bool cleared = false;
    protected ImmutableArray<TagCondition<TTagSet>> conditions = [];
    protected string[][] tagNames = [];



    public bool Check(TTagSet tags)
    {
        if (!resolved)
        {
            throw new InvalidOperationException("Trying to use unresolved TagGroups");
        }

        if (!ReverseCheck)
        {
            return TagCondition<TTagSet>.SupersetOfAtLeastOne(tags, conditions);
        }
        else
        {
            return TagCondition<TTagSet>.OverlapsWithEach(tags, conditions);
        }
    }

    public void Resolve(IWorldAccessor resolver)
    {
        conditions = tagNames.Select(tags => TagCondition<TTagSet>.Get(resolver.Api, tags)).ToImmutableArray();
        resolved = true;
    }

    public TagGroups<TTagSet> Clone()
    {
        return new()
        {
            resolved = resolved,
            conditions = conditions,
            tagNames = tagNames,
            ReverseCheck = ReverseCheck
        };
    }

    public void ClearTagNames()
    {
        tagNames = [];
        cleared = true;
    }

    public void ResolveTagNames(IWorldAccessor resolver)
    {
        if (!cleared)
        {
            return;
        }

        tagNames = conditions.Select(group => group.ToTags(resolver.Api).ToArray()).ToArray();
        cleared = false;
    }

    public void ToBytes(BinaryWriter writer)
    {
        if (cleared)
        {
            throw new InvalidOperationException("Trying to convert TagGroups to bytes after it was cleared");
        }

        writer.Write(ReverseCheck);
        writer.Write(tagNames.Length);
        foreach (string[] tags in tagNames)
        {
            writer.Write(tags.Length);
            foreach (string tag in tags)
            {
                writer.Write(tag);
            }
        }
    }

    public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
    {
        ReverseCheck = reader.ReadBoolean();
        int tagNamesLength = reader.ReadInt32();
        tagNames = new string[tagNamesLength][];
        for (int groupIndex = 0; groupIndex < tagNamesLength; groupIndex++)
        {
            int groupLength = reader.ReadInt32();
            tagNames[groupIndex] = new string[groupLength];
            for (int tagIndex = 0; tagIndex < groupLength; tagIndex++)
            {
                tagNames[groupIndex][tagIndex] = reader.ReadString();
            }
        }

        Resolve(resolver);
    }

    public IEnumerable<TagCondition<TTagSet>> GetResolvedTags() => conditions;



    object ICloneable.Clone() => Clone();

    protected void SetTags(string[][] tags) => tagNames = tags;

    protected void SetTags(JArray tags)
    {
        if (tags.Count == 0)
        {
            return;
        }

        if (tags[0] is JArray)
        {
            tagNames = (new JsonObject(tags)).AsObject<string[][]>() ?? [];
        }
        else
        {
            tagNames = [(new JsonObject(tags)).AsObject<string[]>() ?? []];
        }
    }

    protected string[][] GetTags() => tagNames;
}
