using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Vintagestory.API.Datastructures;

/// <summary>
/// Holds a complex tag condition, either as a set of disjunctive entries ([a and b and c] or [d and e])
/// or as a set of conjunctive entries ([a or b or c] and [d or e]).<br/>
/// In both cases each of the inner sets can also have a set of forbidden tags that is always disjunctive.<br/>
/// <br/>
/// The syntax for using these in json follows one simple rule: If no junction verb is specified, the group is treated as conjunctive (a and b and c)<br/>
/// ["all", "of", "these"]<br/>
/// [["all", "of", "these"]]<br/>
/// [["all", "of", "these"], (or) { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }]<br/>
/// { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }<br/>
/// { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }<br/>
/// { anyOf: [["all", "of", "these"], { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }] }<br/>
/// { allOf: [["anyOf", "of", "these"], (and) { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }] }<br/>
/// </summary>
/// <remarks> You cannot mix junction verbs on the same layer. </remarks>
[DocumentAsJson]
[JsonConverter(typeof(GenericComplexConditionConverter))]
public struct ComplexTagCondition<TTagSet> : IEquatable<ComplexTagCondition<TTagSet>> where TTagSet : IEquatable<TTagSet>
{
    // @perf: The conditions could be stored in a uniform way to get rid of the branch during evaluation.
    // Generally, storing them as [all of [any of], [any of], [any of]] should be more performant, as you would expect most
    // evaluations to fail. This means that path should be optimized, and storing them like this would provide a
    // early exit condition for most evaluations.
    // For now this is basically a straight copy from the original, because it turns out doing the transformation is
    // rather complicated and im not currently up for the task. Unfortunately this means the performance may vary greatly
    // depending on how a condition is specified.

    public Condition[]? conditions;
    public bool isDisjunctive;

    [MemberNotNullWhen(false, nameof(conditions))]
    public readonly bool IsEmpty => conditions == null || conditions.Length == 0;

    public struct Condition
    {
        public TTagSet RequiredTags;
        public TTagSet ForbiddenTags;

        public readonly bool Equals(Condition other)
        {
            if (RequiredTags != null && other.RequiredTags != null)
            {
                if (!RequiredTags.Equals(other.RequiredTags)) return false;
            }
            else if (RequiredTags != null || other.RequiredTags != null) return false;

            if (ForbiddenTags != null && other.ForbiddenTags != null)
            {
                if (!ForbiddenTags.Equals(other.ForbiddenTags)) return false;
            }
            else if (ForbiddenTags != null || other.ForbiddenTags != null) return false;

            return true;
        }

        public readonly override string ToString()
        {
            return (new StringBuilder(64)).Append("Required: ").Append(RequiredTags).Append(", Forbidden: ").Append(ForbiddenTags).ToString();
        }
    }

    public readonly override int GetHashCode()
    {
        int hash = isDisjunctive ? 0x123456 : 0x649871; // arbitrary numbers

        if (conditions != null)
            foreach (var condition in conditions)
                hash ^= condition.GetHashCode();

        return hash;
    }

    public static bool operator ==(in ComplexTagCondition<TTagSet> self, in ComplexTagCondition<TTagSet> other) => self.Equals(other);
    public static bool operator !=(in ComplexTagCondition<TTagSet> self, in ComplexTagCondition<TTagSet> other) => !self.Equals(other);
    public readonly override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is ComplexTagCondition<TTagSet> other) && this.Equals(other);
    }
    public readonly bool Equals(ComplexTagCondition<TTagSet> other)
    {
        if (conditions == null || other.conditions == null) return conditions == null && other.conditions == null;
        if (conditions.Length != other.conditions.Length) return false;
        if (isDisjunctive != other.isDisjunctive) return false;
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!conditions[i].Equals(other.conditions[i])) return false;
        }
        return true;
    }

    public readonly override string ToString()
    {
        if (this.IsEmpty) return "empty";
        return (new StringBuilder(128)).AppendJoin(";", this.conditions).ToString();
    }
}

