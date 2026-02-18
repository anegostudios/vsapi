using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;
public interface ITagRegistry<TTagSet> {
    //NOTE(Rennorb): We provide overloads for both ReadOnlySpan<T> and IEnumerable<T>. :SpanEnumerableDuplication
    // There is no common interface between the two and both have their reason to exist:
    // - Spans cannot be iterated but it would be a waste to turn one into an iterator if you already have it (or an array)
    // - Iterators need to allocate to be turned into span, which would be a waste if it comes from a dynamic selector.

    #region Register
    /// <exception cref="TagRegistryException"> If the registry is full or locked. </exception>
    /// <remarks> Will still register tags until it is at capacity. </remarks>
    public void Register(params ReadOnlySpan<string> tags)
    {
        var err = this.TryRegister(tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags);
    }
    /// <remarks> Will return an error if at capacity or locked. Will still register tags until it is at capacity. </remarks>
    public TagRegistryError TryRegister(params ReadOnlySpan<string> tags);
    /// <inheritdoc cref="TryRegister(ReadOnlySpan{string})"/>
    public TagRegistryError TryRegisterAndLogIssues(params ReadOnlySpan<string> tags)
    {
        var err = this.TryRegister(tags);
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags);
        return err;
    }

    /// <remarks> Will still register tags and consume the iterator until it is at capacity. </remarks>
    /// <exception cref="TagRegistryException"> If the registry is full or locked. </exception>
    public void Register(IEnumerable<string> tags)
    {
        var err = this.TryRegister(tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags.ToArray()); // :ErrorPathDoubleIterate
    }
    /// <remarks> Will return an error if at capacity or locked. Will still consume the iterator and register tags until it is at capacity. </remarks>
    public TagRegistryError TryRegister(IEnumerable<string> tags); // :SpanEnumerableDuplication
    /// <inheritdoc cref="TryRegister(IEnumerable{string})"/>
    public TagRegistryError TryRegisterAndLogIssues(IEnumerable<string> tags)
    {
        var err = this.TryRegister(tags);
        //NOTE(Rennorb): This iterates the enumerable a second time, but since this is the error path its ok to be slow here.
        // We don't allocate a buffer to reuse it in case of error, because that would pollute the desired fast path just for the occasional error to be faster.
        // :ErrorPathDoubleIterate
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags.ToArray());
        return err;
    }
    #endregion

    #region Create
    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided tag marked as active. </summary>
    /// <exception cref="TagRegistryException"> If any of the requested tags cannot be found. </exception>
    public TTagSet CreateTagSet(params ReadOnlySpan<string> tags)
    {
        var err = this.TryCreateTagSet(out var set, tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags);
        return set;
    }
    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided span of tags marked as active. </summary>
    /// <remarks> Will return an error if at least one of the tags does not exist in the registry. The set will still be populated with the remaining found tags. </remarks>
    public TagRegistryError TryCreateTagSet(out TTagSet set, params ReadOnlySpan<string> tags);
    /// <inheritdoc cref="TryCreateTagSet(out TTagSet, ReadOnlySpan{string})"/>
    public TagRegistryError TryCreateTagSetAndLogIssues(out TTagSet set, params ReadOnlySpan<string> tags)
    {
        var err = this.TryCreateTagSet(out set, tags);
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags);
        return err;
    }

    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided tag marked as active. </summary>
    /// <remarks> Will fully consume the iterator even if an exception is thrown. </remarks>
    /// <exception cref="TagRegistryException"> If any of the requested tags cannot be found. </exception>
    public TTagSet CreateTagSet(IEnumerable<string> tags)
    {
        var err = this.TryCreateTagSet(out var set, tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags.ToArray()); // :ErrorPathDoubleIterate
        return set;
    }
    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided enumerable of tags marked as active. </summary>
    /// <remarks>
    /// Will return an error if at least one of the tags does not exist in the registry.
    /// Will still consume the iterator and the set will still be populated with the remaining found tags.
    /// </remarks>
    public TagRegistryError TryCreateTagSet(out TTagSet set, IEnumerable<string> tags); // :SpanEnumerableDuplication
    /// <inheritdoc cref="TryCreateTagSet(out TTagSet, IEnumerable{string})"/>
    public TagRegistryError TryCreateTagSetAndLogIssues(out TTagSet set, IEnumerable<string> tags)
    {
        var err = this.TryCreateTagSet(out set, tags);
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags.ToArray()); // :ErrorPathDoubleIterate
        return err;
    }
    #endregion

    #region RegisterAndCreate
    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided tag marked as active. </summary>
    /// <exception cref="TagRegistryException"> If any of the the requested tags cannot be found and the registry is full or locked. </exception>
    public TTagSet RegisterAndCreateTagSet(params ReadOnlySpan<string> tags)
    {
        var err = this.TryCreateTagSet(out var set, tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags);
        return set;
    }
    /// <remarks>
    /// Will return an error if at capacity or locked.
    /// Will still register tags until it is at capacity.
    /// The returned set will be missing the tags that could not be registered in that case.
    /// </remarks>
    public TagRegistryError TryRegisterAndCreateTagSet(out TTagSet set, params ReadOnlySpan<string> tags);
    /// <inheritdoc cref="TryRegisterAndCreateTagSet(out TTagSet, ReadOnlySpan{string})"/>
    public TagRegistryError TryRegisterAndCreateTagSetAndLogIssues(out TTagSet set, params ReadOnlySpan<string> tags)
    {
        var err = this.TryRegisterAndCreateTagSet(out set, tags);
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags);
        return err;
    }

    /// <summary> Create the corresponding <typeparamref name="TTagSet"/> in its native representation that has the provided tag marked as active. </summary>
    /// <remarks> Will fully consume the iterator even if an exception is thrown. </remarks>
    /// <exception cref="TagRegistryException"> If any of the the requested tags cannot be found and the registry is full or locked. </exception>
    public TTagSet RegisterAndCreateTagSet(IEnumerable<string> tags)
    {
        var err = this.TryCreateTagSet(out var set, tags);
        if (err != TagRegistryError.None) throw new TagRegistryException(this.debugName, err, tags.ToArray()); // :ErrorPathDoubleIterate
        return set;
    }
    /// <remarks>
    /// Will return an error if at capacity or locked.
    /// Will still consume the iterator completely and register tags until it is at capacity.
    /// The returned set will be missing the tags that could not be registered in that case.
    /// </remarks>
    public TagRegistryError TryRegisterAndCreateTagSet(out TTagSet set, IEnumerable<string> tags); // :SpanEnumerableDuplication
    /// <inheritdoc cref="TryRegisterAndCreateTagSet(out TTagSet, IEnumerable{string})"/>
    public TagRegistryError TryRegisterAndCreateTagSetAndLogIssues(out TTagSet set, IEnumerable<string> tags)
    {
        var err = this.TryRegisterAndCreateTagSet(out set, tags);
        if (err != TagRegistryError.None) this.LogIssue(this.debugName, err, tags.ToArray()); // :ErrorPathDoubleIterate
        return err;
    }
    #endregion

    public IEnumerable<string> SlowEnumerateTagNames(TTagSet set);


    #region Logging
    ILogger logger { get; }
    string debugName { get; }

    //NOTE(Rennorb): Putting the logging into separate functions puts them into a colder path [NoInline] and provides the ability to reuse them with custom parameters.

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void LogIssue(string debugLocation, TagRegistryError error, params ReadOnlySpan<string> tags)
    {
        if (error == TagRegistryError.None) return;

        this.logger.Debug(TagRegistry.FormatIssueMessage(debugLocation, error, tags));
    }

    
    #endregion
}