public static partial class ComplexTagConditionExtensions
{
    // A lot of methods are essentially duplicated here, but to get rid of that interfaces need to be added bottom up to TagSets.
    // We also cannot really get around this via the registries as in other cases, because networking code lives in the api.
    // This isn't great, but its also not the end of the world. You cannot have both here, either this or ITag<TSelf>.

    public static bool Matches(in this ComplexTagCondition<TagSet> complexCondition, in TagSet matchAgainst)
    {
        if (complexCondition.conditions == null) return true;

        if (complexCondition.isDisjunctive)
        {
            foreach (var condition in complexCondition.conditions)
            {
                // Has to match at least one sub-condition, so exit if one does:
                if(!condition.ForbiddenTags.Overlaps(in matchAgainst) && condition.RequiredTags.IsFullyContainedIn(in matchAgainst)) return true;
            }
            return false;
        }
        else // conjunctive
        {
            foreach (var condition in complexCondition.conditions)
            {
                // Has to match each sub-condition, so exit if one does not:
                if(condition.ForbiddenTags.Overlaps(in matchAgainst) || !condition.RequiredTags.Overlaps(in matchAgainst)) return false;
            }
            return true;
        }
    }

    public static bool Matches(in this ComplexTagCondition<TagSetFast> complexCondition, in TagSetFast matchAgainst)
    {
        if (complexCondition.conditions == null) return true;

        if (complexCondition.isDisjunctive)
        {
            foreach (var condition in complexCondition.conditions)
            {
                // Has to match at least one sub-condition, so exit if one does:
                // (matchAgainst & condition.RequiredTags) ^ condition.RequiredTags produces zero if condition.RequiredTags is fully contained in matchAgainst.
                // (matchAgainst & condition.ForbiddenTags) produces zero if condition.ForbiddenTags does not overlap matchAgainst.
                // Or'ing the two produces zero (empty) if condition.RequiredTags is fully contained and condition.ForbiddenTags does not overlap.
                // This is branchless compared to the slow version, therefore cannot mispredict and optimizes well.
                if ((((matchAgainst & condition.RequiredTags) ^ condition.RequiredTags) | (matchAgainst & condition.ForbiddenTags)).IsEmpty) return true;
            }
            return false;
        }
        else // conjunctive
        {
            foreach (var condition in complexCondition.conditions)
            {
                // Has to match each sub-condition, so exit if one does not:
                if (!condition.RequiredTags.Overlaps(matchAgainst) || condition.ForbiddenTags.Overlaps(matchAgainst)) return false;
            }
            return true;
        }
    }

    public static void ToBytes(in this ComplexTagCondition<TagSetFast> complexCondition, BinaryWriter writer)
    {
        if (complexCondition.conditions == null)
        {
            writer.Write((UInt16)0);
            return;
        }

        writer.Write((UInt16)complexCondition.conditions.Length); // If this has to be longer than 2^16 all hope is lost anyways.
        writer.Write(complexCondition.isDisjunctive);

        foreach (var condition in complexCondition.conditions)
        {
            condition.RequiredTags.ToBytes(writer);
            condition.ForbiddenTags.ToBytes(writer);
        }
    }

    public static ComplexTagCondition<TagSetFast> FastFromBytes(BinaryReader reader)
    {
        var length = reader.ReadUInt16();
        if (length == 0) return default;

        ComplexTagCondition<TagSetFast> result = default;
        result.conditions = new ComplexTagCondition<TagSetFast>.Condition[length];
        result.isDisjunctive = reader.ReadBoolean();

        for (UInt16 i = 0; i < length; i++)
        {
            result.conditions[i].RequiredTags = TagSetFast.FromBytes(reader);
            result.conditions[i].ForbiddenTags = TagSetFast.FromBytes(reader);
        }

        return result;
    }

    public static void ToBytes(in this ComplexTagCondition<TagSet> complexCondition, BinaryWriter writer)
    {
        if (complexCondition.conditions == null)
        {
            writer.Write((UInt16)0);
            return;
        }

        writer.Write((UInt16)complexCondition.conditions.Length); // If this has to be longer than 2^16 all hope is lost anyways.
        writer.Write(complexCondition.isDisjunctive);

        foreach (var condition in complexCondition.conditions)
        {
            condition.RequiredTags.ToBytes(writer);
            condition.ForbiddenTags.ToBytes(writer);
        }
    }

    public static ComplexTagCondition<TagSet> FromBytes(BinaryReader reader)
    {
        var length = reader.ReadUInt16();
        if (length == 0) return default;

        ComplexTagCondition<TagSet> result = default;
        result.conditions = new ComplexTagCondition<TagSet>.Condition[length];
        result.isDisjunctive = reader.ReadBoolean();

        for (UInt16 i = 0; i < length; i++)
        {
            result.conditions[i].RequiredTags = TagSet.FromBytes(reader);
            result.conditions[i].ForbiddenTags = TagSet.FromBytes(reader);
        }

        return result;
    }
}

public sealed class GenericComplexConditionConverter : JsonConverter
{
    public static ComplexConditionConverter<TagSet> CollectibleConverter = null!;
    public static ComplexConditionConverter<TagSetFast> EntityConverter = null!;

    /// <summary> Called from Main init as soon as the registries are available. </summary>
    public static void StaticInit(ITagRegistry<TagSet> collectibleRegistry, ITagRegistry<TagSetFast> entityRegistry)
    {
        CollectibleConverter = new(collectibleRegistry);
        EntityConverter = new(entityRegistry);
    }


    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (objectType == typeof(ComplexTagCondition<TagSet>)) return CollectibleConverter.ReadJson(reader, objectType, existingValue, serializer);
        else return EntityConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

    public override bool CanConvert(Type objectType) => objectType.GetGenericTypeDefinition() == typeof(ComplexTagCondition<>);
}


public sealed class ComplexConditionConverter<TTagSet>(ITagRegistry<TTagSet> registry) : JsonConverter<ComplexTagCondition<TTagSet>> where TTagSet : IEquatable<TTagSet>
{
    readonly ITagRegistry<TTagSet> registry = registry;

    public override ComplexTagCondition<TTagSet> ReadJson(JsonReader reader, Type objectType, ComplexTagCondition<TTagSet> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var rootToken = JToken.ReadFrom(reader);
        return ReadJson(rootToken);
    }