public static class TagRegistry
{
    public static string? FormatIssueMessage(string debugLocation, TagRegistryError error, ReadOnlySpan<string> tags)
    {
        if (error == TagRegistryError.None) return null;

        var sb = new StringBuilder(1024);

        sb.Append('[').Append(debugLocation).Append("] ");
        sb.Append(tags.Length > 1 ? "Some tags in " : "Tag ");
        for (int i = 0; i < tags.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append('\'').Append(tags[i]).Append('\'');
        }
        switch(error)
        {
            case TagRegistryError.RegistryAtCapacity:  sb.Append(" could not be registered because the registry is full."); break;
            case TagRegistryError.RegistryLocked:      sb.Append(" could not be registered because the registry is locked. You are too late or tying to register a tag clientside. Maybe pre-register the tags you are trying to use?"); break;
            case TagRegistryError.SomeTagsNotFound:    sb.Append(" were not found."); break;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Validates the format of a tag.<br/>
    /// Tags must only contain lowercase alphanumerics (0-9, a-z) and dashes, must not contain two consecutive dashes, and must start with a lowercase letter.<br/>
    /// It must also not be empty and at least three characters long.
    /// </summary>
    public static TagValidationError ValidateTag(ReadOnlySpan<char> tag)
    {
        if (tag.Length == 0) return TagValidationError.TagEmpty;
        if (tag.Length < 3) return TagValidationError.TooShort;

        // the first char should be a letter
        var firstChar = tag[0];
        if (firstChar < 'a' || 'z' < firstChar) return TagValidationError.InvalidStart;

        for (int i = 1; i < tag.Length; i++)
        {
            var c = tag[i];
            if ('a' <= c && c <= 'z')  continue; // good
            if ('0' <= c && c <= '9')  continue; // good
            if (c == '-')
            {
                if (tag[i - 1] == '-') return TagValidationError.DoubleDash;
                else continue; // good
            }

            return TagValidationError.InvalidCharacter + i;
        }

        return TagValidationError.None;
    }

    public static string? FormatIssueMessage(TagValidationError error, ReadOnlySpan<char> tag)
    {
        switch (error)
        {
            case TagValidationError.None: return null;
            case TagValidationError.TagEmpty: return $"A tag must not be empty.";
            case TagValidationError.TooShort: return $"A tag must be at least three cahracters long.";
            case TagValidationError.InvalidStart: return $"Malformed tag '{tag}', tags must start with a lowercase letter (a-z).";
            case TagValidationError.DoubleDash: return $"Malformed tag '{tag}', tags must not contain consecutive dashes.";
            default: // InvalidCharacter
                int index = error - TagValidationError.InvalidCharacter;
                if (index < 0 || index >= tag.Length) return $"Malformed tag '{tag}'."; // the error was packed incorrectly
                return $"Malformed tag '{tag}', invalid character '{tag[index]}' at index {index}. Tags may only contain lowercase alphanumerics (0-9, a-z) and dashes";
        }
    }
}

public enum TagRegistryError
{
    None = 0,
    RegistryLocked,
    RegistryAtCapacity,
    SomeTagsNotFound,
}

public enum TagValidationError : int
{
    None = 0,
    TagEmpty,
    TooShort,
    InvalidStart,
    DoubleDash,
    /// <remarks> Don't test for this directly. If the tag contains an invalid character, InvalidCharacter + index of that character will be returned from <see cref="TagRegistry.ValidateTag(ReadOnlySpan{char})"/>. </remarks>
    InvalidCharacter,
}

public class TagRegistryException(string debugLocation, TagRegistryError error, ReadOnlySpan<string> tags) : Exception
{
    public readonly TagRegistryError Error = error;
    // NOTE(Rennorb): This is the slow error path, so always formatting the message is probably fine.
    // Would need to store the tags otherwise, which makes it so we cannot use Spans.
    public override string Message { get; } = TagRegistry.FormatIssueMessage(debugLocation, error, tags)!;
}