    public ComplexTagCondition<TTagSet> ReadJson(JToken rootToken)
    {
        if (rootToken is JArray rootArray)
        {
            var firstType = rootArray.First?.Type;
            if (firstType == JTokenType.String)
            {
                //  ["all", "of", "these"]
                return new() { conditions = [ new() { RequiredTags = parseSet(rootArray) } ], isDisjunctive = true };
            }
            else if (!firstType.HasValue)
            {
                //  []
                return default;
            }
            else
            {
                //  [["all", "of", "these"]]
                //  [["all", "of", "these"], (or) { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }]
                var conditions = parseArrayOfConditions(rootArray, ConditionParsingMode.Conjunctive);
                return new(){ conditions = conditions, isDisjunctive = true };
            }
        }
        else if (rootToken is JObject rootObject)
        {
            //  { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }
            //  { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }
            //  { anyOf: [["all", "of", "these"], { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }] }
            //  { allOf: [["anyOf", "of", "these"], (and) { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }] }


            var outerMode = OuterParseMode.Unknown; // error checking 
            ComplexTagCondition<TTagSet>.Condition simpleCondition = default; // used for constructing the simple case
            ComplexTagCondition<TTagSet> resultComplexCondition = default;

            foreach (var (name, valueToken) in rootObject)
            {

                switch (name)
                {
                    case "allOf": {
                        //  { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }
                        //  { allOf: [["anyOf", "of", "these"], (and) { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }] }
                        if (valueToken is not JArray valueArray)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, allOf must be an array. json:\n{valueToken?.ToString()}");
                        }

                        if (valueArray.First?.Type == JTokenType.String)
                        {
                            //  { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }
                            if (outerMode == OuterParseMode.Complex)
                            {
                                // The only way we get here is both are present:
                                throw new InvalidOperationException($"Error while parsing tag condition, cannot have both anyOf and allOf here. json:\n{valueToken?.ToString()}");
                            }
                            outerMode = OuterParseMode.Simple;

                            simpleCondition.RequiredTags = parseSet(valueArray);

                            resultComplexCondition.isDisjunctive = true; // This is about the invisible outer set, so it _is_ disjunctive! (We are the inner, which is the conjunctive one).
                        }
                        else
                        {
                            //  { allOf: [["anyOf", "of", "these"], (and) { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }] }
                            if (outerMode == OuterParseMode.Simple)
                            {
                                throw new InvalidOperationException($"Error while parsing tag condition, allOf must be an array of inner conditions here. json:\n{valueToken?.ToString()}");
                            }
                            outerMode = OuterParseMode.Complex;

                            resultComplexCondition.conditions = parseArrayOfConditions(valueArray, ConditionParsingMode.Disjunctive);

                            resultComplexCondition.isDisjunctive = false; // If we are the outer set that set is conjunctive.
                        }

                        break;
                    }

                    case "anyOf": {
                        //  { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }
                        //  { anyOf: [["all", "of", "these"], { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }] }
                        if (valueToken is not JArray valueArray )
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, anyOf must be an array. json:\n{valueToken?.ToString()}");
                        }

                        if (valueArray.First?.Type == JTokenType.String)
                        {
                            //  { anyOf: ["any", "of", "these"], noneOf: ["any", "of", "these"] }
                            if (outerMode == OuterParseMode.Complex)
                            {
                                throw new InvalidOperationException($"Error while parsing tag condition, anyOf must be an array of inner conditions here. json:\n{valueToken?.ToString()}");
                            }
                            outerMode = OuterParseMode.Simple;

                            simpleCondition.RequiredTags = parseSet(valueArray);

                            resultComplexCondition.isDisjunctive = false; // This is about the invisible outer set, so it is _not_ disjunctive! (We are the inner, which is the disjunctive one).
                        }
                        else
                        {
                            //  { anyOf: [["all", "of", "these"], { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }] }
                            if (outerMode == OuterParseMode.Simple)
                            {
                                throw new InvalidOperationException($"Error while parsing tag condition, allOf must be an array of inner conditions here. json:\n{valueToken?.ToString()}");
                            }
                            outerMode = OuterParseMode.Complex;

                            resultComplexCondition.conditions = parseArrayOfConditions(valueArray, ConditionParsingMode.Conjunctive);

                            resultComplexCondition.isDisjunctive = true; // If we are the outer set that set is disjunctive.
                        }

                        break;
                    }

                    case "noneOf": {
                        if (outerMode == OuterParseMode.Complex)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, noneOff cannot be used here with complex matching. Add it to the inner conditions instead. json:\n{valueToken?.ToString()}");
                        }
                        outerMode = OuterParseMode.Simple;

                        if (valueToken is not JArray valueArray || valueArray.First?.Type != JTokenType.String)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, noneOf must be an array of string here. json:\n{valueToken?.ToString()}");
                        }

                        simpleCondition.ForbiddenTags = parseSet(valueArray);
                        break;
                    }

                    default:
                        throw new InvalidOperationException($"Error while parsing tag condition, unknown condition property '{name}'. json:\n{valueToken?.ToString()}");
                }
            }

            if (outerMode == OuterParseMode.Simple) resultComplexCondition.conditions = [ simpleCondition ];

            return resultComplexCondition;
        }
        else
        {
            throw new InvalidOperationException($"Error while parsing tag condition, must be an array or object. json:\n{rootToken.ToString()}");
        }
    }

    // [] is considdered simple, [[], []] is complex
    enum OuterParseMode { Unknown = default, Simple, Complex }
    enum ConditionParsingMode { Disjunctive, Conjunctive }

    ComplexTagCondition<TTagSet>.Condition[] parseArrayOfConditions(JArray array, ConditionParsingMode mode)
    {
        var conditions = new ComplexTagCondition<TTagSet>.Condition[array.Count];

        int i = 0;
        foreach (var element in array)
        {
            conditions[i++] = parseIndividualCondition(element, mode);
        }

        return conditions;
    }

    ComplexTagCondition<TTagSet>.Condition parseIndividualCondition(JToken token, ConditionParsingMode mode)
    {
        if (token is JArray conditionArray)
        {
            var firstType = conditionArray.FirstOrDefault()?.Type;
            if (firstType == JTokenType.String)
            {
                //  ["1", "2", "3"]
                return new() { RequiredTags = parseSet(conditionArray) };
            }
            else if (!firstType.HasValue) // i feel like this should just throw, but previously i didn't.
            {
                //  []
                return default;
            }

            throw new InvalidOperationException($"Error while parsing tag condition, individual conditions must be arrays of strings or objects, got json:\n{token.ToString()}");
        }
        else if (token is JObject conditionObject)
        {
            //  { anyOf: ["any", "of", "these"] }
            //  { allOf: ["all", "of", "these"], noneOf: ["any", "of", "these"] }

            ComplexTagCondition<TTagSet>.Condition condition = default;

            foreach (var (name, valueToken) in conditionObject)
            {
                switch (name)
                {
                    case "allOf": {
                        if (mode != ConditionParsingMode.Conjunctive)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, cannot mix disjunctive and conjunctive forms. json:\n{token.ToString()}");
                        }
                        if (valueToken is not JArray valueArray || valueArray.First?.Type != JTokenType.String)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, allOf must be an array of string here. json:\n{valueToken?.ToString()}");
                        }

                        condition.RequiredTags = parseSet(valueArray);
                        break;
                    }

                    case "anyOf": {
                        if (mode != ConditionParsingMode.Disjunctive)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, cannot mix disjunctive and conjunctive forms. json:\n{token.ToString()}");
                        }
                        if (valueToken is not JArray valueArray || valueArray.First?.Type != JTokenType.String)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, anyOf must be an array of string here. json:\n{valueToken?.ToString()}");
                        }

                        condition.RequiredTags = parseSet(valueArray);
                        break;
                    }

                    case "noneOf": {
                        if (valueToken is not JArray valueArray)
                        {
                            throw new InvalidOperationException($"Error while parsing tag condition, noneOf must be an array of string. json:\n{valueToken?.ToString()}");
                        }

                        condition.ForbiddenTags = parseSet(valueArray);
                        break;
                    }

                    default: throw new InvalidOperationException($"Error while parsing tag condition, unknown condition property '{name}'. json:\n{valueToken?.ToString()}");
                }
            }

            return condition;
        }
        else
        {
            throw new InvalidOperationException($"Error while parsing tag condition, must be an array or object. json:\n{token.ToString()}");
        }
    }

    TTagSet parseSet(JArray array)
    {
        var tags = array.Where(t =>
        {
            if (t.Type != JTokenType.String) return false;
            var str = (string?)t;

            var error = TagRegistry.ValidateTag(str);
            if (error == TagValidationError.None) return true;

            registry.logger.Debug($"[{registry.debugName}] [{t.Path}] "+TagRegistry.FormatIssueMessage(error, str));
            return false;
        })
        .Select(e => (string)e!);
        registry.TryRegisterAndCreateTagSetAndLogIssues(out var set, tags);
        return set;
    }

    public override void WriteJson(JsonWriter writer, ComplexTagCondition<TTagSet> value, JsonSerializer serializer) => throw new NotImplementedException();
}



